using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Infrastructure.Utils;
using Wrly.Infrastuctures.Utils;
using Wrly.Models;
using Wrly.Models.Feeds;
using Wrly.Models.Share;
using Wrly.Storage;
using WrlyInfrastucture.Filters;

namespace Wrly.Controllers
{
    [ValidateInput(false)]
    public class PressController : BaseController
    {

        IPressProcessor _press;
        IPressProcessor Press
        {
            get
            {
                if (_press == null)
                {
                    _press = new PressProcessor();
                }
                return _press;
            }
        }


        public PressController() { }

        //
        // GET: /Press/

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Create(string password)
        {
            return View(new PostViewModel() { PostType = (byte)Enums.PostTypes.PressRelease });
        }

        [AllowAnonymous]
        [CompressFilter]
        public async Task<ActionResult> Posts(long? id)
        {
            if (id > 0)
            {
                var model = await Press.Detail(Convert.ToInt64(id));
                return View(model);
            }
            return null;
        }

        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> Reshares(long? id)
        {
            if (id > 0)
            {
                var model = await Press.ReshareMap(Convert.ToInt64(id));
                return View(model);
            }
            return null;
        }


        [Authorize]
        public ActionResult HomeCare()
        {
            return View();
        }

        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> WriteBig(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("WriteBig", new { Id = Guid.NewGuid().ToString() });
            }
            var data = await Press.GetFromDraft(id);
            return View(data);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(PostViewModel post)
        {
            if (ModelState.IsValid)
            {
                long result = await Press.Create(post, null);
            }
            return View(new PostViewModel());
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit(PostViewModel post)
        {
            if (ModelState.IsValid)
            {
                long result = await Press.Create(post, null);
            }
            return View(new PostViewModel());
        }

        [Authorize]
        public async Task<ActionResult> load(long id, int type)
        {
            var post = await Press.GetSingle(id);
            if (post.PostType == (int)Enums.PostTypes.Blog)
            {
                return PartialView("_PartialDetailArticle", post);
            }
            return PartialView("_PartialDetailNews", post);
        }

        [Authorize]
        [ValidateAntiForgeryToken()]
        [ValidateInput(true)]
        [HttpPost]
        public ActionResult AddReply(PostViewModel model)
        {
            long result = 0;// Press.InsertNewReply(model);
            if (result > 0)
            {
                ModelState.AddModelError("Success", "Your reply has been posted to this thread and is under moderation, you can see once one of administrator allows it to be shown.");
            }
            else
            {
                ModelState.AddModelError("Error", "There is an error while adding reply.");
            }
            List<ReplyViewModel> replies = Press.GetRepliesForPost(model.AddNewReply.PostID);
            return PartialView("_Replies", replies);
        }

        [Authorize]
        [ValidateAntiForgeryToken()]
        [ValidateInput(true)]
        [HttpPost]
        public async Task<ActionResult> Comment(CommentViewModel model)
        {
            var result = await Press.AddComment(model);
            if (result.Type == Enums.ResultType.Success)
            {
                var reply = await Press.GetSingleReply(Convert.ToInt64(result.ReferenceID));
                return PartialView("_FeedComment", reply);
            }
            return PartialView("_ActionResultMessage", result);
        }

        [Authorize]
        public async Task<ActionResult> Reply(string q, string container)
        {
            var replyViewModel = new ReplyViewModel();
            replyViewModel.Hash = q;
            ViewBag.Container = container;
            return PartialView("_CommentOnReply", replyViewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken()]
        [ValidateInput(true)]
        [HttpPost]
        public async Task<ActionResult> Reply(CommentViewModel model)
        {
            var result = await Press.AddComment(model);
            if (result.Type == Enums.ResultType.Success)
            {
                var reply = await Press.GetSingleReply(Convert.ToInt64(result.ReferenceID));
                return PartialView("_FeedReply", reply);
            }
            return PartialView("_CommentOnReply", model);
        }

        [Authorize]
        public ActionResult Statistics(int id, string objAction, string en, string rtn)
        {
            int result = Press.Statistics(id, objAction, en);
            if (Request.IsAjaxRequest())
                return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = result };
            else
                return Redirect(rtn);
        }

        public ActionResult GetReplies(int id)
        {
            List<ReplyViewModel> replies = Press.GetRepliesForPost(id);
            return PartialView("_Replies", replies);
        }

        [Authorize]
        public ActionResult loadoptions(int id)
        {
            var questionDetails = Press.GetQuestionOptions(id);
            return PartialView("_QuestionMoreOptions", questionDetails);
        }

        [Authorize]
        public async Task<ActionResult> ShareNews()
        {
            return PartialView("_ShareUpdate");
        }


        public async Task<ActionResult> moreReplies(string q, string level, int pageNumber = 0, int stock = 0)
        {
            var replies = await Press.MoreReplies(q, stock, pageNumber);
            if (!string.IsNullOrEmpty(level) && level.Equals("first", StringComparison.InvariantCultureIgnoreCase))
                ViewBag.IsLevelOne = true;
            return PartialView("_FeedReplies", replies);
        }
          [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public async Task<ActionResult> ShareNewsToGroup(NewsViewModel model, string q)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(q))
            {
                if (model.PostImage != null)
                {
                    bool isPng = false;
                    byte[] Array = new byte[model.PostImage.ContentLength];
                    model.PostImage.InputStream.Read(Array, 0, Array.Length);
                    var resultFile = ImageProcessor.ValidateImage(Array, model.PostImage.FileName);
                    model.DraftID = Guid.NewGuid().ToString();
                    if (resultFile.IsValidImage)
                    {
                        Array = ImageProcessor.ResizeFile(Array, 900, ref isPng);
                        resultFile = ImageProcessor.UploadImage(Array, Enums.ImageObject.News, string.Empty, true, Enums.FileType.Image, model.DraftID + "__news__", AppConfig.StorageProvider, AppConfig.SiteUrl);
                        model.FilePath = resultFile.FileName;
                    }
                }
                var result = await Press.ShareNewsGroup(model,q);
                if (result.Type == Enums.ResultType.Success)
                {
                    var feeds = await Press.Feed((long)result.ReferenceID);
                    return PartialView("_Feeds", feeds);
                }
                return null;
            }
            return PartialView("_ShareUpdate", model);
        }
        

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public async Task<ActionResult> ShareNews(NewsViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PostImage != null)
                {
                    bool isPng = false;
                    byte[] Array = new byte[model.PostImage.ContentLength];
                    model.PostImage.InputStream.Read(Array, 0, Array.Length);
                    var resultFile = ImageProcessor.ValidateImage(Array, model.PostImage.FileName);
                    model.DraftID = Guid.NewGuid().ToString();
                    if (resultFile.IsValidImage)
                    {
                        Array = ImageProcessor.ResizeFile(Array, 900, ref isPng);
                        resultFile = ImageProcessor.UploadImage(Array, Enums.ImageObject.News, string.Empty, true, Enums.FileType.Image, model.DraftID + "__news__", AppConfig.StorageProvider, AppConfig.SiteUrl);
                        model.FilePath = resultFile.FileName;
                    }
                }
                var result = await Press.ShareNews(model);
                if (result.Type == Enums.ResultType.Success)
                {
                    var feeds = await Press.Feed((long)result.ReferenceID);
                    return PartialView("_Feeds", feeds);
                }
                return null;
            }
            return PartialView("_ShareUpdate", model);
        }

        [Authorize]
        public async Task<ActionResult> ReShare(string q)
        {
            var model = await Press.GetNewsForReshare(q);
            return PartialView("_ReshareFeed", model);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        [HttpPost]
        public async Task<ActionResult> ReShare(NewsViewModel model)
        {
            var result = await Press.Reshare(model);
            if (result.Type == Enums.ResultType.Success)
            {
                var feeds = await Press.Feed((long)result.ReferenceID);
                return PartialView("_Feeds", feeds);
            }
            return null;
        }

        [Authorize]
        public async Task<ActionResult> React(string q, string reactType)
        {
            var result = await Press.React(q, reactType, string.Empty);
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }        

        [Authorize]
        public async Task<ActionResult> apply(string q, string reactType)
        {
            var model = await Press.GetNewsForReshare(q);
            return PartialView("_Apply", model);
        }

        [Authorize]
        public async Task<ActionResult> Refer(string q, string reactType)
        {
            var model = new RerefOpportunityViewModel()
            {
                Opportunity = await Press.GetNewsForReshare(q)
            };
            return PartialView("_Refer", model);
        }

        [CompressFilter]
        public async Task<ActionResult> Insights(string q, string reactType)
        {
            var model = new FeedDetailViewModel()
            {
                Insgihts = await Press.Insights(q, reactType),
                Hash = q
            };
            return PartialView("_Insights", model);
        }


        [CompressFilter]
        public async Task<ActionResult> insightdetails(string q, string type, bool separateWindow = false)
        {
            var insightDetails = await Press.InsightDetails(q, type);
            if (separateWindow)
                return PartialView("_InsightDetailsPopup", insightDetails);
            return PartialView("_InsightDetails", insightDetails);
        }


        [Authorize]
        public async Task<ActionResult> ReferOpportunity(string q, string reactType)
        {
            var model = new RerefOpportunityViewModel()
            {
                Opportunity = await Press.GetNewsForReshare(q)
            };
            return PartialView("ReferOpportunity", model);
        }




        [ValidateAntiForgeryToken]
        [Authorize]
        [ValidateInput(true)]
        [HttpPost]
        public async Task<ActionResult> Refer(RerefOpportunityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Press.ReferOpportunity(model);
                return WJson(result);
            }
            return WJson(new Result() { Type = Enums.ResultType.Warning, Description = "One or more required values are supplied incorrectly." });
        }



        [ValidateAntiForgeryToken]
        [Authorize]
        [ValidateInput(true)]
        [HttpPost]
        public async Task<ActionResult> ReferOpportunity(RerefOpportunityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Press.ReferForAnOpportunity(model);
                return WJson(result);
            }
            return WJson(new Result() { Type = Enums.ResultType.Warning, Description = "One or more required values are supplied incorrectly." });
        }


        [ValidateAntiForgeryToken]
        [Authorize]
        [ValidateInput(true)]
        [HttpPost]
        public async Task<ActionResult> apply(ApplyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Press.ApplyOpportunity(model);
                return WJson(result);
            }
            return WJson(new Result() { Type = Enums.ResultType.Warning, Description = "One or more required values are supplied incorrectly." });
        }

        [Authorize]
        public async Task<ActionResult> ReactReply(string q, string reactType)
        {
            var result = await Press.ReactReply(q, reactType);
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        public async Task<ActionResult> MoreOptions(string q)
        {
            var options = await Press.MoreOptionsPost(q);
            return PartialView("_FeedMoreOptions", options);
        }

        [Authorize]
        public async Task<ActionResult> Report(string q)
        {
            var options = await Press.Report(q);
            return PartialView("_ReportFeed", options);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Report(string hash, string description)
        {
            var result = await Press.React(hash, "report", description);
            return PartialView("_ActionResultMessage", result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> WriteBig(PostViewModel model)
        {
            model.PostType = (int)Enums.PostTypes.Blog;
            var result = await Press.Create(model, null);
            return new JsonResult() { Data = result };
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [CompressFilter]
        public async Task<ActionResult> Updates(long ticks, Enums.FeedType type, int pageNumber = 0)
        {
            var dateTime = new DateTime(ticks);
            var result = await Press.Feeds(pageNumber, dateTime, type);
            if (result.Feeds != null && result.Feeds.Count > 0)
                return PartialView("_Feeds", result.Feeds);
            return null;
        }


        public async Task<ActionResult> Publishing()
        {
            var model = await Press.GetArticles(0,10);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AskOpportunity(AskOpportunityViewModel model)
        {
            // For self
            if (model.OpportunitySource == 1)
            {
                ModelState.Remove("ConnectionName");
                ModelState.Remove("ConnectionID");
            }
            if (ModelState.IsValid)
            {
                var result = await Press.AskOpportunity(model);
                return WJson(result);
            }
            return WJson(new Result() { Type = Enums.ResultType.Warning, Description = "One or more required values are supplied incorrectly." });
        }

        [ValidateAntiForgeryToken]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ShareOpportunity(ShareOpportunityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Press.ShareOpportunity(model);
                return WJson(result);
            }
            return WJson(new Result() { Type = Enums.ResultType.Warning, Description = "One or more required values are supplied incorrectly." });
        }


        [Authorize]
        public async Task<ActionResult> Sharer(string url, bool? isSharer, int? mode)
        {
            if (mode == (int)Enums.SocialSharingContentType.Opportunity)
            {
                ViewBag.Title = "We have posted an opportunity here, please visit and apply.";
            }
            ViewBag.Url = url;
            ViewBag.Shared = isSharer;
            return PartialView("_Sharer");
        }

        [Authorize]
        public async Task<ActionResult> Track(string trackType, string[] posts)
        {
            var result = await Press.Track(trackType, posts);
            return WJson(new { trackType = trackType, posts = posts, recorededOn = DateTime.UtcNow, success = result });
        }    

    }
}
