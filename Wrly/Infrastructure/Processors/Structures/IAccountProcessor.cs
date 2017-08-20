using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using Wrly.Models;
using Wrly.Models.Feeds;
using Wrly.Models.Listing;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface IAccountProcessor
    {
        Task<ProfileViewModel> GetProfile(string userName);
        Task<ProfileViewModel> GetProfileWithStates(string userName, bool inlcudeStatistics = false, bool isProfileName = false);
        long AddUserExtendedInfo(Wrly.Models.ApplicationUser user);
        int UpdateProfile(Dictionary<string, string> values, string mode);
        void DeleteProfileImage(string uid);

        void SendWelcomeEmail(Wrly.Models.RegisterViewModel model);

        void AddUserExtendedInfoWithJobRole(ApplicationUser applicationUser, ConnectAccountViewModel model);

        Task<UserStatisticsViewModel> GetStatistics();

        Task<AboutViewModel> GetAbout(string hash);

        Task<long> SaveAbout(AboutViewModel model);

        Task<List<CareerLineViewModel>> GetCareerLine(string id, bool includeS = true, bool includeA = true);

        Task<GeneralSettingViewModel> GeneralSettings(long entityID);

        Task<NetworkSettingViewModel> NetworkSettings(long entityID);

        Task<PrivacySettingViewModel> PrivacySettings(long entityID);

        Task<List<WidgetSettingViewModel>> WidgetsSettings(long entityID);

        Task<JobSearchViewModel> JobSearchSettings(long entityID);

        Task<Result> SaveProfileName(GeneralSettingViewModel model);

        Task<long> SaveSearchEngineSetting(bool enabled);

        Task<long> SaveNetworkCoverage(NetworkSettingViewModel model);

        Task<long> SetJobAppurtunities(bool enabled);

        Task<long> SetAllowReference(bool enabled);

        Task<long> SetOppurtunityLevel(JobSearchViewModel model);

        Task<long> SetWidgetSubscription(BaseViewModel hash, bool value);

        Task<IntelligenceViewModel> Intelligence();

        Task<bool> SkipIntelligence(BaseViewModel hash);

        Task<List<CareerLineViewModel>> GetCareerLineForProfile(string profileHash, string id, bool includeS, bool includeA);

        Task<List<SkillViewModel>> GetCommonSkills(string q);

        Task<List<OrganizationFaceViewModel>> GetCommonCompanies(string q);

        Task<SnapShotViewModel> SnapShot();

        Task<Result> SendVarification(string hash, bool skipIfAlreadyExisit = false);

        Task<Result> SaveName(NameViewModel model);

        Task<Result> SaveHeading(HeadingViewModel model);

        Task<EntityProfileViewModel> HoverCard(string id);

        Task SendOrganizationWelcomeEmail(OrganizationSignupViewModel model);

        Task<Result> ForgottenPasswordEmail(ForgotPasswordViewModel model);

        Task<Result> GetUserInformationByForgotLink(string hash);

        Task<Result> RemoveForgotPasswordData(string hash);

        Task<Result> WarmUp(long entityID);

        Task<Result> UploadProfileImage(ProfilePicViewModel model);

        Task<Result> UploadCoverImage(ProfilePicViewModel model);

        Task<NonLoggedInProfileViewModel> GetOpenProfileWithStates(string profileName);

        Task<Result> ShowIntelligentContactImport();

        Task<Result> SetIndividualConnectionStatus(bool enabled);

        Task<bool> IsEmailVerified();

        Task<Result> EntityTrack(string hash, string trackType);

        Task<List<EntityInsightViewModel>> Insights(int pageNumber, string id);

        Task<Result> Unsubscribe(string hash);

        Task<Result> EmailPreferences(int type, bool subscribed);

        Task<ProfileCompletetionSuggestionViewModel> GetProfileDataToImprove();
    }
}
