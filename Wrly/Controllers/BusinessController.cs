using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models;
using Wrly.Models.Business;
using Wrly.Models.Listing;
using WrlyInfrastucture.Filters;

namespace Wrly.Controllers
{

    public class BusinessController : BaseController
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

        IPressProcessor _IPressProcessor;
        IPressProcessor PressProcessor
        {
            get
            {
                if (_IPressProcessor == null)
                {
                    _IPressProcessor = new PressProcessor();
                }
                return _IPressProcessor;
            }
        }

        [CompressFilter]
        public ActionResult Register()
        {
            var model = new NewOrganizationViewModel() { Countries = new SelectList(Contries, "Key", "Value"), Industries = new SelectList(Industries, "Key", "Value"), EmployeeStrengths = new SelectList(EmployeeStrengths, "Value", "Value") };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Register(NewOrganizationViewModel model)
        {
            if (ModelState.IsValid)
            {
                string hash = string.Empty;
                var result = Processor.Save(model, out hash);
                if (result == Types.Enums.OrganizationSaveStatus.Success)
                    return RedirectToAction("success", new { hash = hash });
            }
            model.Countries = new SelectList(Contries, "Key", "Value");
            model.Industries = new SelectList(Industries, "Key", "Value");
            model.EmployeeStrengths = new SelectList(EmployeeStrengths, "Value", "Value");
            return View(model);
        }


        public ViewResult Success()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> UpdateName()
        {
            var model = await Processor.GetProfile();
            var result = new OrgNameViewModel()
            {
                Name = model.Name
            };
            return PartialView("_UpdateName", result);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> UpdateName(OrgNameViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Processor.SaveName(model);
                return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = result };
            }
            return null;
        }

        [Authorize]
        public async Task<ActionResult> UpdateCategory()
        {
            var model = await Processor.GetProfile();
            var result = new OrgCategoryViewModel()
            {
                CategoryList = new SelectList(Industries, "Key", "Value", model.CategoryID)
            };
            return PartialView("_UpdateCategory", result);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> UpdateCategory(OrgCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Processor.SaveCategory(model);
                return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = result };
            }
            return null;
        }


        public async Task<ActionResult> Profile(string profilename, string itemType)
        {
            if (Request.IsAuthenticated)
            {
                var profile = await Processor.GetProfileWithStates(profilename, true);
                profile.ProfileMode = Types.Enums.ProfileMode.Default;
                if (!string.IsNullOrEmpty(itemType))
                {
                    switch (itemType.ToLower())
                    {
                        case "timeline":
                            profile.Feed = await PressProcessor.TimeLineFeeds(profile.ProfileHash, 0, 10);
                            profile.ProfileMode = Types.Enums.ProfileMode.Feeds;
                            break;
                        case "connections":
                            profile.Connections = await AssociationProcessor.GetConnections(0, 100);
                            profile.ProfileMode = Types.Enums.ProfileMode.Connections;
                            break;
                        case "followers":
                            profile.Followers = await AssociationProcessor.GetFollowers(0, 100);
                            profile.ProfileMode = Types.Enums.ProfileMode.Followers;
                            break;
                        default:
                            break;
                    }
                }
                return View("PublicProfile", profile);
            }
            else
            {
                var profile = await Processor.GetOpenProfileWithStates(profilename);
                return View("OpenEntityProfile", profile);
            }
        }

        //[Authorize]
        public async Task<JsonResult> Search(string key, string id)
        {
            List<KeyValue> result = null;
            if (!string.IsNullOrEmpty(id) && id.Equals("University", StringComparison.InvariantCultureIgnoreCase))
            {
                result = await SearchUniversities(key);
                return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            result = await SearchOrganizations(key);
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> About(string hash)
        {
            var userModel = await Processor.GetAbout(hash);
            return PartialView("_ManageAbout", userModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> About(BusinessAboutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Processor.SaveAbout(model);
                return new JsonResult() { ContentType = "application/json", Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult() { ContentType = "application/json", Data = -1, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> editbasic()
        {
            var companyProfile = await Processor.BasicCompanyProfile();
            companyProfile.IndustryList = new SelectList(Industries, "Key", "Value", companyProfile.CategoryID);
            companyProfile.EmployeeStrengths = new SelectList(EmployeeStrengths, "Value", "Value", companyProfile.EmployeeStrength);
            return PartialView("_ManageBasic", companyProfile);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditBasic(BusinessProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Processor.SaveBasic(model);
                return PartialView("_ActionResultMessage", result);
            }
            return null;
        }


        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> editAddress()
        {
            var companyProfile = await Processor.GetAddress();
            companyProfile.Countries = new SelectList(Contries, "Key", "Key", companyProfile.Country);
            return PartialView("_ManageAddress", companyProfile);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> editAddress(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Processor.SetPrimaryAddress(model);
                return PartialView("_ActionResultMessage", result);
            }
            return null;
        }

        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> editPhone()
        {
            var companyProfile = await Processor.GetPhone();
            return PartialView("_ManagePhone", companyProfile);
        }

        //[Authorize]
        [CompressFilter]
        public async Task<JsonResult> Similar(string q)
        {
            var companies = await Processor.Similar(q);
            return WJson(companies);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> editPhone(PhoneViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Processor.SetPrimaryPhone(model);
                return PartialView("_ActionResultMessage", result);
            }
            return null;
        }

       
    }
}