using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Types;
using Wrly.Data.Models;
using Wrly.Data.Repositories.Implementors;
using Wrly.Data.Repositories.Signatures;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Infrastructure.Utils;
using Wrly.infrastuctures.Utils;
using Wrly.Models;
using Wrly.Models.Listing;
using System.Data;
using Wrly.Models.Feeds;
using Wrly.Storage;
using System.Drawing;
using System.Drawing.Imaging;
using Wrly.Infrastuctures.Utils;
using Wrly.Notifications.Processors.Implementors;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class AccountProcessor : BaseProcessor, IAccountProcessor
    {

        #region '---- Members ----'

        #endregion
        public async System.Threading.Tasks.Task<Models.ProfileViewModel> GetProfile(string userName)
        {
            using (var repository = new AccountRepository())
            {
                using (var dataSet = await repository.GetProfile(userName))
                {
                    return dataSet.Tables[0].FromDataTable<ProfileViewModel>().FirstOrDefault();
                }
            }

        }

        public async System.Threading.Tasks.Task<Models.ProfileViewModel> GetProfileWithStates(string userName, bool inlcudeStatistics, bool isProfileName = false)
        {
            using (var repository = new AccountRepository())
            {
                var currentEntity = UserHashObject != null ? UserHashObject.EntityID : 0;
                using (var dataSet = await repository.GetProfileWithStates(userName, isProfileName, currentEntity))
                {
                    var profile = dataSet.Tables[0].FromDataTable<ProfileViewModel>().FirstOrDefault();
                    profile.Widgets = dataSet.Tables[1].FromDataTable<WidgetSettingViewModel>();
                    profile.ProfileHash = GetProfileHash(profile);
                    profile.NetworkHash = GetConnectionHash(profile);
                    if (inlcudeStatistics)
                    {
                        using (var states = await repository.States(profile.EntityID))
                        {
                            profile.Statistics = states.Tables[0].FromDataTable<PersonStatisticsViewModel>().FirstOrDefault();
                        }
                    }
                    return profile;
                }
            }
        }


        private string GetConnectionHash(ProfileViewModel profile)
        {
            var table = new Hashtable();
            table.Add("Action", "Associate");
            table.Add("TimeStampGeneratedAt", DateTime.UtcNow.ToString());
            table.Add("EntityID", profile.EntityID);
            table.Add("EntityID2", UserHashObject.EntityID);
            table.Add("Location", "Location.PersonProfileFace");
            return QueryStringHelper.Encrypt(table);
        }

        private string GetProfileHash(ProfileViewModel profile)
        {
            var hashTable = new Hashtable();
            hashTable.Add("EntityID", profile.EntityID);
            hashTable.Add("PersonID", profile.PersonID);
            hashTable.Add("FullName", profile.FullName);
            hashTable.Add("ID", Guid.NewGuid());
            return QueryStringHelper.Encrypt(hashTable);
        }

        public long AddUserExtendedInfo(Models.ApplicationUser user)
        {
            using (var repository = new AccountRepository())
            {
                long entityID = 0;
                var profileName = user.EmailAddress.Split('@')[0].Replace(".", "_");
                bool isExist = repository.IsUserProfileNameExist(profileName);
                if (!isExist)
                {
                    var profiles = new HashSet<PersonProfile>();
                    profiles.Add(new PersonProfile()
                    {
                        ProfileName = profileName,
                        NameFormat = (byte)Enums.PersonNameFormat.FirstNameLastName,
                        FormatedName = string.Format("{0} {1}", user.FirstName, user.LastName),
                        EmailVarified = user.Verified
                    }
                    );
                    var extended = new Person()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Gender = user.Gender,
                        PersonProfiles = profiles,
                        UserID = user.Id,
                        Entity = new Entity()
                        {
                            EntityType = (int)Enums.EntityTypes.Person
                        }
                    };
                    entityID = repository.AddExtendedInfo(extended);
                    return entityID;
                }

                bool isFound = false;
                var count = 0;
                var newProfileName = string.Empty;
                while (!isFound)
                {
                    newProfileName = profileName + count.ToString();
                    isExist = repository.IsUserProfileNameExist(newProfileName);
                    if (!isExist)
                    {
                        var profiles = new HashSet<PersonProfile>();
                        profiles.Add(new PersonProfile()
                        {
                            ProfileName = profileName,
                            NameFormat = (byte)Enums.PersonNameFormat.FirstNameLastName,
                            FormatedName = string.Format("{0} {1}", user.FirstName, user.LastName)
                        });
                        var extended = new Person()
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Gender = user.Gender,
                            PersonProfiles = profiles,
                            UserID = user.Id,
                            Entity = new Entity()
                            {
                                EntityType = (int)Enums.EntityTypes.Person
                            }
                        };
                        entityID = repository.AddExtendedInfo(extended);

                        isFound = true;
                        break;
                    }
                }
                return entityID;
            }
        }

        public int UpdateProfile(Dictionary<string, string> values, string mode)
        {
            string query = string.Empty;
            if (mode == "u")
                query = "Update aspnetusers Set ";
            if (mode == "e")
                query = "Update userextended Set ";

            int count = 0;
            foreach (var item in values)
            {
                if (count + 1 == values.Count)
                    query += item.Key + " = N'" + item.Value + "'";
                else
                    query += item.Key + " = N'" + item.Value + "', ";

                count++;
            }

            if (mode == "u")
                query += " where username=N'" + HttpContext.Current.User.Identity.Name + "'";
            if (mode == "e")
                query += " where userID = (Select top 1 Id from aspnetusers where username=N'" + HttpContext.Current.User.Identity.Name + "')";
            using (var _IAccountService = new AccountRepository())
            {
                return _IAccountService.UpdateProfile(query);
            }
        }



        public async Task<Result> UploadCoverImage(ProfilePicViewModel model)
        {
            var byteArr = new byte[model.Image.ContentLength];
            model.Image.InputStream.Read(byteArr, 0, model.Image.ContentLength);
            var guid = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(model.Image.FileName);
            var nowByte = CommonFunctions.CropImage(byteArr, model.ImgX1, model.ImgY1, model.ImgWidth, model.ImgHeight);
            if (model.ImgWidth > 900)
            {
                var refPng = true;
                nowByte = Biz2Dial.Images.Resizer.General.ResizeFile(byteArr, 900, ref refPng);
            }

            if (UserHashObject.EntityType == (int)Enums.EntityTypes.Person)
            {
                var resultFile = ImageProcessor.UploadImage(nowByte, Enums.ImageObject.UserCoverImage, string.Empty, true, Enums.FileType.Image, UserHashObject.UserID + "__cover__", AppConfig.StorageProvider, AppConfig.SiteUrl);
                var keyValue = new Dictionary<string, string>();
                keyValue.Add("ProfileCoverPath", resultFile.FileName);
                var retry = 1;
                while (retry < 10)
                {
                    using (var repository = new AccountRepository())
                    {
                        var resultProfile = await repository.UpdateProfileItems(keyValue, UserHashObject.PersonID);
                        if (resultProfile >= 0)
                        {
                            await repository.UpdateProfileScore(UserHashObject.EntityID);
                            await CommonFunctions.UpdateUserFaceIndex(UserHashObject.EntityID, (int)Enums.EntityTypes.Person);
                            return new Result() { Type = Enums.ResultType.Success, Description = string.Format(resultFile.FileName, 900) + "?id=" + Guid.NewGuid().ToString() };
                        }
                        retry++;
                    }
                }
                return new Result() { Type = Enums.ResultType.Error, Description = "Unable to update the profile picture." };
            }
            else
            {
                var resultFile = ImageProcessor.UploadImage(nowByte, Enums.ImageObject.BusinessCoverImage, string.Empty, true, Enums.FileType.Image, UserHashObject.UserID + "__cover__", AppConfig.StorageProvider, AppConfig.SiteUrl);
                var keyValue = new Dictionary<string, string>();
                keyValue.Add("ProfileCoverPath", resultFile.FileName);
                var retry = 1;
                while (retry < 10)
                {
                    using (var repository = new BusinessRepository())
                    {
                        var resultProfile = await repository.UpdateOrganizationItems(null, keyValue, UserHashObject.OrganizationID);
                        if (resultProfile >= 0)
                        {
                            await CommonFunctions.UpdateUserFaceIndex(UserHashObject.EntityID, (int)Enums.EntityTypes.Organization);
                            return new Result() { Type = Enums.ResultType.Success, Description = string.Format(resultFile.FileName, 900) + "?id=" + Guid.NewGuid().ToString() };
                        }
                        retry++;
                    }
                }
                return new Result() { Type = Enums.ResultType.Error, Description = "Unable to update the profile picture." };

            }
        }


        public async Task<Result> UploadProfileImage(ProfilePicViewModel model)
        {
            var byteArr = new byte[model.Image.ContentLength];
            model.Image.InputStream.Read(byteArr, 0, model.Image.ContentLength);
            var guid = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(model.Image.FileName);
            var nowByte = CommonFunctions.CropImage(byteArr, model.ImgX1, model.ImgY1, model.ImgWidth, model.ImgHeight);
            var userLocalStorage = AppConfig.UseLocalStorage;

            if (userLocalStorage)
            {
                if (model.ImgWidth > 200)
                {
                    var refPng = true;
                    nowByte = Biz2Dial.Images.Resizer.General.ResizeFile(byteArr, 200, ref refPng);
                }
            }

            if (UserHashObject.EntityType == (int)Enums.EntityTypes.Person)
            {
                var resultFile = ImageProcessor.UploadImage(nowByte, Enums.ImageObject.UserProfileImage, string.Empty, true, Enums.FileType.Image, UserHashObject.UserID + "__profile__", AppConfig.StorageProvider, AppConfig.SiteUrl);
                var keyValue = new Dictionary<string, string>();
                keyValue.Add("ProfileImagePath", resultFile.FileName);
                var retry = 1;
                while (retry < 10)
                {
                    using (var repository = new AccountRepository())
                    {
                        var resultProfile = await repository.UpdateProfileItems(keyValue, UserHashObject.PersonID);
                        if (resultProfile >= 0)
                        {
                            await repository.UpdateProfileScore(UserHashObject.EntityID);
                            await CommonFunctions.UpdateUserFaceIndex(UserHashObject.EntityID, (int)Enums.EntityTypes.Person);
                            return new Result() { Type = Enums.ResultType.Success, Description = string.Format(resultFile.FileName, 200) + "?id=" + Guid.NewGuid().ToString() };
                        }
                        retry++;
                    }
                }
                return new Result() { Type = Enums.ResultType.Error, Description = "Unable to update the profile picture." };
            }
            else
            {
                var resultFile = ImageProcessor.UploadImage(nowByte, Enums.ImageObject.BusinessProfileImage, string.Empty, true, Enums.FileType.Image, UserHashObject.UserID + "__profile__", AppConfig.StorageProvider, AppConfig.SiteUrl);
                var keyValue = new Dictionary<string, string>();
                keyValue.Add("ProfileImagePath", resultFile.FileName);
                var retry = 1;
                while (retry < 10)
                {
                    using (var repository = new BusinessRepository())
                    {
                        var resultProfile = await repository.UpdateOrganizationItems(null, keyValue, UserHashObject.OrganizationID);
                        if (resultProfile >= 0)
                        {
                            await CommonFunctions.UpdateUserFaceIndex(UserHashObject.EntityID, (int)Enums.EntityTypes.Organization);
                            return new Result() { Type = Enums.ResultType.Success, Description = string.Format(resultFile.FileName, 200) + "?id=" + Guid.NewGuid().ToString() };
                        }
                        retry++;
                    }
                }
                return new Result() { Type = Enums.ResultType.Error, Description = "Unable to update the profile picture." };
            }
        }


        public void DeleteProfileImage(string uid)
        {
            var files = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/UserImages"), uid + ".*");
            if (files != null && files.Length > 0)
                foreach (var item in files)
                    File.Delete(item);

            string query = "UPdate UserExtended Set ProfilePic=NULL where userID=N'" + uid + "'";
            using (var _IAccountService = new AccountRepository())
            {
                _IAccountService.UpdateProfile(query);
            }
        }

        public void SendWelcomeEmail(Models.RegisterViewModel model)
        {
            CommonFunctions.SendWelcomeEmail(model);
        }


        public void AddUserExtendedInfoWithJobRole(ApplicationUser applicationUser, ConnectAccountViewModel model)
        {
            var enityID = AddUserExtendedInfo(applicationUser);
            var careerHistory = Mapper.Map<CareerHistory>(model);
            careerHistory.EntityID = enityID;
            careerHistory.Organization = new Organization()
            {
                OrganizationID = model.OrganizationID,
                Name = model.Name
            };
            careerHistory.JobTitle = Mapper.Map<JobTitle>(model);
            careerHistory.JobTitle.JobTitleID = model.JobRoleID;
            careerHistory.JobTitle.Name = model.JobRollName;
            careerHistory.JobTitle.Active = false;
            careerHistory.JobTitle.IpAddress = HttpContext.Current.Request.UserHostAddress;

            using (var careerHistoryRepository = new CareerHistoryRepository())
            {
                careerHistoryRepository.Save(careerHistory);
            }
        }





        public async Task<UserStatisticsViewModel> GetStatistics()
        {
            using (var accountRepository = new AccountRepository())
            {
                using (var dataSet = await accountRepository.GetStates(UserHashObject.EntityID))
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        return dataSet.Tables[0].FromDataTable<UserStatisticsViewModel>().FirstOrDefault();
                    }
                    return new UserStatisticsViewModel();
                }
            }
        }


        public async Task<AboutViewModel> GetAbout(string hash)
        {
            if (UserHash == hash)
            {
                using (var accountRepository = new AccountRepository())
                {
                    using (var dataSet = await accountRepository.GetBasicUserProfile(UserHashObject.PersonID, "ProfileSummary"))
                    {
                        var model = dataSet.Tables[0].FromDataTable<AboutViewModel>()[0];
                        model.UserHash = UserHash;
                        return model;
                    }
                }
            }
            return null;
        }

        public async Task<long> SaveAbout(AboutViewModel model)
        {
            using (var accountRepository = new AccountRepository())
            {
                var result = await accountRepository.SaveProfileInfo("ProfileSummary", model.ProfileSummary, UserHashObject.PersonID);
                if (result > 0)
                    await accountRepository.UpdateProfileScore(UserHashObject.EntityID);

                return result;
            }
        }



        public async Task<List<CareerLineViewModel>> GetCareerLine(string id, bool includeS = true, bool includeA = true)
        {
            using (var accountRepository = new AccountRepository())
            {
                if (!string.IsNullOrEmpty(id) && id.Equals("basic", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (var dataSet = await accountRepository.GetBasicCareerLine(UserHashObject.EntityID, includeS))
                    {
                        return dataSet.Tables[0].FromDataTable<CareerLineViewModel>();
                    }
                }
                else
                {
                    using (var dataSet = await accountRepository.GetAdvancedCareerLine(UserHashObject.EntityID, includeS, includeA))
                    {
                        return dataSet.Tables[0].FromDataTable<CareerLineViewModel>();
                    }
                }
            }
        }


        public async Task<GeneralSettingViewModel> GeneralSettings(long entityID)
        {
            using (var repository = new SettingsRepository())
            {
                using (var jobSetting = await repository.GetSetting(UserHashObject.EntityID, Enums.SettingType.General))
                {
                    return jobSetting.Tables[0].FromDataTable<GeneralSettingViewModel>().FirstOrDefault();
                }
            }
        }

        private string GetProfileNameHash()
        {
            var hashTable = new Hashtable();
            hashTable.Add("EntityID", UserHashObject.EntityID);
            hashTable.Add("TableNameIndex", Enums.TableNameIndex.EntityProfile);
            hashTable.Add("ColumnNameIndex", Enums.ColumnNameIndex.ProfileName);
            hashTable.Add("ID", Guid.NewGuid());
            return QueryStringHelper.Encrypt(hashTable);
        }

        public async Task<NetworkSettingViewModel> NetworkSettings(long entityID)
        {
            using (var repository = new SettingsRepository())
            {
                using (var jobSetting = await repository.GetSetting(UserHashObject.EntityID, Enums.SettingType.Network))
                {
                    return jobSetting.Tables[0].FromDataTable<NetworkSettingViewModel>().FirstOrDefault();
                }
            }
        }

        public async Task<PrivacySettingViewModel> PrivacySettings(long entityID)
        {
            using (var repository = new SettingsRepository())
            {
                using (var jobSetting = await repository.GetSetting(UserHashObject.EntityID, Enums.SettingType.Privacy))
                {
                    return jobSetting.Tables[0].FromDataTable<PrivacySettingViewModel>().FirstOrDefault();
                }
            }
        }

        public async Task<List<WidgetSettingViewModel>> WidgetsSettings(long entityID)
        {
            using (var repository = new SettingsRepository())
            {
                using (var jobSetting = await repository.GetSetting(UserHashObject.EntityID, Enums.SettingType.Widget))
                {
                    var result = jobSetting.Tables[0].FromDataTable<WidgetSettingViewModel>();
                    foreach (var item in result)
                    {
                        var hashTable = new Hashtable();
                        hashTable.Add("EntityID", UserHashObject.EntityID);
                        hashTable.Add("WidgetID", item.WidgetID);
                        hashTable.Add("WidgetName", item.WidgetName);
                        hashTable.Add("ID", Guid.NewGuid());
                        item.Hash = QueryStringHelper.Encrypt(hashTable);
                    }
                    return result;
                }
            }
        }


        public async Task<JobSearchViewModel> JobSearchSettings(long entityID)
        {
            using (var repository = new SettingsRepository())
            {
                using (var jobSetting = await repository.GetSetting(UserHashObject.EntityID, Enums.SettingType.JobSearch))
                {
                    return jobSetting.Tables[0].FromDataTable<JobSearchViewModel>().FirstOrDefault();
                }
            }
        }


        public async Task<Result> SaveProfileName(GeneralSettingViewModel model)
        {
            using (var repository = new SettingsRepository())
            {
                long data = 0;
                if (UserHashObject.EntityType == (int)Enums.EntityTypes.Person)
                    data = await repository.SaveProfileName(model.ProfileName, UserHashObject.PersonID);
                else
                    data = await repository.SaveBusinessProfileName(model.ProfileName, UserHashObject.OrganizationID);
                if (data >= 0)
                {
                    if (UserHashObject.EntityType == (int)Enums.EntityTypes.Person)
                        await CommonFunctions.UpdateUserFaceIndex(UserHashObject.EntityID, (int)Enums.EntityTypes.Person);
                    else
                        await CommonFunctions.UpdateUserFaceIndex(UserHashObject.EntityID, (int)Enums.EntityTypes.Organization);
                }
                return new Result() { Type = Enums.ResultType.Success, Description = "Profile name has been changed." };
                if (data == -2)
                    return new Result() { Type = Enums.ResultType.Error, Description = "Profile name already taken, try another name." };

                return new Result() { Type = Enums.ResultType.Error, Description = "An error while processing the request, give it another try." };
            }
        }

        public async Task<long> SaveSearchEngineSetting(bool enabled)
        {
            using (var repository = new SettingsRepository())
            {
                return await repository.SaveSearchEngineVisibility(enabled, UserHashObject.EntityID);
            }
        }

        public async Task<Result> SetIndividualConnectionStatus(bool enabled)
        {
            if (UserHashObject.EntityType == (int)Enums.EntityTypes.Organization)
            {
                var keyValue = new Dictionary<string, string>();
                keyValue.Add("AllowIndividualToConnect", (enabled ? 1 : 0).ToString());

                using (var repository = new SettingsRepository())
                {
                    var result = await repository.SaveNetworkSettings(keyValue, UserHashObject.EntityID);
                    if (result > 0)
                        return new Result() { Type = Enums.ResultType.Success, Description = "Connection preference has been saved." };
                    else
                        return new Result() { Type = Enums.ResultType.Error, Description = "An error occured while Saving data." };
                }
            }
            return null;
        }

        public async Task<long> SaveNetworkCoverage(NetworkSettingViewModel model)
        {
            var keyValue = new Dictionary<string, string>();
            keyValue.Add("NetworkCoverageLevel", model.NetworkCoverageLevel.ToString());
            if (model.NetworkCoverageLevel == (int)Enums.NetworkCoverageLevel.ToIndustry)
                keyValue.Add("IndustryID", model.IndustryID.ToString());
            else
                keyValue.Add("IndustryID", null);

            using (var repository = new SettingsRepository())
            {
                var result = await repository.SaveNetworkSettings(keyValue, UserHashObject.EntityID);
                if (result > 0)
                {
                    CacheManager.Delete("UserSettings_" + UserHashObject.EntityID.ToString());
                    using (var associationRepository = new AssociationRepository())
                    {
                        await associationRepository.ShuffleStock(UserHashObject.EntityID);
                    }
                }
                return result;
            }
        }


        public async Task<long> SetJobAppurtunities(bool enabled)
        {
            using (var repository = new SettingsRepository())
            {
                return await repository.SetJobAppurtunities(enabled, UserHashObject.EntityID);
            }
        }

        public async Task<long> SetAllowReference(bool enabled)
        {
            using (var repository = new SettingsRepository())
            {
                return await repository.SetAllowReference(enabled, UserHashObject.EntityID);
            }
        }


        public async Task<long> SetOppurtunityLevel(JobSearchViewModel model)
        {
            using (var repository = new SettingsRepository())
            {
                return await repository.SetOppurtunityLevel(model.JobInterestLevel, UserHashObject.EntityID);
            }
        }


        public async Task<long> SetWidgetSubscription(BaseViewModel model, bool value)
        {
            var entityWidget = Mapper.Map<BaseViewModel, EntityWidget>(model);
            entityWidget = model.Hash.ToObject<EntityWidget>(entityWidget);
            if (entityWidget.EntityID != UserHashObject.EntityID)
                return -1;
            entityWidget.Active = value;
            using (var repository = new SettingsRepository())
            {
                return await repository.SetWidgetStatus(entityWidget);
            }
        }


        public async Task<IntelligenceViewModel> Intelligence()
        {
            using (var repository = new AccountRepository())
            {
                using (var dsIntelligence = await repository.Intelligence(UserHashObject.PersonID, UserHashObject.EntityID))
                {
                    if (dsIntelligence != null && dsIntelligence.Tables[0].Rows.Count > 0)
                    {
                        return dsIntelligence.Tables[0].FromDataTable<IntelligenceViewModel>()[0];
                    }
                }
            }
            return new IntelligenceViewModel();
        }


        public async Task<bool> SkipIntelligence(BaseViewModel hash)
        {
            var intelligenceType = hash.Hash.GetSingleValue("___T___");
            using (var repository = new AccountRepository())
            {
                return await repository.SkipIntelligence(UserHashObject.EntityID, Convert.ToInt16(intelligenceType), (int)Enums.InteligenceAction.Skip);
            }
        }


        public async Task<List<CareerLineViewModel>> GetCareerLineForProfile(string profileHash, string id, bool includeS, bool includeA)
        {
            var baseModel = profileHash.ToObject<ProfileHashViewModel>(null);
            using (var accountRepository = new AccountRepository())
            {
                var entityID = UserHashObject == null ? 0 : UserHashObject.EntityID;

                if (!string.IsNullOrEmpty(id) && id.Equals("basic", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (var dataSet = await accountRepository.GetBasicCareerLine(baseModel.EntityID, includeS))
                    {
                        return dataSet.Tables[0].FromDataTable<CareerLineViewModel>();
                    }
                }
                else
                {
                    using (var dataSet = await accountRepository.GetAdvancedCareerLine(baseModel.EntityID, includeS, includeA))
                    {
                        return dataSet.Tables[0].FromDataTable<CareerLineViewModel>();
                    }
                }
            }
        }


        public async Task<List<SkillViewModel>> GetCommonSkills(string q)
        {
            var baseModel = q.ToObject<ProfileHashViewModel>(null);
            using (var accountRepository = new AccountRepository())
            {
                var entityID = UserHashObject == null ? 0 : UserHashObject.EntityID;
                using (var dataSet = await accountRepository.GetCommonSkills(baseModel.EntityID, entityID))
                {
                    return dataSet.Tables[0].FromDataTable<SkillViewModel>();
                }
            }
        }


        public async Task<List<OrganizationFaceViewModel>> GetCommonCompanies(string q)
        {
            var baseModel = q.ToObject<ProfileHashViewModel>(null);
            using (var accountRepository = new AccountRepository())
            {
                var entityID = UserHashObject == null ? 0 : UserHashObject.EntityID;
                using (var dataSet = await accountRepository.GetCommonCompanies(baseModel.EntityID, entityID))
                {
                    return dataSet.Tables[0].FromDataTable<OrganizationFaceViewModel>();
                }
            }
        }

        public async Task<SnapShotViewModel> SnapShot()
        {
            using (var accountRepository = new AccountRepository())
            {
                using (var dataSet = await accountRepository.SnapShot(UserHashObject.EntityID))
                {
                    return dataSet.Tables[0].FromDataTable<SnapShotViewModel>().FirstOrDefault();
                }
            }
        }

        public async Task<Result> SendVarification(string hash, bool skipIfAlreadyExisit = false)
        {
            var wizardStep = hash.ToObject<WizardHashViewModel>(null);

            var guid = Guid.NewGuid().ToString();

            using (var repository = new AccountRepository())
            {
                if (skipIfAlreadyExisit)
                {
                    var varifications = await repository.VarificationSent(wizardStep.EntityID);
                    if (varifications > 0)
                    {
                        return new Result() { Type = Enums.ResultType.Warning, Description = "Email sent" };
                    }
                }

                var profile = await repository.GetProfile(wizardStep.EntityID);
                var model = profile.Tables[0].FromDataTable<ProfileViewModel>(null)[0];
                var hashTable = new Hashtable();
                hashTable.Add("UserName", model.FormatedName);
                hashTable.Add("EmailAddress", model.EmailAddress);
                hashTable.Add("EntityID", model.EntityID);
                hashTable.Add("PersonID", model.PersonID);
                hashTable.Add("Ticks", Now.Ticks);
                hashTable.Add("Id", guid);
                var varificationHash = QueryStringHelper.Encrypt(hashTable);
                hashTable.Clear();
                hashTable.Add("**Hash**", varificationHash);
                hashTable.Add("**UserName**", model.FormatedName);
                var result = await NotificationProcessor.SendEmail(hashTable, Enums.EmailFilePaths.Varification, model.EmailAddress);
                if (result == Enums.ResultType.Success)
                {
                    var keyValue = new Dictionary<string, string>();
                    keyValue.Add("LastVarificationSent", Now.ToString());
                    keyValue.Add("VarificationID", guid);
                    var retry = 1;
                    while (retry < 10)
                    {
                        var resultProfile = await repository.UpdateProfileItems(keyValue, model.PersonID);
                        if (resultProfile >= 0)
                        {
                            return new Result() { Type = Enums.ResultType.Success, Description = string.Format("Verification email contains the link to verify your email address has been sent to {0}.", model.EmailAddress) };
                        }
                        retry++;
                    }
                }
                return new Result() { Type = Enums.ResultType.Error, Description = "There is an error on server while sending verification email, please give it another try." };
            }
        }


        public async Task<Result> SaveName(NameViewModel model)
        {
            var resultName = model.NameFormat == (int)Enums.PersonNameFormat.FirstNameLastName ? string.Format("{0} {1}", model.FirstName, model.LastName) : string.Format("{0} {1}", model.LastName, model.FirstName);
            var profile = new Dictionary<string, string>();
            profile.Add("NameFormat", model.NameFormat.ToString());
            profile.Add("FormatedName", resultName);
            var person = new Dictionary<string, string>();
            person.Add("FirstName", model.FirstName);
            person.Add("LastName", model.LastName);
            using (var repository = new AccountRepository())
            {
                var resultProfile = await repository.UpdateProfileItems(profile, UserHashObject.PersonID, person);
                if (resultProfile >= 0)
                {

                    return new Result() { Type = Enums.ResultType.Success, Description = resultName };
                }
            }
            return new Result() { Type = Enums.ResultType.Error, Description = "Error occured while saving changes, give it another try." };
        }


        public async Task<Result> SaveHeading(HeadingViewModel model)
        {
            var profile = new Dictionary<string, string>();
            profile.Add("FormatedJobTitle", model.Heading);

            using (var repository = new AccountRepository())
            {
                var resultProfile = await repository.UpdateProfileItems(profile, UserHashObject.PersonID);
                if (resultProfile >= 0)
                {
                    await CommonFunctions.UpdateUserFaceIndex(UserHashObject.EntityID, (int)Enums.EntityTypes.Person);
                    return new Result() { Type = Enums.ResultType.Success, Description = model.Heading };
                }
            }
            return new Result() { Type = Enums.ResultType.Error, Description = "Error occured while saving changes, give it another try." };
        }


        public async Task<Result> UploadProfileImage(string imgCropped, string fileName)
        {
            var guid = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(fileName);
            imgCropped = imgCropped.Substring("data:image/png;base64,".Length);
            var byteArr = Convert.FromBase64String(imgCropped); ;
            var refPng = true;
            var nowByte = Biz2Dial.Images.Resizer.General.ResizeFile(byteArr, 200, ref refPng);
            if (UserHashObject.EntityType == (int)Enums.EntityTypes.Person)
            {
                var resultFile = ImageProcessor.UploadImage(nowByte, Enums.ImageObject.UserCoverImage, string.Empty, true, Enums.FileType.Image, UserHashObject.UserID + "__profile__", AppConfig.StorageProvider, AppConfig.SiteUrl);
                var keyValue = new Dictionary<string, string>();
                keyValue.Add("ProfileImagePath", resultFile.FileName);
                var retry = 1;
                while (retry < 10)
                {
                    using (var repository = new AccountRepository())
                    {
                        var resultProfile = await repository.UpdateProfileItems(keyValue, UserHashObject.PersonID);
                        if (resultProfile >= 0)
                        {
                            return new Result() { Type = Enums.ResultType.Success, Description = "Profile has been changed successfully." };
                        }
                        retry++;
                    }
                }
                return new Result() { Type = Enums.ResultType.Error, Description = "Unable to update the profile picture." };
            }
            else
            {
                var resultFile = ImageProcessor.UploadImage(nowByte, Enums.ImageObject.BusinessProfileImage, string.Empty, true, Enums.FileType.Image, UserHashObject.UserID + "__profile__", AppConfig.StorageProvider, AppConfig.SiteUrl);
                var keyValue = new Dictionary<string, string>();
                keyValue.Add("ProfileImagePath", resultFile.FileName);
                var retry = 1;
                while (retry < 10)
                {
                    using (var repository = new BusinessRepository())
                    {
                        var resultProfile = await repository.UpdateOrganizationItems(null, keyValue, UserHashObject.OrganizationID);
                        if (resultProfile >= 0)
                        {
                            return new Result() { Type = Enums.ResultType.Success, Description = "Profile has been changed successfully." };
                        }
                        retry++;
                    }
                }
                return new Result() { Type = Enums.ResultType.Error, Description = "Unable to update the profile picture." };
            }
        }





        public async Task<EntityProfileViewModel> HoverCard(string id)
        {
            using (var repository = new AccountRepository())
            {
                var entityID = UserHashObject != null ? UserHashObject.EntityID : 0;
                var cardDetail = await repository.HoverCard(id, entityID);
                var info = cardDetail.Tables[0].FromDataTable<EntityProfileViewModel>().FirstOrDefault();
                var hashTable = new Hashtable();
                hashTable.Add("UserName", info.FormatedName);
                hashTable.Add("EntityID", info.EntityID);
                hashTable.Add("Ticks", Now.Ticks);
                hashTable.Add("Id", Guid.NewGuid().ToString());
                info.Hash = QueryStringHelper.Encrypt(hashTable);
                return info;
            }
        }


        public async Task SendOrganizationWelcomeEmail(OrganizationSignupViewModel model)
        {
            CommonFunctions.SendOrganizationWelcomeEmail(model);
        }


        public async Task<Result> ForgottenPasswordEmail(ForgotPasswordViewModel model)
        {
            using (var repository = new AccountRepository())
            {
                using (var dataSet = await repository.GetUserByEmailAddress(model.EmailAddress))
                {
                    if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                    {
                        var profile = dataSet.Tables[0].FromDataTable<ProfileViewModel>().FirstOrDefault();
                        var hash = Guid.NewGuid().ToString();
                        var result = await repository.SetForgotpasswordData(profile.EmailAddress, profile.EntityID, hash);
                        if (result > 0)
                        {
                            profile.Hash = hash;
                            var emailSent = GeneralExtentions.Do<bool>(() => CommonFunctions.SendForgotPasswordEmail(profile), TimeSpan.FromSeconds(1), 5);
                            if (emailSent)
                                return new Result() { Type = Enums.ResultType.Success, Description = "We have sent an email cotaining a link to create new password, please remember your security is our top priority and hence nobody can see your password and hence our system can't show you your old password." };
                            else
                                return new Result() { Type = Enums.ResultType.Error, Description = "An error occured while sending an email to create new password, please give it another try." };
                        }
                        else
                            return new Result() { Type = Enums.ResultType.Error, Description = "We cannot reach to your request right now, please give it another try." };

                    }
                    else
                        return new Result() { Type = Enums.ResultType.Error, Description = "The email address could not be found, please check if it is spelled correctly or you have not used some other email address." };
                }
            }
        }


        public async Task<Result> GetUserInformationByForgotLink(string hash)
        {
            var profileModel = hash.ToObject<ProfileViewModel>(null);
            using (var repository = new AccountRepository())
            {
                using (var dataSet = await repository.GetUserByForgottPassword(profileModel.Hash, profileModel.EntityID))
                {
                    if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                    {
                        return new Result() { Type = Enums.ResultType.Success };
                    }
                }
            }
            return new Result() { Type = Enums.ResultType.Error, Description = "Cannot find the user information by given link, either link might be expired or tempered, please use forgot password link to generate the link again." };
        }


        public async Task<Result> RemoveForgotPasswordData(string hash)
        {
            using (var repository = new AccountRepository())
            {
                var profileViewModel = hash.ToObject<ProfileViewModel>(null);
                var result = await repository.SetForgotpasswordData(profileViewModel.EmailAddress, profileViewModel.EntityID, null);
                if (result > 0)
                    return new Result() { Type = Enums.ResultType.Success, Description = string.Empty };
                else
                    return new Result() { Type = Enums.ResultType.Error, Description = "An error occured while reverting the profile back." };
            }
        }


        public async Task<Result> WarmUp(long entityID)
        {
            using (var repository = new AccountRepository())
            {
                await repository.PrepareSuggestions(entityID);
                await repository.PrepareWhatsOn(entityID);
                await repository.SetLastLoginData(Now, entityID);
                return new Result() { Type = Enums.ResultType.Success };
            }
        }


        public async Task<NonLoggedInProfileViewModel> GetOpenProfileWithStates(string profileName)
        {
            using (var _IAccountService = new AccountRepository())
            {
                using (var dataSet = await _IAccountService.GetOpenProfile(profileName))
                {
                    var profile = dataSet.Tables[0].FromDataTable<NonLoggedInProfileViewModel>().FirstOrDefault();
                    if (profile != null)
                    {
                        profile.ProfileHash = GetProfileHash(profile);
                    }
                    return profile;
                }
            }
        }


        public async Task<Result> ShowIntelligentContactImport()
        {
            using (var repository = new AccountRepository())
            {
                var allow = await repository.ShowContactImport(UserHashObject.EntityID);
                if (allow)
                    return new Result() { Type = Enums.ResultType.Success };
            }
            return new Result() { Type = Enums.ResultType.Error };
        }





        public async Task<bool> IsEmailVerified()
        {
            using (var repository = new AccountRepository())
            {
                return await repository.VarificationDone(UserHashObject.EntityID);
            }
        }


        public async Task<Result> EntityTrack(string hash, string trackType)
        {
            var baseModel = hash.ToObject<ProfileHashViewModel>(null);
            var state = Mapper.Map<EntityState>(baseModel);
            state.EntityID = UserHashObject.EntityID;
            state.EntityID2 = baseModel.EntityID;
            state.IpAddress = IpAddress;
            state.CreatedOn = Now;
            state.Status = (byte)Enums.Status.Active;

            switch (trackType.ToLower())
            {
                case "profileview":
                    state.Type = (byte)Enums.EntityStateType.ProfileView;
                    break;
                default:
                    break;
            }
            using (var Repository = new AccountRepository())
            {
                var result = await Repository.EntityTrack(state);
                if (result > 0)
                {
                    switch (trackType.ToLower())
                    {
                        case "profileview":
                            {
                                if (EntityHash.ContainsKey("Url"))
                                    EntityHash.Remove("Url");
                                EntityHash.Add("Url", "/insights/visitors");
                                var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                                await notificationProcessor.AddNotification
                                        (
                                            Enums.NotificationType.ProfileVisited,
                                            null,
                                            UserHashObject.EntityID,
                                            Convert.ToInt64(baseModel.EntityID)
                                         );
                            }
                            break;
                        default:
                            break;
                    }
                    return new Result { Type = Enums.ResultType.Success, Description = "State added to analytics.", ReferenceID = result };
                }
                return new Result { Type = Enums.ResultType.Error, Description = "An error while tracking data.", ReferenceID = result };
            }
        }


        public async Task<List<EntityInsightViewModel>> Insights(int pageNumber, string id)
        {
            byte insighType = (byte)Enums.EntityStateType.ProfileView;
            switch (id.ToLower())
            {
                default:
                    break;
            }
            using (var _IAccountService = new AccountRepository())
            {
                using (var dataSet = await _IAccountService.Insights(pageNumber, insighType, UserHashObject.EntityID))
                {
                    return dataSet.Tables[0].FromDataTable<EntityInsightViewModel>();
                }
            }
        }

        public async Task<Result> Unsubscribe(string hash)
        {
            var emailType = Convert.ToInt32(hash.GetSingleValue("Type"));
            var email = hash.GetSingleValue("Email");
            long? entityID = default(long?);
            using (var repository = new AccountRepository())
            {
                using (var dataSet = await repository.GetUserByEmailAddress(email))
                {
                    if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
                    {
                        var profile = dataSet.Tables[0].FromDataTable<ProfileViewModel>().FirstOrDefault();
                        entityID = profile.EntityID;
                    }
                }
                var result = await repository.Unsubscribe(email, entityID, emailType, Now, false, IpAddress);
                if (result > 0)
                {
                    return new Result() { Type = Enums.ResultType.Success, Description = "You have successfully unsubscribed from this email category, you will no longer receive this email in future, we thank you for your feedback, we understand your priorities about your emails and we value it." };
                }
                return new Result() { Type = Enums.ResultType.Success, Description = "There is an error occurred while unsubscribing from this email category, we recommend you to refresh this page or try again later." };
            }
        }


        public async Task<Result> EmailPreferences(int type, bool subscribed)
        {
            using (var repository = new AccountRepository())
            {
                var result = await repository.Unsubscribe(null, UserHashObject.EntityID, type, Now, subscribed, IpAddress);
                if (result > 0)
                {
                    return new Result() { Type = Enums.ResultType.Success, Description = "You have successfully set preferences for this email category." };
                }
                return new Result() { Type = Enums.ResultType.Success, Description = "There is an error occurred." };
            }
        }


        public async Task<ProfileCompletetionSuggestionViewModel> GetProfileDataToImprove()
        {
            using (var _IAccountService = new AccountRepository())
            {
                using (var dataSet = await _IAccountService.GetProfileDataToImprove(UserHashObject.EntityID))
                {
                    return dataSet.Tables[0].FromDataTable<ProfileCompletetionSuggestionViewModel>().FirstOrDefault();
                }
            }
        }
    }
}