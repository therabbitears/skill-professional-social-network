using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Data.Repositories.Signatures;
using Wrly.Data.Repositories.Implementors;
using Wrly.Models;
using Wrly.infrastuctures.Utils;
using Types;
using Wrly.Data.Models;
using AutoMapper;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wrly.Models.Share;
using Wrly.Models.Feeds;
using System.Collections;
using Wrly.Notifications.Processors.Implementors;
using Wrly.Models.Listing;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class PressProcessor : BaseProcessor, IPressProcessor
    {
        IAccountRepository _accountRepository;

        public IAccountRepository AccountRepository
        {
            get
            {
                if (_accountRepository == null)
                {
                    _accountRepository = new AccountRepository();
                }
                return _accountRepository;
            }
            set
            {
                _accountRepository = value;
            }
        }


        public async Task<long> Create(PostViewModel model, byte[] arr)
        {
            if (!string.IsNullOrEmpty(model.DraftID))
            {
                var existing = await GetFromDraft(model.DraftID);
                model.ID = existing.ID;
            }

            var post = Mapper.Map<Post>(model);

            post.PostTags = new EntityCollection<PostTag>();
            if (model.TagIDList != null && model.TagIDList.Count() > 0)
            {
                foreach (var item in model.TagIDList)
                {
                    post.PostTags.Add(new PostTag() { TagID = item });
                }
            }

            if (string.IsNullOrEmpty(model.UrlSlug))
            {
                if (model.Title.Length > 120)
                    post.UrlSlug = model.Title;
                else
                    post.UrlSlug = model.Title;

                post.UrlSlug = System.Text.RegularExpressions.Regex.Replace(post.UrlSlug, "[^a-zA-Z0-9_-]+", "-", System.Text.RegularExpressions.RegexOptions.Compiled);
            }
            post.IPAddress = IpAddress;
            post.EntityID = UserHashObject.EntityID;
            post.WritterID = UserHashObject.EntityID;
            post.SharingType = (int)Enums.SharingType.Share;
            using (var Repository = new PressRepository())
            {
                await Repository.Save(post);
                return 1;
            }
        }

        private List<PostViewModel> ConvertToPost(System.Data.DataSet dataSet)
        {
            var posts = dataSet.Tables[0].FromDataTable<PostViewModel>();
            foreach (var item in posts)
            {
                item.PostTags = dataSet.Tables[1].FromDataTable<PostTagViewModel>("postID = " + item.ID.ToString());
            }
            return posts;
        }

        private List<PostViewModel> ConvertToPostWithTags(System.Data.DataSet dataSet)
        {
            var posts = dataSet.Tables[0].FromDataTable<PostViewModel>();
            foreach (var item in posts)
            {
                item.Replies = dataSet.Tables[1].FromDataTable<ReplyViewModel>("postID = " + item.ID.ToString());
                item.PostTags = dataSet.Tables[2].FromDataTable<PostTagViewModel>("postID = " + item.ID.ToString());
            }
            return posts;
        }


        public async Task<FeedDetailViewModel> GetSingle(long id)
        {
            using (var Repository = new PressRepository())
            {
                var dataSetSingleQuestion = await Repository.GetDetails(id, UserHashObject.EntityID);
                var post = dataSetSingleQuestion.Tables[0].FromDataTable<FeedDetailViewModel>().FirstOrDefault();
                var dataTable = new Hashtable();
                dataTable.Add("EntityID", UserHashObject.EntityID);
                dataTable.Add("PostID", post.ID);
                dataTable.Add("Writter", post.EntityID);
                dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                dataTable.Add("PostType", "Feed");
                post.Hash = QueryStringHelper.Encrypt(dataTable);
                post.Replies = dataSetSingleQuestion.Tables[1].FromDataTable<ReplyViewModel>();
                foreach (var reply in post.Replies)
                {
                    dataTable = new Hashtable();
                    dataTable.Add("EntityID", UserHashObject.EntityID);
                    dataTable.Add("PostID", post.ID);
                    dataTable.Add("Writter", post.EntityID);
                    dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                    dataTable.Add("ReplyID", reply.ID);
                    reply.Hash = QueryStringHelper.Encrypt(dataTable);
                }
                return post;
            }
        }

        public async Task<PostViewModel> GetFromDraft(string id)
        {
            using (var Repository = new PressRepository())
            {
                var dataSetSingleQuestion = await Repository.FromDraft(id);
                if (dataSetSingleQuestion.Tables[0].Rows.Count > 0)
                {
                    var post = dataSetSingleQuestion.Tables[0].FromDataTable<PostViewModel>().FirstOrDefault();
                    return post;
                }
                return new PostViewModel() { DraftID = id };
            }
        }


        public List<ReplyViewModel> GetRepliesForPost(long? postID)
        {
            List<ReplyViewModel> repliesModel = new List<ReplyViewModel>();

            using (var Repository = new PressRepository())
            {
                var dsReplies = Repository.GetRepliesForPost(postID);

                var replies = dsReplies.Tables[0].FromDataTable<ReplyViewModel>();
                return replies;
            }
        }


        public async Task<long> InsertNewReply(PostViewModel model)
        {
            PostReply reply = Mapper.Map<PostReply>(model);
            using (var Repository = new PressRepository())
            {
                var result = await Repository.InsertReply(reply);
                return 0;
            }
        }


        public List<PostViewModel> GetUserAskedQuestions(string userName)
        {
            using (var Repository = new PressRepository())
            {
                var dataSet = Repository.GetUserAskedQuestions(userName);
                var posts = ConvertToPost(dataSet);
                return posts;
            }
        }

        public List<PostViewModel> GetUserAnsweredQuestions(string userName)
        {
            using (var Repository = new PressRepository())
            {
                var dataSet = Repository.GetUserAnsweredQuestions(userName);
                var posts = ConvertToPostWithTags(dataSet);
                return posts;
            }
        }

        public async Task<PostViewModel> GetSingle(string url)
        {
            using (var Repository = new PressRepository())
            {
                var dataSetSingleQuestion = await Repository.GetDetails(url);
                var post = dataSetSingleQuestion.Tables[0].FromDataTable<PostViewModel>().FirstOrDefault();
                post.Replies = dataSetSingleQuestion.Tables[1].FromDataTable<ReplyViewModel>();
                post.PostTags = dataSetSingleQuestion.Tables[2].FromDataTable<PostTagViewModel>();
                return post;
            }
        }


        public int Statistics(long id, string action, string en)
        {
            var statistics = new PostInteraction();
            //if (en.Equals("q", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    statistics.PostID = id;
            //}
            //if (en.Equals("r", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    statistics.ReplyID = id;
            //}

            //if (action.Equals("upvote", StringComparison.InvariantCultureIgnoreCase))
            //    statistics.StatisticsType = (int)Enums.ObjectStatisticType.Upvote;

            //if (action.Equals("downvote", StringComparison.InvariantCultureIgnoreCase))
            //    statistics.StatisticsType = (int)Enums.ObjectStatisticType.Downvote;

            //if (action.Equals("accept-answer", StringComparison.InvariantCultureIgnoreCase))
            //    statistics.StatisticsType = (int)Enums.ObjectStatisticType.AcceptAnswer;

            //if (action.Equals("visit", StringComparison.InvariantCultureIgnoreCase))
            //    statistics.StatisticsType = (int)Enums.ObjectStatisticType.Visit;

            //if (action.Equals("bookmark", StringComparison.InvariantCultureIgnoreCase))
            //    statistics.StatisticsType = (int)Enums.ObjectStatisticType.BookMark;

            //if (action.Equals("follow", StringComparison.InvariantCultureIgnoreCase))
            //    statistics.StatisticsType = (int)Enums.ObjectStatisticType.Follow;

            //if (HttpContext.Current.Request.IsAuthenticated)
            //    statistics.UserID = HttpContext.Current.User.Identity.Name;
            using (var Repository = new PressRepository())
            {
                var result = Repository.RecordStatistics(statistics);
                return result;
            }
        }


        public PostOptionsViewModel GetQuestionOptions(int id)
        {
            using (var Repository = new PressRepository())
            {
                var user = HttpContext.Current.User.Identity.Name;
                var dataSet = Repository.GetQuestionOptions(id, user);
                return dataSet.Tables[0].FromDataTable<PostOptionsViewModel>().FirstOrDefault();
            }
        }


        public async System.Threading.Tasks.Task<Result> ShareNews(NewsViewModel model)
        {
            var post = Mapper.Map<Post>(model);
            post = model.UserHash.ToObject<Post>(post);
            post.IPAddress = IpAddress;
            post.Published = true;
            post.PublishOn = model.CreatedOn;
            post.SharingType = (byte)Enums.SharingType.Share;
            post.Status = (byte)Enums.PostStatus.Published;
            post.WritterID = UserHashObject.EntityID;
            var regex = new Regex(@"(?<=#)\w");
            var tags = regex.Matches(model.Text);
            if (tags != null && tags.Count > 0)
            {
                post.PostTags = new EntityCollection<PostTag>();
                foreach (Match item in tags)
                {
                    post.PostTags.Add(new PostTag() { Tag = new Data.Models.Tag() { Name = item.Value } });
                }
            }

            using (var Repository = new PressRepository())
            {
                var storyID = await Repository.Save(post);
                if (storyID > 0)
                {
                    return new Result() { ReferenceID = storyID, Description = "News has been posted to your timeline.", Type = Enums.ResultType.Success };
                }
                return new Result() { Description = "We cannot reach to your request, please have one more try.", Type = Enums.ResultType.Error };
            }
        }

        public async System.Threading.Tasks.Task<Result> ShareNewsGroup(NewsViewModel model, string q)
        {
            var post = Mapper.Map<Post>(model);
            post = model.UserHash.ToObject<Post>(post);
            post.IPAddress = IpAddress;
            post.Published = true;
            post.PublishOn = model.CreatedOn;
            post.SharingType = (byte)Enums.SharingType.Share;
            post.Status = (byte)Enums.PostStatus.Published;
            post.WritterID = UserHashObject.EntityID;
            post.GroupEntityID = Convert.ToInt64(q.GetSingleValue("EntityID"));
            var regex = new Regex(@"(?<=#)\w");
            var tags = regex.Matches(model.Text);

            if (model.IsDiscussion)
            {
                post.PostType = (byte)Enums.PostTypes.Discussion;
            }

            if (tags != null && tags.Count > 0)
            {
                post.PostTags = new EntityCollection<PostTag>();
                foreach (Match item in tags)
                {
                    post.PostTags.Add(new PostTag() { Tag = new Data.Models.Tag() { Name = item.Value } });
                }
            }

            using (var Repository = new PressRepository())
            {
                var storyID = await Repository.Save(post);
                if (storyID > 0)
                {
                    return new Result() { ReferenceID = storyID, Description = "News has been posted to your timeline.", Type = Enums.ResultType.Success };
                }
                return new Result() { Description = "We cannot reach to your request, please have one more try.", Type = Enums.ResultType.Error };
            }
        }


        public async Task<Result> AskOpportunity(AskOpportunityViewModel model)
        {
            var post = Mapper.Map<Post>(model);
            post.IPAddress = IpAddress;
            post.Published = true;
            post.PublishOn = Now;
            post.SharingType = (byte)Enums.SharingType.Share;
            post.Status = (byte)Enums.PostStatus.Published;

            post.EntityID = UserHashObject.EntityID;
            if (!string.IsNullOrEmpty(model.GroupHash))
            {
                post.GroupEntityID = Convert.ToInt64(model.GroupHash.GetSingleValue("EntityID"));
            }
            // Asking for self
            if (model.OpportunitySource == 1)
            {
                post.WritterID = UserHashObject.EntityID;
            }
            // Asking for a connection
            else
            {
                post.WritterID = model.ConnectionID;
            }

            if (model.Skills != null && model.Skills.Count > 0)
            {
                post.Skills = new EntityCollection<Skill>();
                foreach (var item in model.Skills)
                {
                    post.Skills.Add(new Skill() { SkillID = item });
                }
            }

            if (model.JobTitles != null && model.JobTitles.Count > 0)
            {
                post.JobTitles = new EntityCollection<JobTitle>();
                foreach (var item in model.JobTitles)
                {
                    post.JobTitles.Add(new JobTitle() { JobTitleID = item });
                }
            }

            using (var Repository = new PressRepository())
            {
                var storyID = await Repository.Save(post);
                if (storyID > 0)
                {
                    return new Result() { ReferenceID = storyID, Description = "The opportunity has been broadcasted with your network.", Type = Enums.ResultType.Success };
                }
                return new Result() { Description = "We cannot reach to your request, please have one more try.", Type = Enums.ResultType.Error };
            }
        }

        public async System.Threading.Tasks.Task<Result> AddComment(CommentViewModel model)
        {
            var reply = Mapper.Map<PostReply>(model);
            reply = model.Hash.ToObject<PostReply>(reply);
            reply.EntityID = UserHashObject.EntityID;
            var writterID = model.Hash.GetSingleValue("OriginalWritterID");
            string url = string.Format("/press/posts/{0}", reply.PostID);
            var notitificationType = Enums.NotificationType.PostComment;
            if (reply.PostType == (byte)Enums.PostTypes.ShareOpportunity || reply.PostType == (byte)Enums.PostTypes.AskOpportunity)
            {
                reply.ReplyType = (byte)Enums.ReplyType.ResponseOnOpportunity;
                url = string.Format("/opportunities/{0}", reply.PostID);
                notitificationType = Enums.NotificationType.OpportunityResponse;
            }

            using (var Repository = new PressRepository())
            {
                var result = await Repository.InsertReply(reply);
                if (result >= 0)
                {
                    if ((reply.ReplyID == null || reply.ReplyID == 0) && Convert.ToInt64(writterID) != reply.EntityID)
                    {
                        if (EntityHash.ContainsKey("Url"))
                            EntityHash.Remove("Url");
                        EntityHash.Add("Url", url);
                        var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                        await notificationProcessor.AddNotification(notitificationType, reply.PostID, null, Convert.ToInt64(writterID));
                    }
                    return new Result() { Type = Enums.ResultType.Success, ReferenceID = result };
                }
                return new Result() { Type = Enums.ResultType.Error, ReferenceID = -1 };
            }
        }


        public async Task<ReplyViewModel> GetSingleReply(long replyID)
        {
            using (var Repository = new PressRepository())
            {
                var result = await Repository.SingleReply(replyID);
                var reply = result.Tables[0].FromDataTable<ReplyViewModel>().FirstOrDefault();
                var dataTable = new Hashtable();
                dataTable.Add("EntityID", UserHashObject.EntityID);
                dataTable.Add("PostID", reply.PostID);
                dataTable.Add("Writter", reply.EntityID);
                dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                dataTable.Add("ReplyID", reply.ID);
                reply.Hash = QueryStringHelper.Encrypt(dataTable);
                return reply;
            }
        }


        public async Task<Models.Feeds.FeedDetailViewModel> GetNewsForReshare(string q)
        {
            var newsViewModel = q.ToObject<FeedDetailViewModel>(null);
            using (var Repository = new PressRepository())
            {
                var postID = newsViewModel.ParentPostID > 0 ? newsViewModel.ParentPostID : newsViewModel.PostID;
                var result = await Repository.Single(UserHashObject.EntityID, Convert.ToInt64(postID));
                newsViewModel = result.Tables[0].FromDataTable<FeedDetailViewModel>().FirstOrDefault();
                if (result.Tables.Count > 2)
                {
                    newsViewModel.Skills = result.Tables[2].FromDataTable<SkillViewModel>("PostID=" + newsViewModel.ID);
                    newsViewModel.JobTitles = result.Tables[3].FromDataTable<CareerHistoryViewModel>("PostID=" + newsViewModel.ID);
                }
                newsViewModel.Hash = q;
            }
            return newsViewModel;
        }

        public async Task<List<FeedDetailViewModel>> ReshareMap(long postID)
        {
            using (var repository = new PressRepository())
            {
                using (var feedDataset = await repository.ReshareMap(UserHashObject.EntityID, postID))
                {
                    var feeds = feedDataset.Tables[0].FromDataTable<FeedDetailViewModel>();
                    foreach (var feed in feeds)
                    {
                        var dataTable = new Hashtable();

                        dataTable.Add("EntityID", UserHashObject.EntityID);
                        dataTable.Add("PostID", feed.ID);
                        dataTable.Add("ParentPostID", feed.PostID > 0 ? feed.PostID : 0);
                        dataTable.Add("PostType", feed.PostType);
                        dataTable.Add("OriginalWritterID", feed.EntityID);
                        dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                        dataTable.Add("FeedType", "Feed");

                        feed.Hash = QueryStringHelper.Encrypt(dataTable);
                        feed.Replies = feedDataset.Tables[1].FromDataTable<ReplyViewModel>("PostID=" + feed.ID);
                        foreach (var reply in feed.Replies)
                        {
                            dataTable = new Hashtable();
                            dataTable.Add("EntityID", UserHashObject.EntityID);
                            dataTable.Add("PostID", feed.ID);
                            dataTable.Add("Writter", feed.EntityID);
                            dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                            dataTable.Add("ReplyID", reply.ID);
                            reply.Hash = QueryStringHelper.Encrypt(dataTable);
                        }
                    }
                    return feeds;
                }
            }
        }

        public async Task<FeedDetailViewModel> Detail(long postID)
        {
            var entityID = UserHashObject != null ? UserHashObject.EntityID : 0;
            using (var repository = new PressRepository())
            {
                using (var feeds = await repository.Single(entityID, postID))
                {
                    var feed = feeds.Tables[0].FromDataTable<FeedDetailViewModel>().FirstOrDefault();
                    var dataTable = new Hashtable();
                    dataTable.Add("EntityID", entityID);
                    dataTable.Add("PostID", feed.ID);
                    dataTable.Add("ParentPostID", feed.ParentPostID ?? 0);
                    dataTable.Add("OriginalWritterID", feed.EntityID);
                    dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                    dataTable.Add("PostType", "Feed");
                    feed.Hash = QueryStringHelper.Encrypt(dataTable);
                    feed.Replies = feeds.Tables[1].FromDataTable<ReplyViewModel>("PostID=" + feed.ID);
                    foreach (var reply in feed.Replies)
                    {
                        dataTable = new Hashtable();
                        dataTable.Add("EntityID", entityID);
                        dataTable.Add("PostID", feed.ID);
                        dataTable.Add("Writter", feed.EntityID);
                        dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                        dataTable.Add("ReplyID", reply.ID);
                        reply.Hash = QueryStringHelper.Encrypt(dataTable);
                    }
                    if (feeds.Tables.Count > 2)
                    {
                        feed.Skills = feeds.Tables[2].FromDataTable<SkillViewModel>("PostID=" + feed.ID);
                        feed.JobTitles = feeds.Tables[3].FromDataTable<CareerHistoryViewModel>("PostID=" + feed.ID);
                    }
                    return feed;
                }
            }
        }

        public async Task<HomeFeedViewModel> Feeds(int pageNumber, DateTime? stamp, Types.Enums.FeedType type)
        {
            stamp = stamp ?? DateTime.UtcNow;
            var model = new HomeFeedViewModel();
            using (var repository = new PressRepository())
            {
                DataSet feeds = null;
                if (type == Enums.FeedType.Default)
                {
                    using (feeds = await repository.Feeds(UserHashObject.EntityID, pageNumber, 10, stamp))
                    {
                        model.Feeds = feeds.Tables[0].FromDataTable<FeedDetailViewModel>();
                        CreateFeedModel(model, feeds);
                        model.LoadedOn = Tickes;
                    }
                }
                else if (type == Enums.FeedType.MyUpdates)
                {
                    using (feeds = await repository.MyUpdateFeeds(UserHashObject.EntityID, pageNumber, 10, stamp))
                    {
                        model.Feeds = feeds.Tables[0].FromDataTable<FeedDetailViewModel>();
                        CreateFeedModel(model, feeds);
                        model.LoadedOn = Tickes;
                    }
                }
                else if (type == Enums.FeedType.Saved)
                {
                    using (feeds = await repository.MySavedFeeds(UserHashObject.EntityID, pageNumber, 10, stamp))
                    {
                        model.Feeds = feeds.Tables[0].FromDataTable<FeedDetailViewModel>();
                        CreateFeedModel(model, feeds);
                        model.LoadedOn = Tickes;
                    }
                }
            }
            return model;
        }

        private void CreateFeedModel(HomeFeedViewModel model, DataSet feeds)
        {
            var entityID = UserHashObject == null ? 0 : UserHashObject.EntityID;
            foreach (var feed in model.Feeds)
            {
                var dataTable = new Hashtable();
                dataTable.Add("EntityID", entityID);
                dataTable.Add("PostID", feed.ID);
                dataTable.Add("ParentPostID", feed.PostID > 0 ? feed.PostID : 0);
                dataTable.Add("PostType", feed.PostType);
                dataTable.Add("OriginalWritterID", feed.EntityID);
                dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                dataTable.Add("FeedType", "Feed");

                feed.Hash = QueryStringHelper.Encrypt(dataTable);
                feed.Replies = feeds.Tables[1].FromDataTable<ReplyViewModel>("PostID=" + feed.ID);
                if (feeds.Tables.Count > 2)
                {
                    feed.Skills = feeds.Tables[2].FromDataTable<SkillViewModel>("PostID=" + feed.ID);
                    feed.JobTitles = feeds.Tables[3].FromDataTable<CareerHistoryViewModel>("PostID=" + feed.ID);
                }

                foreach (var reply in feed.Replies)
                {
                    dataTable = new Hashtable();
                    dataTable.Add("EntityID", entityID);
                    dataTable.Add("PostID", feed.ID);
                    dataTable.Add("PostType", feed.PostType);
                    dataTable.Add("Writter", feed.EntityID);
                    dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                    dataTable.Add("ReplyID", reply.ID);
                    reply.Hash = QueryStringHelper.Encrypt(dataTable);
                }
            }
        }


        public async Task<Result> Reshare(NewsViewModel model)
        {
            FeedDetailViewModel temp = model.Hash.ToObject<FeedDetailViewModel>(null);
            var postID = temp.ParentPostID > 0 ? temp.ParentPostID : temp.PostID;

            var post = Mapper.Map<Post>(model);
            post = model.UserHash.ToObject<Post>(post);
            post.PostID = postID;
            post.WritterID = temp.EntityID;
            post.PostType = temp.PostType;
            post.SharingType = (byte)Enums.SharingType.Reshare;
            post.IPAddress = IpAddress;
            post.Published = true;
            post.PublishOn = model.CreatedOn;
            var regex = new Regex(@"(?<=#)\w");
            var tags = regex.Matches(model.Text);
            if (tags != null && tags.Count > 0)
            {
                post.PostTags = new EntityCollection<PostTag>();
                foreach (Match item in tags)
                {
                    post.PostTags.Add(new PostTag() { Tag = new Data.Models.Tag() { Name = item.Value } });
                }
            }

            using (var Repository = new PressRepository())
            {
                var authorID = temp.OriginalWritterID > 0 ? temp.OriginalWritterID : temp.EntityID;
                var storyID = await Repository.Save(post);
                if (storyID > 0)
                {
                    string url = string.Format("/press/reshares/{0}", temp.PostID);
                    if (EntityHash.ContainsKey("Url"))
                        EntityHash.Remove("Url");
                    EntityHash.Add("Url", url);
                    var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                    await notificationProcessor.AddNotification(Enums.NotificationType.Reshare, postID, null, authorID);

                    return new Result() { ReferenceID = storyID, Description = "News has been posted to your timeline.", Type = Enums.ResultType.Success };
                }
                return new Result() { Description = "We cannot reach to your request, please have one more try.", Type = Enums.ResultType.Error };
            }
        }


        public async Task<Result> React(string q, string reactType, string description, long? entityID2 = default(long?))
        {
            var feedModel = q.ToObject<FeedDetailViewModel>(null);
            var interaction = Mapper.Map<PostInteraction>(feedModel);
            interaction.EntityID = UserHashObject.EntityID;
            interaction.IpAddress = IpAddress;
            interaction.Description = description;
            interaction.EntityID2 = entityID2;

            switch (reactType.ToLower())
            {
                case "like":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.Like;
                    break;
                case "unlike":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.Unlike;
                    break;
                case "save-fav":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.Save;
                    break;
                case "remove-fav":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.RemoveFavorite;
                    break;
                case "report":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.Report;
                    break;
                case "remove":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.Remove;
                    break;
                case "unremove":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.UnRemove;
                    break;
                case "hide":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.Hide;
                    break;
                case "unhide":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.Unhide;
                    break;
                case "apply":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.Apply;
                    break;
                case "refer":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.Refer;
                    break;
                case "referopportunity":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.ReferForOpportunity;
                    break;

                default:
                    break;
            }
            using (var Repository = new PressRepository())
            {
                var result = await Repository.React(interaction);
                switch (reactType.ToLower())
                {
                    case "like":
                        if (result == 1)
                        {
                            if (feedModel.OriginalWritterID != interaction.EntityID)
                            {
                                string url = string.Format("/press/posts/{0}", interaction.PostID);
                                if (EntityHash.ContainsKey("Url"))
                                    EntityHash.Remove("Url");
                                EntityHash.Add("Url", url);
                                var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                                await notificationProcessor.AddNotification(Enums.NotificationType.PostLike, interaction.PostID, null, feedModel.OriginalWritterID);
                            }
                            var likes = await Repository.GetLikes(interaction.PostID, (byte)Enums.PostInteractionType.Like);
                            return new Result { Type = Enums.ResultType.Success, Description = "Your action has been recorded.", ReferenceID = likes };
                        }
                        break;
                    case "apply":
                        if (result == 1)
                        {
                            string url = string.Format("/opportunities/{0}", interaction.PostID);
                            if (EntityHash.ContainsKey("Url"))
                                EntityHash.Remove("Url");
                            EntityHash.Add("Url", url);
                            var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                            await notificationProcessor.AddNotification(Enums.NotificationType.OpportunityApplied, interaction.PostID, null, feedModel.OriginalWritterID);
                            var singlePost = await GetOne((long)interaction.PostID);
                            // Sending email.
                            using (var repository = new AccountRepository())
                            {

                                using (var employer = await repository.HoverCard(feedModel.OriginalWritterID.ToString(), UserHashObject.EntityID))
                                {
                                    var employerCard = employer.Tables[0].FromDataTable<EntityProfileViewModel>().FirstOrDefault();
                                    EntityProfileViewModel currentCard = null;
                                    using (var candidate = await repository.HoverCard(UserHashObject.EntityID.ToString(), UserHashObject.EntityID))
                                    {
                                        currentCard = candidate.Tables[0].FromDataTable<EntityProfileViewModel>().FirstOrDefault();
                                    }

                                    string nameTitle = employerCard.EntityType == (int)Enums.EntityTypes.Person ? employerCard.FormatedName : employerCard.Name;
                                    var hashTable = new Hashtable();
                                    hashTable.Add("**CurrentName**", currentCard.FormatedName);
                                    hashTable.Add("**CurrentHeading**", currentCard.ProfileHeading);
                                    hashTable.Add("**Title**", singlePost.Title);
                                    hashTable.Add("**Name**", nameTitle);
                                    hashTable.Add("**Url**", string.Format("/opportunities/{0}", interaction.PostID));
                                    await NotificationProcessor.SendEmail(hashTable, Enums.EmailFilePaths.AppliedAnOpportunity, employerCard.EmailAddress, true);
                                }
                            }
                            return new Result { Type = Enums.ResultType.Success, Description = "Your action has been recorded." };
                        }
                        break;
                    case "refer":
                        if (result == 1)
                        {
                            string url = string.Format("/opportunities/{0}", interaction.PostID);
                            if (EntityHash.ContainsKey("Url"))
                                EntityHash.Remove("Url");
                            EntityHash.Add("Url", url);
                            var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                            await notificationProcessor.AddNotification(Enums.NotificationType.OpportunityReference, interaction.PostID, null, feedModel.OriginalWritterID);
                            var likes = await Repository.GetLikes(interaction.PostID, (byte)Enums.PostInteractionType.Refer);
                            var singlePost = await GetOne((long)interaction.PostID);
                            // Sending email.
                            using (var repository = new AccountRepository())
                            {
                                using (var employer = await repository.HoverCard(singlePost.EntityID.ToString(), UserHashObject.EntityID))
                                {
                                    var employerCard = employer.Tables[0].FromDataTable<EntityProfileViewModel>().FirstOrDefault();
                                    EntityProfileViewModel candidateCard = null;
                                    using (var candidate = await repository.HoverCard(entityID2.ToString(), UserHashObject.EntityID))
                                    {
                                        candidateCard = candidate.Tables[0].FromDataTable<EntityProfileViewModel>().FirstOrDefault();
                                    }

                                    EntityProfileViewModel currentCard = null;
                                    using (var candidate = await repository.HoverCard(UserHashObject.EntityID.ToString(), UserHashObject.EntityID))
                                    {
                                        currentCard = candidate.Tables[0].FromDataTable<EntityProfileViewModel>().FirstOrDefault();
                                    }
                                    string nameTitle = employerCard.EntityType == (int)Enums.EntityTypes.Person ? employerCard.FormatedName : employerCard.Name;
                                    var hashTable = new Hashtable();
                                    hashTable.Add("**CurrentName**", currentCard.FormatedName);
                                    hashTable.Add("**CurrentHeading**", currentCard.ProfileHeading);
                                    hashTable.Add("**Name**", nameTitle);
                                    hashTable.Add("**Name2**", candidateCard.FormatedName);
                                    hashTable.Add("**Title**", singlePost.Title);
                                    hashTable.Add("**Heading2**", candidateCard.ProfileHeading);
                                    hashTable.Add("**Url**", string.Format("/opportunities/{0}", interaction.PostID));
                                    await NotificationProcessor.SendEmail(hashTable, Enums.EmailFilePaths.ReferedOnThierOpportunity, employerCard.EmailAddress, true);
                                }
                            }

                            return new Result { Type = Enums.ResultType.Success, Description = "Your action has been recorded.", ReferenceID = likes };
                        }
                        break;
                    case "referopportunity":
                        if (result == 1)
                        {
                            // Refer to a connection
                            string url = string.Format("/opportunities/{0}", interaction.PostID);
                            if (EntityHash.ContainsKey("Url"))
                                EntityHash.Remove("Url");
                            EntityHash.Add("Url", url);
                            var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                            await notificationProcessor.AddNotification(Enums.NotificationType.ReferenceForAnOpportunity, interaction.PostID, null, (long)entityID2);
                            var singlePost = await GetOne((long)interaction.PostID);
                            // Sending email.
                            using (var repository = new AccountRepository())
                            {
                                using (var employer = await repository.HoverCard(entityID2.ToString(), UserHashObject.EntityID))
                                {
                                    var employerCard = employer.Tables[0].FromDataTable<EntityProfileViewModel>().FirstOrDefault();
                                    EntityProfileViewModel candidateCard = null;
                                    using (var candidate = await repository.HoverCard(singlePost.EntityID.ToString(), UserHashObject.EntityID))
                                    {
                                        candidateCard = candidate.Tables[0].FromDataTable<EntityProfileViewModel>().FirstOrDefault();
                                    }

                                    EntityProfileViewModel currentCard = null;
                                    using (var candidate = await repository.HoverCard(UserHashObject.EntityID.ToString(), UserHashObject.EntityID))
                                    {
                                        currentCard = candidate.Tables[0].FromDataTable<EntityProfileViewModel>().FirstOrDefault();
                                    }

                                    var hashTable = new Hashtable();
                                    hashTable.Add("**CurrentName**", currentCard.FormatedName);
                                    hashTable.Add("**CurrentHeading**", currentCard.ProfileHeading);
                                    hashTable.Add("**Name**", employerCard.FormatedName);
                                    hashTable.Add("**Name2**", candidateCard.FormatedName);
                                    hashTable.Add("**Heading2**", candidateCard.ProfileHeading);
                                    hashTable.Add("**Url**", string.Format("/opportunities/{0}", interaction.PostID));
                                    await NotificationProcessor.SendEmail(hashTable, Enums.EmailFilePaths.AskingAnOpportunity, employerCard.EmailAddress, true);
                                }
                            }

                            // Notification to the user shared post
                            if (singlePost.WritterID != singlePost.EntityID)
                            { // That means someone has shared job looking for their connection.

                                // Insert to original user shared an opportunity for.
                                url = string.Format("/opportunities/{0}", interaction.PostID);
                                if (EntityHash.ContainsKey("Url"))
                                    EntityHash.Remove("Url");
                                EntityHash.Add("Url", url);
                                notificationProcessor = new InstantNotificationProcessor(EntityHash);
                                await notificationProcessor.AddNotification(Enums.NotificationType.ReferedForAnOpportunity, interaction.PostID, null, singlePost.WritterID);

                                // To author of this opportunity.
                                url = string.Format("/opportunities/{0}", interaction.PostID);
                                if (EntityHash.ContainsKey("Url"))
                                    EntityHash.Remove("Url");
                                EntityHash.Add("Url", url);
                                notificationProcessor = new InstantNotificationProcessor(EntityHash);
                                await notificationProcessor.AddNotification(Enums.NotificationType.ReferenceOnAnOpportunity, interaction.PostID, null, singlePost.EntityID);


                            }
                            else
                            {
                                // Insert to original user shared an opportunity for.
                                url = string.Format("/opportunities/{0}", interaction.PostID);
                                if (EntityHash.ContainsKey("Url"))
                                    EntityHash.Remove("Url");
                                EntityHash.Add("Url", url);
                                notificationProcessor = new InstantNotificationProcessor(EntityHash);
                                await notificationProcessor.AddNotification(Enums.NotificationType.ReferedForAnOpportunity, interaction.PostID, null, singlePost.WritterID);
                            }



                            return new Result { Type = Enums.ResultType.Success, Description = "Your action has been recorded." };
                        }
                        break;
                    case "unlike":
                        if (result == 1)
                        {
                            var likes = await Repository.GetLikes(interaction.PostID, (byte)Enums.PostInteractionType.Like);
                            return new Result { Type = Enums.ResultType.Success, Description = "Your action has been recorded.", ReferenceID = likes };
                        }
                        break;
                    case "report":
                        if (result == 1)
                        {
                            return new Result() { Description = "The post has been reported successfully, and will be reviewed soon per your information.", Type = Enums.ResultType.Success };
                        }
                        else
                        {
                            return new Result() { Description = "We are not able to reach to your post, please give another try.", Type = Enums.ResultType.Error };
                        }
                    case "remove":
                        if (await Repository.AllowOwnerAction(UserHashObject.EntityID, interaction.PostID))
                            result = await Repository.UpdateStatus(interaction.PostID, (int)Enums.PostStatus.Removed);
                        else
                            result = -1;

                        if (result == 1)
                        {
                            return new Result() { Description = "The post has been removed successfully, you can undone the previous action using 'Undo' button below.", Type = Enums.ResultType.Success };
                        }
                        else
                        {
                            return new Result() { Description = "We are not able to remove this post, please give another try.", Type = Enums.ResultType.Error };
                        }
                    case "hide":
                        return new Result() { Description = "The post has been marked as hidden from your timeline, you can unhide it using below button.", Type = Enums.ResultType.Success };
                    case "unhide":
                        await Repository.RemoveInteractionForNews(interaction.PostID, UserHashObject.EntityID, (byte)Enums.PostInteractionType.Hide);
                        return new Result() { Description = "The post have been set to available on your feeds.", Type = Enums.ResultType.Success };
                    case "unremove":
                        if (await Repository.AllowOwnerAction(UserHashObject.EntityID, interaction.PostID))
                            result = await Repository.UpdateStatus(interaction.PostID, (int)Enums.PostStatus.Published);
                        else
                            result = -1;

                        if (result == 1)
                        {
                            return new Result() { Description = "The post has been restrored successfully.", Type = Enums.ResultType.Success };
                        }
                        else
                        {
                            return new Result() { Description = "We are not able to restore this post, please give another try.", Type = Enums.ResultType.Error };
                        }
                    default:
                        break;
                }
            }
            return new Result { Type = Enums.ResultType.Success, Description = "Your action has been recorded." };
        }


        public async Task<Result> ReactReply(string q, string reactType)
        {
            var feedModel = q.ToObject<ReplyViewModel>(null);
            var interaction = Mapper.Map<PostReplyInteraction>(feedModel);
            interaction.EntityID = UserHashObject.EntityID;
            interaction.IpAddress = IpAddress;
            switch (reactType.ToLower())
            {
                case "like":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.Like;
                    break;
                case "unlike":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.Unlike;
                    break;
                case "remove":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.Remove;
                    break;
                default:
                    break;
            }
            using (var Repository = new PressRepository())
            {
                var result = await Repository.React(interaction);
                if (reactType.Equals("like", StringComparison.InvariantCultureIgnoreCase) || reactType.Equals("unlike", StringComparison.InvariantCultureIgnoreCase))
                {
                    var likes = await Repository.GetReplyLikes(interaction.ReplyID);
                    return new Result { Type = Enums.ResultType.Success, Description = "Your action has been recorded.", ReferenceID = likes };
                }
                else
                {
                    await Repository.RemoveReply(interaction.ReplyID, UserHashObject.EntityID);
                    return new Result { Type = Enums.ResultType.Success, Description = "Comment has been deleted." };
                }
            }
        }


        public async Task<List<ReplyViewModel>> MoreReplies(string q, int stock, int pageNumber, int pageSize = 10)
        {
            var model = q.ToObject<ReplyViewModel>(null);
            using (var repository = new PressRepository())
            {
                using (var feeds = await repository.Replies(model.PostID, model.ReplyID, stock, pageNumber, pageSize, UserHashObject.EntityID))
                {
                    var replies = feeds.Tables[0].FromDataTable<ReplyViewModel>();
                    var dataTable = new Hashtable();
                    foreach (var reply in replies)
                    {
                        dataTable = new Hashtable();
                        dataTable.Add("EntityID", UserHashObject.EntityID);
                        dataTable.Add("PostID", reply.PostID);
                        dataTable.Add("Writter", reply.EntityID);
                        dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                        dataTable.Add("ReplyID", reply.ID);
                        reply.Hash = QueryStringHelper.Encrypt(dataTable);
                    }
                    return replies;
                }
            }
        }


        public async Task<HomeFeedViewModel> TimeLineFeeds(string profileHash, int pageNo, int pageSize, bool group)
        {
            var profileObject = profileHash.ToObject<ProfileHashViewModel>(null);
            var model = new HomeFeedViewModel();
            using (var repository = new PressRepository())
            {
                var entiyID = UserHashObject != null ? UserHashObject.EntityID : 0;
                using (var feeds = await repository.TimeLine(profileObject.EntityID, pageNo, pageSize, group))
                {
                    model.Feeds = feeds.Tables[0].FromDataTable<FeedDetailViewModel>();
                    foreach (var feed in model.Feeds)
                    {

                        feed.Replies = feeds.Tables[1].FromDataTable<ReplyViewModel>("PostID=" + feed.ID);
                        if (feeds.Tables.Count > 2)
                        {
                            feed.Skills = feeds.Tables[2].FromDataTable<SkillViewModel>("PostID=" + feed.ID);
                            feed.JobTitles = feeds.Tables[3].FromDataTable<CareerHistoryViewModel>("PostID=" + feed.ID);
                        }

                        var dataTable = new Hashtable();
                        dataTable.Add("EntityID", entiyID);
                        dataTable.Add("PostID", feed.ID);
                        dataTable.Add("ParentPostID", feed.PostID > 0 ? feed.PostID : 0);
                        dataTable.Add("PostType", feed.PostType);
                        dataTable.Add("OriginalWritterID", feed.EntityID);
                        dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                        dataTable.Add("FeedType", "Feed");

                        feed.Hash = QueryStringHelper.Encrypt(dataTable);
                        feed.Replies = feeds.Tables[1].FromDataTable<ReplyViewModel>("PostID=" + feed.ID);
                        foreach (var reply in feed.Replies)
                        {
                            dataTable = new Hashtable();
                            dataTable.Add("EntityID", entiyID);
                            dataTable.Add("PostID", feed.ID);
                            dataTable.Add("PostType", feed.PostType);
                            dataTable.Add("Writter", feed.EntityID);
                            dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                            dataTable.Add("ReplyID", reply.ID);
                            reply.Hash = QueryStringHelper.Encrypt(dataTable);
                        }

                        if (feeds.Tables.Count > 2)
                        {
                            feed.Skills = feeds.Tables[2].FromDataTable<SkillViewModel>("PostID=" + feed.ID);
                            feed.JobTitles = feeds.Tables[3].FromDataTable<CareerHistoryViewModel>("PostID=" + feed.ID);
                        }
                    }
                    model.LoadedOn = Tickes;
                }
            }
            return model;
        }


        public async Task<List<FeedOption>> MoreOptionsPost(string q)
        {
            using (var repository = new PressRepository())
            {
                var feedModel = q.ToObject<FeedDetailViewModel>(null);
                var awardDetail = await GetOne(feedModel.PostID);
                var list = new List<FeedOption>();
                using (var dataSet = await repository.LoadOptionsForPost(awardDetail.ID, UserHashObject.EntityID))
                {
                    var row = dataSet.Tables[0].FromDataTable<FeedForEntity>().FirstOrDefault();
                    var option = new FeedOption();
                    var hashTable = new Hashtable();
                    hashTable.Add("PostType", awardDetail.PostType);

                    hashTable.Add("SharingType", awardDetail.SharingType);
                    hashTable.Add("EntityID", awardDetail.EntityID);
                    hashTable.Add("DbID", Guid.NewGuid());
                    if (awardDetail.EntityID == UserHashObject.EntityID)
                    {
                        hashTable.Add("ID", awardDetail.ID);
                        hashTable.Add("PostID", awardDetail.ID);
                        hashTable.Add("Action", "Remove");
                        option = new FeedOption()
                        {
                            DisplayText = "Remove",
                            Hash = QueryStringHelper.Encrypt(hashTable),
                            Action = "remove",
                            Type = ((Enums.PostTypes)awardDetail.PostType).GetDescription().ToLower()
                        };
                        list.Add(option);
                    }
                    else
                    {
                        hashTable.Add("ID", awardDetail.ID);
                        hashTable.Add("PostID", awardDetail.ID);
                        hashTable.Add("Action", "Report");
                        option = new FeedOption()
                        {
                            DisplayText = "Report",
                            Hash = QueryStringHelper.Encrypt(hashTable),
                            Action = "report",
                            Type = ((Enums.PostTypes)awardDetail.PostType).GetDescription().ToLower()
                        };
                        list.Add(option);
                        if (!row.IsInFavorite)
                        {
                            hashTable.Remove("Action");
                            hashTable.Add("Action", "save-fav");
                            option = new FeedOption()
                            {
                                DisplayText = "Save as favorite",
                                Hash = QueryStringHelper.Encrypt(hashTable),
                                Action = "save-fav",
                                Type = ((Enums.PostTypes)awardDetail.PostType).GetDescription().ToLower()
                            };
                            list.Add(option);
                        }
                        else
                        {
                            hashTable.Remove("Action");
                            hashTable.Add("Action", "remove-fav");
                            option = new FeedOption()
                            {
                                DisplayText = "Remove from favorite",
                                Hash = QueryStringHelper.Encrypt(hashTable),
                                Action = "remove-fav",
                                Type = ((Enums.PostTypes)awardDetail.PostType).GetDescription().ToLower()
                            };
                            list.Add(option);
                        }

                        hashTable.Remove("Action");
                        hashTable.Add("Action", "Hide");
                        option = new FeedOption()
                        {
                            DisplayText = "Hide",
                            Hash = QueryStringHelper.Encrypt(hashTable),
                            Action = "Hide",
                            Type = ((Enums.PostTypes)awardDetail.PostType).GetDescription().ToLower()
                        };
                        list.Add(option);

                    }
                }
                return list;
            }
        }

        private async Task<FeedDetailViewModel> GetOne(long postID)
        {
            using (var repository = new PressRepository())
            {
                var dataSet = await repository.SingleFeedRow(postID);
                var result = dataSet.Tables[0].FromDataTable<FeedDetailViewModel>()[0];
                var table = new Hashtable();
                table.Add("ID", result.ID);
                table.Add("EntityID", result.EntityID);
                table.Add("DbMapID", DateTime.UtcNow.Ticks);
                result.Hash = QueryStringHelper.Encrypt(table);
                return result;
            }
        }


        public async Task<FeedDetailViewModel> Report(string q)
        {
            var feedModel = q.ToObject<FeedDetailViewModel>(null);
            using (var repository = new PressRepository())
            {
                var dataSet = await repository.SingleFeedRow(feedModel.ID);
                feedModel = dataSet.Tables[0].FromDataTable<FeedDetailViewModel>()[0];
                var table = new Hashtable();
                table.Add("ID", feedModel.ID);
                table.Add("PostID", feedModel.ID);
                table.Add("EntityID", feedModel.EntityID);
                table.Add("DbMapID", DateTime.UtcNow.Ticks);
                table.Add("Action", "Report");
                feedModel.Hash = QueryStringHelper.Encrypt(table);
                return feedModel;
            }
        }

        public async Task<List<FeedDetailViewModel>> Feed(long feedID)
        {
            var model = new List<FeedDetailViewModel>();
            using (var repository = new PressRepository())
            {
                using (var feeds = await repository.TimeLine(UserHashObject.EntityID, feedID))
                {
                    model = feeds.Tables[0].FromDataTable<FeedDetailViewModel>();
                    foreach (var feed in model)
                    {
                        var dataTable = new Hashtable();
                        dataTable.Add("EntityID", UserHashObject.EntityID);
                        dataTable.Add("PostID", feed.ID);
                        dataTable.Add("ParentPostID", feed.PostID > 0 ? feed.PostID : 0);
                        dataTable.Add("PostType", feed.PostType);
                        dataTable.Add("OriginalWritterID", feed.EntityID);
                        dataTable.Add("VarificationTokenInDb", Guid.NewGuid().ToString());
                        dataTable.Add("FeedType", "Feed");
                        feed.Hash = QueryStringHelper.Encrypt(dataTable);

                        if (feeds.Tables.Count > 2)
                        {
                            feed.Skills = feeds.Tables[2].FromDataTable<SkillViewModel>("PostID=" + feed.ID);
                            feed.JobTitles = feeds.Tables[3].FromDataTable<CareerHistoryViewModel>("PostID=" + feed.ID);
                        }
                    }
                }
            }
            return model;
        }





        public async Task<Result> ShareOpportunity(ShareOpportunityViewModel model)
        {
            var post = Mapper.Map<Post>(model);
            post.IPAddress = IpAddress;
            post.Published = true;
            post.PublishOn = Now;
            post.SharingType = (byte)Enums.SharingType.Share;
            post.Status = (byte)Enums.PostStatus.Published;
            post.WritterID = UserHashObject.EntityID;
            post.EntityID = UserHashObject.EntityID;
            if (!string.IsNullOrEmpty(model.GroupHash))
            {
                post.GroupEntityID = Convert.ToInt64(model.GroupHash.GetSingleValue("EntityID"));
            }
            if (model.Skills != null && model.Skills.Count > 0)
            {
                post.Skills = new EntityCollection<Skill>();
                foreach (var item in model.Skills)
                {
                    post.Skills.Add(new Skill() { SkillID = item });
                }
            }

            if (model.JobTitles != null && model.JobTitles.Count > 0)
            {
                post.JobTitles = new EntityCollection<JobTitle>();
                foreach (var item in model.JobTitles)
                {
                    post.JobTitles.Add(new JobTitle() { JobTitleID = item });
                }
            }

            using (var Repository = new PressRepository())
            {
                var storyID = await Repository.Save(post);
                if (storyID > 0)
                {
                    return new Result() { ReferenceID = storyID, Description = string.Format("https://www.sklative.com/opportunities/{0}", storyID), Type = Enums.ResultType.Success };
                }
                return new Result() { Description = "We cannot reach to your request, please have one more try.", Type = Enums.ResultType.Error };
            }
        }


        public async Task<Result> ApplyOpportunity(ApplyViewModel model)
        {
            return await React(model.Hash, "apply", model.Text);
        }


        public async Task<Result> ReferOpportunity(RerefOpportunityViewModel model)
        {
            return await React(model.Opportunity.Hash, "refer", model.Text, model.ConnectionID);
        }


        public async Task<Result> ReferForAnOpportunity(RerefOpportunityViewModel model)
        {
            return await React(model.Opportunity.Hash, "referopportunity", model.Text, model.ConnectionID);
        }


        public async Task<FeedInsightsViewModel> Insights(string q, string reactType)
        {
            var feedModel = q.ToObject<FeedDetailViewModel>(null);
            using (var Repository = new PressRepository())
            {

                if (await Repository.AllowOwnerAction(UserHashObject.EntityID, feedModel.PostID))
                {
                    using (var insights = await Repository.Insights(feedModel.PostID))
                    {
                        return insights.Tables[0].FromDataTable<FeedInsightsViewModel>()[0];
                    }
                }
                return null;
            }
        }


        public async Task<List<InsightDetailsViewModel>> InsightDetails(string q, string type)
        {
            var feedModel = q.ToObject<FeedDetailViewModel>(null);
            using (var Repository = new PressRepository())
            {
                if (type != null && type.Equals("insights-my-referals", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (var insights = await Repository.MyInsights(feedModel.PostID, UserHashObject.EntityID))
                    {
                        return insights.Tables[0].FromDataTable<InsightDetailsViewModel>();
                    }
                }
                int insightType = 0;
                if (type == "applications")
                {
                    insightType = (int)Enums.PostInteractionType.Apply;
                }
                if (type == "referals")
                {
                    insightType = (int)Enums.PostInteractionType.Refer;
                }
                if (type == "opportunity-referals")
                {
                    insightType = (int)Enums.PostInteractionType.ReferForOpportunity;
                }
                if (await Repository.AllowOwnerAction(UserHashObject.EntityID, feedModel.PostID))
                {
                    using (var insights = await Repository.InsightDetails(feedModel.PostID, insightType))
                    {
                        return insights.Tables[0].FromDataTable<InsightDetailsViewModel>();
                    }
                }
                return null;
            }
        }


        public async Task<Result> Track(string trackType, string[] posts)
        {
            var interaction = Mapper.Map<PostInteraction>(new FeedDetailViewModel());
            interaction.EntityID = UserHashObject.EntityID;
            interaction.IpAddress = IpAddress;

            switch (trackType.ToLower())
            {
                case "views":
                    interaction.InteractionType = (byte)Enums.PostInteractionType.View;
                    break;
                default:
                    break;
            }
            using (var Repository = new PressRepository())
            {
                var result = await Repository.React(interaction, string.Join(",", posts));
                if (result > 0)
                {
                    return new Result { Type = Enums.ResultType.Success, Description = "Your action has been recorded.", ReferenceID = result };
                }
                return new Result { Type = Enums.ResultType.Error, Description = "An error while tracking data.", ReferenceID = result };
            }
        }


        public async Task<HomeFeedViewModel> GetArticles(int pageNumber, int pageSize)
        {
            var stamp = DateTime.UtcNow;
            var model = new HomeFeedViewModel();
            using (var repository = new PressRepository())
            {
                using (var feeds = await repository.FeedsByType((int)Enums.PostTypes.Blog , pageNumber, 10, stamp))
                {
                    model.Feeds = feeds.Tables[0].FromDataTable<FeedDetailViewModel>();
                    CreateFeedModel(model, feeds);
                    model.LoadedOn = Tickes;
                }
            }
            return model;
        }
    }
}