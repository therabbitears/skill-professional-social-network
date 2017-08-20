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
using Wrly.Notifications.Processors.Implementors;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class SkillHistoryProcessor : BaseProcessor, ISkillHistoryProcessor
    {
        public async Task<List<SkillViewModel>> GetSkillHisotry()
        {
            using (var repository = new EntitySkillRepository())
            {
                var dataSet = await repository.ForUser(UserHashObject.EntityID);
                var result = dataSet.Tables[0].FromDataTable<SkillViewModel>();
                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        if (Request.IsAuthenticated)
                        {
                            var table = new Hashtable();
                            table.Add("EntitySkillID", item.EntitySkillID);
                            table.Add("EntityID", item.EntityID);
                            item.Hash = QueryStringHelper.Encrypt(table);
                            item.AllowEdit = true;
                        }
                        item.StartFromMonthName = item.StartFromMonth != null ? ((Enums.Months)item.StartFromMonth).GetDescription() : string.Empty;
                        item.EndFromMonthName = item.EndFromMonth != null ? ((Enums.Months)item.EndFromMonth).GetDescription() : string.Empty;
                    }
                }
                return result;
            }
        }

        public async Task<SkillViewModel> GetOneSkill(string hash)
        {
            var skillModel = hash.ToObject<SkillViewModel>(null);
            using (var repository = new EntitySkillRepository())
            {
                var dataSet = await repository.Single(skillModel.EntitySkillID);
                var item = dataSet.Tables[0].FromDataTable<SkillViewModel>()[0];
                var table = new Hashtable();
                table.Add("EntitySkillID", item.EntitySkillID);
                table.Add("Stamp", DateTime.UtcNow);
                item.Hash = QueryStringHelper.Encrypt(table);
                item.AllowEdit = true;
                item.StartFromMonthName = item.StartFromMonth != null ? ((Enums.Months)item.StartFromMonth).GetDescription() : string.Empty;
                item.EndFromMonthName = item.EndFromMonth != null ? ((Enums.Months)item.EndFromMonth).GetDescription() : string.Empty;
                return item;
            }
        }

        public async Task<long> Save(SkillViewModel model)
        {
            var entitySkill = new EntitySkill();
            if (!string.IsNullOrEmpty(model.Hash))
            {
                entitySkill = Mapper.Map<SkillViewModel, EntitySkill>(model, entitySkill);
                entitySkill = model.Hash.ToObject<EntitySkill>(entitySkill);
                if (entitySkill.EntitySkillID != model.EntitySkillID)
                    return -1;
            }
            else
            {
                entitySkill = Mapper.Map<SkillViewModel, EntitySkill>(model, entitySkill);
                entitySkill = model.UserHash.ToObject<EntitySkill>(entitySkill);
            }
            entitySkill.IpAddress = IpAddress;
            entitySkill.Skill = Mapper.Map<Skill>(model);
            entitySkill.Skill.IpAddress = IpAddress;
            using (var repository = new EntitySkillRepository())
            {
                var skillID = await repository.Save(entitySkill);
                await repository.CalculateAndSetSkillScore(skillID, UserHashObject.EntityID);
                if (skillID > 0 && entitySkill.EntitySkillID == 0)
                {
                    var activity = new NetworkActivity()
                    {
                        Type = (int)Enums.NetworkActivityType.AddedSkill,
                        SkillID = skillID,
                        IpAddress = IpAddress,
                        Identifier = string.Format("{0}_{1}_{2}", skillID, (int)Enums.NetworkActivityType.AddedSkill, entitySkill.SkillID),
                        EntityID = UserHashObject.EntityID,
                        EditedOn = Now,
                        EditedBy = User,
                        CreatedOn = Now,
                        ActionTaken = false
                    };

                    using (var networkActivity = new CommonRepository())
                    {
                        await networkActivity.AddActivity(activity);
                    }

                    using (var accountRepository = new AccountRepository())
                    {
                        await accountRepository.UpdateProfileScore(UserHashObject.EntityID);
                    }
                }
                return skillID;
            }
           
        }

        public string IpAddress { get { return HttpContext.Current.Request.UserHostAddress; } }

        public HttpRequest Request { get { return HttpContext.Current.Request; } }


        public Task<SkillDetailViewModel> Details(string id)
        {
            throw new NotImplementedException();
        }


        public async Task<List<PublicSkillViewModel>> GetSkillHisotryForProfile(string profileHash)
        {
            using (var repository = new EntitySkillRepository())
            {
                var entityID = UserHashObject == null ? 0 : UserHashObject.EntityID;
                var model = profileHash.ToObject<ProfileHashViewModel>(null);
                var dataSet = await repository.PublicSkills(model.EntityID, entityID);
                var result = dataSet.Tables[0].FromDataTable<PublicSkillViewModel>();
                if (result != null && result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        if (Request.IsAuthenticated)
                        {
                            var table = new Hashtable();
                            table.Add("EntitySkillID", item.EntitySkillID);
                            table.Add("EntityID", item.EntityID);
                            table.Add("Name", item.Name);
                            item.Hash = QueryStringHelper.Encrypt(table);
                            item.AllowEdit = true;
                        }
                        item.StartFromMonthName = item.StartFromMonth != null ? ((Enums.Months)item.StartFromMonth).GetDescription() : string.Empty;
                        item.EndFromMonthName = item.EndFromMonth != null ? ((Enums.Months)item.EndFromMonth).GetDescription() : string.Empty;
                    }
                }
                return result;
            }
        }


        public async Task<List<SkillHistoryOption>> LoadSkillOptions(string q)
        {
            using (var repository = new EntitySkillRepository())
            {
                var skillModel = q.ToObject<SkillViewModel>(null);
                var awardDetail = await GetOneSkill(q);
                var list = new List<SkillHistoryOption>();
                using (var dataSet = await repository.LoadOptionsForSkill(skillModel.EntitySkillID, UserHashObject.EntityID))
                {
                    var row = dataSet.Tables[0].FromDataTable<SkillActionsForEntity>().FirstOrDefault();
                    var option = new SkillHistoryOption();
                    var hashTable = new Hashtable();
                    hashTable.Add("DbID", Guid.NewGuid());
                    hashTable.Add("Name", awardDetail.Name);
                    hashTable.Add("EntityID", awardDetail.EntityID);
                    hashTable.Add("EntitySkillID", awardDetail.EntitySkillID);
                    if (row.AlreadyRecommended == 0)
                    {
                        hashTable.Add("Action", "Recommend");
                        option = new SkillHistoryOption()
                        {
                            DisplayText = "Recommend skill",
                            Hash = QueryStringHelper.Encrypt(hashTable),
                            Action = "recommend",
                            Type = "skill"
                        };
                        list.Add(option);
                    }
                    else
                    {
                        option = new SkillHistoryOption()
                        {
                            DisplayText = "Recommended",
                            NoAction = true,
                            Strong = true
                        };
                        list.Add(option);
                    }

                    if (row.AlreadyEndorsed == 0)
                    {
                        if (hashTable.Contains("Action"))
                        {
                            hashTable.Remove("Action");
                        }
                        hashTable.Add("Action", "Endorse");
                        option = new SkillHistoryOption()
                        {
                            DisplayText = "+1 Endorse",
                            Hash = QueryStringHelper.Encrypt(hashTable),
                            Action = "endorse",
                            Type = "skill"
                        };
                        list.Add(option);
                    }
                    else
                    {
                        if (hashTable.Contains("Action"))
                        {
                            hashTable.Remove("Action");
                        }
                        hashTable.Add("Action", "Remove-Endorse");
                        option = new SkillHistoryOption()
                        {
                            DisplayText = "-1 Endorse",
                            Hash = QueryStringHelper.Encrypt(hashTable),
                            Action = "remove-endorse",
                            Type = "skill"
                        };
                        list.Add(option);
                    }
                }
                return list;
            }
        }


        public async Task<Result> Endorse(string q)
        {
            var entitySkill = q.ToObject<SkillViewModel>(null);
            var state = Mapper.Map<SkillViewModel, EntitySkillState>(entitySkill);
            state.Type = (int)Enums.SkillStateType.Endorcement;
            state.SubType = (int)Enums.SkillStateSubType.General;
            state.Status = (int)Enums.SkillStateStatus.Active;

            state.IpAddress = IpAddress;
            state.EntityID = UserHashObject.EntityID;
            using (var repositoty = new EntitySkillRepository())
            {
                var result = await repositoty.RecordState(state);
                if (result > 0)
                {
                    string url = string.Format("/profile/#skills{0}", entitySkill.EntitySkillID);
                    if (EntityHash.ContainsKey("Url"))
                        EntityHash.Remove("Url");
                    EntityHash.Add("Url", url);
                    var notificationProcessor = new InstantNotificationProcessor(EntityHash);
                    await notificationProcessor.AddNotification(Enums.NotificationType.Endoreced, null, null, entitySkill.EntityID, null, entitySkill.EntitySkillID);

                    await repositoty.CalculateAndSetSkillScore(entitySkill.EntitySkillID, entitySkill.EntityID);
                    return new Result() { Type = Enums.ResultType.Success, Description = "Successfully endorsed the skill." };
                }
                else
                    return new Result() { Type = Enums.ResultType.Error, Description = "We cannot reach to your request, please give another try." };
            }
        }
        public async Task<Result> RemoveEndorse(string q)
        {
            var entitySkill = q.ToObject<SkillViewModel>(null);
            var state = Mapper.Map<SkillViewModel, EntitySkillState>(entitySkill);
            state.EntityID = UserHashObject.EntityID;
            state.Status = (byte)Enums.SkillStateStatus.Reverted;
            using (var repositoty = new EntitySkillRepository())
            {
                var result = await repositoty.ChangeSkillStateStatus(state);
                if (result > 0)
                {
                    await repositoty.CalculateAndSetSkillScore(entitySkill.EntitySkillID, entitySkill.EntityID);
                    return new Result() { Type = Enums.ResultType.Success, Description = "Successfully endorsed the skill." };
                }
                else
                    return new Result() { Type = Enums.ResultType.Error, Description = "We cannot reach to your request, please give another try." };
            }
        }


        public async Task<long> SaveWizard(ListSkillViewModel model, bool ispopup)
        {
            if (model != null && model.Skills != null && model.Skills.Count() > 0)
            {
                if (!ispopup)
                {
                    model = model.Hash.ToObject<ListSkillViewModel>(model);
                    if (UserHashObject.EntityID != model.EntityID)
                        return -1;
                }
                else
                {
                    model.EntityID = UserHashObject.EntityID;
                }


                var entitySkill = new EntitySkill();
                using (var repository = new EntitySkillRepository())
                {
                    foreach (var item in model.Skills)
                    {
                        entitySkill = Mapper.Map<EntitySkill>(model);
                        // Default score;
                        entitySkill.Score = 5;
                        entitySkill.IpAddress = IpAddress;
                        entitySkill.Skill = Mapper.Map<Skill>(model);
                        entitySkill.Skill.Name = item;
                        entitySkill.Skill.IpAddress = IpAddress;
                        var skillID = await repository.Save(entitySkill);
                        if (skillID < 0)
                            return -1;
                    }
                }
            }

            if (!ispopup)
            {
                var keyValue = new Dictionary<string, string>();
                keyValue.Add("WizardStep", ((int)Enums.WizardStep.AddSkills).ToString());
                using (var accountRepository = new AccountRepository())
                {
                    await accountRepository.UpdateProfileItems(keyValue, UserHashObject.PersonID);
                }
            }
            return 1;
        }


        public async Task<Result> Remove(string q)
        {
            var skill = q.ToObject<EntitySkill>(null);
            if (skill.EntityID == UserHashObject.EntityID)
            {
                using (var repository = new SkillRepository())
                {
                    var result = await repository.Remove(skill);
                    if (result > 0)
                    {
                        using (var accountRepository = new AccountRepository())
                        {
                            await accountRepository.UpdateProfileScore(UserHashObject.EntityID);
                        }
                        return new Result() { Type = Enums.ResultType.Success };
                    }
                    return new Result() { Type = Enums.ResultType.Error };
                }
            }
            return null;
        }

        public async Task<Result> RevertRemove(string q)
        {
            var skill = q.ToObject<EntitySkill>(null);
            if (skill.EntityID == UserHashObject.EntityID)
            {
                using (var repository = new SkillRepository())
                {
                    var result = await repository.Revert(skill);
                    if (result > 0)
                    {
                        using (var accountRepository = new AccountRepository())
                        {
                            await accountRepository.UpdateProfileScore(UserHashObject.EntityID);
                        }
                        return new Result() { Type = Enums.ResultType.Success };
                    }
                    return new Result() { Type = Enums.ResultType.Error };
                }
            }
            return null;
        }
    }
}
