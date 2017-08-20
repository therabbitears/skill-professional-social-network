using System.Collections.Generic;
using System.Threading.Tasks;
using Wrly.Models;
using Wrly.Models.Business;
using Wrly.Models.Listing;
public interface IBusinessProcessor
{
    Types.Enums.OrganizationSaveStatus Save(NewOrganizationViewModel model, out string hash);

    Task<BusinessProfileViewModel> GetProfileWithStates(string profilename, bool includeStatistics);
    Task<BusinessProfileViewModel> GetProfileWithStates();

    Task<Types.Enums.OrganizationSaveStatus> Save(OrganizationSignupViewModel model);

    Task<AddressViewModel> GetPrimaryAddress();

    Task<Result> SetPrimaryAddress(AddressViewModel model);

    Task<int> AddExtendedInformation(ExtendedInfoViewModel model);

    Task<BusinessProfileViewModel> GetProfile();

    Task<Result> SaveName(OrgNameViewModel model);

    Task<Result> SaveCategory(OrgCategoryViewModel model);

    Task<BusinessAboutViewModel> GetAbout(string hash);

    Task<long> SaveAbout(BusinessAboutViewModel model);

    Task<BusinessProfileViewModel> BasicCompanyProfile();

    Task<Result> SaveBasic(BusinessProfileViewModel model);

    Task<AddressViewModel> GetAddress();

    Task<PhoneViewModel> GetPhone();

    Task<Result> SetPrimaryPhone(PhoneViewModel model);

    Task<List<OrganizationFaceViewModel>> Similar(string q);

    Task<BusinessProfileViewModel> GetOpenProfileWithStates(string profilename);

    Task<List<EmailConfigurationViewModel>> EmailConfiguration();

    Task<List<GroupViewModel>> Groups(string keyword);

    Task<GroupViewModel> Group(string id);

    Task<Result> CreateGroup(GroupViewModel model);

    Task<List<ActionAssociateProfileViewModel>> LoadMembers(string q, bool pending, bool group, int pageNumber, int pageSize = 20);
}