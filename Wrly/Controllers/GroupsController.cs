using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Utils;
using Wrly.Models.Business;
using WrlyInfrastucture.Filters;

namespace Wrly.Controllers
{
    public class GroupsController : BaseController
    {
        IBusinessProcessor _Processor;
        IBusinessProcessor Processor
        {
            get
            {
                if (_Processor == null)
                {
                    _Processor = new BusinessProcessor();
                }
                return _Processor;
            }
        }

        [CompressFilter]
        public async Task<ActionResult> Index(string id, string tab, string keyword = "", bool publicView = false)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var group = await Processor.Group(id);
                group.Mode = Enums.GroupMode.General;
                if (group.Owner != null && !publicView)
                {
                    if (!string.IsNullOrEmpty(tab))
                    {
                        switch (tab.ToLower())
                        {
                            case "member-requests":
                                group.Mode = Enums.GroupMode.Requests;
                                break;
                            case "members":
                                group.Mode = Enums.GroupMode.Members;
                                break;
                            default:
                                break;
                        }
                    }
                    return View("GroupManage", group);
                }
                return View("Group", group);
            }
            var groups = await Processor.Groups(keyword);
            return View("Groups", groups);
        }

        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> New()
        {
            var model = new GroupViewModel() { Types = new SelectList(CommonData.GetGroupTypes(), "Key", "Value") };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CompressFilter]
        public async Task<ActionResult> New(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Processor.CreateGroup(model);
                if (result.Type == Types.Enums.ResultType.Success)
                {
                    return WJson(result);
                }
            }
            return WJson(ModelState);
        }

        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> Members(string q, bool pending = false, bool group = true, int pageNumber = 0)
        {
            var model = await Processor.LoadMembers(q, pending, group, pageNumber);
            if (pending)
            {
                return PartialView("_PendingMembers", model);
            }
            return PartialView("_Members", model);
        }

        [CompressFilter]
        public async Task<ActionResult> MemmberList(string q, bool group = true, int pageNumber = 0, int pageSize=20)
        {
            var model = await Processor.LoadMembers(q, false, group, pageNumber, pageSize);
            return PartialView("_Members", model);
        }
    }
}