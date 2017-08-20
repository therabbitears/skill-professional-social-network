#region '---- Includes ---- '
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models;
using Wrly.Infrastructure.Processors.Implementations;
using Types;
using Wrly.infrastuctures.Utils;
using WrlyInfrastucture.Filters;
using Wrly.Models.Listing;
using Wrly.Infrastuctures.Utils;
using Wrly.Infrastructure.Utils;
using Wrly.Infrastructure.Extended;
using Microsoft.AspNet.Identity.Owin;
using Wrly.Infrastructure;
#endregion
namespace Wrly.Controllers
{
    public class AccountController : BaseController
    {
        #region '---- Members ----'

        IAccountProcessor _IAccountProcessor;
        public UserManager<ApplicationUser> UserManager { get; private set; }

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


        ISkillHistoryProcessor _ISkillHistoryProcessor;
        ISkillHistoryProcessor SkillProcessor
        {
            get
            {
                if (_ISkillHistoryProcessor == null)
                {
                    _ISkillHistoryProcessor = new SkillHistoryProcessor();
                }
                return _ISkillHistoryProcessor;
            }
        }


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

        #endregion

        #region '---- Constructors ----'

        public AccountController()
            : this(new AccountProcessor(), new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {

        }



        public AccountController(IAccountProcessor objIAccountProcessor, UserManager<ApplicationUser> userManager)
        {
            _IAccountProcessor = objIAccountProcessor;
            UserManager = userManager;
            UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
        }

        #endregion

        #region ' ---- Method(s) ---- '

        [CompressFilter]
        [Authorize]
        public async Task<ActionResult> userstatistics()
        {
            var statistics = await _IAccountProcessor.GetStatistics();
            return new JsonResult() { ContentType = "application/json", Data = statistics, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> UploadProfileImage(ProfilePicViewModel model)
        {
            if (model.Image.IsImageFile())
            {
                var result = await _IAccountProcessor.UploadProfileImage(model);
                return WJson(result);
            }
            return WJson(new Result() { Type = Enums.ResultType.Error, Description = "Invalid image data." });
        }


        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> UploadCoverImage(ProfilePicViewModel model)
        {
            if (model.Image.IsImageFile())
            {
                var result = await _IAccountProcessor.UploadCoverImage(model);
                return WJson(result);
            }
            return WJson(new Result() { Type = Enums.ResultType.Error, Description = "Invalid image data." });
        }


        [Authorize]
        [HttpPost]
        public ActionResult DeleteProfileImage(string uid)
        {
            _IAccountProcessor.DeleteProfileImage(uid);
            return Redirect(Request.UrlReferrer.ToString());
        }

        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> About(string hash, bool ispopup=false)
        {
            ViewBag.IsPopup = ispopup;
            var userModel = await _IAccountProcessor.GetAbout(hash);
            return PartialView("_ManageAbout", userModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> About(AboutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _IAccountProcessor.SaveAbout(model);
                return Content(model.FormatedProfileSummary);
            }
            return new JsonResult() { ContentType = "application/json", Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> careerline(string id, bool includeS = true, bool includeA = true)
        {
            var data = await _IAccountProcessor.GetCareerLine(id, includeS, includeA);
            return new JsonResult() { ContentType = "application/json", Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl, string src)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!string.IsNullOrEmpty(src))
            {
                if (src.Equals("evn", StringComparison.InvariantCultureIgnoreCase))
                {
                    ViewBag.SuccessMessage = "Email has been verified successfully, please login to continue.";
                }
            }
            return View();
        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Authentication(string returnUrl, string load)
        {
            if (!string.IsNullOrEmpty(load))
            {
                if (load.Equals("register"))
                {
                    ViewBag.ReturnUrl = returnUrl;
                    var registerModel = new RegisterViewModel();
                    return PartialView("_AuthenticationRegister", registerModel);
                }
                else
                {
                    ViewBag.ReturnUrl = returnUrl;
                    var registerModel = new LoginViewModel();
                    return PartialView("_AuthenticationLogin", registerModel);
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            var model = new LoginViewModel();
            return PartialView("_Authentication", model);
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Home(string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                if (EntityType == (int)Enums.EntityTypes.Person)
                {
                    var findByName = User.Identity.Name;
                    var profile = await _IAccountProcessor.GetProfileWithStates(findByName, false);
                    profile.AllowEdit = User.Identity.Name.Equals(profile.UserName, StringComparison.InvariantCultureIgnoreCase);
                    profile.Feed = await PressProcessor.Feeds(0, DateTime.UtcNow);
                    return View(profile);
                }
                if (EntityType == (int)Enums.EntityTypes.Organization)
                {
                    var profile = await Processor.GetProfileWithStates();
                    profile.AllowEdit = User.Identity.Name.Equals(profile.UserName, StringComparison.InvariantCultureIgnoreCase);
                    profile.Feed = await PressProcessor.Feeds(0, DateTime.UtcNow);
                    return View("BusinessHome", profile);
                }
            }   
            ViewBag.ReturnUrl = returnUrl;
            return View("Index");
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Wrly.Models.LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    if (!Request.IsAjaxRequest())
                        return RedirectToLocal(returnUrl);
                    else
                        return WJson(new Result() { Type = Enums.ResultType.Success, Description = "User authenticated successfully." });
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password entered, please note passwords are case sensitive.");
                }
            }
            if (!Request.IsAjaxRequest())
            {
                // If we got this far, something failed, redisplay form
                return View(model);
            }
            else
            {
                var stringError = "<ul>";
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                        stringError += string.Format("<li>{0}</li>", item.Value.Errors[0].ErrorMessage);
                }
                stringError += "</ul>";
                return WJson(new Result() { Type = Enums.ResultType.Warning, Description = stringError });
            }
        }

        [Authorize]
        public ActionResult ConnectAccount(string hash)
        {
            var model = hash.ToObject<ConnectAccountViewModel>(null);
            model.RegisterInfo = new RegisterViewModel();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ConnectAccount(string hash, ConnectAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser()
                {
                    UserName = model.RegisterInfo.EmailAddress,
                    EmailAddress = model.RegisterInfo.EmailAddress,
                    FirstName = !string.IsNullOrEmpty(model.RegisterInfo.FullName) && model.RegisterInfo.FullName.Split(' ').Length > 1 ? model.RegisterInfo.FullName.Split(' ')[0] : model.RegisterInfo.FullName,
                    LastName = !string.IsNullOrEmpty(model.RegisterInfo.FullName) && model.RegisterInfo.FullName.Split(' ').Length > 1 ? model.RegisterInfo.FullName.Split(' ')[1] : string.Empty,
                    Status = (int)Enums.UserAccountStatus.Registered
                };
                var userInfo = await UserManager.CreateAsync(applicationUser);
                if (userInfo.Succeeded)
                {
                    _IAccountProcessor.AddUserExtendedInfoWithJobRole(applicationUser, model);
                }
            }
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> UpdateName()
        {
            var model = await _IAccountProcessor.GetProfile(User.Identity.Name);
            var result = new NameViewModel()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                NameFormat = model.NameFormat,
                Formats = new SelectList(CommonData.EnumToDictionary(typeof(Enums.PersonNameFormat), isStringDefault: false), "Key", "Value", model.NameFormat)
            };
            return PartialView("_UpdateName", result);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> UpdateName(NameViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _IAccountProcessor.SaveName(model);
                return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = result };
            }
            return null;
        }

        [Authorize]
        public async Task<ActionResult> UpdateHeading()
        {
            var model = await _IAccountProcessor.GetProfile(User.Identity.Name);
            var result = new HeadingViewModel()
            {
                Heading = model.FormatedJobTitle
            };
            return PartialView("_UpdateHeading", result);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> UpdateHeading(HeadingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _IAccountProcessor.SaveHeading(model);
                return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = result };
            }
            return null;
        }

        [Authorize]
        public ActionResult Update(string SaveFor, string mode)
        {
            var values = GetValues(SaveFor);
            var result = _IAccountProcessor.UpdateProfile(values, mode);
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private Dictionary<string, string> GetValues(string SaveFor)
        {
            var values = new Dictionary<string, string>();
            var parameters = SaveFor.Split(',');
            foreach (var item in parameters)
            {
                values.Add(item, Request.Params[item]);
            }
            return values;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(Wrly.Models.RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = model.EmailAddress,
                    EmailAddress = model.EmailAddress,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Status = (int)Enums.UserAccountStatus.Registered,
                    Type = (int)Enums.EntityTypes.Person
                };
                var result = await CreateUser(user, model.Password);
                if (result.Item2)
                {
                    _IAccountProcessor.SendWelcomeEmail(model);
                    await SignInAsync(user, isPersistent: false);
                    // If we got this far, something failed, redisplay form
                    if (!Request.IsAjaxRequest())
                        return RedirectToAction("SetCareerOption", "Wizard", new { setup = "cs", mode = "email", stamp = DateTime.UtcNow.Ticks });
                    else
                        return WJson(new Result() { Type = Enums.ResultType.Success, Description = "User registered successfully." });
                }
                else
                {
                    AddErrors(result.Item1);
                }
            }
            // If we got this far, something failed, redisplay form
            if (!Request.IsAjaxRequest())
                return View(model);
            else
            {
                var stringError = "<ul>";
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                        stringError += string.Format("<li>{0}</li>", item.Value.Errors[0].ErrorMessage);
                }
                stringError += "</ul>";
                return WJson(new Result() { Type = Enums.ResultType.Warning, Description = stringError });
            }

        }


        [CompressFilter]
        public ActionResult RegisterOrganization()
        {
            var model = new OrganizationSignupViewModel() { Industries = new SelectList(Industries, "Key", "Value") };
            return View(model);
        }


        [CompressFilter]
        public async Task<ActionResult> Forgot()
        {
            var model = new ForgotPasswordViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Forgot(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _IAccountProcessor.ForgottenPasswordEmail(model);
                if (result.Type == Enums.ResultType.Success)
                    return View("ForgottenPasswordMailSent");

                ModelState.AddModelError("EmailAddress", result.Description);
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterOrganization(OrganizationSignupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = model.EmailAddress,
                    EmailAddress = model.EmailAddress,
                    FirstName = model.Name,
                    Status = (int)Enums.UserAccountStatus.Registered,
                    Type = (int)Enums.EntityTypes.Organization
                };
                var result = await CreateOrganizationUser(user, model.Password, model);
                if (result.Item2)
                {
                    await _IAccountProcessor.SendOrganizationWelcomeEmail(model);
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("SetAddress", "Wizard", new { setup = "add", mode = "adress", stamp = DateTime.UtcNow.Ticks });
                }
                else
                {
                    AddErrors(result.Item1);
                }
            }
            model.Industries = new SelectList(Industries, "Key", "Value");
            return View(model);
        }

        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> Profile(string userName, string id)
        {
            if (EntityType == (int)Enums.EntityTypes.Person)
            {
                var isUserName = !string.IsNullOrEmpty(userName);
                var findByName = userName ?? User.Identity.Name;
                var profile = await _IAccountProcessor.GetProfileWithStates(findByName, isUserName);
                profile.ProfileMode = Types.Enums.ProfileMode.Default;
                if (!string.IsNullOrEmpty(id))
                {
                    if (id.Equals("timeline"))
                    {
                        profile.Feed = await PressProcessor.TimeLineFeeds(profile.ProfileHash, 0, 10);
                        profile.ProfileMode = Types.Enums.ProfileMode.Feeds;
                    }

                    if (id.Equals("connections", StringComparison.InvariantCultureIgnoreCase))
                    {
                        profile.Connections = await AssociationProcessor.GetConnections(0, 100);
                        profile.ProfileMode = Types.Enums.ProfileMode.Connections;
                    }

                    if (id.Equals("followers", StringComparison.InvariantCultureIgnoreCase))
                    {
                        profile.Followers = await AssociationProcessor.GetFollowers(0, 100);
                        profile.ProfileMode = Types.Enums.ProfileMode.Followers;
                    }

                    if (id.Equals("following", StringComparison.InvariantCultureIgnoreCase))
                    {
                        profile.Followers = await AssociationProcessor.GetFollowings(0, 100);
                        profile.ProfileMode = Types.Enums.ProfileMode.Following;
                    }
                }
                return View(profile);
            }
            if (EntityType == (int)Enums.EntityTypes.Organization)
            {
                var profile = await Processor.GetProfileWithStates();
                profile.ProfileMode = Types.Enums.ProfileMode.Default;
                if (!string.IsNullOrEmpty(id))
                {
                    if (id.Equals("timeline"))
                    {
                        profile.Feed = await PressProcessor.TimeLineFeeds(profile.ProfileHash, 0, 10);
                        profile.ProfileMode = Types.Enums.ProfileMode.Feeds;
                    }

                    if (id.Equals("connections", StringComparison.InvariantCultureIgnoreCase))
                    {
                        profile.Connections = await AssociationProcessor.GetConnections(0, 100);
                        profile.ProfileMode = Types.Enums.ProfileMode.Connections;
                    }

                    if (id.Equals("followers", StringComparison.InvariantCultureIgnoreCase))
                    {
                        profile.Followers = await AssociationProcessor.GetFollowers(0, 100);
                        profile.ProfileMode = Types.Enums.ProfileMode.Followers;
                    }

                    if (id.Equals("following", StringComparison.InvariantCultureIgnoreCase))
                    {
                        profile.Followers = await AssociationProcessor.GetFollowings(0, 100);
                        profile.ProfileMode = Types.Enums.ProfileMode.Following;
                    }
                }
                return View("BusinessProfile", profile);
            }
            return null;
        }

        [CompressFilter]
        public async Task<ActionResult> EntityProfile(string profileName, string itemType)
        {
            if (Request.IsAuthenticated)
            {
                var profile = await _IAccountProcessor.GetProfileWithStates(profileName, true, true);
                if (profile != null)
                {
                    if (!string.IsNullOrEmpty(itemType))
                    {
                        switch (itemType.ToLower())
                        {
                            case "timeline":
                                profile.Feed = await PressProcessor.TimeLineFeeds(profile.ProfileHash, 0, 10);
                                profile.ProfileMode = Types.Enums.ProfileMode.Feeds;
                                break;
                            //return View("EntityTimeLine", profile);
                            case "followers":
                                profile.Followers = await AssociationProcessor.GetFollowers(profile.EntityID, 0, 10);
                                profile.ProfileMode = Types.Enums.ProfileMode.Followers;
                                break;
                            //return View("EntityFollowers", profile);
                            case "connections":
                                profile.Connections = await AssociationProcessor.GetConnections(profile.EntityID, 0, 10);
                                profile.ProfileMode = Types.Enums.ProfileMode.Connections;
                                break;
                            //return View("EntityConnections", profile);
                        }
                    }
                    return View(profile);
                }
                return View("IncorrectProfile");
            }
            else
            {
                var profile = await _IAccountProcessor.GetOpenProfileWithStates(profileName);
                if (profile != null)
                {
                    return View("OpenEntityProfile", profile);
                }
                return View("IncorrectProfile");
            }
        }

        [CompressFilter]
        public async Task<PartialViewResult> Timeline(string q, bool group = true, int pageNumber = 0, int pageSize = 10)
        {
            var feeds = await PressProcessor.TimeLineFeeds(q, pageNumber, pageSize, group);
            return PartialView("_Feeds", feeds.Feeds);
        }


        [Authorize]
        public async Task<ActionResult> ManageProfile(string userName, string tab)
        {
            var isUserName = !string.IsNullOrEmpty(userName);
            var findByName = userName ?? User.Identity.Name;
            var profile = await _IAccountProcessor.GetProfileWithStates(findByName, isUserName);
            if (User.Identity.Name.Equals(profile.UserName, StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(tab))
                {
                }
                return View(profile);
            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);
        }

        public async Task<ActionResult> PartialProfile(string userName, string viewName)
        {
            var profile = await _IAccountProcessor.GetProfileWithStates(userName);
            return new JsonResult() { Data = profile, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }



        //
        // POST: /Account/Manage
        public async Task<ActionResult> Reset(string hash)
        {
            var result = await _IAccountProcessor.GetUserInformationByForgotLink(hash);
            if (result.Type == Enums.ResultType.Success)
            {
                var model = new ResetPasswordViewModel() { Hash = hash };
                return View(model);
            }
            return View("InvalidLink");

        }
        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Reset(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var profileViewModel = model.Hash.ToObject<ProfileViewModel>(null);
                UserManager.RemovePassword(profileViewModel.UserID);

                var result = UserManager.AddPassword(profileViewModel.UserID, model.NewPassword);
                if (result.Succeeded)
                {
                    await _IAccountProcessor.RemoveForgotPasswordData(model.Hash);
                    return RedirectToAction("Login", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                else
                {
                    ModelState.AddModelError("Error", result.Errors.FirstOrDefault());
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize]
        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            SetManagePasswordData(message);
            return View();
        }

        [Authorize]
        //
        // GET: /Account/Manage
        public async Task<ActionResult> ManagePartial(ManageMessageId? message)
        {
            SetManagePasswordData(message);
            return PartialView("_ManagePasswordPartial");
        }

        private void SetManagePasswordData(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        if (!Request.IsAjaxRequest())
                            return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                        else
                            return PartialView("_ActionResultMessage", new Result() { Type = Enums.ResultType.Success, Description = "Your password has been changed." });
                    }
                    else
                    {
                        AddErrors(result);
                        if (Request.IsAjaxRequest())
                            return PartialView("_ActionResultMessage", new Result() { Type = Enums.ResultType.Error });
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        if (!Request.IsAjaxRequest())
                            return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                        else
                            return PartialView("_ActionResultMessage", new Result() { Type = Enums.ResultType.Success, Description = "Your password has been set." });
                    }
                    else
                    {
                        AddErrors(result);
                        if (Request.IsAjaxRequest())
                            return PartialView("_ActionResultMessage", new Result() { Type = Enums.ResultType.Error });
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }


        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            if (result == null || result.Identity == null)
            {
                return RedirectToAction("Login");
            }

            var idClaim = result.Identity.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                return RedirectToAction("Login");
            }

            var login = new UserLoginInfo(idClaim.Issuer, idClaim.Value);
            var name = result.Identity.Name == null ? "" : result.Identity.Name.Replace(" ", "");

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                string firstName = string.Empty;
                string lastName = string.Empty;
                string email = string.Empty;
                var emailClaim = result.Identity.FindFirst(ClaimTypes.Email);
                var nameClaim = result.Identity.FindFirst(ClaimTypes.Name);
                if (nameClaim != null)
                {
                    if (nameClaim.Value.Split(' ').Length >= 2)
                    {
                        firstName = nameClaim.Value.Split(' ')[0];
                        lastName = nameClaim.Value.Split(' ')[1];
                    }
                }
                if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
                {
                    email = emailClaim.Value;
                }

                var model = new ExternalLoginConfirmationViewModel() { FirstName = firstName, LastName = lastName, EmailAddress = email };
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(firstName))
                {
                    return await ExternalLoginConfirmation(model, returnUrl);
                }

                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = login.LoginProvider;
                return View("ExternalLoginConfirmation", model);
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }


        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        private async Task<ExternalLoginInfo> AuthenticationManager_GetExternalLoginInfoAsync_Workaround()
        {
            ExternalLoginInfo loginInfo = null;

            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);

            if (result != null && result.Identity != null)
            {
                var idClaim = result.Identity.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim != null)
                {
                    loginInfo = new ExternalLoginInfo()
                    {
                        DefaultUserName = result.Identity.Name == null ? "" : result.Identity.Name.Replace(" ", ""),
                        Login = new UserLoginInfo(idClaim.Issuer, idClaim.Value)
                    };
                }
            }
            return loginInfo;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager_GetExternalLoginInfoAsync_Workaround();

                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.EmailAddress, FirstName = model.FirstName, LastName = model.LastName, EmailAddress = model.EmailAddress, Type = (int)Enums.EntityTypes.Person, Verified = true };
                var result = await CreateUser(user);
                if (result.Item2)
                {
                    var signInResult = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (signInResult.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("SetCareerOption", "Wizard", new { setup = "cs", mode = "email", stamp = DateTime.UtcNow.Ticks });
                    }
                }
                else if ((result.Item1.Errors != null && result.Item1.Errors.Count() > 0 && result.Item1.Errors.Any(c => c.ToLower().Contains("already taken"))))
                {
                    var profile = await _IAccountProcessor.GetProfile(model.EmailAddress);
                    var signInResult = await UserManager.AddLoginAsync(profile.UserID, info.Login);
                    if (signInResult.Succeeded)
                    {
                        var extInfo = await AuthenticationManager.GetExternalLoginInfoAsync();

                        var login = new UserLoginInfo(extInfo.Login.LoginProvider, extInfo.Login.ProviderKey);
                        // Sign in the user with this external login provider if the user already has a login
                        var userToLogin = await UserManager.FindAsync(login);
                        if (userToLogin != null)
                        {
                            await SignInAsync(userToLogin, isPersistent: false);
                            return RedirectToLocal(returnUrl);
                        }
                    }
                }
                AddErrors(result.Item1);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        private async Task<Tuple<IdentityResult, bool>> CreateOrganizationUser(ApplicationUser user, string password, OrganizationSignupViewModel model)
        {
            var userInfo = await UserManager.CreateAsync(user, password);
            if (userInfo.Succeeded)
            {
                model.UserID = user.Id;
                var result = await Processor.Save(model);
                if (result == Enums.OrganizationSaveStatus.Success)
                    return new Tuple<IdentityResult, bool>(userInfo, true);
                else
                {
                    user.EmailAddress = string.Format("{0}_{1}", Guid.NewGuid(), user.EmailAddress);
                    user.UserName = string.Format("{0}_{1}", Guid.NewGuid(), user.UserName);
                    userInfo = await UserManager.UpdateAsync(user);
                }
            }
            return new Tuple<IdentityResult, bool>(userInfo, false);
        }

        private async Task<Tuple<IdentityResult, bool>> CreateUser(ApplicationUser user, string password)
        {
            var userInfo = await UserManager.CreateAsync(user, password);
            if (userInfo.Succeeded)
            {
                var result = _IAccountProcessor.AddUserExtendedInfo(user);
                if (result > 0)
                    return new Tuple<IdentityResult, bool>(userInfo, true);
                else
                {
                    user.EmailAddress = string.Format("{0}_{1}", Guid.NewGuid(), user.EmailAddress);
                    user.UserName = string.Format("{0}_{1}", Guid.NewGuid(), user.UserName);
                    userInfo = await UserManager.UpdateAsync(user);
                }
            }
            return new Tuple<IdentityResult, bool>(userInfo, false);
        }

        [Authorize]
        public async Task<ActionResult> Configuration(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                switch (id.ToLower())
                {
                    case "general":
                        var generalSettingModel = await _IAccountProcessor.GeneralSettings(UserHashObject.EntityID);
                        if (UserHashObject.EntityType == (int)Enums.EntityTypes.Person)
                            return PartialView("_GeneralSettings", generalSettingModel);
                        else
                            return PartialView("_BusinessGeneralSettings", generalSettingModel);
                    case "network":
                        var networkSettingModel = await _IAccountProcessor.NetworkSettings(UserHashObject.EntityID);
                        if (UserHashObject.EntityType == (int)Enums.EntityTypes.Person)
                            return PartialView("_NetworkSettings", networkSettingModel);
                        else
                            return PartialView("_BusinessNetworkSettings", networkSettingModel);

                    case "privacy":
                        var privacySettingModel = await _IAccountProcessor.PrivacySettings(UserHashObject.EntityID);
                        privacySettingModel.HasPassword = HasPassword();
                        return PartialView("_PrivacySettings", privacySettingModel);
                    case "widgets":
                        var widgetsSettingModel = await _IAccountProcessor.WidgetsSettings(UserHashObject.EntityID);
                        return PartialView("_WidgetSettings", widgetsSettingModel);
                    case "job-search":
                        var jobSettingModel = await _IAccountProcessor.JobSearchSettings(UserHashObject.EntityID);
                        return PartialView("_JobSearchSettings", jobSettingModel);
                    case "company":
                        var companyProfile = await Processor.BasicCompanyProfile();
                        return PartialView("_CompanyProfileConfiguration", companyProfile);
                    case "email":
                        var emailConfiguration = await Processor.EmailConfiguration();
                        return PartialView("_EmailPreferences", emailConfiguration);


                }
            }
            if (UserHashObject.EntityType == (int)Enums.EntityTypes.Person)
                return View();
            else
                return View("BusinessConfiguration");
        }

        private async Task<Tuple<IdentityResult, bool>> CreateUser(ApplicationUser user)
        {
            var userInfo = await UserManager.CreateAsync(user);
            if (userInfo.Succeeded)
            {
                var result = _IAccountProcessor.AddUserExtendedInfo(user);
                if (result > 0)
                    return new Tuple<IdentityResult, bool>(userInfo, true);
                else
                {
                    user.EmailAddress = string.Format("{0}_{1}", Guid.NewGuid(), user.EmailAddress);
                    user.UserName = string.Format("{0}_{1}", Guid.NewGuid(), user.UserName);
                    userInfo = await UserManager.UpdateAsync(user);
                }
            }
            return new Tuple<IdentityResult, bool>(userInfo, false);
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            UserCacheManager.ClearAll(UserHashObject.EntityID);
            AuthenticationManager.SignOut();
            Session.Clear();
            Session.Abandon();
            Session["___UserHash"] = null;
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> ConfigurationProfileName()
        {
            return PartialView("_ProfileName", new GeneralSettingViewModel());
        }

        [Authorize]
        public async Task<ActionResult> OppurtunityLevel()
        {
            return PartialView("_OppurtunityLevel", new JobSearchViewModel());
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> OppurtunityLevel(JobSearchViewModel model)
        {
            var result = await _IAccountProcessor.SetOppurtunityLevel(model);
            return new JsonResult() { ContentType = "application/json", Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        public async Task<ActionResult> ConfigurationNetworkCoverage()
        {
            var networkSetting = await _IAccountProcessor.NetworkSettings(UserHashObject.EntityID);
            return PartialView("_NetworkCoverage", networkSetting);
        }

        public async Task<ActionResult> AskOpportunity(string groupHash)
        {
            ViewBag.GroupHash = groupHash;
            return PartialView("_AskOpportunity");
        }

        public async Task<ActionResult> ShareOpportunity(string groupHash)
        {
            ViewBag.GroupHash = groupHash;
            return PartialView("_ShareOpportunity");
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ConfigurationNetworkCoverage(NetworkSettingViewModel model)
        {
            if (model.NetworkCoverageLevel != (int)Enums.NetworkCoverageLevel.ToIndustry)
            {
                ModelState.Remove("IndustryName");
                ModelState.Remove("IndustryID");
            }

            if (ModelState.IsValid)
            {
                var result = await _IAccountProcessor.SaveNetworkCoverage(model);
                return new JsonResult() { ContentType = "application/json", Data = new Result() { Type = Enums.ResultType.Success }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            var stringError = "<ul>";
            foreach (var item in ModelState)
            {
                if (item.Value.Errors.Count > 0)
                    stringError += string.Format("<li>{0}</li>", item.Value.Errors[0].ErrorMessage);
            }
            stringError += "</ul>";
            return WJsonError(stringError);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ConfigurationProfileName(GeneralSettingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _IAccountProcessor.SaveProfileName(model);
                return new JsonResult() { ContentType = "application/json", Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return WJsonError("Profile name cannot be left empty.");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SetSearchEngineVisibility(bool enabled)
        {
            var result = await _IAccountProcessor.SaveSearchEngineSetting(enabled);
            return new JsonResult() { ContentType = "application/json", Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SetIndividualConnectionStatus(bool enabled)
        {
            var result = await _IAccountProcessor.SetIndividualConnectionStatus(enabled);
            return new JsonResult() { ContentType = "application/json", Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SetJobAppurtunities(bool enabled)
        {
            var result = await _IAccountProcessor.SetJobAppurtunities(enabled);
            return new JsonResult() { ContentType = "application/json", Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SetAllowReference(bool enabled)
        {
            var result = await _IAccountProcessor.SetAllowReference(enabled);
            return new JsonResult() { ContentType = "application/json", Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UpdateWidget(BaseViewModel model, bool value)
        {
            var result = await _IAccountProcessor.SetWidgetSubscription(model, value);
            return new JsonResult() { ContentType = "application/json", Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        public async Task<JsonResult> SkipIntelligence(BaseViewModel model)
        {
            var result = await _IAccountProcessor.SkipIntelligence(model);
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        public async Task<ActionResult> UserMenu()
        {
            return PartialView("_UserMenu");
        }

        [Authorize]
        public async Task<ActionResult> MobileUserMenu()
        {
            return PartialView("_MobileUserMenu");
        }

        [Authorize]
        public async Task<JsonResult> Feeds(int pageNumber, long ticks)
        {
            var dateTime = new DateTime(ticks);
            var result = await PressProcessor.Feeds(pageNumber, dateTime);
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> FeedContent(int pageNumber, long ticks, Enums.FeedType type = Enums.FeedType.Default)
        {
            var dateTime = new DateTime(ticks);
            var result = await PressProcessor.Feeds(pageNumber, dateTime, type);
            if (result.Feeds != null && result.Feeds.Count > 0)
                return PartialView("_Feeds", result.Feeds);
            return null;
        }

        [Authorize]
        [CompressFilter]
        public async Task<ActionResult> Insights(string id, int pageNumber = 0)
        {
            var insights = await _IAccountProcessor.Insights(pageNumber, id);
            return View(insights);
        }


        [Authorize]
        public async Task<ActionResult> ContactImportWizard()
        {
            var itelligentAllowContactImport = await _IAccountProcessor.ShowIntelligentContactImport();
            if (itelligentAllowContactImport.Type == Enums.ResultType.Success)
                return PartialView("_ImportContact");
            return null;
        }

        [Authorize]
        public async Task<ActionResult> Intelligence()
        {
            var itelligentData = await _IAccountProcessor.Intelligence();
            switch (itelligentData.IntelligenceType)
            {
                case Enums.InteligenceType.RequireAddingSkills:
                    var skills = await SkillProcessor.GetSkillHisotry();
                    return PartialView("_IntelligenceAddSkill", skills);
                case Enums.InteligenceType.RequireConnections:
                    break;
                case Enums.InteligenceType.RequireAddingWorkInfo:
                    return PartialView("_IntelligenceAddCareerHistory", new CareerHistoryViewModel());
                case Enums.InteligenceType.RequireEducation:
                    return PartialView("_IntelligenceAddEducation", new CareerHistoryViewModel());
                //case Enums.InteligenceType.RequireAssignments:
                //    break;
                //case Enums.InteligenceType.RequirePhoto:
                //    break;
                case Enums.InteligenceType.RequireAbout:
                    return PartialView("_IntelligenceAddAbout", new AboutViewModel());
                case Enums.InteligenceType.RequireAddingCareerLineCompany:
                    return PartialView("_IntelligenceAddCompanyName", await CareerHistoryProcessor.GetImprovableCareerHistory((int)Enums.CareerHistoryMode.Profession));
                case Enums.InteligenceType.RequireAddingTimeToCareerLine:
                    var model = await CareerHistoryProcessor.GetImprovableCareerHistoryForTime((int)Enums.CareerHistoryMode.Profession);
                    model.MonthList = new SelectList(Month, "Key", "Value");
                    model.YearList = new SelectList(Year, "Key", "Value");
                    return PartialView("_IntelligenceAddTime", model);
                case Enums.InteligenceType.RequireAddingAwardAndAchievement:
                    var modelAward = new IntelligenceAwardViewModel() { DisplayText = string.Format("Add award, achievements and honors to your profile") };
                    modelAward.MonthList = new SelectList(Month, "Key", "Value");
                    modelAward.YearList = new SelectList(Year, "Key", "Value");
                    return PartialView("_IntelligenceAddAward", modelAward);
                case Enums.InteligenceType.RequireAddingCertification:
                    return PartialView("_IntelligenceAddCertification", new IntelligenceCareerHistoryViewModel() { DisplayText = string.Format("Add your certification to your career profile") });
                case Enums.InteligenceType.RequireTeamIntoAward:
                    var modelAwardTeamMember = await CareerHistoryProcessor.GetImprovableAward();
                    return PartialView("_IntelligenceAddTeamAward", modelAwardTeamMember);
                case Enums.InteligenceType.RequireTeamIntoAssignment:
                    var modelAssignmentTeamMember = await CareerHistoryProcessor.GetImprovableAssignment();
                    return PartialView("_IntelligenceAddTeamAssignment", modelAssignmentTeamMember);
                default:
                    break;
            }
            return null;
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.GenerateUserIdentityAsync(user);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
            var entityID = await identity.GetCustomValue("EntityID");
            var result = await GeneralExtentions.Do<Task<Result>>(() => _IAccountProcessor.WarmUp(Convert.ToInt64(entityID)), TimeSpan.FromSeconds(1), 5);
        }

        private void AddErrors(IdentityResult result)
        {
            ModelState.AddModelError("GeneralError", "There is an error occured while processing the account, please give it another try.");
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Profile");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }


        #endregion
        #endregion
        }

        [Authorize]
        public async Task<ActionResult> changeProfilePicture()
        {
            return PartialView("_ChangeProfilePicture");
        }

        [Authorize]
        public async Task<ActionResult> EmailPreferences(int type, bool subscribed)
        {
            var result = await _IAccountProcessor.EmailPreferences(type, subscribed);
            return WJson(result);
        }

        [Authorize]
        public async Task<ActionResult> EmailPreferencesUnsubscribe(string hash)
        {
            var result = await _IAccountProcessor.Unsubscribe(hash);
            return View("Response", result);
        }

        [Authorize]
        public async Task<ActionResult> changecoverpicture()
        {
            return PartialView("_ChangeCoverPicture");
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> sendvarificationEmail(string hash)
        {
            var result = await _IAccountProcessor.SendVarification(hash);
            return PartialView("_ActionResultMessage", result);
        }

        [HttpPost]
        public async Task<ActionResult> Track(string q, string trackType)
        {
            var result = await _IAccountProcessor.EntityTrack(q, trackType);
            return WJson(new { trackType = trackType, hash = q, recorededOn = DateTime.UtcNow, success = result });
        }

        public async Task<ActionResult> Improve() 
        {
            var model = await _IAccountProcessor.GetProfileDataToImprove();
            return PartialView("_Improve", model);
        }

        public async Task<ActionResult> ImproveContent()
        {
            var model = await _IAccountProcessor.GetProfileDataToImprove();
            return PartialView("_ImproveContent", model);
        }

        public async Task<JsonResult> ProfileRankData() 
        {
            return WJson(await _IAccountProcessor.GetProfileDataToImprove());
        }
    }
}

