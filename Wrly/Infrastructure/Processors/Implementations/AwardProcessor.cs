using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Types;
using Wrly.Data.Models;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.infrastuctures.Utils;
using Wrly.Models;
using Wrly.Models.Listing;
using Wrly.Notifications.Processors.Implementors;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class AwardProcessor : BaseProcessor, IAwardProcessor
    {
        public async Task<List<AwardViewModel>> GetAwards(string userName)
        {
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(UserHashObject.EntityID, false, (short)Enums.AwardAndAssignmentMode.Award);
                return CreateAwardList(dataSet);
            }
        }

        private List<AwardViewModel> CreateAwardList(System.Data.DataSet dataSet, bool isPublic = false)
        {
            var result = dataSet.Tables[0].FromDataTable<AwardViewModel>();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    if (Request.IsAuthenticated)
                    {
                        var table = new Hashtable();
                        table.Add("AwardID", item.AwardID);
                        table.Add("EntityID", item.EntityID);
                        table.Add("Name", item.Name);
                        table.Add("TimeStamp", DateTime.UtcNow);
                        item.Hash = QueryStringHelper.Encrypt(table);
                        item.AllowEdit = true;
                    }
                    if (item.JobTitleID > 0)
                    {
                        item.DisplayJobTitleText =
                             item.JobTitleType == (short)Enums.CareerHistoryMode.Education ?
                    string.Format("Student at {0} while {1}", item.OrganizationName, item.JobTitle) :
                    string.Format("Working at {0} as {1}", item.OrganizationName, item.JobTitle);
                    }
                    item.SkillIncluded = dataSet.Tables[1].FromDataTable<AwardSkillViewModel>("AwardId=" + item.AwardID);
                    foreach (var skill in item.SkillIncluded)
                    {
                        var table = new Hashtable();
                        table.Add("AwardID", item.AwardID);
                        table.Add("EntityID", item.EntityID);
                        table.Add("AwardSkillID", skill.AwardSkillID);
                        table.Add("TimeStamp", DateTime.UtcNow);
                        skill.Hash = QueryStringHelper.Encrypt(table);
                        skill.AllowEdit = true;
                    }
                    item.ParticipantIncluded = dataSet.Tables[2].FromDataTable<AwardParticipantViewModel>("EntityAwardId=" + item.AwardID);
                    if (Request.IsAuthenticated && item.EntityID == UserHashObject.EntityID)
                    {
                        var currentEntity = item.ParticipantIncluded.FirstOrDefault(c => c.EntityID.Equals(UserHashObject.EntityID));
                        item.ParticipantIncluded.Remove(currentEntity);
                    }
                    else
                    {
                        var currentEntity = item.ParticipantIncluded.FirstOrDefault(c => c.EntityID.Equals(item.EntityID));
                        item.Role = currentEntity.Role;
                    }

                    foreach (var participant in item.ParticipantIncluded)
                    {
                        var table = new Hashtable();
                        table.Add("AwardID", item.AwardID);
                        table.Add("EntityID", item.EntityID);
                        table.Add("ID", participant.ID);
                        table.Add("Status", participant.Status);
                        table.Add("TimeStamp", DateTime.UtcNow);
                        participant.Hash = QueryStringHelper.Encrypt(table);
                        participant.ShowApproval = participant.Status == (int)Enums.AccomplishmentParticipantStatus.PendingForApproval && (participant.RefrenceEntity != item.EntityID);
                        participant.AllowEdit = true;
                    }
                }
            }
            return result;
        }


        public async Task<AwardViewModel> GetOneAward(string hash)
        {
            var awardAndAssignment = hash.ToObject<AwardViewModel>(null);
            return await GetForAwardAndEntity(awardAndAssignment.AwardID, UserHashObject.EntityID);
        }

        private async Task<AwardViewModel> GetForAwardAndEntity(long awardID, long entityID)
        {
            using (var repository = new AwardRepository())
            {
                using (var dataSet = await repository.Single(awardID, entityID))
                {
                    var model = dataSet.Tables[0].FromDataTable<AwardViewModel>()[0];
                    var table = new Hashtable();
                    table.Add("AwardID", model.AwardID);
                    table.Add("EntityID", model.EntityID);
                    table.Add("TimeStamp", DateTime.UtcNow);
                    model.Hash = QueryStringHelper.Encrypt(table);
                    model.AllowEdit = true;
                    model.SkillIncluded = dataSet.Tables[1].FromDataTable<AwardSkillViewModel>();
                    model.ParticipantIncluded = dataSet.Tables[2].FromDataTable<AwardParticipantViewModel>();
                    return model;
                }
            }
        }

        public async Task<int> Save(AwardViewModel model)
        {
            var award = new EntityAwardAndCompletion();
            if (!string.IsNullOrEmpty(model.Hash))
            {
                award = Mapper.Map<AwardViewModel, EntityAwardAndCompletion>(model, award);
                award = model.Hash.ToObject<EntityAwardAndCompletion>(award);
                if (award.EntityID != UserHashObject.EntityID)
                    return -1;
            }
            else
            {
                award = Mapper.Map<AwardViewModel, EntityAwardAndCompletion>(model, award);
                award = model.UserHash.ToObject<EntityAwardAndCompletion>(award);
            }
            if (model.Skills != null && model.Skills.Length > 0)
            {
                award.AwardSkills = new HashSet<AwardSkill>();
                foreach (var item in model.Skills)
                {
                    award.AwardSkills.Add(new AwardSkill()
                    {
                        AwardID = award.AwardID,
                        EntitySkillID = item,
                        Status = (int)Enums.SkillStateStatus.Active
                    });
                }
            }

            award.EntityAwardParticipants = new HashSet<EntityAwardParticipant>();
            award.EntityAwardParticipants.Add(new EntityAwardParticipant()
            {
                EntityAwardID = model.AwardID,
                EntityID = UserHashObject.EntityID,
                RefrenceEntity = UserHashObject.EntityID,
                Role = model.Role,
                Status = (int)Enums.AccomplishmentParticipantStatus.Active
            });

            if (model.Participants != null && model.Participants.Length > 0)
            {
                foreach (var item in model.Participants.Where(c => c != UserHashObject.EntityID))
                {
                    award.EntityAwardParticipants.Add(new EntityAwardParticipant()
                    {
                        EntityAwardID = model.AwardID,
                        EntityID = item,
                        RefrenceEntity = UserHashObject.EntityID,
                        Status = (int)Enums.AccomplishmentParticipantStatus.Active
                    });
                }
            }


            award.PottentialStartDate = GetPottentialStartDateForFormData(award.StartFromYear, award.StartFromMonth, award.StartFromDay);
            award.PottentialEndDate = GetPottentialEndDateForFormData(award.EndFromYear, award.EndFromMonth, award.EndFromDay);
            award.PottentialCurrent = GetPottentialCurrent(award.PottentialEndDate);
            award.IpAddress = IpAddress;
            using (var repository = new AwardRepository())
            {
                var result = await repository.Save(award);
                if (model.Skills != null && model.Skills.Count() > 0)
                {
                    using (var skillRepository = new EntitySkillRepository())
                    {
                        foreach (var item in model.Skills)
                        {
                            await skillRepository.CalculateAndSetSkillScore(item, UserHashObject.EntityID);
                        }
                    }
                }
                if (result > 0 && award.AwardID == 0 && (award.Type == (int)Enums.AwardAndAssignmentMode.Award))
                {
                    var activity = new NetworkActivity()
                    {
                        Type = (int)Enums.NetworkActivityType.AddedAward,
                        IpAddress = IpAddress,
                        Identifier = string.Format("{0}_{1}_{2}", UserHashObject.EntityID, (int)Enums.NetworkActivityType.AddedAward, result),
                        EntityID = UserHashObject.EntityID,
                        EditedOn = Now,
                        EditedBy = User,
                        CreatedOn = Now,
                        AwardID = result,
                        ActionTaken = false
                    };
                    using (var networkActivity = new CommonRepository())
                    {
                        await networkActivity.AddActivity(activity);
                    }
                }
                if ((award.Type != (int)Enums.AwardAndAssignmentMode.Award))
                {
                    using (var accountRepository = new AccountRepository())
                    {
                        await accountRepository.UpdateProfileScore(UserHashObject.EntityID);
                    }
                }
            }
            return 1;
        }

        public async Task<List<AwardViewModel>> GetAssignments(string userName)
        {
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(UserHashObject.EntityID, false, (short)Enums.AwardAndAssignmentMode.Assignment);
                return CreateAwardList(dataSet);
            }
        }


        public int RemoveParticipant(string hash)
        {
            var participant = hash.ToObject<AppreciationAndRecommendationParticipant>(null);
            if (participant.EntityID == UserHashObject.EntityID)
            {
                using (var repository = new AwardParticipantRepository())
                {
                    return repository.Delete(participant.ID);
                }
            }
            return -1;
        }

        public int RemoveSkill(string hash)
        {
            var participant = hash.ToObject<AwardSkill>(null);
            var generalEntity = hash.ToObject<GeneralEntity>(null);
            if (generalEntity.EntityID == UserHashObject.EntityID)
            {
                using (var repository = new AwardSkillRepository())
                {
                    return repository.Delete(participant.AwardSkillID);
                }
            }
            return -1;
        }


        public async Task<PublilcationViewModel> GetOnePublication(string hash)
        {
            var awardAndAssignment = hash.ToObject<PublilcationViewModel>(null);
            if (awardAndAssignment.EntityID == UserHashObject.EntityID)
            {
                using (var repository = new AwardRepository())
                {
                    using (var dataSet = await repository.Single(awardAndAssignment.AwardID, UserHashObject.EntityID))
                    {
                        var model = dataSet.Tables[0].FromDataTable<PublilcationViewModel>()[0];
                        var table = new Hashtable();
                        table.Add("AwardID", model.AwardID);
                        table.Add("EntityID", model.EntityID);
                        table.Add("TimeStamp", DateTime.UtcNow);
                        model.Hash = QueryStringHelper.Encrypt(table);
                        model.AllowEdit = true;
                        model.SkillIncluded = dataSet.Tables[1].FromDataTable<AwardSkillViewModel>();
                        model.ParticipantIncluded = dataSet.Tables[2].FromDataTable<AwardParticipantViewModel>();
                        return model;
                    }
                }
            }
            return null;
        }


        public async Task<int> SavePublication(PublilcationViewModel model)
        {
            var award = new EntityAwardAndCompletion();
            if (!string.IsNullOrEmpty(model.Hash))
            {
                award = Mapper.Map<AwardViewModel, EntityAwardAndCompletion>(model, award);
                award = model.Hash.ToObject<EntityAwardAndCompletion>(award);
                if (award.EntityID != UserHashObject.EntityID)
                    return -1;
            }
            else
            {
                award = Mapper.Map<AwardViewModel, EntityAwardAndCompletion>(model, award);
                award = model.UserHash.ToObject<EntityAwardAndCompletion>(award);
            }
            if (model.Skills != null && model.Skills.Length > 0)
            {
                award.AwardSkills = new HashSet<AwardSkill>();
                foreach (var item in model.Skills)
                {
                    award.AwardSkills.Add(new AwardSkill()
                    {
                        AwardID = award.AwardID,
                        EntitySkillID = item,
                        Status = (int)Enums.SkillStateStatus.Active
                    });
                }
            }


            if (model.Participants != null && model.Participants.Length > 0)
            {
                award.EntityAwardParticipants = new HashSet<EntityAwardParticipant>();
                foreach (var item in model.Participants)
                {
                    award.EntityAwardParticipants.Add(new EntityAwardParticipant()
                    {
                        EntityAwardID = model.AwardID,
                        EntityID = item,
                        RefrenceEntity = UserHashObject.EntityID,
                        Role = model.Role,
                        Status = (int)Enums.AccomplishmentParticipantStatus.Active
                    });
                }
            }


            award.PottentialStartDate = GetPottentialStartDateForFormData(award.StartFromYear, award.StartFromMonth, award.StartFromDay);
            award.PottentialEndDate = GetPottentialEndDateForFormData(award.EndFromYear, award.EndFromMonth, award.EndFromDay);
            award.PottentialCurrent = GetPottentialCurrent(award.PottentialEndDate);
            award.IpAddress = IpAddress;
            using (var repository = new AwardRepository())
            {
                await repository.Save(award);
                using (var accountRepository = new AccountRepository())
                {
                    await accountRepository.UpdateProfileScore(UserHashObject.EntityID);
                }
            }
            return 1;
        }




        private List<PublilcationViewModel> CreatePublicationList(System.Data.DataSet dataSet)
        {
            var result = dataSet.Tables[0].FromDataTable<PublilcationViewModel>();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    if (Request.IsAuthenticated)
                    {
                        var table = new Hashtable();
                        table.Add("AwardID", item.AwardID);
                        table.Add("EntityID", item.EntityID);
                        table.Add("TimeStamp", DateTime.UtcNow);
                        item.Hash = QueryStringHelper.Encrypt(table);
                        item.AllowEdit = true;
                    }
                    if (item.JobTitleID > 0)
                    {
                        item.DisplayJobTitleText =
                             item.JobTitleType == (short)Enums.CareerHistoryMode.Education ?
                    string.Format("Student at {0} while {1}", item.OrganizationName, item.JobTitle) :
                    string.Format("Working at {0} as {1}", item.OrganizationName, item.JobTitle);
                    }
                    item.SkillIncluded = dataSet.Tables[1].FromDataTable<AwardSkillViewModel>("AwardId=" + item.AwardID);
                    foreach (var skill in item.SkillIncluded)
                    {
                        var table = new Hashtable();
                        table.Add("AwardID", item.AwardID);
                        table.Add("EntityID", item.EntityID);
                        table.Add("AwardSkillID", skill.AwardSkillID);
                        table.Add("TimeStamp", DateTime.UtcNow);
                        skill.Hash = QueryStringHelper.Encrypt(table);
                        skill.AllowEdit = true;
                    }
                    item.ParticipantIncluded = dataSet.Tables[2].FromDataTable<AwardParticipantViewModel>("EntityAwardId=" + item.AwardID);
                    foreach (var participant in item.ParticipantIncluded)
                    {
                        var table = new Hashtable();
                        table.Add("AwardID", item.AwardID);
                        table.Add("EntityID", item.EntityID);
                        table.Add("ID", participant.ID);
                        table.Add("TimeStamp", DateTime.UtcNow);
                        participant.Hash = QueryStringHelper.Encrypt(table);
                        participant.AllowEdit = true;
                    }
                }
            }
            return result;
        }








        public async Task<List<PublilcationViewModel>> GetFindings(string userName)
        {
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(UserHashObject.EntityID, false, (short)Enums.AwardAndAssignmentMode.Finding);
                return CreatePublicationList(dataSet);
            }
        }

        public async Task<List<PublilcationViewModel>> GetFindingsForProfile(string profileHash)
        {
            var baseModel = profileHash.ToObject<ProfileHashViewModel>(null);
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(baseModel.EntityID, false, (short)Enums.AwardAndAssignmentMode.Finding);
                return CreatePublicationList(dataSet);
            }
        }


        public async Task<AwardViewModel> GetProjectToRequestParticipant(string hash)
        {
            var awardAndAssignment = await GetOneAward(hash);
            var newAssignement = new AwardViewModel() { Name = awardAndAssignment.Name };
            newAssignement.ParticipantIncluded = new List<AwardParticipantViewModel>();
            using (var repository = new AwardRepository())
            {
                newAssignement.Participants = new long[] { awardAndAssignment.EntityID, UserHashObject.EntityID };
                using (var participantDataSet = await repository.GetParticipantHeads(awardAndAssignment.AwardID, string.Join(",", awardAndAssignment.Participants)))
                {
                    var result = participantDataSet.Tables[2].FromDataTable<AwardParticipantViewModel>();
                    foreach (var participant in result)
                    {
                        var table = new Hashtable();
                        table.Add("ParentID", awardAndAssignment.AwardID);
                        table.Add("EntityID", UserHashObject.EntityID);
                        table.Add("TimeStamp", DateTime.UtcNow);
                        participant.Hash = QueryStringHelper.Encrypt(table);
                        participant.AllowEdit = true;
                        newAssignement.ParticipantIncluded.Add(participant);
                    }
                }
            }
            return newAssignement;
        }


        public async Task<long> SaveAddParticipantRequest(AwardViewModel model)
        {
            var currentModel = model.Hash.ToObject<AwardViewModel>(null);
            currentModel = await GetForAwardAndEntity(currentModel.ParentID, currentModel.EntityID);

            var award = Mapper.Map<AwardViewModel, EntityAwardAndCompletion>(model);
            award = model.UserHash.ToObject<EntityAwardAndCompletion>(award);
            award.ParentID = currentModel.AwardID;
            award.Type = currentModel.Type;
            award.SubType = currentModel.SubType;
            award.Status = (byte)Enums.AccomplishmentStatus.Active;
            if (model.Participants != null && model.Participants.Length > 0)
            {
                award.EntityAwardParticipants = new HashSet<EntityAwardParticipant>();
                foreach (var item in model.Participants)
                {
                    award.EntityAwardParticipants.Add(new EntityAwardParticipant()
                    {
                        EntityAwardID = model.AwardID,
                        EntityID = item,
                        RefrenceEntity = UserHashObject.EntityID,
                        Status = (int)Enums.AccomplishmentParticipantStatus.PendingForApproval
                    });
                }
            }

            var groupID = Guid.NewGuid().ToString();
            award.EntityAwardParticipants = new HashSet<EntityAwardParticipant>();
            award.EntityAwardParticipants.Add(new EntityAwardParticipant()
            {
                EntityAwardID = 0,
                EntityID = currentModel.EntityID,
                RefrenceEntity = UserHashObject.EntityID,
                Role = currentModel.ParticipantIncluded.FirstOrDefault(c => c.EntityID == currentModel.EntityID).Role,
                Status = (int)Enums.AccomplishmentParticipantStatus.PendingForApproval,
                GroupID = groupID
            });

            award.EntityAwardParticipants.Add(new EntityAwardParticipant()
            {
                EntityAwardID = 0,
                EntityID = UserHashObject.EntityID,
                RefrenceEntity = UserHashObject.EntityID,
                Role = model.Role,
                Status = (int)Enums.AccomplishmentParticipantStatus.Active,
            });

            //award.ParentAwardParticipants = new HashSet<EntityAwardParticipant>();
            //award.ParentAwardParticipants.Add(new EntityAwardParticipant()
            //{
            //    EntityAwardID = currentModel.AwardID,
            //    EntityID = UserHashObject.EntityID,
            //    RefrenceEntity = UserHashObject.EntityID,
            //    Role = model.Role,
            //    Status = (int)Enums.AccomplishmentParticipantStatus.PendingForApproval,
            //    GroupID = groupID
            //});

            award.PottentialStartDate = GetPottentialStartDateForFormData(award.StartFromYear, award.StartFromMonth, award.StartFromDay);
            award.PottentialEndDate = GetPottentialEndDateForFormData(award.EndFromYear, award.EndFromMonth, award.EndFromDay);
            award.PottentialCurrent = GetPottentialCurrent(award.PottentialEndDate);
            award.IpAddress = IpAddress;
            using (var repository = new AwardRepository())
            {
                return await repository.Save(award);
            }
        }


        public async Task<List<AssignmentOption>> LoadAssginementOptions(string hash)
        {
            var list = new List<AssignmentOption>();
            using (var repository = new AwardRepository())
            {
                var awardModel = hash.ToObject<AwardViewModel>(null);
                if (awardModel.EntityID != UserHashObject.EntityID)
                {
                    var awardDetail = await GetOneAward(awardModel.AwardID);
                    using (var dataSet = await repository.LoadOptionsForAssignment(awardModel.AwardID, UserHashObject.EntityID))
                    {
                        var row = dataSet.Tables[0].FromDataTable<AwardStatusForEntity>().FirstOrDefault();
                        var option = new AssignmentOption();
                        var hashTable = new Hashtable();

                        hashTable.Add("Name", awardDetail.Name);
                        hashTable.Add("Type", awardDetail.Type);
                        hashTable.Add("SubType", awardDetail.SubType);
                        hashTable.Add("EntityID", awardDetail.EntityID);
                        hashTable.Add("DbID", Guid.NewGuid());
                        if (row.AlreadyAppriciated == 0)
                        {
                            if (hashTable.Contains("Action"))
                            {
                                hashTable.Remove("Action");
                            }
                            hashTable.Add("AwardID", awardDetail.AwardID);
                            hashTable.Add("Action", "Appriciate");
                            option = new AssignmentOption()
                            {
                                DisplayText = "Appreciate",
                                Hash = QueryStringHelper.Encrypt(hashTable),
                                Action = "appriciate",
                                Type = ((Enums.AwardAndAssignmentMode)awardDetail.Type).GetDescription().ToLower()
                            };
                            list.Add(option);
                        }
                        option = new AssignmentOption()
                        {
                            DisplayText = "Divider",
                            Hash = string.Empty
                        };
                        list.Add(option);
                        if (hashTable.Contains("Action"))
                        {
                            hashTable.Remove("Action");
                        }
                        if (!hashTable.Contains("AwardID"))
                            hashTable.Add("AwardID", awardDetail.AwardID);
                        hashTable.Add("Action", "Report");
                        option = new AssignmentOption()
                        {
                            DisplayText = "Report",
                            Hash = QueryStringHelper.Encrypt(hashTable),
                            Action = "report",
                            Type = ((Enums.AwardAndAssignmentMode)awardDetail.Type).GetDescription().ToLower()
                        };
                        list.Add(option);
                    }
                }
            }
            return list;
        }

        private async Task<AwardViewModel> GetOneAward(long awardID)
        {
            using (var repository = new AwardRepository())
            {
                using (var dataSet = await repository.Basic(awardID, UserHashObject.EntityID))
                {
                    var model = dataSet.Tables[0].FromDataTable<AwardViewModel>()[0];
                    return model;
                }
            }
        }


        private List<PublicAwardViewModel> CreateAwardListForPublic(System.Data.DataSet dataSet, bool isPublic = false)
        {
            var result = dataSet.Tables[0].FromDataTable<PublicAwardViewModel>();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    var table = new Hashtable();
                    table.Add("AwardID", item.AwardID);
                    table.Add("EntityID", item.EntityID);
                    table.Add("Name", item.Name);
                    table.Add("TimeStamp", DateTime.UtcNow);
                    item.Hash = QueryStringHelper.Encrypt(table);
                    if (item.JobTitleID > 0)
                    {
                        item.DisplayJobTitleText =
                             item.JobTitleType == (short)Enums.CareerHistoryMode.Education ?
                    string.Format("Student at {0} while {1}", item.OrganizationName, item.JobTitle) :
                    string.Format("Working at {0} as {1}", item.OrganizationName, item.JobTitle);
                    }
                    item.SkillIncluded = dataSet.Tables[1].FromDataTable<AwardSkillViewModel>("AwardId=" + item.AwardID);
                    item.ParticipantIncluded = dataSet.Tables[2].FromDataTable<AwardParticipantViewModel>("EntityAwardId=" + item.AwardID);
                }
            }
            return result;
        }

        public async Task<List<PublicAwardViewModel>> GetAwardsForProfile(string profileHash)
        {
            var baseModel = profileHash.ToObject<ProfileHashViewModel>(null);
            using (var repository = new AwardRepository())
            {
                var currentEntityID = UserHashObject == null ? 0 : UserHashObject.EntityID;
                var dataSet = await repository.AccomplitionForProfile(baseModel.EntityID, currentEntityID, (short)Enums.AwardAndAssignmentMode.Award);
                return CreateAwardListForPublic(dataSet);
            }
        }


        public async Task<List<AwardViewModel>> GetAssignmentsForProfile(string profileHash)
        {
            var baseModel = profileHash.ToObject<ProfileHashViewModel>(null);
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(baseModel.EntityID, false, (short)Enums.AwardAndAssignmentMode.Assignment);
                return CreateAwardList(dataSet);
            }
        }


        public async Task<AwardViewModel> GetBasicDetailsToAdd(string hash)
        {
            var awardViewModel = hash.ToObject<AwardViewModel>(null);
            return awardViewModel;
        }


        public async Task<List<PublilcationViewModel>> GetResearches(string userName)
        {
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(UserHashObject.EntityID, false, (short)Enums.AwardAndAssignmentMode.Research);
                return CreatePublicationList(dataSet);
            }
        }


        public async Task<List<PublilcationViewModel>> GetResearchesForProfile(string profileHash)
        {
            var baseModel = profileHash.ToObject<ProfileHashViewModel>(null);
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(baseModel.EntityID, false, (short)Enums.AwardAndAssignmentMode.Research);
                return CreatePublicationList(dataSet);
            }
        }

        public async Task<List<PublilcationViewModel>> GetPublication(string userName)
        {
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(UserHashObject.EntityID, false, (short)Enums.AwardAndAssignmentMode.Publication);
                return CreatePublicationList(dataSet);
            }
        }

        public async Task<List<PublilcationViewModel>> GetPublicationForProfile(string profileHash)
        {
            var baseModel = profileHash.ToObject<ProfileHashViewModel>(null);
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(baseModel.EntityID, false, (short)Enums.AwardAndAssignmentMode.Publication);
                return CreatePublicationList(dataSet);
            }
        }

        public async Task<List<PublilcationViewModel>> GetCompositions(string userName)
        {
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(UserHashObject.EntityID, false, (short)Enums.AwardAndAssignmentMode.Composition);
                return CreatePublicationList(dataSet);
            }
        }

        public async Task<List<PublilcationViewModel>> GetCompositionsForProfile(string profileHash)
        {
            var baseModel = profileHash.ToObject<ProfileHashViewModel>(null);
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(baseModel.EntityID, false, (short)Enums.AwardAndAssignmentMode.Composition);
                return CreatePublicationList(dataSet);
            }
        }


        public async Task<AccomplishmentReportViewModel> GetReportableAccomplishment(string hash)
        {
            var awardModel = hash.ToObject<AwardViewModel>(null);
            var model = new AccomplishmentReportViewModel()
            {
                Accomplishment = await GetOneAward(awardModel.AwardID),
            };
            var table = new Hashtable();
            table.Add("AccomplishmentID", awardModel.AwardID);
            table.Add("Name", awardModel.Name);
            table.Add("EntityID", UserHashObject.EntityID);
            table.Add("Type", ((byte)Enums.AccomplishmentStateType.Report));
            table.Add("SubType", ((byte)Enums.AccomplishmentStateSubType.General));
            table.Add("TimeStamp", DateTime.UtcNow);

            model.Hash = QueryStringHelper.Encrypt(table);
            return model;
        }


        public async Task<Result> Report(AccomplishmentReportViewModel model)
        {
            var accomplishmentState = Mapper.Map<AccomplishmentReportViewModel, AccomplishmentState>(model);
            accomplishmentState = model.Hash.ToObject<AccomplishmentState>(accomplishmentState);
            accomplishmentState.IpAddress = IpAddress;
            accomplishmentState.Status = (byte)Enums.AccomplishmentStateStatus.PendingForReview;
            accomplishmentState.Type = ((byte)Enums.AccomplishmentStateType.Report);
            accomplishmentState.SubType = ((byte)Enums.AccomplishmentStateSubType.General);
            using (var repository = new AwardRepository())
            {
                var result = await repository.RecordState(accomplishmentState);
                if (result > 0)
                    return new Result() { Type = Enums.ResultType.Success, Description = string.Format("The review request for {0} has been sent.", model.Accomplishment.Name) };

                return new Result() { Type = Enums.ResultType.Error, Description = "We are unable to reach to your request, please try again, or you can also use the feture 'Abuse report' in order to complaint." };
            }
        }


        public async Task<Result> Congratulate(string q)
        {
            var awardViewModel = q.ToObject<AwardViewModel>(null);
            var accomplishmentState = Mapper.Map<AccomplishmentState>(awardViewModel);
            accomplishmentState.AccomplishmentID = awardViewModel.AwardID;
            accomplishmentState.EntityID = UserHashObject.EntityID;
            accomplishmentState.IpAddress = IpAddress;
            accomplishmentState.Status = (byte)Enums.AccomplishmentStateStatus.Active;
            accomplishmentState.Type = ((byte)Enums.AccomplishmentStateType.Congratulate);
            accomplishmentState.SubType = ((byte)Enums.AccomplishmentStateSubType.General);
            if (accomplishmentState.EntityID != awardViewModel.EntityID)
            {
                using (var repository = new AwardRepository())
                {
                    var result = await repository.RecordState(accomplishmentState);
                    if (result > 0)
                    {
                        string url = string.Format("/profile/#awards{0}", awardViewModel.AwardID);
                        if (EntityHash.ContainsKey("Url"))
                            EntityHash.Remove("Url");
                        EntityHash.Add("Url", url);
                        var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                        await notificationProcessor.AddNotification(Enums.NotificationType.AwardCongratulated, null, null, awardViewModel.EntityID, null, null, awardViewModel.AwardID);

                        return new Result() { Type = Enums.ResultType.Success, Description = string.Format("Congratulation has been sent for {0} successfully.", awardViewModel.Name) };
                    }

                    return new Result() { Type = Enums.ResultType.Error, Description = "We are unable to reach to your request, please try again." };
                }
            }
            else
                return new Result() { Type = Enums.ResultType.Error, Description = "Unable to record the action." };
        }


        public async Task<List<PublilcationViewModel>> GetServices()
        {
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(UserHashObject.EntityID, false, (short)Enums.AwardAndAssignmentMode.Services);
                return CreatePublicationList(dataSet);
            }
        }


        public async Task<List<PublilcationViewModel>> GetProducts()
        {
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(UserHashObject.EntityID, false, (short)Enums.AwardAndAssignmentMode.Products);
                return CreatePublicationList(dataSet);
            }
        }


        public async Task<List<PublilcationViewModel>> GetServicesForProfile(string q)
        {
            var baseModel = q.ToObject<ProfileHashViewModel>(null);
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(baseModel.EntityID, false, (short)Enums.AwardAndAssignmentMode.Services);
                return CreatePublicationList(dataSet);
            }
        }

        public async Task<List<PublilcationViewModel>> GetProductsForProfile(string q)
        {
            var baseModel = q.ToObject<ProfileHashViewModel>(null);
            using (var repository = new AwardRepository())
            {
                var dataSet = await repository.ForUser(baseModel.EntityID, false, (short)Enums.AwardAndAssignmentMode.Products);
                return CreatePublicationList(dataSet);
            }
        }


        public async Task<List<AwardViewModel>> GetServicesAndProductBasic(string hash)
        {
            var baseModel = hash.ToObject<ProfileHashViewModel>(null);
            using (var repository = new AwardRepository())
            {
                var intArray = new int[2] { (short)Enums.AwardAndAssignmentMode.Products, (short)Enums.AwardAndAssignmentMode.Services };
                var dataSet = await repository.BasicForUser(baseModel.EntityID, intArray);
                return dataSet.Tables[0].FromDataTable<AwardViewModel>();
            }
        }


        public async Task<Result> Remove(string mode, string hash)
        {
            var baseModel = hash.ToObject<AwardViewModel>(null);
            using (var repository = new AwardRepository())
            {
                await repository.Remove(baseModel.AwardID, UserHashObject.EntityID, Now);
                using (var accountRepository = new AccountRepository())
                {
                    await accountRepository.UpdateProfileScore(UserHashObject.EntityID);
                }
                return new Result() { ReferenceID = baseModel.AwardID, Description = "Removed", Type = Enums.ResultType.Success };
            }
        }
    }
}