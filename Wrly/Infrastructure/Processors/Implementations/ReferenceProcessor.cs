using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Types;
using Wrly.Data.Models;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models;
using System.Threading.Tasks;
using Wrly.Models.Listing;
using System.Collections;
using Wrly.infrastuctures.Utils;
using Wrly.Notifications.Processors.Implementors;
using Wrly;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class ReferenceProcessor : BaseProcessor, IReferenceProcessor
    {
        public Models.AppreciationAndRecommendationViewModel GetAppriciation(string hash)
        {
            return hash.ToObject<AppreciationAndRecommendationViewModel>(null);
        }

        public Models.AppreciationAndRecommendationViewModel GetRecommendation(string hash)
        {
            return hash.ToObject<AppreciationAndRecommendationViewModel>(null);
        }

        public async Task<Result> Save(AppreciationAndRecommendationViewModel model)
        {
            var notificationProcessor = new InstantNotificationProcessor(EntityHash);
            using (var repository = new ReferenceRepository())
            {
                var appreciationAndRecommendation = new AppreciationAndRecommendation();
                if (!string.IsNullOrEmpty(model.Hash))
                {
                    appreciationAndRecommendation = Mapper.Map<AppreciationAndRecommendationViewModel, AppreciationAndRecommendation>(model, appreciationAndRecommendation);
                    appreciationAndRecommendation = model.Hash.ToObject<AppreciationAndRecommendation>(appreciationAndRecommendation);
                    if (appreciationAndRecommendation.ReferenceID != model.ReferenceID)
                        return new Result() { Type = Enums.ResultType.Error, Description = "Reference cannot be saved as there is some validation of security rules failed." };
                }
                else
                {
                    appreciationAndRecommendation = model.UserHash.ToObject<AppreciationAndRecommendation>(appreciationAndRecommendation);
                    appreciationAndRecommendation = Mapper.Map<AppreciationAndRecommendationViewModel, AppreciationAndRecommendation>(model, appreciationAndRecommendation);
                }
                appreciationAndRecommendation.IpAddress = IpAddress;
                appreciationAndRecommendation.AppreciationAndRecommendationParticipants = new HashSet<AppreciationAndRecommendationParticipant>();
                appreciationAndRecommendation.SourceEntityID = UserHashObject.EntityID;
                appreciationAndRecommendation.AppreciationAndRecommendationParticipants.Add(new AppreciationAndRecommendationParticipant()
                {
                    EntityID = appreciationAndRecommendation.EntityID
                });
                var entitySkillID = default(long?);
                if (model.SkillID > 0 && model.For == (int)Enums.RecommedationFor.Skill)
                {
                    appreciationAndRecommendation.AppreciationAndRecommendationSkills.Add(new AppreciationAndRecommendationSkill()
                    {
                        EntitySkillID = Convert.ToInt64(model.SkillID)
                    });
                    entitySkillID = model.SkillID;
                }
                else
                {
                    appreciationAndRecommendation.AppreciationAndRecommendationSkills = null;
                }
                if (model.For != (int)Enums.RecommedationFor.Role)
                {
                    appreciationAndRecommendation.CareerHistoryID = null;
                }

                string url = string.Format("/reference/index");
                if (EntityHash.ContainsKey("Url"))
                    EntityHash.Remove("Url");
                EntityHash.Add("Url", url);

                var result = await repository.Save(appreciationAndRecommendation);
                if (appreciationAndRecommendation.Type == (int)Enums.ReferenceMode.Appreciation)
                    await notificationProcessor.AddNotification(Enums.NotificationType.UserAppriciatesMe, null, result, appreciationAndRecommendation.EntityID);
                else
                    await notificationProcessor.AddNotification(Enums.NotificationType.UserRecommendsMe, null, result, appreciationAndRecommendation.EntityID, null, entitySkillID);

                if (result >= 0)
                    return new Result() { Type = Enums.ResultType.Success, Description = "Reference has been saved." };
                else
                    return new Result() { Type = Enums.ResultType.Error, Description = "Reference cannot be saved as there is some server error." };
            }
        }

        public async System.Threading.Tasks.Task<List<Models.AppreciationAndRecommendationViewModel>> GetRecommendations(string userName)
        {
            using (var repository = new ReferenceRepository())
            {
                var dataSet = await repository.GetReference(UserHashObject.EntityID, true, (byte)Enums.ReferenceMode.Recommendation);
                var result = dataSet.Tables[0].FromDataTable<AppreciationAndRecommendationViewModel>();
                foreach (var item in result)
                {
                    var table = new Hashtable();
                    table.Add("ReferenceID", item.ReferenceID);
                    table.Add("CareerHistoryID", item.CareerHistoryID);
                    table.Add("OrganizationName", item.OrganizationName);
                    table.Add("JobTitleName", item.JobTitleName);
                    table.Add("SkillID", item.SkillID);
                    table.Add("Name", item.SkillName);
                    table.Add("ID", DateTime.UtcNow.Ticks);
                    item.Hash = QueryStringHelper.Encrypt(table);
                }
                return result;
            }
        }

        public async System.Threading.Tasks.Task<List<Models.AppreciationAndRecommendationViewModel>> GetAppriciations(string userName)
        {
            using (var repository = new ReferenceRepository())
            {
                var dataSet = await repository.GetReference(UserHashObject.EntityID, true, (byte)Enums.ReferenceMode.Appreciation);
                var result = dataSet.Tables[0].FromDataTable<AppreciationAndRecommendationViewModel>();
                foreach (var item in result)
                {
                    var table = new Hashtable();
                    table.Add("ReferenceID", item.ReferenceID);
                    table.Add("CareerHistoryID", item.CareerHistoryID);
                    table.Add("OrganizationName", item.OrganizationName);
                    table.Add("JobTitleName", item.JobTitleName);
                    table.Add("SkillID", item.SkillID);
                    table.Add("Name", item.SkillName);
                    table.Add("ID", DateTime.UtcNow.Ticks);
                    item.Hash = QueryStringHelper.Encrypt(table);
                }
                return result;
            }
        }


        public async System.Threading.Tasks.Task<ReferenceViewModel> GetRecommendationsForProfile(string profileHash)
        {
            var baseModel = profileHash.ToObject<ProfileHashViewModel>(null);
            using (var repository = new ReferenceRepository())
            {
                var dataSet = await repository.Received(baseModel.EntityID, (byte)Enums.ReferenceMode.Recommendation, (byte)Enums.AppriciationAndRecommedationStatus.Approved);
                var model = new ReferenceViewModel()
                {
                    References = dataSet.Tables[0].FromDataTable<PublicAppreciationAndRecommendationViewModel>()
                };
                return model;
            }
        }

        public async System.Threading.Tasks.Task<List<AppreciationAndRecommendationViewModel>> GetAppriciationsForProfile(string profileHash)
        {
            var baseModel = profileHash.ToObject<ProfileHashViewModel>(null);
            using (var repository = new ReferenceRepository())
            {
                var dataSet = await repository.Received(baseModel.EntityID, (byte)Enums.ReferenceMode.Appreciation, (byte)Enums.AppriciationAndRecommedationStatus.Approved);
                return dataSet.Tables[0].FromDataTable<AppreciationAndRecommendationViewModel>();
            }
        }


        public async Task<AccompishmentAppriciation> GetAppriciationModelForAccomplishment(string hash)
        {
            var awardModel = hash.ToObject<AwardViewModel>(null);
            var model = new AccompishmentAppriciation()
            {
                Accomplishment = await GetAccomplishment(awardModel.AwardID, awardModel.EntityID),
            };
            return model;
        }

        private async Task<AwardViewModel> GetAccomplishment(long accomplshmentID, long entityID)
        {
            using (var repository = new AwardRepository())
            {
                using (var dataSet = await repository.Basic(accomplshmentID, entityID))
                {
                    var model = dataSet.Tables[0].FromDataTable<AwardViewModel>()[0];
                    return model;
                }
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
                table.Add("ID", DateTime.UtcNow.Ticks);
                result.Hash = QueryStringHelper.Encrypt(table);
                result.SkillIncluded = dataSet.Tables[1].FromDataTable<CareerHistorySkillViewModel>("CareerHistoryID=" + result.CareerHistoryID);
                return result;
            }
        }


        public async Task<CareerHistoryReferenceViewModel> GetRecommendForRole(string q)
        {
            var awardModel = q.ToObject<CareerHistoryReferenceViewModel>(null);
            var model = new CareerHistoryReferenceViewModel()
            {
                CareerHistory = await GetOneCareerHisotry(q),
            };
            var table = new Hashtable();
            table.Add("CareerHistoryID", awardModel.CareerHistoryID);
            table.Add("JobTitleID", model.CareerHistory.JobTitleID);
            table.Add("ReferenceID", awardModel.ReferenceID);
            table.Add("EntityID", model.CareerHistory.EntityID);
            table.Add("ID", DateTime.UtcNow.Ticks);
            model.Hash = QueryStringHelper.Encrypt(table);
            return model;
        }


        public async Task<Result> SaveForRole(CareerHistoryReferenceViewModel model)
        {
            using (var repository = new ReferenceRepository())
            {
                var appreciationAndRecommendation = new AppreciationAndRecommendation();
                appreciationAndRecommendation = Mapper.Map<AppreciationAndRecommendationViewModel, AppreciationAndRecommendation>(model, appreciationAndRecommendation);
                appreciationAndRecommendation = model.Hash.ToObject<AppreciationAndRecommendation>(appreciationAndRecommendation);
                appreciationAndRecommendation.IpAddress = IpAddress;
                appreciationAndRecommendation.AppreciationAndRecommendationParticipants = new HashSet<AppreciationAndRecommendationParticipant>();
                appreciationAndRecommendation.SourceEntityID = UserHashObject.EntityID;
                // Set the entity id of the career history owner.
                appreciationAndRecommendation.AppreciationAndRecommendationParticipants.Add(new AppreciationAndRecommendationParticipant()
                {
                    EntityID = appreciationAndRecommendation.EntityID
                });
                if (appreciationAndRecommendation.ReferenceID > 0)
                {
                    appreciationAndRecommendation.Status = (byte)Enums.AppriciationAndRecommedationStatus.Approved;
                }
                var result = await repository.Save(appreciationAndRecommendation);
                if (result > 0)
                {
                    var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                    string url = string.Format("/profile/#reference{0}", result);
                    if (EntityHash.ContainsKey("Url"))
                        EntityHash.Remove("Url");
                    EntityHash.Add("Url", url);
                    await notificationProcessor.AddNotification(Enums.NotificationType.UserRecommendsMe, null, result, appreciationAndRecommendation.EntityID);

                    return new Result() { Type = Enums.ResultType.Success, Description = string.Format("Your recommendation has been sent for role {0}, will be visible once approval.", model.CareerHistory.JobTitleName) };
                }

                return new Result() { Type = Enums.ResultType.Error, Description = "We are unable to reach to your request, please have another try." };
            }
        }


        public async Task<Result> SaveForSkill(SkillReferenceViewModel model)
        {
            using (var repository = new ReferenceRepository())
            {
                var skillModel = model.Hash.ToObject<SkillViewModel>(null);
                var appreciationAndRecommendation = new AppreciationAndRecommendation();
                appreciationAndRecommendation = Mapper.Map<SkillReferenceViewModel, AppreciationAndRecommendation>(model, appreciationAndRecommendation);
                appreciationAndRecommendation = model.Hash.ToObject<AppreciationAndRecommendation>(appreciationAndRecommendation);
                appreciationAndRecommendation.IpAddress = IpAddress;
                appreciationAndRecommendation.AppreciationAndRecommendationParticipants = new HashSet<AppreciationAndRecommendationParticipant>();
                appreciationAndRecommendation.SourceEntityID = UserHashObject.EntityID;
                // Set the entity id of the career history owner.
                appreciationAndRecommendation.AppreciationAndRecommendationParticipants.Add(new AppreciationAndRecommendationParticipant()
                {
                    EntityID = appreciationAndRecommendation.EntityID,
                });

                appreciationAndRecommendation.AppreciationAndRecommendationSkills.Add(new AppreciationAndRecommendationSkill()
                {
                    EntitySkillID = skillModel.EntitySkillID
                });

                if (appreciationAndRecommendation.ReferenceID > 0)
                {
                    appreciationAndRecommendation.Status = (byte)Enums.AppriciationAndRecommedationStatus.Approved;
                }

                var result = await repository.Save(appreciationAndRecommendation);
                if (result > 0)
                {
                    var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                    string url = string.Format("/profile/#reference{0}", result);
                    if (EntityHash.ContainsKey("Url"))
                        EntityHash.Remove("Url");
                    EntityHash.Add("Url", url);
                    await notificationProcessor.AddNotification(Enums.NotificationType.UserRecommendsMe, null, result, appreciationAndRecommendation.EntityID, null, skillModel.EntitySkillID);

                    return new Result() { Type = Enums.ResultType.Success, Description = string.Format("Your recommendation has been sent for skill {0}, will be visible once approval.", skillModel.Name) };
                }

                return new Result() { Type = Enums.ResultType.Error, Description = "We are unable to reach to your request, please have another try." };
            }
        }

        //INSERT NOTIFICATION
        public async Task<Result> Ask(AskRecommendationViewModel model)
        {
            using (var repository = new ReferenceRepository())
            {
                var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                foreach (var item in model.Providers)
                {
                    var appreciationAndRecommendation = new AppreciationAndRecommendation();
                    appreciationAndRecommendation = Mapper.Map<AskRecommendationViewModel, AppreciationAndRecommendation>(model, appreciationAndRecommendation);
                    appreciationAndRecommendation.IpAddress = IpAddress;
                    appreciationAndRecommendation.AppreciationAndRecommendationParticipants = new HashSet<AppreciationAndRecommendationParticipant>();
                    appreciationAndRecommendation.SourceEntityID = Convert.ToInt64(item);
                    appreciationAndRecommendation.Status = (int)Enums.AppriciationAndRecommedationStatus.Requested;
                    // Set the entity id of the career history owner.
                    appreciationAndRecommendation.AppreciationAndRecommendationParticipants.Add(new AppreciationAndRecommendationParticipant()
                    {
                        EntityID = UserHashObject.EntityID,
                    });
                    var hashTable = new Hashtable();
                    var entitySkillID = default(long?);
                    if (model.RecommedationType == "Skill")
                    {
                        appreciationAndRecommendation.AppreciationAndRecommendationSkills.Add(new AppreciationAndRecommendationSkill()
                        {
                            EntitySkillID = (long)model.SkillID
                        });
                        entitySkillID = model.SkillID;
                        hashTable.Add("SkillID", model.SkillID);
                    }
                    else if (model.RecommedationType == "Role")
                    {
                        appreciationAndRecommendation.CareerHistoryID = model.CareerHistoryID;
                    }

                    string url = string.Format("/reference/requests");
                    if (EntityHash.ContainsKey("Url"))
                        EntityHash.Remove("Url");
                    EntityHash.Add("Url", url);
                    // And set then the entity Id who have posted this recomendation.
                    appreciationAndRecommendation.EntityID = UserHashObject.EntityID;
                    var referenceID = await repository.Save(appreciationAndRecommendation);
                    await notificationProcessor.AddNotification(Enums.NotificationType.UserAsksForReccomend, null, referenceID, Convert.ToInt64(item), null, entitySkillID);

                }
                return new Result() { Type = Enums.ResultType.Success, Description = string.Format("You request to recommend you has been sent to your {0} connections, please note you can always manage the recommedation by deleting and even hiding from your profile.", model.Providers.Length) };
            }
        }


        public async Task<List<ReferenceRequestViewModel>> GetRequests(string dir, long? id, int status)
        {
            if (!string.IsNullOrEmpty(dir))
            {
                if (dir.Equals("out", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (var repository = new ReferenceRepository())
                    {
                        var dataSet = await repository.SentRequests(UserHashObject.EntityID, (byte)Enums.ReferenceMode.Recommendation, status);
                        var model = dataSet.Tables[0].FromDataTable<ReferenceRequestViewModel>();
                        foreach (var item in model)
                        {
                            var table = new Hashtable();
                            table.Add("ReferenceID", item.ReferenceID);
                            table.Add("CareerHistoryID", item.CareerHistoryID);
                            table.Add("OrganizationName", item.OrganizationName);
                            table.Add("JobTitleName", item.JobTitleName);
                            table.Add("SkillID", item.SkillID);
                            table.Add("Name", item.SkillName);
                            table.Add("ID", DateTime.UtcNow.Ticks);
                            item.Hash = QueryStringHelper.Encrypt(table);
                        }

                        return model;
                    }
                }
            }
            using (var repository = new ReferenceRepository())
            {
                var dataSet = await repository.ReceivedRequests(UserHashObject.EntityID, (byte)Enums.ReferenceMode.Recommendation, status);
                var model = dataSet.Tables[0].FromDataTable<ReferenceRequestViewModel>();
                foreach (var item in model)
                {
                    var table = new Hashtable();
                    table.Add("ReferenceID", item.ReferenceID);
                    table.Add("CareerHistoryID", item.CareerHistoryID);
                    table.Add("OrganizationName", item.OrganizationName);
                    table.Add("JobTitleName", item.JobTitleName);
                    table.Add("SkillID", item.SkillID);
                    table.Add("Name", item.SkillName);
                    table.Add("ID", DateTime.UtcNow.Ticks);
                    item.Hash = QueryStringHelper.Encrypt(table);
                }
                return model;
            }
        }


        public async Task<List<ReferenceRequestViewModel>> GetReferences(string dir, long? id, int status)
        {
            if (!string.IsNullOrEmpty(dir))
            {
                if (dir.Equals("out", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (var repository = new ReferenceRepository())
                    {
                        var dataSet = await repository.Sent(UserHashObject.EntityID, null, status);
                        var model = dataSet.Tables[0].FromDataTable<ReferenceRequestViewModel>();
                        foreach (var item in model)
                        {
                            var table = new Hashtable();
                            table.Add("ReferenceID", item.ReferenceID);
                            table.Add("CareerHistoryID", item.CareerHistoryID);
                            table.Add("OrganizationName", item.OrganizationName);
                            table.Add("JobTitleName", item.JobTitleName);
                            table.Add("SkillID", item.SkillID);
                            table.Add("Name", item.SkillName);
                            table.Add("ID", DateTime.UtcNow.Ticks);
                            item.Hash = QueryStringHelper.Encrypt(table);
                        }

                        return model;
                    }
                }
            }
            using (var repository = new ReferenceRepository())
            {
                var dataSet = await repository.Received(UserHashObject.EntityID, null, null);
                var model = dataSet.Tables[0].FromDataTable<ReferenceRequestViewModel>();
                foreach (var item in model)
                {
                    var table = new Hashtable();
                    table.Add("ReferenceID", item.ReferenceID);
                    table.Add("CareerHistoryID", item.CareerHistoryID);
                    table.Add("OrganizationName", item.OrganizationName);
                    table.Add("JobTitleName", item.JobTitleName);
                    table.Add("SkillID", item.SkillID);
                    table.Add("Name", item.SkillName);
                    table.Add("ID", DateTime.UtcNow.Ticks);
                    item.Hash = QueryStringHelper.Encrypt(table);
                }
                return model;
            }
        }


        public async Task<Result> Execute(string hash, string actn)
        {
            var appreciationAndRecommendation = hash.ToObject<AppreciationAndRecommendation>(null);
            if (actn.Equals("skip", StringComparison.InvariantCultureIgnoreCase))
            {
                appreciationAndRecommendation.Status = (byte)Enums.AppriciationAndRecommedationStatus.Rejected;
            }
            if (actn.Equals("cancel", StringComparison.InvariantCultureIgnoreCase))
            {
                appreciationAndRecommendation.Status = (byte)Enums.AppriciationAndRecommedationStatus.Cancel;
            }
            if (actn.Equals("remove", StringComparison.InvariantCultureIgnoreCase))
            {
                appreciationAndRecommendation.Status = (byte)Enums.AppriciationAndRecommedationStatus.Removed;
            }
            if (actn.Equals("approve", StringComparison.InvariantCultureIgnoreCase))
            {
                appreciationAndRecommendation.Status = (byte)Enums.AppriciationAndRecommedationStatus.Approved;
            }
            using (var repository = new ReferenceRepository())
            {
                var result = await repository.Save(appreciationAndRecommendation);
                if (result >= 0)
                {
                    if (actn.Equals("skip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new Result() { Type = Enums.ResultType.Success, Description = "The request has been skipped successfully." };
                    }
                    if (actn.Equals("approve", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new Result() { Type = Enums.ResultType.Success, Description = "The request has been skipped successfully." };
                    }
                    if (actn.Equals("remove", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new Result() { Type = Enums.ResultType.Success, Description = "The reference has been removed from you profile." };
                    }
                    if (actn.Equals("cancel", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new Result() { Type = Enums.ResultType.Success, Description = "The request has been canceled successfully, in order to make one, please goto your profile and request a new." };
                    }
                }
                return new Result() { Type = Enums.ResultType.Success, Description = "There is an error reaching to your request, please give it another try." };
            }
        }
    }
}