using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models;
using WrlyInfrastucture.Filters;

namespace Wrly.Controllers
{
    //[Authorize]
    public class ReferenceController : BaseController
    {


        IReferenceProcessor _Processor;
        IReferenceProcessor Processor
        {
            get
            {
                if (_Processor == null)
                {
                    _Processor = new ReferenceProcessor();
                }
                return _Processor;
            }
        }


        IAwardProcessor _AwardProcessor;
        IAwardProcessor AwardProcessor
        {
            get
            {
                if (_AwardProcessor == null)
                {
                    _AwardProcessor = new AwardProcessor();
                }
                return _AwardProcessor;
            }
        }

        [CompressFilter]
        public async Task<ActionResult> Appriciations()
        {
            var history = await Processor.GetAppriciations(User.Identity.Name);
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        [CompressFilter]
        public async Task<ActionResult> Requests(string dir = "received", long? id = null)
        {
            var model = await Processor.GetRequests(dir, id, (int)Enums.AppriciationAndRecommedationStatus.Requested);
            return View(model);
        }


        [CompressFilter]
        public async Task<ActionResult> Index(string dir = "received", long? id = null)
        {
            var model = await Processor.GetReferences(dir, id, (int)Enums.AppriciationAndRecommedationStatus.Approved);
            return View(model);
        }

        [CompressFilter]
        public async Task<ActionResult> Recommendation()
        {
            var history = await Processor.GetRecommendations(User.Identity.Name);
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public async Task<ActionResult> AppriciateforAccomplishment(string q)
        {
            var model = await Processor.GetAppriciationModelForAccomplishment(q);
            return PartialView("_manageAppriciationForAccomplishment", model);
        }

        public async Task<ActionResult> RecommendForRole(string q)
        {
            var model = await Processor.GetRecommendForRole(q);
            return PartialView("_manageAppriciationForRole", model);


        }

        public async Task<PartialViewResult> RecommendForSkill(string q)
        {
            var model = new SkillReferenceViewModel()
            {
                Skill = q.ToObject<SkillViewModel>(null),
                Hash = q
            };
            return PartialView("_manageAppriciationForSkill", model);
        }

        public async Task<PartialViewResult> RecommendGeneral(string q)
        {
            var model = q.ToObject<AppreciationAndRecommendationViewModel>(null);
            model.Hash = q;
            return PartialView("_manageGeneralRecommendation", model);
        }

        [CompressFilter]
        public ActionResult manageAppriciation(string hash)
        {
            var entityID = Convert.ToInt64(hash.GetSingleValue("EntityID"));
            if (entityID != UserHashObject.EntityID)
            {
                var history = new AppreciationAndRecommendationViewModel();
                if (!string.IsNullOrEmpty(hash))
                {
                    history = Processor.GetAppriciation(hash);
                }
                history.Project = new SelectList(Projects(), "Key", "Value");
                return PartialView("_manageAppriciation", history);
            }
            else
            {
                return PartialView("_InvalidData");
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> manageAppriciation(AppreciationAndRecommendationViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.ReferenceMode.Appreciation;
                var result = await Processor.Save(model);
                return PartialView("_ActionResultMessage", result);
            }
            else
            {
                model.Project = new SelectList(Projects(), "Key", "Value");
                return PartialView("_manageAppriciation", model);
            }
        }

        [CompressFilter]
        public async Task<ActionResult> askRecommendation(string hash)
        {
            var history = new AskRecommendationViewModel();
            history.CareerHistoryList = new SelectList(await CareerHistoryList(), "Key", "Value");
            history.Skills = new SelectList(await MySkills(), "Key", "Value");
            return PartialView("_AskRecommendation", history);
        }


        [HttpPost]
        public async Task<ActionResult> askRecommendation(AskRecommendationViewModel model)
        {
            SetModelStateForAsk(model);
            if (ModelState.IsValid && model.Providers != null && model.Providers.Length > 0)
            {
                model.Type = (byte)Enums.ReferenceMode.Recommendation;
                var result = await Processor.Ask(model);
                return PartialView("_ActionResultMessage", result);
            }
            else
            {
                model.Skills = new SelectList(await MySkills(), "Key", "Value");
                model.CareerHistoryList = new SelectList(await CareerHistoryList(), "Key", "Value");
                return PartialView("_AskRecommendation", model);
            }
        }

        private void SetModelStateForAsk(AskRecommendationViewModel model)
        {
            ModelState.Remove("Description");
            if (model.Providers == null || model.Providers.Length == 0)
            {
                ModelState.AddModelError("ParticipentText", "Requires at least one connection selected.");
            }
            if (model.RecommedationType == "Skill" && (model.SkillID == null || model.SkillID == -1))
            {
                ModelState.AddModelError("SkillID", "Require skill to be selected while asking for recommedation for skill.");
            }
            if (model.RecommedationType == "Role" && (model.CareerHistoryID == null || model.CareerHistoryID == -1))
            {
                ModelState.AddModelError("CareerHistoryID", "Require role to be selected while asking for recommedation for a role.");
            }
        }


        [CompressFilter]
        public async Task<ActionResult> manageRecommendation(string hash)
        {
            var history = new AppreciationAndRecommendationViewModel();
            history.Hash = hash;
            var entityID =Convert.ToInt64(hash.GetSingleValue("EntityID"));
            if (entityID != UserHashObject.EntityID)
            {
                history.CareerHistoryList = new SelectList(await CareerHistoryList(), "Key", "Value");
                history.Skills = new SelectList(await MySkills(entityID), "Key", "Value");
                return PartialView("_manageRecommendation", history);
            }
            else {
                return PartialView("_InvalidData");
            }
        }


        [CompressFilter]
        public async Task<ActionResult> manageBusinessRecommendation(string hash)
        {
            var history = new AppreciationAndRecommendationViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                history = Processor.GetRecommendation(hash);
            }
            history.Hash = hash;
            history.ServicesAndProducts = await AwardProcessor.GetServicesAndProductBasic(hash);
            return PartialView("_manageBusinessRecommendation", history);
        }

        [HttpPost]
        public async Task<ActionResult> manageBusinessRecommendation(AppreciationAndRecommendationViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.ReferenceMode.Recommendation;
                var result = await Processor.Save(model);
                return PartialView("_ActionResultMessage", result);
            }
            else
            {
                return PartialView("_manageBusinessRecommendation", model);
            }
        }

        [CompressFilter]
        public async Task<ActionResult> manageBusinessAppriciation(string hash)
        {
            var history = new AppreciationAndRecommendationViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                history = Processor.GetRecommendation(hash);
            }
            history.Hash = hash;
            history.ServicesAndProducts = await AwardProcessor.GetServicesAndProductBasic(hash);
            return PartialView("_manageBusinessAppriciation", history);
        }

        [HttpPost]
        public async Task<ActionResult> manageBusinessAppriciation(AppreciationAndRecommendationViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.ReferenceMode.Appreciation;
                var result = await Processor.Save(model);
                return PartialView("_ActionResultMessage", result);
            }
            else
            {
                return PartialView("_manageBusinessAppriciation", model);
            }
        }

        [CompressFilter]
        public async Task<ActionResult> Action(string hash, string actn)
        {
            if (!string.IsNullOrEmpty(hash))
            {
                var result = await Processor.Execute(hash, actn);
                return PartialView("_ActionResultMessage", result);
            }
            return null;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> manageRecommendation(AppreciationAndRecommendationViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.ReferenceMode.Recommendation;
                var result = await Processor.Save(model);
                return PartialView("_ActionResultMessage", result);
            }
            else
            {
                return PartialView("_manageRecommendation", model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> manageRecommendationForRole(CareerHistoryReferenceViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.ReferenceMode.Recommendation;
                var result = await Processor.SaveForRole(model);
                return PartialView("_ActionResultMessage", result);
            }
            else
            {
                return PartialView("_manageRecommendation", model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> RecommendForSkill(SkillReferenceViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.ReferenceMode.Recommendation;
                var result = await Processor.SaveForSkill(model);
                return PartialView("_ActionResultMessage", result);
            }
            else
            {
                return PartialView("_manageAppriciationForSkill", model);
            }
        }
    }
}