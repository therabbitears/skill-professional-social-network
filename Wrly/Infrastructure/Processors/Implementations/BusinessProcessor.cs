using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using AutoMapper;
using Types;
using Wrly.Data.Models;
using Wrly.Data.Repositories.Implementors;
using Wrly.infrastuctures.Utils;
using Wrly.Models;
using Wrly.Utils;
using System.Threading.Tasks;
using Wrly.Models.Business;
using System.Linq;
using Wrly.Models.Listing;
using System.IO;
using Wrly.Infrastructure.Utils;
using Wrly.Storage;
using Wrly.Infrastuctures.Utils;
using System.Text.RegularExpressions;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class BusinessProcessor : BaseProcessor, IBusinessProcessor
    {
        private const string REQUEST_ACTION = "ActionOnRequest";
        public Types.Enums.OrganizationSaveStatus Save(Wrly.Models.NewOrganizationViewModel model, out string hash)
        {
            var organization = new Organization();
            organization = Mapper.Map<Wrly.Models.NewOrganizationViewModel, Organization>(model);
            organization.Entity = new Entity() { EntityType = (short)Enums.EntityTypes.Organization };
            var address = Mapper.Map<Wrly.Models.NewOrganizationViewModel, Address>(model);
            organization.Entity.Addresses = new HashSet<Address>() { { address } };
            organization.Entity.Phones = new HashSet<Phone>() { { Mapper.Map<Wrly.Models.NewOrganizationViewModel, Phone>(model) } };
            organization.Entity.Emails = new HashSet<Email>() { { Mapper.Map<Wrly.Models.NewOrganizationViewModel, Email>(model) } };
            var profile = Mapper.Map<Wrly.Models.NewOrganizationViewModel, OrganizationProfile>(model);
            profile.IpAddress = HttpContext.Current.Request.UserHostAddress;
            profile.ProfileName = model.Name.Replace(" ", string.Empty).ToLower();
            organization.OrganizationProfiles = new HashSet<OrganizationProfile>() { profile };
            if (model.Logo != null && model.Logo.ContentLength > 0 && model.Logo.IsImageFile())
            {
                string filePath = HttpContext.Current.Server.MapPath("~/imagestore/organization/" + Guid.NewGuid());
                model.Logo.SaveAs(filePath);
                organization.Entity.EntityMedias = new HashSet<EntityMedia>() { { new EntityMedia() { MediaType = (int)Enums.MediaType.Image, Path = filePath } } };
            }
            long organizationId = 0;
            hash = string.Empty;
            organization.Type = (int)Enums.OrganizationType.Company;
            using (var repository = new BusinessRepository())
            {
                var result = repository.Save(organization, out organizationId);
                if (result == Enums.OrganizationSaveStatus.Success)
                {
                    var hasTable = new Hashtable();
                    hasTable.Add("OrganizationID", organizationId);
                    hasTable.Add("Name", model.Name);
                    hasTable.Add("EmailAddress", model.EmailAddress);
                    hash = QueryStringHelper.Encrypt(hasTable);
                }
                return result;
            }
        }


        public async Task<BusinessProfileViewModel> GetProfileWithStates(string profilename, bool inlcudeStatistics)
        {
            using (var repository = new BusinessRepository())
            {
                using (var dataset = await repository.GetProfileWithStates(profilename, UserHashObject.EntityID))
                {
                    var profile = dataset.Tables[0].FromDataTable<BusinessProfileViewModel>().FirstOrDefault();
                    profile.ProfileHash = GetProfileHash(profile);
                    profile.NetworkHash = GetConnectionHash(profile);
                    if (inlcudeStatistics)
                    {
                        using (var states = await repository.States(profile.EntityID))
                        {
                            profile.Statistics = states.Tables[0].FromDataTable<BusinessStatisticsViewModel>().FirstOrDefault();
                        }
                    }
                    return profile;
                }
            }
        }

        public async Task<BusinessProfileViewModel> GetOpenProfileWithStates(string profilename)
        {
            using (var _IAccountService = new BusinessRepository())
            {
                using (var dataSet = await _IAccountService.GetOpenProfile(profilename))
                {
                    var profile = dataSet.Tables[0].FromDataTable<BusinessProfileViewModel>().FirstOrDefault();
                    profile.ProfileHash = GetProfileHash(profile);
                    return profile;
                }
            }
        }


        public async Task<BusinessProfileViewModel> GetProfileWithStates()
        {
            using (var repository = new BusinessRepository())
            {
                using (var dataset = await repository.GetProfileWithStates(UserHashObject.EntityID))
                {
                    var profile = dataset.Tables[0].FromDataTable<BusinessProfileViewModel>().FirstOrDefault();
                    profile.ProfileHash = GetProfileHash(profile);
                    profile.NetworkHash = GetConnectionHash(profile);
                    return profile;
                }
            }
        }

        private string GetConnectionHash(BusinessProfileViewModel profile)
        {
            var table = new Hashtable();
            table.Add("Action", "Associate");
            table.Add("TimeStampGeneratedAt", DateTime.UtcNow.ToString());
            table.Add("EntityID", profile.EntityID);
            table.Add("EntityID2", UserHashObject.EntityID);
            table.Add("Location", "Location.ProfileFace");
            return QueryStringHelper.Encrypt(table);
        }

        private string GetProfileHash(BusinessProfileViewModel profile)
        {
            var hashTable = new Hashtable();
            hashTable.Add("EntityID", profile.EntityID);
            hashTable.Add("OrganizationID", profile.OrganizationID);
            hashTable.Add("Name", profile.Name);
            hashTable.Add("ID", Guid.NewGuid());
            return QueryStringHelper.Encrypt(hashTable);
        }


        public async Task<Enums.OrganizationSaveStatus> Save(OrganizationSignupViewModel model)
        {
            var organization = new Organization();
            organization = Mapper.Map<Wrly.Models.OrganizationSignupViewModel, Organization>(model);
            organization.Entity = new Entity() { EntityType = (short)Enums.EntityTypes.Organization };
            organization.Entity.Emails = new HashSet<Email>() { { Mapper.Map<OrganizationSignupViewModel, Email>(model) } };
            var profile = Mapper.Map<OrganizationSignupViewModel, OrganizationProfile>(model);
            profile.IpAddress = IpAddress;
            profile.ProfileName = model.Name.Replace(" ", string.Empty).ToLower();
            organization.OrganizationProfiles = new HashSet<OrganizationProfile>() { profile };
            organization.Type = (int)Enums.OrganizationType.Company;
            long organizationId = 0;
            using (var repository = new BusinessRepository())
            {
                var result = repository.Save(organization, out organizationId);
                return result;
            }
        }


        public async Task<AddressViewModel> GetPrimaryAddress()
        {
            using (var repository = new BusinessRepository())
            {
                var model = new AddressViewModel();
                var result = await repository.GetAddress(UserHashObject.EntityID);
                if (result != null && result.Tables[0].Rows.Count > 0)
                {
                    model = result.Tables[0].FromDataTable<AddressViewModel>().FirstOrDefault();
                }
                model.Hash = GetOrganizationHash();
                return model;
            }
        }

        private string GetOrganizationHash()
        {
            var hashTable = new Hashtable();
            hashTable.Add("EntityID", UserHashObject.EntityID);
            hashTable.Add("OrganizationID", UserHashObject.OrganizationID);
            hashTable.Add("ID", Guid.NewGuid());
            return QueryStringHelper.Encrypt(hashTable);
        }


        public async Task<Result> SetPrimaryAddress(AddressViewModel model)
        {
            var address = Mapper.Map<Wrly.Models.AddressViewModel, Address>(model);
            address = model.Hash.ToObject<Address>(address);
            address.EntityID = UserHashObject.EntityID;
            address.AddressType = (int)Enums.AddressType.Primary;
            using (var addressRepository = new AddressRepository())
            {
                address.EntityID = UserHashObject.EntityID;
                var result = await addressRepository.Save(address);

                if (result > 0)
                    return new Result() { Type = Enums.ResultType.Success, Description = "Primary address has been saved successfully." };

                return new Result() { Type = Enums.ResultType.Error, Description = "An error while updating primary address, please try it again." };
            }
        }


        public async Task<int> AddExtendedInformation(ExtendedInfoViewModel model)
        {
            var phone = Mapper.Map<Wrly.Models.ExtendedInfoViewModel, Phone>(model);
            var keyValue = new Dictionary<string, string>();
            keyValue.Add("EmployeeStrength", model.EmployeeStrength);
            keyValue.Add("EstablishedYear", model.EstablishedYear);

            var profileKeyValue = new Dictionary<string, string>();
            profileKeyValue.Add("Description", model.Description);
            using (var repository = new BusinessRepository())
            {
                var resultProfile = await repository.UpdateOrganizationItems(keyValue, profileKeyValue, UserHashObject.OrganizationID);
                if (resultProfile > 0)
                {
                    using (var addressRepository = new PhoneRespository())
                    {
                        phone.EntityID = UserHashObject.EntityID;
                        resultProfile = await addressRepository.Save(phone);
                        return resultProfile;
                    }
                }
            }
            return -1;
        }


        public async Task<BusinessProfileViewModel> GetProfile()
        {
            using (var repository = new BusinessRepository())
            {
                using (var dataset = await repository.GetProfileWithStates(UserHashObject.EntityID))
                {
                    var profile = dataset.Tables[0].FromDataTable<BusinessProfileViewModel>().FirstOrDefault();
                    profile.ProfileHash = GetProfileHash(profile);
                    return profile;
                }
            }
        }

        public async Task<Result> SaveName(OrgNameViewModel model)
        {
            var profileKeyValue = new Dictionary<string, string>();
            profileKeyValue.Add("Name", model.Name);
            using (var repository = new BusinessRepository())
            {
                var result = await repository.UpdateOrganizationItems(profileKeyValue, null, UserHashObject.OrganizationID);
                if (result > 0)
                    return new Result() { Type = Enums.ResultType.Success, Description = model.Name };
                return new Result() { Type = Enums.ResultType.Error, Description = string.Empty };
            }
        }


        public async Task<Result> SaveCategory(OrgCategoryViewModel model)
        {
            var profileKeyValue = new Dictionary<string, string>();
            profileKeyValue.Add("CategoryID", model.CategoryID.ToString());
            using (var repository = new BusinessRepository())
            {
                var result = await repository.UpdateOrganizationItems(profileKeyValue, null, UserHashObject.OrganizationID);
                if (result > 0)
                {
                    var profile = await GetProfileWithStates();
                    return new Result() { Type = Enums.ResultType.Success, Description = profile.Category.ToString() };
                }
                return new Result() { Type = Enums.ResultType.Error, Description = string.Empty };
            }
        }


        public async Task<BusinessAboutViewModel> GetAbout(string hash)
        {
            using (var repository = new BusinessRepository())
            {
                using (var dataSet = await repository.GetProfile(UserHashObject.EntityID))
                {
                    return dataSet.Tables[0].FromDataTable<BusinessAboutViewModel>().FirstOrDefault();
                }
            }
        }

        public async Task<long> SaveAbout(BusinessAboutViewModel model)
        {
            var profileKeyValue = new Dictionary<string, string>();
            profileKeyValue.Add("Description", model.Description.ToString());
            using (var repository = new BusinessRepository())
            {
                return await repository.UpdateOrganizationItems(null, profileKeyValue, UserHashObject.OrganizationID);
            }
        }


        public async Task<BusinessProfileViewModel> BasicCompanyProfile()
        {
            using (var repository = new BusinessRepository())
            {
                using (var dataset = await repository.GetProfile(UserHashObject.EntityID))
                {
                    var profile = dataset.Tables[0].FromDataTable<BusinessProfileViewModel>().FirstOrDefault();
                    profile.ProfileHash = GetProfileHash(profile);
                    profile.NetworkHash = GetConnectionHash(profile);
                    return profile;
                }
            }
        }


        public async Task<Result> SaveBasic(BusinessProfileViewModel model)
        {
            var keyValue = new Dictionary<string, string>();
            keyValue.Add("EmployeeStrength", model.EmployeeStrength);
            keyValue.Add("CategoryID", model.CategoryID.ToString());

            var profileKeyValue = new Dictionary<string, string>();
            profileKeyValue.Add("Website", model.Website);
            using (var repository = new BusinessRepository())
            {
                var resultProfile = await repository.UpdateOrganizationItems(keyValue, profileKeyValue, UserHashObject.OrganizationID);
                if (resultProfile > 0)
                    return new Result() { Type = Enums.ResultType.Success, Description = "Saved successfully." };
            }
            return new Result() { Type = Enums.ResultType.Error, Description = "An error while saving information, please try it again." };
        }


        public async Task<AddressViewModel> GetAddress()
        {
            using (var repository = new BusinessRepository())
            {
                using (var dataset = await repository.GetProfile(UserHashObject.EntityID))
                {
                    var address = dataset.Tables[0].FromDataTable<AddressViewModel>().FirstOrDefault();
                    var hashTable = new Hashtable();
                    hashTable.Add("EntityID", UserHashObject.EntityID);
                    hashTable.Add("AddressId", address.AddressId);
                    hashTable.Add("ID", Guid.NewGuid());
                    address.Hash = QueryStringHelper.Encrypt(hashTable);
                    return address;
                }
            }
        }


        public async Task<PhoneViewModel> GetPhone()
        {
            using (var repository = new BusinessRepository())
            {
                using (var dataset = await repository.GetProfile(UserHashObject.EntityID))
                {
                    var phone = dataset.Tables[0].FromDataTable<PhoneViewModel>().FirstOrDefault();
                    var hashTable = new Hashtable();
                    hashTable.Add("EntityID", UserHashObject.EntityID);
                    hashTable.Add("PhoneID", phone.PhoneID);
                    hashTable.Add("PhoneType", phone.PhoneType);
                    hashTable.Add("ID", Guid.NewGuid());
                    phone.Hash = QueryStringHelper.Encrypt(hashTable);
                    return phone;
                }
            }
        }

        public async Task<Result> SetPrimaryPhone(PhoneViewModel model)
        {
            var phoneModel = Mapper.Map<Phone>(model);
            phoneModel = model.Hash.ToObject<Phone>(phoneModel);
            phoneModel.Phone1 = model.Phone;
            using (var repository = new BusinessRepository())
            {
                var resultProfile = await repository.SavePhone(phoneModel, UserHashObject.EntityID);
                if (resultProfile > 0)
                    return new Result() { Type = Enums.ResultType.Success, Description = "Primary phone has been saved successfully." };
            }
            return new Result() { Type = Enums.ResultType.Error, Description = "An error while updating primary phone number, please try it again." };
        }


        public async Task<List<OrganizationFaceViewModel>> Similar(string q)
        {
            var baseModel = q.ToObject<ProfileHashViewModel>(null);
            using (var repository = new BusinessRepository())
            {
                using (var similarCompanies = await repository.GetSimilar(baseModel.EntityID))
                {
                    return similarCompanies.Tables[0].FromDataTable<OrganizationFaceViewModel>();
                }
            }
        }


        public async Task<List<EmailConfigurationViewModel>> EmailConfiguration()
        {
            using (var repository = new BusinessRepository())
            {
                using (var dataset = await repository.EmailConfiguration(UserHashObject.EntityID))
                {
                    return dataset.Tables[0].FromDataTable<EmailConfigurationViewModel>();
                }
            }
        }


        public async Task<List<GroupViewModel>> Groups(string keyword)
        {
            using (var repository = new BusinessRepository())
            {
                var entityID = UserHashObject != null ? UserHashObject.EntityID : 0;
                using (var dataset = await repository.Groups(keyword, entityID))
                {
                    var groups = dataset.Tables[0].FromDataTable<GroupViewModel>();
                    foreach (var group in groups)
                    {
                        var table = new Hashtable();
                        table.Add("Action", REQUEST_ACTION);
                        table.Add("TimeStampGeneratedAt", DateTime.UtcNow.ToString());
                        table.Add("EntityID", group.EntityID);
                        table.Add("EntityID2", entityID);
                        table.Add("Location", "Location.ProfileFace");
                        table.Add("RequirePermission", group.RequirePermission);
                        group.NetworkHash = QueryStringHelper.Encrypt(table);
                    }
                    return groups;
                }
            }
        }

        public async Task<GroupViewModel> Group(string id)
        {
            using (var repository = new BusinessRepository())
            {
                var entityID = UserHashObject != null ? UserHashObject.EntityID : 0;
                using (var dataset = await repository.Groups(null, entityID, id))
                {
                    var group = dataset.Tables[0].FromDataTable<GroupViewModel>().FirstOrDefault();
                    group.Associations = dataset.Tables[1].FromDataTable<AssociationViewModel>();
                    var hashTable = new Hashtable();
                    hashTable.Add("EntityID", group.EntityID);
                    hashTable.Add("GroupID", group.OrganizationID);
                    hashTable.Add("Name", group.Name);
                    hashTable.Add("ID", Guid.NewGuid());
                    group.ProfileHash = QueryStringHelper.Encrypt(hashTable);

                    var table = new Hashtable();
                    table.Add("Action", REQUEST_ACTION);
                    table.Add("TimeStampGeneratedAt", DateTime.UtcNow.ToString());
                    table.Add("EntityID", group.EntityID);
                    table.Add("EntityID2", entityID);
                    table.Add("Location", "Location.ProfileFace");
                    table.Add("RequirePermission", group.RequirePermission);
                    group.NetworkHash = QueryStringHelper.Encrypt(table);
                    return group;
                }
            }
        }


        public async Task<Result> CreateGroup(GroupViewModel model)
        {
            var organization = new Organization();
            organization = Mapper.Map<GroupViewModel, Organization>(model);
            organization.Entity = new Entity() { EntityType = (short)Enums.EntityTypes.Group };
            organization.Entity.Associations = new HashSet<Association>() 
            { 
                new Association() 
                {
                    EntityID2 = UserHashObject.EntityID, 
                    AssociationType = (int) Enums.AssociationType.GroupOwner,
                    CreatedBy  =User,
                    CreatedOn = Now,
                    EditedBy  =User,
                    EditedOn = Now,
                    IpAddress=IpAddress,
                    ObjectStatus= (int)Enums.AssociationRequestStatus.Approve
                } ,
                new Association() 
                {
                    EntityID2 = UserHashObject.EntityID, 
                    AssociationType = (int) Enums.AssociationType.Follow,
                    CreatedBy  =User,
                    CreatedOn = Now,
                    EditedBy  =User,
                    EditedOn = Now,
                    IpAddress=IpAddress,
                    ObjectStatus= (int)Enums.AssociationRequestStatus.Approve
                } 
            };
            var profile = Mapper.Map<GroupViewModel, OrganizationProfile>(model);
            profile.IpAddress = HttpContext.Current.Request.UserHostAddress;
            var rgx = new Regex("[^a-zA-Z0-9]");
            profile.ProfileName = rgx.Replace(model.Name.Length > 30 ? model.Name.Substring(0, 30) : model.Name, "-").ToLower();
            organization.OrganizationProfiles = new HashSet<OrganizationProfile>() { profile };
            long organizationId = 0;
            organization.Type = (int)Enums.OrganizationType.Group;
            if (organization.CategoryID == 0)
            {
                organization.CategoryID = (int)Enums.StaticCategories.Media;
            }

            using (var repository = new BusinessRepository())
            {
                if (model.Logo != null)
                {
                    var byteArr = new byte[model.Logo.ContentLength];
                    model.Logo.InputStream.Read(byteArr, 0, model.Logo.ContentLength);
                    var guid = Guid.NewGuid().ToString();
                    var nowByte = CommonFunctions.CropImage(byteArr, model.ImgX1, model.ImgY1, model.ImgWidth, model.ImgHeight);
                    if (model.ImgWidth > 200)
                    {
                        var refPng = true;
                        nowByte = Biz2Dial.Images.Resizer.General.ResizeFile(byteArr, 200, ref refPng);
                    }

                    var resultFile = ImageProcessor.UploadImage(nowByte, Enums.ImageObject.GroupImage, string.Empty, true, Enums.FileType.Image, guid + "__group__", AppConfig.StorageProvider, AppConfig.SiteUrl);
                    organization.OrganizationProfiles.FirstOrDefault().ProfileImagePath = resultFile.FileName;
                }

                var result = repository.Save(organization, out organizationId);
                if (result == Enums.OrganizationSaveStatus.Success)
                {
                    return new Result() { Type = Enums.ResultType.Success, Description = "/groups/" + profile.ProfileName };
                }
                return new Result() { Type = Enums.ResultType.Error, Description = "There is an error while creating the group, please try again." };
            }
        }


        public async Task<List<ActionAssociateProfileViewModel>> LoadMembers(string q, bool pending, bool group, int pageNumber, int pageSize = 20)
        {
            var baseModel = q.ToObject<ProfileHashViewModel>(null);
            using (var repository = new BusinessRepository())
            {
                var entityID = UserHashObject != null ? UserHashObject.EntityID : 0;
                using (var dataset = await repository.Members
                        (
                            entityID,
                            baseModel.EntityID,
                            pending ? (int)Enums.AssociationRequestStatus.Pending : (int)Enums.AssociationRequestStatus.Approve,
                            pageNumber,
                            20,
                            (int)Enums.AssociationType.Follow
                        )
                       )
                {
                    var list = dataset.Tables[0].FromDataTable<ActionAssociateProfileViewModel>();
                    foreach (var item in list)
                    {
                        var table = new Hashtable();
                        table.Add("Action", REQUEST_ACTION);
                        table.Add("TimeStampGeneratedAt", DateTime.UtcNow.ToString());
                        table.Add("EntityID", item.EntityID2);
                        table.Add("AssociationID", item.AssociationID);
                        table.Add("EntityID2", item.EntityID);
                        item.Hash = QueryStringHelper.Encrypt(table);
                    }
                    return list;
                }
            }
        }
    }
}