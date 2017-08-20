using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Infrastructure.Processors.Implementations;
using Types;
using Wrly.Models.Listing;
using Wrly.Models;

namespace Wrly.Controllers
{
    public class WizardController : BaseController
    {
        IWizardProcessor _Processor;
        public IWizardProcessor Processor
        {
            get
            {
                if (_Processor == null)
                {
                    _Processor = new WizardProcessor();
                }
                return _Processor;
            }
        }

        IBusinessProcessor _BusinessProcessor;
        IBusinessProcessor BusinessProcessor
        {
            get
            {
                if (_BusinessProcessor == null)
                {
                    _BusinessProcessor = new BusinessProcessor();
                }
                return _BusinessProcessor;
            }
        }

        ISkillHistoryProcessor _SkillHistoryProcessor;
        public ISkillHistoryProcessor SkillHistoryProcessor
        {
            get
            {
                if (_SkillHistoryProcessor == null)
                {
                    _SkillHistoryProcessor = new SkillHistoryProcessor();
                }
                return _SkillHistoryProcessor;
            }
        }

        ICareerHistoryProcessor _CareerProcessor;
        public ICareerHistoryProcessor CareerProcessor
        {
            get
            {
                if (_CareerProcessor == null)
                {
                    _CareerProcessor = new CareerHistoryProcessor();
                }
                return _CareerProcessor;
            }
        }

        IAccountProcessor _AccountProcessor;
        public IAccountProcessor AccountProcessor
        {
            get
            {
                if (_AccountProcessor == null)
                {
                    _AccountProcessor = new AccountProcessor();
                }
                return _AccountProcessor;
            }
        }

        IAssociationProcessor _AssociationProcessor;
        IAssociationProcessor AssociationProcessor
        {
            get
            {
                if (_AssociationProcessor == null)
                {
                    _AssociationProcessor = new AssociationProcessor();
                }
                return _AssociationProcessor;
            }
        }

        public async Task<ActionResult> SetSkiilsPartial()
        {
            ViewBag.IsPopup = true;
            ViewBag.SkipButtonText = "Save";
            var model = new SkillSnapShotViewModel()
            {
                Stats = await AccountProcessor.SnapShot(),
                Skills = await SkillHistoryProcessor.GetSkillHisotry()
            };
            return PartialView("_SetSkills", model);

        }

        public async Task<ActionResult> SetSkills(string hash)
        {
            if (!string.IsNullOrEmpty(hash))
            {
                if (await Processor.IsValidStep(Enums.WizardStep.AddSkills, hash))
                {
                    var model = new SkillSnapShotViewModel()
                    {
                        Stats = await AccountProcessor.SnapShot(),
                        Skills = await SkillHistoryProcessor.GetSkillHisotry()
                    };
                    return View(model);
                }
                else
                {
                    var step = await Processor.GetStepToComplete();
                    if (step == Enums.WizardStep.VarifyEmail)
                    {
                        hash = await Processor.GetHashForWizard(Enums.WizardStep.VarifyEmail);
                        return RedirectToAction("VarifyEmail", "Wizard", new { hash = hash, setup = "email", mode = "varify", stamp = DateTime.UtcNow.Ticks });
                    }
                    if (step == Enums.WizardStep.AddCareerHistory)
                    {
                        hash = await Processor.GetHashForWizard(Enums.WizardStep.AddCareerHistory);
                        return RedirectToAction("SetCareerOption", new { hash = hash, setup = "cs", mode = "email", stamp = DateTime.UtcNow.Ticks });
                    }
                    return Redirect("/feed?rds=wzd");
                }
            }
            else
            {
                hash = await Processor.GetHashForWizard(Enums.WizardStep.AddSkills);
                return RedirectToAction("SetSkills", new { hash = hash, setup = "skill", mode = "next", stamp = DateTime.UtcNow.Ticks });
            }
        }

        public async Task<ActionResult> SetExtednedInformation()
        {
            var model = new ExtendedInfoViewModel()
            {
                EmployeeStrengths = new SelectList(EmployeeStrengths, "Value", "Value")
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetExtednedInformation(ExtendedInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await BusinessProcessor.AddExtendedInformation(model);
                if (result > 0)
                    return RedirectToAction("Feedback");
            }
            model.EmployeeStrengths = new SelectList(EmployeeStrengths, "Value", "Value");
            return View(model);
        }

        public async Task<ActionResult> SetAddress(string hash)
        {
            var address = await BusinessProcessor.GetPrimaryAddress();
            address.Countries = new SelectList(Contries, "Key", "Value");
            return View(address);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetAddress(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var address = await BusinessProcessor.SetPrimaryAddress(model);
                return RedirectToAction("SetExtednedInformation", new { setup = "ex", mode = "extended", stamp = DateTime.UtcNow.Ticks });
            }
            else
            {
                model.Countries = new SelectList(Contries, "Key", "Value");
                return View(model);
            }
        }

        public async Task<ActionResult> AddConnections()
        {
            var result = await AssociationProcessor.FeedIntialNetworkData();
            if (result != null && result.Type == Enums.ResultType.Success)
            {
                result = await AssociationProcessor.PrepareStock();
                if (result != null && result.Type == Enums.ResultType.Success)
                {
                    var requests = await AssociationProcessor.GetRequests(0, 5, Enums.AssociationRequestDirection.Received);
                    //var suggestions = await AssociationProcessor.GetSuggestions(0, 5);
                    //if ((requests != null && requests.Count > 0) || (suggestions != null && suggestions.Count > 0))
                    if ((requests != null && requests.Count > 0))
                    {
                        //ViewBag.Suggestions = suggestions;
                        ViewBag.Requests = requests;
                        return View(result);
                    }
                }
                return RedirectToAction("Feedback");
            }
            return RedirectToAction("Feedback");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> AddConnections(string value)
        {
            await Processor.FinishWizard();
            return RedirectToAction("Feedback");
        }

        public async Task<ActionResult> Feedback()
        {
            var snapshot = await AccountProcessor.SnapShot();
            return View(snapshot);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Feedback(string feedback, string action)
        {
            if (!string.IsNullOrEmpty(action) && action.Equals("send", StringComparison.InvariantCultureIgnoreCase))
            {
                await Processor.Feeback(feedback);
            }
            return Redirect("/feed?rds=wzd");
        }

        public async Task<ActionResult> ProcessVarification(string hash)
        {
            if (!string.IsNullOrEmpty(hash))
            {
                if (await Processor.IsValidVarification(hash))
                {
                    var result = await Processor.VarifyEmail(hash);
                    if (result == Enums.VarificationResult.Varified)
                    {
                        if (Request.IsAuthenticated)
                        {
                            return RedirectToAction("AddConnections");
                        }
                        return RedirectToAction("Login", "Account", new { src = "evn" });

                    }
                    return View("IncorrectData");
                }
                else
                {
                    var result = await Processor.VarificationStatus(hash);
                    if (result == Enums.VarificationStatus.AlreadyVarified)
                    {
                        if (await Processor.IsValidStep(Enums.WizardStep.AddConnections, hash))
                        {
                            if (Request.IsAuthenticated)
                            {
                                return RedirectToAction("AddConnections");
                            }
                            return RedirectToAction("Login", "Account", new { src = "evn" });
                        }
                    }
                    if (result == Enums.VarificationStatus.InvalidLink)
                    {
                        return View("IncorrectData");
                    }
                }
            }
            return Redirect("/feed?rds=wzd");
        }

        public async Task<ActionResult> VarifyEmail(string hash)
        {
            if (!string.IsNullOrEmpty(hash))
            {
                if (await Processor.IsValidStep(Enums.WizardStep.AddSkills, hash))
                {
                    if (!await AccountProcessor.IsEmailVerified())
                    {
                        await AccountProcessor.SendVarification(hash, true);
                        var model = new SkillSnapShotViewModel()
                        {
                            Stats = await AccountProcessor.SnapShot(),
                            ProfileSnap = await AccountProcessor.GetProfileWithStates(User.Identity.Name, false)
                        };
                        return View(model);
                    }
                    else
                        return RedirectToAction("AddConnections");
                }
                else
                {
                    var step = await Processor.GetStepToComplete();
                    if (step == Enums.WizardStep.AddSkills)
                    {
                        hash = await Processor.GetHashForWizard(Enums.WizardStep.VarifyEmail);
                        return RedirectToAction("SetSkills", new { hash = hash, setup = "skill", mode = "add", stamp = DateTime.UtcNow.Ticks });
                    }
                    if (step == Enums.WizardStep.AddCareerHistory)
                    {
                        hash = await Processor.GetHashForWizard(Enums.WizardStep.AddCareerHistory);
                        return RedirectToAction("SetCareerOption", new { hash = hash, setup = "cs", mode = "email", stamp = DateTime.UtcNow.Ticks });
                    }
                    return Redirect("/feed?rds=wzd");
                }
            }
            else
            {
                hash = await Processor.GetHashForWizard(Enums.WizardStep.AddSkills);
                return RedirectToAction("VarifyEmail", new { hash = hash, setup = "skill", mode = "next", stamp = DateTime.UtcNow.Ticks });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> SetSkills(string hash, ListSkillViewModel model, bool ispopup = false)
        {
            var result = await SkillHistoryProcessor.SaveWizard(model, ispopup);
            var redirectResult = new WizardResultViewModel();
            if (result > 0)
            {
                redirectResult = await Processor.GetWizardResultData(Enums.WizardStep.VarifyEmail);
            }
            return new JsonResult() { Data = redirectResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public async Task<ActionResult> SetCareerOption(string hash)
        {
            if (!string.IsNullOrEmpty(hash))
            {
                if (await Processor.IsValidStep(Enums.WizardStep.AddCareerHistory, hash))
                {
                    var careerHistoryWizard = new CareerHistoryWizardViewModel()
                    {
                        Hash = hash,
                        CareerStage = (int)Enums.CareerStage.Employement,
                        EmployementEndedStage = (int)Enums.EmployementEndedStage.LookingOpportunity,
                        Working = true,
                    };
                    careerHistoryWizard.MonthList = new SelectList(Month, "Key", "Value");
                    careerHistoryWizard.YearList = new SelectList(Year, "Key", "Value");
                    return View("CareerHistoryWizard", careerHistoryWizard);
                }
                else
                {
                    var step = await Processor.GetStepToComplete();
                    if (step == Enums.WizardStep.VarifyEmail)
                    {
                        hash = await Processor.GetHashForWizard(Enums.WizardStep.VarifyEmail);
                        return RedirectToAction("VarifyEmail", "Wizard", new { hash = hash, setup = "email", mode = "varify", stamp = DateTime.UtcNow.Ticks });
                    }
                    if (step == Enums.WizardStep.AddSkills)
                    {
                        hash = await Processor.GetHashForWizard(Enums.WizardStep.AddSkills);
                        return RedirectToAction("SetSkills", new { hash = hash, setup = "skill", mode = "next", stamp = DateTime.UtcNow.Ticks });
                    }

                }
                return Redirect("/feed?rds=wzd");
            }
            else
            {
                hash = await Processor.GetHashForWizard(Enums.WizardStep.AddCareerHistory);
                return RedirectToAction("SetCareerOption", new { hash = hash, setup = "cs", mode = "email", stamp = DateTime.UtcNow.Ticks });
            }
        }

        public async Task<ActionResult> SetCareerOptionPartial(string hash)
        {
            hash = await Processor.GetHashForWizard(Enums.WizardStep.AddCareerHistory);
            var careerHistoryWizard = new CareerHistoryWizardViewModel()
            {
                Hash = hash,
                CareerStage = (int)Enums.CareerStage.Employement,
                EmployementEndedStage = (int)Enums.EmployementEndedStage.LookingOpportunity,
                Working = true,
            };
            careerHistoryWizard.MonthList = new SelectList(Month, "Key", "Value");
            careerHistoryWizard.YearList = new SelectList(Year, "Key", "Value");
            ViewBag.IsPopup = true;
            return PartialView("_WizardSetCareer", careerHistoryWizard);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> SetCareerOption(CareerHistoryWizardViewModel careerHistoryWizard, bool ispopup)
        {
            SetModelState(careerHistoryWizard);
            if (ModelState.IsValid)
            {
                var result = await CareerProcessor.SaveWizard(careerHistoryWizard, ispopup);
                var redirectResult = new WizardResultViewModel();
                if (result > 0)
                {
                    redirectResult = await Processor.GetWizardResultData(Enums.WizardStep.AddSkills);
                }
                return new JsonResult() { Data = redirectResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            careerHistoryWizard.MonthList = new SelectList(Month, "Key", "Value");
            careerHistoryWizard.YearList = new SelectList(Year, "Key", "Value");
            return View("CareerHistoryWizard", careerHistoryWizard);
        }

        public async Task<ActionResult> Employeement()
        {
            var careerHistoryWizard = new CareerHistoryWizardViewModel()
            {
                EmployementEndedStage = (int)Enums.EmployementEndedStage.OnABreak,
                Working = true
            };
            careerHistoryWizard.MonthList = new SelectList(Month, "Key", "Value");
            careerHistoryWizard.YearList = new SelectList(Year, "Key", "Value");
            return PartialView("_CareerWizardEmployee", careerHistoryWizard);
        }


        public async Task<ActionResult> Student()
        {
            var careerHistoryWizard = new CareerHistoryWizardViewModel()
            {
                EmployementEndedStage = (int)Enums.EmployementEndedStage.OnABreak,
                Working = false
            };
            careerHistoryWizard.MonthList = new SelectList(Month, "Key", "Value");
            careerHistoryWizard.YearList = new SelectList(Year, "Key", "Value");
            return PartialView("_CareerWizardStudent", careerHistoryWizard);
        }



        private void SetModelState(CareerHistoryWizardViewModel careerHistoryWizard)
        {
            if (careerHistoryWizard.CareerStage == (int)Enums.CareerStage.Employement)
            {
                ModelState.Remove("UniversityName");
                ModelState.Remove("CourseName");
                ModelState.Remove("EducationStartFromMonth");
                ModelState.Remove("EducationStartFromYear");
                ModelState.Remove("EducationEndFromMonth");
                ModelState.Remove("EducationEndFromYear");
                if (careerHistoryWizard.OrganizationID > 0)
                    ModelState.Remove("IndustryName");
            }
            else
            {
                ModelState.Remove("OrganizationName");
                ModelState.Remove("IndustryName");
                ModelState.Remove("JobTitleName");
                ModelState.Remove("StartFromMonth");
                ModelState.Remove("StartFromYear");
                ModelState.Remove("EndFromMonth");
                ModelState.Remove("EndFromYear");
            }
        }
    }
}