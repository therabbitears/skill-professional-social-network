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
using Wrly.Models;
using WrlyInfrastucture.Filters;

namespace Wrly.Controllers
{
    //[Authorize]
    public class AwardController : BaseController
    {

        IAwardProcessor _Processor;
        IAwardProcessor Processor
        {

            get
            {
                if (_Processor == null)
                {
                    _Processor = new AwardProcessor();
                }
                return _Processor;
            }
        }

        ICareerHistoryProcessor _CareerHistoryProcessor;
        ICareerHistoryProcessor CareerHistoryProcessor
        {

            get
            {
                if (_CareerHistoryProcessor == null)
                {
                    _CareerHistoryProcessor = new CareerHistoryProcessor();
                }
                return _CareerHistoryProcessor;
            }
        }

        ISkillHistoryProcessor _SkillProcessor;
        ISkillHistoryProcessor SkillProcessor
        {

            get
            {
                if (_SkillProcessor == null)
                {
                    _SkillProcessor = new SkillHistoryProcessor();
                }
                return _SkillProcessor;
            }
        }

        [CompressFilter]
        public async Task<ActionResult> Index()
        {
            var history = await Processor.GetAwards(User.Identity.Name);
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> Assignments()
        {
            var history = await Processor.GetAssignments(User.Identity.Name);
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> Services()
        {
            var history = await Processor.GetServices();
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        [CompressFilter]
        public async Task<ActionResult> Products()
        {
            var history = await Processor.GetProducts();
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        [CompressFilter]
        public async Task<ActionResult> Publications()
        {
            var history = await Processor.GetPublication(User.Identity.Name);
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> Compositions()
        {
            var history = await Processor.GetCompositions(User.Identity.Name);
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> Researches()
        {
            var history = await Processor.GetResearches(User.Identity.Name);
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> Findings()
        {
            var history = await Processor.GetFindings(User.Identity.Name);
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> Manage(string hash)
        {
            var model = new AwardViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                model = await Processor.GetOneAward(hash);
            }

            var careerList = await CareerHistoryProcessor.GetCareerHisotry();
            var skills = await SkillProcessor.GetSkillHisotry();
            model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
            model.SkillList = new SelectList(skills, "SkillID", "Name");
            model.MonthList = new SelectList(Month, "Key", "Value");
            model.YearList = new SelectList(Year, "Key", "Value");
            return PartialView("_ManageAwards", model);
        }

        [CompressFilter]
        public async Task<ActionResult> ManagePublication(string hash)
        {
            var model = new PublilcationViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                model = await Processor.GetOnePublication(hash);
            }

            var careerList = await CareerHistoryProcessor.GetCareerHisotry();
            var skills = await SkillProcessor.GetSkillHisotry();
            model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
            model.SkillList = new SelectList(skills, "SkillID", "Name");
            model.MonthList = new SelectList(Month, "Key", "Value");
            model.YearList = new SelectList(Year, "Key", "Value");
            model.PublicationType = new SelectList(CommonData.EnumToDictionary(typeof(Enums.PublicationType), "Select", "None"), "Key", "Value");
            return PartialView("_ManagePublication", model);
        }

        [HttpPost]
        public async Task<ActionResult> ManagePublication(PublilcationViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.AwardAndAssignmentMode.Publication;
                Processor.SavePublication(model);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var careerList = await CareerHistoryProcessor.GetCareerHisotry();
                var skills = await SkillProcessor.GetSkillHisotry();
                skills.Insert(0, new SkillViewModel() { SkillID = -1, Name = "Select" });
                model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
                model.SkillList = new SelectList(skills, "SkillID", "Name");
                model.PublicationType = new SelectList(CommonData.EnumToDictionary(typeof(Enums.PublicationType), "Select", "None"), "Key", "Value");
                return PartialView("_ManagePublication", model);
            }
        }

        [CompressFilter]
        public async Task<ActionResult> ManageComposition(string hash)
        {
            var model = new PublilcationViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                model = await Processor.GetOnePublication(hash);
            }

            var careerList = await CareerHistoryProcessor.GetCareerHisotry();
            var skills = await SkillProcessor.GetSkillHisotry();
            model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
            model.SkillList = new SelectList(skills, "SkillID", "Name");
            model.MonthList = new SelectList(Month, "Key", "Value");
            model.YearList = new SelectList(Year, "Key", "Value");
            model.PublicationType = new SelectList(CommonData.EnumToDictionary(typeof(Enums.CompositionType), "Select", "None"), "Key", "Value");
            return PartialView("_ManageComposition", model);
        }

        [HttpPost]
        public async Task<ActionResult> ManageComposition(PublilcationViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.AwardAndAssignmentMode.Composition;
                Processor.SavePublication(model);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var careerList = await CareerHistoryProcessor.GetCareerHisotry();
                var skills = await SkillProcessor.GetSkillHisotry();
                skills.Insert(0, new SkillViewModel() { SkillID = -1, Name = "Select" });
                model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
                model.SkillList = new SelectList(skills, "SkillID", "Name");
                model.PublicationType = new SelectList(CommonData.EnumToDictionary(typeof(Enums.CompositionType), "Select", "None"), "Key", "Value");
                return PartialView("_ManageComposition", model);
            }
        }


        [CompressFilter]
        public async Task<ActionResult> ManageResearch(string hash)
        {
            var model = new PublilcationViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                model = await Processor.GetOnePublication(hash);
            }

            var careerList = await CareerHistoryProcessor.GetCareerHisotry();
            var skills = await SkillProcessor.GetSkillHisotry();
            model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
            model.SkillList = new SelectList(skills, "SkillID", "Name");
            model.MonthList = new SelectList(Month, "Key", "Value");
            model.YearList = new SelectList(Year, "Key", "Value");
            model.PublicationType = new SelectList(CommonData.EnumToDictionary(typeof(Enums.ResearchType), "Select", "None"), "Key", "Value");
            return PartialView("_ManageResearch", model);
        }

        [HttpPost]
        public async Task<ActionResult> ManageResearch(PublilcationViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.AwardAndAssignmentMode.Research;
                Processor.SavePublication(model);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var careerList = await CareerHistoryProcessor.GetCareerHisotry();
                var skills = await SkillProcessor.GetSkillHisotry();
                skills.Insert(0, new SkillViewModel() { SkillID = -1, Name = "Select" });
                model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
                model.SkillList = new SelectList(skills, "SkillID", "Name");
                model.PublicationType = new SelectList(CommonData.EnumToDictionary(typeof(Enums.ResearchType), "Select", "None"), "Key", "Value");
                return PartialView("_ManageResearch", model);
            }
        }



        [CompressFilter]
        public async Task<ActionResult> ManageService(string hash)
        {
            var model = new PublilcationViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                model = await Processor.GetOnePublication(hash);
            }
            model.MonthList = new SelectList(Month, "Key", "Value");
            model.YearList = new SelectList(Year, "Key", "Value");
            return PartialView("_ManageService", model);
        }

        [HttpPost]
        public async Task<ActionResult> ManageService(PublilcationViewModel model)
        {
            ModelState.Remove("Role");
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.AwardAndAssignmentMode.Services;
                Processor.SavePublication(model);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                model.MonthList = new SelectList(Month, "Key", "Value");
                model.YearList = new SelectList(Year, "Key", "Value");
                return PartialView("_ManageService", model);
            }
        }

        [CompressFilter]
        public async Task<ActionResult> ManageProduct(string hash)
        {
            var model = new PublilcationViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                model = await Processor.GetOnePublication(hash);
            }
            return PartialView("_ManageProduct", model);
        }

        [HttpPost]
        public async Task<ActionResult> ManageProduct(PublilcationViewModel model)
        {
            ModelState.Remove("Role");
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.AwardAndAssignmentMode.Products;
                Processor.SavePublication(model);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return PartialView("_ManageProduct", model);
            }
        }

        [CompressFilter]
        public async Task<ActionResult> ManageFinding(string hash)
        {
            var model = new PublilcationViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                model = await Processor.GetOnePublication(hash);
            }

            var careerList = await CareerHistoryProcessor.GetCareerHisotry();
            var skills = await SkillProcessor.GetSkillHisotry();
            model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
            model.SkillList = new SelectList(skills, "SkillID", "Name");
            model.MonthList = new SelectList(Month, "Key", "Value");
            model.YearList = new SelectList(Year, "Key", "Value");
            model.PublicationType = new SelectList(CommonData.EnumToDictionary(typeof(Enums.FindingType), "Select", "None"), "Key", "Value");
            return PartialView("_ManageFinding", model);
        }

        [HttpPost]
        public async Task<ActionResult> ManageFinding(PublilcationViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.AwardAndAssignmentMode.Finding;
                await Processor.SavePublication(model);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var careerList = await CareerHistoryProcessor.GetCareerHisotry();
                var skills = await SkillProcessor.GetSkillHisotry();
                skills.Insert(0, new SkillViewModel() { SkillID = -1, Name = "Select" });
                model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
                model.SkillList = new SelectList(skills, "SkillID", "Name");
                model.PublicationType = new SelectList(CommonData.EnumToDictionary(typeof(Enums.FindingType), "Select", "None"), "Key", "Value");
                return PartialView("_ManageFinding", model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Manage(AwardViewModel model, bool intelligence = false)
        {
            ModelState.Remove("Role");
            if (intelligence)
                ModelState.Remove("Description");

            if (ModelState.IsValid)
            {
                model.Type = (int)Enums.AwardAndAssignmentMode.Award;
                await Processor.Save(model);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var careerList = await CareerHistoryProcessor.GetCareerHisotry();
                var skills = await SkillProcessor.GetSkillHisotry();
                skills.Insert(0, new SkillViewModel() { SkillID = -1, Name = "Select" });
                model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
                model.SkillList = new SelectList(skills, "SkillID", "Name");
                model.MonthList = new SelectList(Month, "Key", "Value");
                model.YearList = new SelectList(Year, "Key", "Value");
                return PartialView("_ManageAwards", model);
            }
        }

        [CompressFilter]
        public async Task<ActionResult> ManageBusinessAward(string hash)
        {
            var model = new AwardViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                model = await Processor.GetOneAward(hash);
            }
            return PartialView("_ManageBusinessAwards", model);
        }

        [HttpPost]
        public async Task<ActionResult> ManageBusinessAward(AwardViewModel model)
        {
            ModelState.Remove("Role");
            if (ModelState.IsValid)
            {
                model.Type = (int)Enums.AwardAndAssignmentMode.Award;
                Processor.Save(model);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return PartialView("_ManageBusinessAwards", model);
            }
        }

        [CompressFilter]
        public async Task<ActionResult> ManageAssignment(string hash)
        {
            var model = new AwardViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                model = await Processor.GetOneAward(hash);
            }

            model.CareerHistoryList = new SelectList(await CareerHistoryList(), "Key", "Value");
            model.SkillList = new SelectList(await MySkills(), "SkillID", "Name");
            model.MonthList = new SelectList(Month, "Key", "Value");
            model.YearList = new SelectList(Year, "Key", "Value");
            return PartialView("_ManageAssignment", model);
        }

        [CompressFilter]
        [HttpPost]
        public async Task<ActionResult> RequestToAdd(AwardViewModel model)
        {
            if (ModelState.IsValid)
            {
                await Processor.SaveAddParticipantRequest(model);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var careerList = await CareerHistoryProcessor.GetCareerHisotry();
                var skills = await SkillProcessor.GetSkillHisotry();
                skills.Insert(0, new SkillViewModel() { SkillID = -1, Name = "Select" });
                model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
                model.SkillList = new SelectList(skills, "SkillID", "Name");
                model.MonthList = new SelectList(Month, "Key", "Value");
                model.YearList = new SelectList(Year, "Key", "Value");
                return PartialView("_RequestToAddToAssignment", model);
            }
        }

        [CompressFilter]
        public async Task<ActionResult> LoadOptions(string hash)
        {
            var actions = await Processor.LoadAssginementOptions(hash);
            return PartialView("_AssignmentMoreOptions", actions);
        }

        public async Task<ActionResult> RequestToAdd(string hash)
        {
            var model = new AwardViewModel();
            if (!string.IsNullOrEmpty(hash))
            {
                model = await Processor.GetBasicDetailsToAdd(hash);
            }

            var careerList = await CareerHistoryProcessor.GetCareerHisotry();
            var skills = await SkillProcessor.GetSkillHisotry();
            model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
            model.SkillList = new SelectList(skills, "SkillID", "Name");
            return PartialView("_RequestToAddToAssignment", model);
        }


        [HttpPost]
        public async Task<ActionResult> ManageGenericAssignment(AwardViewModel model)
        {
            Processor.Save(model);
            return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public async Task<ActionResult> ManageAssignment(AwardViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (byte)Enums.AwardAndAssignmentMode.Assignment;
                Processor.Save(model);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var careerList = await CareerHistoryProcessor.GetCareerHisotry();
                var skills = await SkillProcessor.GetSkillHisotry();
                skills.Insert(0, new SkillViewModel() { SkillID = -1, Name = "Select" });
                model.CareerHistoryList = new SelectList(careerList, "Key", "Value");
                model.SkillList = new SelectList(skills, "SkillID", "Name");
                return PartialView("_ManageAssignment", model);
            }
        }

        [HttpPost]
        public JsonResult RemoveSkill(string hash)
        {
            var result = Processor.RemoveSkill(hash);
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult RemoveParticipant(string hash)
        {
            var result = Processor.RemoveParticipant(hash);
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public async Task<ActionResult> Report(string q)
        {
            var model = await Processor.GetReportableAccomplishment(q);
            return PartialView("_ReportAccomplishment", model);
        }

        [HttpPost]
        public async Task<ActionResult> Report(AccomplishmentReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Processor.Report(model);
                return PartialView("_ActionResultMessage", result);
            }
            return PartialView("_ReportAccomplishment", model);
        }

        [HttpPost]
        public async Task<ActionResult> Congratulate(string q)
        {
            if (!string.IsNullOrEmpty(q))
            {
                var result = await Processor.Congratulate(q);
                return PartialView("_ActionResultMessage", result);
            }
            return new EmptyResult();
        }

        public async Task<PartialViewResult> RemoveConfirmation(string mode, string q)
        {
            ViewBag.Hash = q;
            ViewBag.Mode = mode;
            return PartialView("_RemoveConfirmation");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<JsonResult> Remove(string mode, string hash)
        {
            var result = await Processor.Remove(mode, hash);
            return WJson(result);
        }
    }
}