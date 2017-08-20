using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Infrastructure.Utils;
using Wrly.Models;
using WrlyInfrastucture.Filters;

namespace Wrly.Controllers
{
    //[Authorize]
    public class SkillHistoryController : BaseController
    {
        ISkillHistoryProcessor _processor;
        public ISkillHistoryProcessor Processor
        {
            get
            {
                if (_processor == null)
                {
                    _processor = new SkillHistoryProcessor();
                }
                return _processor;
            }
        }

        [CompressFilter]
        public async Task<ActionResult> Manage(string hash)
        {
            var history = new SkillViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                history = await Processor.GetOneSkill(hash);
            }
            else
            {
                history.IsCurrent = true;
            }
            history.MonthList = new SelectList(Month, "Key", "Value");
            history.YearList = new SelectList(Year, "Key", "Value");
            history.ExpetiseLevels = new SelectList(CommonData.ExpertiseLevel(), "Key", "Value");
            return PartialView("_ManageSkill", history);
        }

        public async Task<ActionResult> Wizard()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Wizard(string action)
        {
            return RedirectToAction("Wizard", "CareerHistory");
        }

        public async Task<JsonResult> SearchAllSkill(string key)
        {
            var result = await AllSkills(key);
            return new JsonResult() { ContentType = "application/json", Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public async Task<JsonResult> SearchSkill(string key)
        {
            var result = await MySkills(key);
            return new JsonResult() { ContentType = "application/json", Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        [HttpPost]
        public async Task<ActionResult> Manage(string hash, SkillViewModel model)
        {
            if (ModelState.IsValid)
            {
             await   Processor.Save(model);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                model.MonthList = new SelectList(Month, "Key", "Value");
                model.YearList = new SelectList(Year, "Key", "Value");
                model.ExpetiseLevels = new SelectList(CommonData.ExpertiseLevel(), "Key", "Value");
                return PartialView("_ManageSkill", model);
            }
        }

        [CompressFilter]
        public async Task<ActionResult> Skill()
        {
            var history = await Processor.GetSkillHisotry();
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public async Task<PartialViewResult> Remove(string q)
        {
            await Processor.Remove(q);
            ViewBag.Hash = q;
            return PartialView("_RevertSkillRemoval");
        }

        //[Authorize]
        public async Task<JsonResult> RevertRemove(string q)
        {
            await Processor.RevertRemove(q);
            return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        //[Authorize]
        public async Task<JsonResult> Add(long id, string name)
        {
            SkillViewModel model = new SkillViewModel() { SkillID = id, Name = name };
           await Processor.Save(model);
            return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public async Task<ActionResult> detail(string id)
        {
            var detailsViewModel = await Processor.Details(id);
            return PartialView("_SkillDetails", detailsViewModel);
        }

        public async Task<PartialViewResult> LoadOptions(string q)
        {
            var actions = await Processor.LoadSkillOptions(q);
            return PartialView("_SkillHistoryMoreOptions", actions);
        }

        [HttpPost]
        public async Task<JsonResult> Endorse(string q)
        {
            var result = await Processor.Endorse(q);
            return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = result };
        }

        [HttpPost]
        public async Task<JsonResult> RemoveEndorse(string q)
        {
            var result = await Processor.RemoveEndorse(q);
            return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = result };
        }

    }
}