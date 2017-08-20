using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Types;
using Wrly.Data.Models;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.infrastuctures.Utils;
using Wrly.Infrastuctures.Utils;
using Wrly.Models;
using Wrly.Models.Listing;
using Wrly.Utils;
using Wrly.Infrastructure.Utils;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class CareerHistoryProcessor : BaseProcessor, ICareerHistoryProcessor
    {
        public async Task<List<Models.Listing.CareerHistoryViewModel>> GetCareerHisotry(short type, string subType)
        {
            using (var repository = new CareerHistoryRepository())
            {
                var dataSet = await repository.ForUser(UserHashObject.EntityID, false, type, subType);
                var result = dataSet.Tables[0].FromDataTable<CareerHistoryViewModel>();
                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        if (Request.IsAuthenticated)
                        {
                            var table = new Hashtable();
                            table.Add("CareerHistoryID", item.CareerHistoryID);
                            table.Add("ID", DateTime.UtcNow.Ticks);
                            item.Hash = QueryStringHelper.Encrypt(table);
                            item.AllowEdit = true;
                        }
                        item.StartFromMonthName = item.StartFromMonth != null ? ((Enums.Months)item.StartFromMonth).GetDescription() : string.Empty;
                        item.EndFromMonthName = item.EndFromMonth != null ? ((Enums.Months)item.EndFromMonth).GetDescription() : string.Empty;
                        item.SkillIncluded = dataSet.Tables[1].FromDataTable<CareerHistorySkillViewModel>("CareerHistoryID=" + item.CareerHistoryID);
                        foreach (var skill in item.SkillIncluded)
                        {
                            var table = new Hashtable();
                            table.Add("CareerHistoryID", item.CareerHistoryID);
                            table.Add("EntityID", item.EntityID);
                            table.Add("ID", skill.ID);
                            table.Add("TimeStamp", DateTime.UtcNow);
                            skill.Hash = QueryStringHelper.Encrypt(table);
                            skill.AllowEdit = true;
                        }
                    }
                }
                return result;
            }
        }


        public async Task<CareerHistoryViewModel> GetOneCareerHisotry(string hash)
        {
            var careerModel = hash.ToObject<CareerHistoryViewModel>(null);
            using (var repository = new CareerHistoryRepository())
            {
                var dataSet = await repository.Single(careerModel.CareerHistoryID);
                var result = dataSet.Tables[0].FromDataTable<CareerHistoryViewModel>()[0];
                var table = new Hashtable();
                table.Add("CareerHistoryID", result.CareerHistoryID);
                table.Add("EntityID", result.EntityID);
                table.Add("ID", DateTime.UtcNow.Ticks);
                result.Hash = QueryStringHelper.Encrypt(table);
                result.SkillIncluded = dataSet.Tables[1].FromDataTable<CareerHistorySkillViewModel>("CareerHistoryID=" + result.CareerHistoryID);
                return result;
            }
        }


        public async Task<long> Save(CareerHistoryViewModel model, bool wizard = false)
        {
            var careerHistory = new CareerHistory();
            bool processStage = false;
            if (!string.IsNullOrEmpty(model.Hash))
            {
                model = model.Hash.ToObject<CareerHistoryViewModel>(model);
                careerHistory = Mapper.Map<CareerHistoryViewModel, CareerHistory>(model, careerHistory);
                if (!wizard)
                {
                    if (careerHistory.EntityID != UserHashObject.EntityID)
                        return -1;
                }
                else
                {
                    if (careerHistory.EntityID != UserHashObject.EntityID || careerHistory.CareerHistoryID <= 0)
                        return -1;
                    model = model.Hash.ToObject<CareerHistoryViewModel>(model);
                }
            }
            else
            {
                processStage = true;
                careerHistory = Mapper.Map<CareerHistoryViewModel, CareerHistory>(model, careerHistory);
                careerHistory = model.UserHash.ToObject<CareerHistory>(careerHistory);
            }
            careerHistory.JobTitle = Mapper.Map<JobTitle>(model);
            careerHistory.JobTitle.Active = false;
            careerHistory.JobTitle.IpAddress = IpAddress;

            FixDates(careerHistory);

            if (model.Skills != null && model.Skills.Length > 0)
            {
                careerHistory.CareerHistorySkills = new HashSet<CareerHistorySkill>();
                foreach (var item in model.Skills)
                {
                    careerHistory.CareerHistorySkills.Add(new CareerHistorySkill() { EntitySkillID = Convert.ToInt64(item) });
                }
            }

            if (model.Type == (int)Enums.CareerHistoryMode.Education)
            {
                processStage = false;
                careerHistory.Organization = new Organization()
                {
                    Name = model.OrganizationName,
                    Type = (int)Enums.OrganizationType.University,
                    Entity = new Entity()
                    {
                        EntityType = (short)Enums.EntityTypes.Organization
                    },
                    CategoryID = (int)Enums.StaticCategories.EducationAndCertificationProvider
                };

                careerHistory.Type = (int)Enums.CareerHistoryMode.Education;
                var profile = Mapper.Map<CareerHistoryViewModel, OrganizationProfile>(model);
                profile.IpAddress = HttpContext.Current.Request.UserHostAddress;
                profile.ProfileName = careerHistory.Organization.Name.Replace(" ", "-").ToLower();
                careerHistory.Organization.OrganizationProfiles = new HashSet<OrganizationProfile>() { profile };
            }

            using (var repository = new CareerHistoryRepository())
            {
                var result = await repository.Save(careerHistory);
                if (result > 0)
                {
                    if (model.Type == (int)Enums.CareerHistoryMode.Profession)
                    {
                        bool success = false;
                        using (var accountRepository = new AccountRepository())
                        {
                            var keyValue = new Dictionary<string, string>();
                            if (model.Type == (int)Enums.CareerHistoryMode.Profession)
                            {
                                await UpdateCurrentCareerHistory(model.Type);
                            }
                            if (model.Type == (int)Enums.CareerHistoryMode.Education && model.SubType == Enums.EducationType.Course.ToString())
                            {
                                await UpdateCurrentCareerHistory(model.Type);
                            }
                            if (processStage)
                                keyValue.Add("ProfileLevel", ((int)Enums.CareerStage.Employement).ToString());

                            if (keyValue.Count > 0)
                                success = await accountRepository.UpdateProfileItems(keyValue, UserHashObject.PersonID) > 0;

                            await accountRepository.UpdateProfileScore(UserHashObject.EntityID);
                        }

                        if (careerHistory.CareerHistoryID == 0 && careerHistory.PottentialStartDate != null && careerHistory.PottentialStartDate.Value.AddDays(3) >= Now)
                        {
                            var activity = new NetworkActivity()
                            {
                                Type = (int)Enums.NetworkActivityType.JoinedCompany,
                                IpAddress = IpAddress,
                                Identifier = string.Format("{0}_{1}_{2}", UserHashObject.EntityID, (int)Enums.NetworkActivityType.JoinedCompany, result),
                                EntityID = UserHashObject.EntityID,
                                EditedOn = Now,
                                EditedBy = User,
                                CareerHistoryID = result,
                                CreatedOn = Now,
                                ActionTaken = false
                            };
                            using (var networkActivity = new CommonRepository())
                            {
                                await networkActivity.AddActivity(activity);
                            }
                        }
                    }

                    if (careerHistory.CareerHistorySkills != null && careerHistory.CareerHistorySkills.Count > 0)
                    {
                        using (var skillRepository = new EntitySkillRepository())
                        {
                            foreach (var item in careerHistory.CareerHistorySkills)
                            {
                                await skillRepository.CalculateAndSetSkillScore(item.EntitySkillID, UserHashObject.EntityID);
                            }
                        }
                    }
                    await CommonFunctions.UpdateUserFaceIndex(UserHashObject.EntityID, (int)Enums.EntityTypes.Person);
                }
                return result;
            }
        }

        private void FixDates(CareerHistory careerHistory)
        {
            if (careerHistory.StartFromMonth == -1)
                careerHistory.StartFromMonth = null;

            if (careerHistory.StartFromYear == -1)
                careerHistory.StartFromYear = null;

            if (careerHistory.EndFromMonth == -1)
                careerHistory.EndFromMonth = null;

            if (careerHistory.EndFromYear == -1)
                careerHistory.EndFromYear = null;

            careerHistory.PottentialStartDate = GetPottentialStartDateForFormData(careerHistory.StartFromYear, careerHistory.StartFromMonth, careerHistory.StartFromDay);
            careerHistory.PottentialEndDate = GetPottentialEndDateForFormData(careerHistory.EndFromYear, careerHistory.EndFromMonth, careerHistory.EndFromDay);
            careerHistory.PottentialCurrent = GetPottentialCurrent(careerHistory.PottentialEndDate);
        }

        public async Task<Dictionary<long, string>> GetCareerHisotry()
        {
            var result = new Dictionary<long, string>() { { -1, "Select" } };
            using (var repository = new CareerHistoryRepository())
            {
                var dataSet = await repository.ForUser(UserHashObject.EntityID, false);
                foreach (DataRow item in dataSet.Tables[0].Rows)
                {
                    string name = Convert.ToInt16(item["Type"]) == (short)Enums.CareerHistoryMode.Education ?
                        string.Format("Student at {0} while {1}", item["OrganizationName"], item["JobTitleName"]) :
                        string.Format("Working at {0} as {1}", item["OrganizationName"], item["JobTitleName"]);
                    result.Add(Convert.ToInt64(item["CareerHistoryID"]), name);
                }
            }
            return result;
        }


        public async Task<long> RemoveSkill(string hash)
        {
            var participant = hash.ToObject<CareerHistorySkill>(null);
            var generalEntity = hash.ToObject<GeneralEntity>(null);
            if (generalEntity.EntityID == UserHashObject.EntityID)
            {
                using (var repository = new CareerHistorySkillRepository())
                {
                    return await repository.Delete(participant.ID);
                }
            }
            return -1;
        }


        public async Task<string> GenerateTokenForCareerHistoryForEntity(long result)
        {
            var table = new Hashtable();
            table.Add("CareerHistoryID", result);
            table.Add("EntityID", UserHashObject.EntityID);
            table.Add("ID", Guid.NewGuid());
            table.Add("TimeStamp", DateTime.UtcNow);
            var hash = QueryStringHelper.Encrypt(table);
            var single = await GetOneCareerHisotry(hash);
            table.Add("JobTitleID", single.JobTitleID);
            table.Add("JobTitleName", single.JobTitleName);
            return QueryStringHelper.Encrypt(table);
        }


        public async Task<IntelligenceCareerHistoryViewModel> GetImprovableCareerHistory(int type)
        {
            using (var repository = new CareerHistoryRepository())
            {
                var dataSet = await repository.GetImprovableCareerHistoryRow(type, UserHashObject.EntityID);
                var careerHisory = new IntelligenceCareerHistoryViewModel();
                careerHisory.Hash = GetDataRowAsHash(dataSet);
                careerHisory.DisplayText = string.Format("Add where you have been worked as {1}", dataSet.Tables[0].Rows[0]["JobTitleName"]);
                return careerHisory;
            }
        }

        public async Task<IntelligenceCareerHistoryViewModel> GetImprovableCareerHistoryForTime(int type)
        {
            using (var repository = new CareerHistoryRepository())
            {
                var dataSet = await repository.GetImprovableCareerHistoryForTime(type, UserHashObject.EntityID);
                var careerHisory = new IntelligenceCareerHistoryViewModel();
                careerHisory.Hash = GetDataRowAsHash(dataSet);
                careerHisory.DisplayText = string.Format("Add when you did started working at {0} as {1}", dataSet.Tables[0].Rows[0]["OrganizationName"], dataSet.Tables[0].Rows[0]["JobTitleName"]);
                return careerHisory;
            }
        }

        private string GetDataRowAsHash(DataSet dataSet)
        {
            var hashTable = new Hashtable();
            foreach (DataColumn item in dataSet.Tables[0].Columns)
            {
                hashTable.Add(item.ColumnName, dataSet.Tables[0].Rows[0][item.ColumnName]);
            }
            return QueryStringHelper.Encrypt(hashTable);
        }


        public async Task<IntelligenceAwardViewModel> GetImprovableAward()
        {
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.GetImprovableAwardForTeam(UserHashObject.EntityID);
                var award = new IntelligenceAwardViewModel();
                award.Hash = GetDataRowAsHash(dataSet);
                award.DisplayText = string.Format("Add your team members to your honor {0}", dataSet.Tables[0].Rows[0]["Name"]);
                return award;
            }
        }


        public async Task<IntelligenceAwardViewModel> GetImprovableAssignment()
        {
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.GetImprovableAssignment(UserHashObject.EntityID);
                var award = new IntelligenceAwardViewModel();
                award.Hash = GetDataRowAsHash(dataSet);
                award.Type = Convert.ToByte(dataSet.Tables[0].Rows[0]["Type"]);
                if (award.Type == (int)Enums.AwardAndAssignmentMode.Assignment)
                {
                    award.DisplayText = string.Format("Add your team members to your project {0}", dataSet.Tables[0].Rows[0]["Name"]);
                }
                else if (award.Type == (int)Enums.AwardAndAssignmentMode.Composition)
                {
                    award.DisplayText = string.Format("Add your team members to your composition {0}", dataSet.Tables[0].Rows[0]["Name"]);
                }
                else if (award.Type == (int)Enums.AwardAndAssignmentMode.Finding)
                {
                    award.DisplayText = string.Format("Add your team members to your finding {0}", dataSet.Tables[0].Rows[0]["Name"]);
                }
                else if (award.Type == (int)Enums.AwardAndAssignmentMode.Publication)
                {
                    award.DisplayText = string.Format("Add your team members to your publication {0}", dataSet.Tables[0].Rows[0]["Name"]);
                }
                else if (award.Type == (int)Enums.AwardAndAssignmentMode.Research)
                {
                    award.DisplayText = string.Format("Add your team members to your research {0}", dataSet.Tables[0].Rows[0]["Name"]);
                }
                else
                    award.DisplayText = string.Format("Add your team members to your accomplition {0}", dataSet.Tables[0].Rows[0]["Name"]);
                return award;
            }
        }


        public async Task<List<CareerHistoryViewModel>> GetCareerHisotryForProfile(short mode, string profileHash, string subType = null)
        {
            using (var repository = new CareerHistoryRepository())
            {
                var baseModel = profileHash.ToObject<ProfileHashViewModel>(null);
                var dataSet = await repository.ForUser(baseModel.EntityID, false, mode, subType);
                var result = dataSet.Tables[0].FromDataTable<CareerHistoryViewModel>();
                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        if (Request.IsAuthenticated)
                        {
                            var table = new Hashtable();
                            table.Add("CareerHistoryID", item.CareerHistoryID);
                            table.Add("ID", DateTime.UtcNow.Ticks);
                            table.Add("EntityID", item.EntityID);
                            item.Hash = QueryStringHelper.Encrypt(table);
                            item.AllowEdit = true;
                        }
                        item.StartFromMonthName = item.StartFromMonth != null ? ((Enums.Months)item.StartFromMonth).GetDescription() : string.Empty;
                        item.EndFromMonthName = item.EndFromMonth != null ? ((Enums.Months)item.EndFromMonth).GetDescription() : string.Empty;
                        item.SkillIncluded = dataSet.Tables[1].FromDataTable<CareerHistorySkillViewModel>("CareerHistoryID=" + item.CareerHistoryID);
                        foreach (var skill in item.SkillIncluded)
                        {
                            var table = new Hashtable();
                            table.Add("CareerHistoryID", item.CareerHistoryID);
                            table.Add("EntityID", item.EntityID);
                            table.Add("ID", skill.ID);
                            table.Add("TimeStamp", DateTime.UtcNow);
                            skill.Hash = QueryStringHelper.Encrypt(table);
                            skill.AllowEdit = true;
                        }
                    }
                }
                return result;
            }
        }


        public async Task<List<CareerHistoryOption>> LoadAssginementOptions(string q)
        {
            var list = new List<CareerHistoryOption>();
            using (var repository = new CareerHistoryRepository())
            {
                var awardModel = q.ToObject<CareerHistoryViewModel>(null);
                if (awardModel.EntityID != UserHashObject.EntityID)
                {
                    var awardDetail = await GetOneCareerHisotry(q);
                    using (var dataSet = await repository.LoadOptionsForCareerHistory(awardModel.CareerHistoryID, UserHashObject.EntityID))
                    {
                        var row = dataSet.Tables[0].FromDataTable<CareerHistoryForEntity>().FirstOrDefault();
                        var option = new CareerHistoryOption();
                        var hashTable = new Hashtable();
                        hashTable.Add("JobTitleName", awardDetail.JobTitleName);
                        hashTable.Add("Type", awardDetail.Type);

                        hashTable.Add("SubType", awardDetail.SubType);
                        hashTable.Add("EntityID", awardDetail.EntityID);
                        hashTable.Add("DbID", Guid.NewGuid());
                        if (row.AlreadyRecommended == 0)
                        {
                            hashTable.Add("CareerHistoryID", awardDetail.CareerHistoryID);
                            hashTable.Add("Action", "Recommend");
                            option = new CareerHistoryOption()
                            {
                                DisplayText = "Recommend role",
                                Hash = QueryStringHelper.Encrypt(hashTable),
                                Action = "recommend",
                                Type = ((Enums.CareerHistoryMode)awardDetail.Type).GetDescription().ToLower()
                            };
                            list.Add(option);
                        }
                    }
                }
                return list;
            }
        }


        public async Task<long> SaveWizard(CareerHistoryWizardViewModel model, bool ispopup)
        {
            using (var repository = new CareerHistoryRepository())
            {
                var careerHistory = new CareerHistory();
                model = model.Hash.ToObject<CareerHistoryWizardViewModel>(model);
                careerHistory = Mapper.Map<CareerHistoryWizardViewModel, CareerHistory>(model, careerHistory);
                if (careerHistory.EntityID != UserHashObject.EntityID)
                    return -1;
                using (var existing = await repository.GetIfAny(UserHashObject.EntityID))
                {
                    if (existing != null && existing.Tables[0].Rows.Count > 0)
                    {
                        careerHistory.CareerHistoryID = Convert.ToInt64(existing.Tables[0].Rows[0]["CareerHistoryID"]);
                    }
                }
                careerHistory.JobTitle = Mapper.Map<JobTitle>(model);
                careerHistory.JobTitle.Active = false;
                careerHistory.JobTitle.IpAddress = IpAddress;
                careerHistory.PottentialStartDate = GetPottentialStartDateForFormData(careerHistory.StartFromYear, careerHistory.StartFromMonth, careerHistory.StartFromDay);
                careerHistory.PottentialEndDate = GetPottentialEndDateForFormData(careerHistory.EndFromYear, careerHistory.EndFromMonth, careerHistory.EndFromDay);
                careerHistory.PottentialCurrent = GetPottentialCurrent(careerHistory.PottentialEndDate);
                careerHistory.EntityID = UserHashObject.EntityID;
                if (model.Skills != null && model.Skills.Length > 0)
                {
                    careerHistory.CareerHistorySkills = new HashSet<CareerHistorySkill>();
                    foreach (var item in model.Skills)
                    {
                        careerHistory.CareerHistorySkills.Add(new CareerHistorySkill() { EntitySkillID = Convert.ToInt64(item) });
                    }
                }
                if (model.CareerStage == (int)Enums.CareerStage.Employement)
                {
                    careerHistory.Type = (int)Enums.CareerHistoryMode.Profession;
                }
                else
                {
                    careerHistory.Organization = new Organization()
                    {
                        Name = model.UniversityName,
                        Type = (int)Enums.OrganizationType.University,
                        Entity = new Entity()
                        {
                            EntityType = (short)Enums.EntityTypes.Organization
                        },
                        CategoryID = (int)Enums.StaticCategories.EducationAndCertificationProvider
                    };

                    careerHistory.Type = (int)Enums.CareerHistoryMode.Education;
                    var profile = Mapper.Map<CareerHistoryWizardViewModel, OrganizationProfile>(model);
                    profile.IpAddress = HttpContext.Current.Request.UserHostAddress;
                    profile.ProfileName = careerHistory.Organization.Name.Replace(" ", "-").ToLower();
                    careerHistory.Organization.OrganizationProfiles = new HashSet<OrganizationProfile>() { profile };

                }

                var result = await repository.Save(careerHistory);
                if (result > 0)
                {
                    long category = 0;
                    var keyValue = new Dictionary<string, string>();
                    var jobSearchKeyValue = new Dictionary<string, string>();
                    string profileSummary = string.Empty;
                    int profileLevel = 0;
                    if (model.CareerStage == ((int)Enums.CareerStage.Employement))
                    {
                        if (careerHistory.PottentialEndDate != null)
                        {
                            profileSummary = string.Format("Worked as {0} at {1}", model.JobTitleName, model.OrganizationName);
                            if (model.EmployementEndedStage == (int)Enums.EmployementEndedStage.Retired)
                            {
                                profileSummary = string.Format("{0}(Retired)", profileSummary);
                                profileLevel = (int)Enums.CareerStage.Retired;
                            }
                            if (model.EmployementEndedStage == (int)Enums.EmployementEndedStage.LookingOpportunity)
                            {
                                profileSummary = string.Format("{0}(open for an opportunity)", profileSummary);
                                profileLevel = (int)Enums.CareerStage.PostEmployement;
                                jobSearchKeyValue.Add("JobInterestLevel", ((int)Enums.OppurtunityLevel.Active).ToString());
                            }
                        }
                        else
                        {
                            profileSummary = string.Format("{0} at {1}", model.JobTitleName, model.OrganizationName);
                            profileLevel = (int)Enums.CareerStage.Employement;
                        }
                        keyValue.Add("FormatedJobTitle", profileSummary);
                        keyValue.Add("ProfileLevel", profileLevel.ToString());

                        if (model.OrganizationID > 0)
                        {
                            using (var businessRepository = new BusinessRepository())
                            {
                                category = await businessRepository.IndustryByOrganization(model.OrganizationID);
                                keyValue.Add("ProfileIndustry", category.ToString());
                            }
                        }
                        else
                        {
                            using (var commonRepository = new CommonRepository())
                            {
                                var categories = await commonRepository.Industries(model.IndustryName);
                                if (categories != null && categories.Tables[0].Rows.Count > 0)
                                {
                                    category = Convert.ToInt64(categories.Tables[0].Rows[0]["IndustryID"]);
                                    keyValue.Add("ProfileIndustry", categories.Tables[0].Rows[0]["IndustryID"].ToString());
                                }
                                else
                                {
                                    keyValue.Add("ProfileIndustry", "1");
                                }
                            }
                        }
                        if (jobSearchKeyValue.Count > 0)
                        {
                            using (var jobSearchRepository = new SettingsRepository())
                            {
                                await jobSearchRepository.SaveKeyValue(jobSearchKeyValue, UserHashObject.EntityID);
                            }
                        }
                    }
                    if (model.CareerStage == ((int)Enums.CareerStage.Student))
                    {
                        if (careerHistory.PottentialEndDate != null)
                        {
                            profileSummary = string.Format("Studied {0} at {1}", model.CourseName, model.UniversityName);
                        }
                        else
                        {
                            profileSummary = string.Format("Studying {0} at {1}", model.CourseName, model.UniversityName);
                        }
                        profileLevel = (int)Enums.CareerStage.Student;
                        keyValue.Add("FormatedStudyTitle", profileSummary);
                        keyValue.Add("ProfileLevel", ((int)Enums.CareerStage.Student).ToString());
                    }

                    if (!ispopup)
                    {
                        keyValue.Add("WizardStep", ((int)Enums.WizardStep.AddCareerHistory).ToString());
                    }

                    // Gets widgtes
                    var widgets = await GetWidgetsForProfile(model, category);
                    if (widgets.Count > 0)
                    {
                        using (var accountRepository = new AccountRepository())
                        {
                            await GeneralExtentions.Do<Task<int>>(() => accountRepository.AddWidgets(UserHashObject.EntityID, widgets), TimeSpan.FromSeconds(1), 5);
                        }
                    }

                    using (var accountRepository = new AccountRepository())
                    {
                        await accountRepository.UpdateProfileItems(keyValue, UserHashObject.PersonID);
                        await accountRepository.UpdateProfileScore(UserHashObject.EntityID);
                    }
                    await CommonFunctions.UpdateUserFaceIndex(UserHashObject.EntityID, (int)Enums.EntityTypes.Person);
                }
                return result;
            }
        }

        private async Task<List<EntityWidget>> GetWidgetsForProfile(CareerHistoryWizardViewModel model, long categoryID)
        {
            var list = new List<EntityWidget>();

            // For student we cannot determine the industry and hence we need to add a generic assignment.
            if (model.CareerStage == ((int)Enums.CareerStage.Student))
            {
                list.Add(new EntityWidget() { EntityID = UserHashObject.EntityID, WidgetID = (int)Enums.BasicWidgets.Projects });
            }

            using (var commonRepository = new CommonRepository())
            {
                var industryList = await commonRepository.Industries();
                var industry = industryList.Tables[0].Select("IndustryID = " + categoryID.ToString());
                if (industry.Length > 0)
                {
                    var name = industry[0]["IndustryName"].ToString();
                    switch (name.ToLower())
                    {
                        case "information technology":
                            list.Add(new EntityWidget() { EntityID = UserHashObject.EntityID, WidgetID = (int)Enums.BasicWidgets.Projects });
                            break;
                        case "book":
                        case "custom":
                        case "directory & guide":
                        case "periodicals":
                        case "distribution":
                        case "music":
                        case "printing":
                        case "literary agents":
                        case "publishing":
                            list.Add(new EntityWidget() { EntityID = UserHashObject.EntityID, WidgetID = (int)Enums.BasicWidgets.Publications });
                            if (name == "music")
                                list.Add(new EntityWidget() { EntityID = UserHashObject.EntityID, WidgetID = (int)Enums.BasicWidgets.Compositions });
                            break;
                        case "research & development":
                            list.Add(new EntityWidget() { EntityID = UserHashObject.EntityID, WidgetID = (int)Enums.BasicWidgets.Research });
                            list.Add(new EntityWidget() { EntityID = UserHashObject.EntityID, WidgetID = (int)Enums.BasicWidgets.Findings });
                            break;
                        default:
                            break;
                    }

                }
            }
            return list;
        }


        public async Task<Result> Remove(string mode, string q)
        {
            var model = q.ToObject<CareerHistoryWizardViewModel>(null);
            using (var repository = new CareerHistoryRepository())
            {
                await repository.Remove(model.CareerHistoryID, UserHashObject.EntityID, Now);
                if (mode.Equals("career"))
                {
                    using (var accountRepository = new AccountRepository())
                    {
                        await accountRepository.UpdateProfileScore(UserHashObject.EntityID);
                    }
                }
                var type = mode.Equals("career", StringComparison.InvariantCultureIgnoreCase) ? (int)Enums.CareerHistoryMode.Profession : (int)Enums.CareerHistoryMode.Education;
                await UpdateCurrentCareerHistory(type);
                await CommonFunctions.UpdateUserFaceIndex(UserHashObject.EntityID, UserHashObject.EntityType);
                return new Result() { Description = mode, Type = Enums.ResultType.Success, ReferenceID = model.CareerHistoryID };
            }
        }

        private async Task UpdateCurrentCareerHistory(int type)
        {
            var keyValue = new Dictionary<string, string>();
            using (var repository = new CareerHistoryRepository())
            {
                if (type == (int)Enums.CareerHistoryMode.Profession)
                {
                    using (var dataSet = await repository.TakeLastTwo(UserHashObject.EntityID, type, null))
                    {
                        var histories = dataSet.Tables[0].FromDataTable<CareerHistoryViewModel>(null);
                        if (histories != null && histories.Count > 0)
                        {
                            if (histories[0].HasValidEndDate)
                            {
                                keyValue.Add("FormatedJobTitle", string.Format("Worked as {0} at {1}", histories[0].JobTitleName, histories[0].OrganizationName));
                            }
                            else
                            {
                                keyValue.Add("FormatedJobTitle", string.Format("{0} at {1}", histories[0].JobTitleName, histories[0].OrganizationName));
                            }
                            if (histories.Count > 1)
                            {
                                if (histories[1].HasValidEndDate)
                                {
                                    keyValue.Add("PreviousJobTitle", string.Format("Previously {0} at {1}", histories[1].JobTitleName, histories[1].OrganizationName));
                                }
                                else
                                {
                                    keyValue.Add("PreviousJobTitle", string.Format("{0} at {1}", histories[1].JobTitleName, histories[1].OrganizationName));
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (type == (int)Enums.CareerHistoryMode.Education)
                    {
                        using (var dataSet = await repository.TakeLastTwo(UserHashObject.EntityID, type, null))
                        {
                            var histories = dataSet.Tables[0].FromDataTable<CareerHistoryViewModel>(null);
                            keyValue.Add("FormatedStudyTitle", string.Format("{0} at {1}", histories[0].JobTitleName, histories[0].OrganizationName));
                        }
                    }
                }
            }
            using (var repository = new AccountRepository())
            {
                await repository.UpdateProfileItems(keyValue, UserHashObject.PersonID);
            }
        }


        public async Task<OpportunityDataViewModel> GetDataForOpportunity(long? entityID)
        {
            entityID = entityID ?? UserHashObject.EntityID;
            using (var repository = new CareerHistoryRepository())
            {
                using (var data = await repository.GetDataForOpportunity(entityID))
                {
                    var model = new OpportunityDataViewModel()
                    {
                        Careers = data.Tables[0].FromDataTable<CareerHistoryViewModel>(null),
                        Skills = data.Tables[1].FromDataTable<SkillViewModel>(null)
                    };
                    return model;
                }
            }
        }
    }
}