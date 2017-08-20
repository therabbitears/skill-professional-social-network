using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models.Chat;
using WrlyInfrastucture.Filters;

namespace Wrly.Controllers
{
    //[Authorize]
    public class ChatGroupController : BaseController
    {

        private IGroupChatProcessor _processor;

        public IGroupChatProcessor Processor
        {
            get
            {
                if (_processor == null)
                {
                    _processor = new GroupChatProcessor();
                }
                return _processor;
            }
        }

        public async Task<ActionResult> Index()
        {
            return View();
        }


        public async Task<ActionResult> StartChat(string id, string sr)
        {
            await Processor.UpdateLastSeen();
            if (!string.IsNullOrEmpty(id))
            {
                var group = await Processor.StartChat(id);
                await Processor.UpdateMessageStatusForGroup(group.GroupInfo.ID, (int)Enums.MessageReadingStatus.ReadByReceiver);
                return View("Group", group);
            }
            else
            {
                if (string.IsNullOrEmpty(sr))
                {
                    var chatFaceLastes = await Processor.GetChatFaces(0, 1);
                    if (chatFaceLastes!=null && chatFaceLastes.Count>0)
                    {
                        return RedirectToRoute("ConversationsUser", new { id = chatFaceLastes.FirstOrDefault().UserID });
                    }
                    return View("Group", new GroupChatViewModel());
                }
                else
                {
                    return View("Group", new GroupChatViewModel());
                }
            }
        }


        public async Task<ActionResult> ChatHistory(long group, int page, int pageSize)
        {
            var history = await Processor.GetChatHistory(group, page, pageSize);
            history.Reverse();
            return PartialView("_Messages", history);
        }

        public async Task<ActionResult> ChatUsers(int page = 0, int pageSize = 20, long? gid = null, string connectionID = null)
        {
            ViewBag.ConnectionID = connectionID;
            var history = await Processor.GetChatFaces(page, pageSize, gid);
            return PartialView("_ChatFaces", history);
        }

        public async Task<ActionResult> StartNew()
        {
            return PartialView("_NewConversation");
        }

        public async Task<JsonResult> ChatSession(long eID)
        {
            var data = await Processor.ChatSession(eID);
            await Processor.UpdateMessageStatusForGroup(data.GroupInfo.ID, (int)Enums.MessageReadingStatus.ReadByReceiver);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<JsonResult> Send(GroupChatViewModel model, string message)
        {
            var data = await Processor.Send(model, message);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> QuickChatSession(long eID)
        {
            var data = await Processor.ChatSession(eID);
            return PartialView("_QuickChatSession", data);
        }
    }
}