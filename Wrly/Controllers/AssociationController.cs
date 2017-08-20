using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Types;
using Wrly.Infrastructure.Filters;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Infrastuctures.Utils;
using Wrly.Models;
using Wrly.Models.Listing;
using WrlyInfrastucture.Filters;

namespace Wrly.Controllers
{
    [Authorize]
    public class AssociationController : BaseController
    {
        IAssociationProcessor _Processor;
        IAssociationProcessor Processor
        {
            get
            {
                if (_Processor == null)
                {
                    _Processor = new AssociationProcessor();
                }
                return _Processor;
            }
        }

        //[Authorize]
        [CompressFilter]
        public async Task<ActionResult> Connections(int pageNumber = 1, int pageSize = 50)
        {
            var connections = await Processor.GetConnections(pageNumber, pageSize);
            return View(connections);
        }

        //[Authorize]
        [CompressFilter]
        public async Task<ActionResult> ConnectionsToInvite(string q,string keyword, int pageNumber = 0, int pageSize = 20)
        {
            var connections = await Processor.GetConnections(q,keyword, pageNumber, pageSize);
            return PartialView("_ConnectionToInvite", connections);
        }

        //[Authorize]
        [CompressFilter]
        public async Task<ActionResult> SuggestionList(int pageNumber = 0, int pageSize = 50)
        {
            var suggestions = await Processor.GetSuggestions(pageNumber, pageSize);
            return View("Suggestions", suggestions);
        }

        //[Authorize]
        [CompressFilter]
        public async Task<ActionResult> ConnectionRequests(string dir, int pageNumber = 1, int pageSize = 50)
        {
            if (!string.IsNullOrEmpty(dir) && dir.Equals("out"))
            {
                var requests = await Processor.GetRequests(pageNumber, pageSize, Enums.AssociationRequestDirection.Sent);
                return View("Requests", requests);
            }
            else
            {
                var requests = await Processor.GetRequests(pageNumber, pageSize);
                return View("Requests", requests);
            }
        }

        //[Authorize]
        [CompressFilter]
        public async Task<ActionResult> Followings(int pageNumber = 1, int pageSize = 50)
        {
            var requests = await Processor.GetFollowings(pageNumber, pageSize);
            return View(requests);
        }

        //[Authorize]
        [CompressFilter]
        public async Task<ActionResult> Followers(int pageNumber = 1, int pageSize = 50)
        {
            var requests = await Processor.GetFollowers(pageNumber, pageSize);
            return View(requests);
        }


        //[Authorize]
        [CompressFilter]
        public async Task<ActionResult> Suggestions(int pageNumber = 0, int pageSize = 5)
        {
            var suggestions = await Processor.GetSuggestions(pageNumber, pageSize);
            return new JsonResult() { ContentType = "application/json", Data = suggestions, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //[Authorize]
        [CompressFilter]
        public async Task<ActionResult> Connect(SendAssociationViewModel model)
        {
            var result = await Processor.SendConnectRequest(model);
            return new JsonResult() { ContentType = "application/json", Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //[Authorize]
        [CompressFilter]
        public async Task<JsonResult> Requests(int pageNumber = 1, int pageSize = 6)
        {
            var requests = await Processor.GetRequests(pageNumber, pageSize);
            return new JsonResult() { ContentType = "application/json", Data = requests, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //[Authorize]
        [CompressFilter]
        public async Task<ActionResult> Action(ActionAssociateProfileViewModel model, string actn)
        {
            var result = await Processor.Action(model, actn);
            return WJson(result);
        }

        //[Authorize]
        public async Task<JsonResult> ConnectionHeads(string keyword)
        {
            var connections = await MyConnections(keyword);
            return WJson(connections);
        }

        //[Authorize]
        public async Task<ActionResult> Wizard()
        {
            return View();
        }

        //[Authorize]
        public async Task<PartialViewResult> Actionable(string q)
        {
            var result = await Processor.GetConnectionActions(q);
            return PartialView("_NetworkActions", result);
        }

        //[Authorize]
        public async Task<PartialViewResult> GroupActionable(string q)
        {
            var result = await Processor.GetConnectionActions(q);
            return PartialView("_GroupActions", result);
        }


        //[Authorize]
        public async Task<PartialViewResult> HoverCardactionable(string q)
        {
            var result = await Processor.GetConnectionActions(q);
            return PartialView("_NetworkHoverCardActions", result);
        }

        //[Authorize]
        public async Task<ActionResult> HappeningsAll()
        {
            var happenings = await Processor.GetNetworkHappenings(0, int.MaxValue);
            return View("Happenings", happenings);
        }

        //[Authorize]
        public async Task<JsonResult> happenings(int pageNumber = 0, int pageSize = int.MaxValue)
        {
            var happenings = await Processor.GetNetworkHappenings(pageNumber, pageSize);
            return WJson(happenings);
        }

        //[Authorize]
        public async Task<ActionResult> MyHappenings(int pageNumber = 0, int pageSize = int.MaxValue)
        {
            var happenings = await Processor.GetMyHappenings(pageNumber, pageSize);
            return View(happenings);
        }



        //[Authorize]
        [HttpPost]
        public async Task<JsonResult> ActivityAction(BaseViewModel model, string action)
        {
            var result = await Processor.ActivityAction(model, action);
            return WJson(result);
        }


        //[Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<JsonResult> InviteToGroup(string hash)
        {
            var result = await Processor.InviteToGroup(hash);
            return WJson(result);
        }

        //[Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<JsonResult> ApproveAll(string profileHash)
        {
            var result = await Processor.ApproveAll(profileHash);
            return WJson(result);
        }
    }
}