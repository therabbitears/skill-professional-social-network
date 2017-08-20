using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models.Listing;
using WrlyInfrastucture.Filters;

namespace Wrly.Controllers
{
    //[Authorize]
    public class CareerHistoryController : BaseController
    {
        ICareerHistoryProcessor Processor;

        public CareerHistoryController()
            : this(new CareerHistoryProcessor())
        {

        }

        public CareerHistoryController(ICareerHistoryProcessor objProcessor)
        {
            Processor = objProcessor;
        }

        [CompressFilter]
        public async Task<ActionResult> Manage(string q)
        {
            var history = new CareerHistoryViewModel() { Type = (short)Enums.CareerHistoryMode.Education, IsCurrent = true };
            if (!string.IsNullOrEmpty(q))
            {
                history = await Processor.GetOneCareerHisotry(q);
                history.IsCurrent = history.EndFromMonth == null || history.EndFromMonth == -1;
                if (history.StartFromDay > 0 && history.StartFromMonth > 0 && history.StartFromYear > 0)
                {
                    history.DayList = new SelectList(FillDays(history.StartFromYear, history.StartFromMonth), "Key", "Value", history.StartFromDay);
                }
                if (history.EndFromDay > 0 && history.EndFromMonth > 0 && history.EndFromYear > 0)
                {
                    history.EndDayList = new SelectList(FillDays(history.EndFromYear, history.EndFromMonth), "Key", "Value", history.EndFromDay);
                }
            }
            else
            {
                history.IsCurrent = true;
            }
            history.MonthList = new SelectList(Month, "Key", "Value");
            history.YearList = new SelectList(Year, "Key", "Value");
            return PartialView("_ManageCareerHistory", history);
        }

        private Dictionary<int?, string> FillDays(int? year, int? month)
        {
            var dictionary = new Dictionary<int?, string>();
            for (var i = 1; i <= DateTime.DaysInMonth((int)year, (int)month); i++)
            {
                dictionary.Add((int)i, i.ToString());
            }
            return dictionary;
        }

        public async Task<JsonResult> SkillList(string keyword)
        {
            var skills = await MySkills(keyword);
            return new JsonResult() { Data = skills, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        [CompressFilter]
        public async Task<ActionResult> manageAffiliation(string q)
        {
            var history = new CareerHistoryViewModel() { Type = (short)Enums.CareerHistoryMode.Affiliation, SubType = "Affiliation" };
            if (!string.IsNullOrEmpty(q))
            {
                history = await Processor.GetOneCareerHisotry(q);
            }
            else
            {
                history.IsCurrent = true;
            }
            history.MonthList = new SelectList(Month, "Key", "Value");
            history.YearList = new SelectList(Year, "Key", "Value");
            return PartialView("_ManageAffiliationHistory", history);
        }



        [HttpPost]
        public async Task<ActionResult> manageAffiliation(CareerHistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (int)Enums.CareerHistoryMode.Affiliation;
                var result = await Processor.Save(model);
                return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                model.MonthList = new SelectList(Month, "Key", "Value");
                model.YearList = new SelectList(Year, "Key", "Value");
                return PartialView("_ManageAffiliationHistory", model);
            }
        }

        [CompressFilter]
        public async Task<ActionResult> ManageEducation(string q)
        {
            var history = new CareerHistoryViewModel() { Type = (short)Enums.CareerHistoryMode.Education, SubType = "Course" };
            if (!string.IsNullOrEmpty(q))
            {
                history = await Processor.GetOneCareerHisotry(q);
            }
            else
            {
                history.IsCurrent = true;
            }
            history.MonthList = new SelectList(Month, "Key", "Value");
            history.YearList = new SelectList(Year, "Key", "Value");
            return PartialView("_ManageEducationHistory", history);
        }



        [HttpPost]
        public async Task<ActionResult> ManageEducation(CareerHistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (int)Enums.CareerHistoryMode.Education;
                model.SubType = Enums.EducationType.Course.ToString();
                var result = await Processor.Save(model);
                return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                model.MonthList = new SelectList(Month, "Key", "Value");
                model.YearList = new SelectList(Year, "Key", "Value");
                return PartialView("_ManageEducationHistory", model);
            }
        }

        [CompressFilter]
        public async Task<ActionResult> ManageCertification(string q)
        {
            var history = new CareerHistoryViewModel() { Type = (short)Enums.CareerHistoryMode.Education };
            if (!string.IsNullOrEmpty(q))
            {
                history = await Processor.GetOneCareerHisotry(q);
                history.IsCurrent = history.HasValidEndDate;
            }
            else
            {
                history.IsCurrent = false;
            }
            history.MonthList = new SelectList(Month, "Key", "Value");
            history.YearList = new SelectList(Year, "Key", "Value");
            history.ComingYearList = new SelectList(Coming30Years, "Key", "Value");
            return PartialView("_ManageCertificationHistory", history);
        }



        [HttpPost]
        public async Task<ActionResult> ManageCertification(CareerHistoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Type = (int)Enums.CareerHistoryMode.Education;
                model.SubType = Enums.EducationType.Certification.ToString();
                await Processor.Save(model);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                model.MonthList = new SelectList(Month, "Key", "Value");
                model.YearList = new SelectList(Year, "Key", "Value");
                return PartialView("_ManageCertificationHistory", model);
            }
        }


        [HttpPost]
        public async Task<ActionResult> Manage(CareerHistoryViewModel model, bool wizard = false)
        {
            if (ModelState.IsValid)
            {
                model.Type = (int)Enums.CareerHistoryMode.Profession;
                await Processor.Save(model, wizard);
                return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                model.MonthList = new SelectList(Month, "Key", "Value");
                model.YearList = new SelectList(Year, "Key", "Value");
                return PartialView("_ManageCareerHistory", model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ManageIntelligence(CareerHistoryViewModel model, bool wizard = false)
        {
            model.Type = (int)Enums.CareerHistoryMode.Profession;
            await Processor.Save(model, wizard);
            return new JsonResult() { Data = 1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        [CompressFilter]
        public async Task<ActionResult> Career()
        {
            var history = await Processor.GetCareerHisotry((int)Enums.CareerHistoryMode.Profession);
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> Affiliation()
        {
            var history = await Processor.GetCareerHisotry((int)Enums.CareerHistoryMode.Affiliation);
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> Education()
        {
            var history = await Processor.GetCareerHisotry((int)Enums.CareerHistoryMode.Education, Enums.EducationType.Course.ToString());
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> Certification()
        {
            var history = await Processor.GetCareerHisotry((int)Enums.CareerHistoryMode.Education, Enums.EducationType.Certification.ToString());
            return new JsonResult() { ContentType = "application/json", Data = history, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        public async Task<JsonResult> RemoveSkill(string hash)
        {
            var result = await Processor.RemoveSkill(hash);
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public async Task<JsonResult> SearchJobTitle(string key)
        {
            var result = await JobTitles(key);
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public async Task<JsonResult> SearchCourse(string key)
        {
            var result = await SearchCourses(key);
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        public async Task<JsonResult> Add(long id, string name)
        {
            var history = new CareerHistoryViewModel() { Type = (short)Enums.CareerHistoryMode.Profession, JobTitleName = name, JobTitleID = id, EntityID = UserHashObject.EntityID };
            var result = await Processor.Save(history);
            if (result > 0)
            {
                var data = await Processor.GenerateTokenForCareerHistoryForEntity(result);
                return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult() { Data = -1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> LoadOptions(string q)
        {
            var actions = await Processor.LoadAssginementOptions(q);
            return PartialView("_CareerHistoryMoreOptions", actions);
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

        public async Task<ActionResult> GetDataForOpportunity(long? eid )
        {
            var result = await Processor.GetDataForOpportunity(eid);
            return PartialView("_OpportunityData",result);
        }
    }
}