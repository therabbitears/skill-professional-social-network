using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Types;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.infrastuctures.Utils;
using Wrly.Models;
using Wrly.Models.Business;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class WizardProcessor : BaseProcessor, IWizardProcessor
    {
        public async Task<bool> IsValidStep(Enums.WizardStep step, string hash)
        {
            var wizardViewModel = hash.ToObject<WizardViewModel>(null);
            if (wizardViewModel.EntityID == UserHashObject.EntityID)
            {
                using (var repository = new AccountRepository())
                {
                    return await repository.IsValidWizardStep(wizardViewModel.PersonID, wizardViewModel.StepCreatedFor);
                }
            }
            return false;
        }


        public async Task<Enums.WizardStep> GetStepToComplete()
        {
            using (var repository = new AccountRepository())
            {
                var result = await repository.GetCurrentStepOfAward(UserHashObject.PersonID);
                if (result == -1)
                    return Enums.WizardStep.Invalid;
                return (Enums.WizardStep)result;
            }
        }

        public async Task<WizardResultViewModel> GetWizardResultData(Enums.WizardStep step)
        {
            using (var repository = new AccountRepository())
            {
                var hash = await GetHashForWizard(step);
                string url = string.Empty;
                if (step == Enums.WizardStep.VarifyEmail)
                {
                    url = "/Wizard/VarifyEmail?hash=" + hash + "&setupe=mail&mode&varify&stamp=" + DateTime.UtcNow.Ticks.ToString();
                }
                if (step == Enums.WizardStep.AddSkills)
                {
                    url = "/Wizard/SetSkills?hash=" + hash + "&setupe=mail&mode&varify&stamp=" + DateTime.UtcNow.Ticks.ToString();
                }
                return new WizardResultViewModel() { IsSuccess = true, RedirectUrl = url };
            }
        }

        public async Task<string> GetHashForWizard(Enums.WizardStep step)
        {
            using (var repository = new AccountRepository())
            {
                var result = await repository.GetCurrentStepOfAward(UserHashObject.PersonID);
                var hashTable = new Hashtable();
                hashTable.Add("EntityID", UserHashObject.EntityID);
                hashTable.Add("PersonID", UserHashObject.PersonID);
                hashTable.Add("IsWizard", true);
                hashTable.Add("StepCreatedFor", (int)step);
                hashTable.Add("Stamp", Now);
                hashTable.Add("DbID", Guid.NewGuid());
                hashTable.Add("EmailAddress", "temporary.recovery@vidura.com");
                return QueryStringHelper.Encrypt(hashTable);
            }
        }


        public async Task<bool> IsValidVarification(string hash)
        {
            var model = hash.ToObject<WizardHashViewModel>(null);
            using (var repository = new AccountRepository())
            {
                using (DataSet ds = await repository.FindPersonDetailByVarification(model.PersonID, model.Id, model.EmailAddress))
                {
                    return ds != null && ds.Tables[0].Rows.Count > 0;
                }
            }
        }

        public async Task<Enums.VarificationResult> VarifyEmail(string hash)
        {
            var model = hash.ToObject<WizardHashViewModel>(null);
            if (await IsValidVarification(hash))
            {
                var keyValue = new Dictionary<string, string>();
                keyValue.Add("EmailVarified", true.ToString());
                keyValue.Add("WizardStep", ((int)Enums.WizardStep.AddConnections).ToString());
                using (var repository = new AccountRepository())
                {
                    var result = await repository.UpdateProfileItems(keyValue, model.PersonID);
                    if (result >= 0)
                    {
                        return Enums.VarificationResult.Varified;
                    }
                    return Enums.VarificationResult.Error;
                }
            }
            return Enums.VarificationResult.AlreadyVarified;
        }

        public async Task<Enums.VarificationStatus> VarificationStatus(string hash)
        {
            var model = hash.ToObject<WizardHashViewModel>(null);
            using (var repository = new AccountRepository())
            {
                using (DataSet ds = await repository.FindPersonDetailByVarification(model.PersonID, model.Id, model.EmailAddress))
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0]["EmailVarified"] == DBNull.Value)
                        {
                            return Enums.VarificationStatus.InvalidLink;
                        }
                        if (Convert.ToBoolean(ds.Tables[0].Rows[0]["EmailVarified"]))
                        {
                            return Enums.VarificationStatus.AlreadyVarified;
                        }
                    }
                }
            }
            return Enums.VarificationStatus.InvalidLink;
        }


        public async Task<Result> FinishWizard()
        {
            var keyValue = new Dictionary<string, string>();
            keyValue.Add("WizardFinished", true.ToString());
            keyValue.Add("WizardStep", ((int)Enums.WizardStep.Feedback).ToString());
            using (var repository = new AccountRepository())
            {
                var result = await repository.UpdateProfileItems(keyValue, UserHashObject.PersonID);
                if (result >= 0)
                {
                    return new Result() { Type = Enums.ResultType.Success };
                }
                return new Result() { Type = Enums.ResultType.Error };
            }
        }


        public async Task<Result> Feeback(string feedback)
        {
            var result = await SendFeedback(feedback);
            if (result == Enums.ResultType.Success)
            {
                return new Result() { Type = Enums.ResultType.Success, Description = string.Format("We received your feedback, and we will try to follow your suggestions, advice all the way.") };
            }
            return new Result() { Type = Enums.ResultType.Error, Description = "There is an error on server while sending feed email, please give it another try." };
        }

        private async Task<Types.Enums.ResultType> SendFeedback(string feedback)
        {
            var hashTable = new Hashtable();
            string emailAddress = string.Empty;
            if (UserHashObject.EntityType == (int)Enums.EntityTypes.Person)
            {
                using (var repository = new AccountRepository())
                {
                    var profile = await repository.GetProfile(UserHashObject.EntityID);
                    var model = profile.Tables[0].FromDataTable<ProfileViewModel>(null)[0];
                    emailAddress = model.EmailAddress;
                    hashTable.Add("**Name**", model.FormatedName);
                    hashTable.Add("**EmailAddress**", emailAddress);
                    hashTable.Add("**Feedback**", feedback);
                    hashTable.Add("**JobTitle**", model.FormatedJobTitle);
                    hashTable.Add("**EntityID**", model.EntityID);
                }
            }
            else
            {
                using (var repository = new BusinessRepository())
                {
                    var profile = await repository.GetProfile(UserHashObject.EntityID);
                    var model = profile.Tables[0].FromDataTable<BusinessProfileViewModel>(null)[0];
                    hashTable.Add("**Name**", model.Name);
                    hashTable.Add("**Feedback**", feedback);
                    hashTable.Add("**JobTitle**", model.Category);
                    hashTable.Add("**EntityID**", UserHashObject.EntityType);
                }
            }
            var result = await NotificationProcessor.SendEmail(hashTable, Enums.EmailFilePaths.Feedback, "banshi003@gmail.com");
            if (result == Enums.ResultType.Success)
            {
                if (!string.IsNullOrEmpty(emailAddress))
                    result = await NotificationProcessor.SendEmail(hashTable, Enums.EmailFilePaths.FeedbackThank, emailAddress);
            }
            return result;
        }


        public async Task<Result> SendAbuse(AbuseViewModel model)
        {
            var hashTable = new Hashtable();
            hashTable.Add("**Name**", model.Name);
            hashTable.Add("**EmailAddress**", model.EmailAddress);
            hashTable.Add("**Type**", model.Type);
            hashTable.Add("**Value**", model.Particular);
            hashTable.Add("**Description**", model.Description);

            var result = await NotificationProcessor.SendEmail(hashTable, Enums.EmailFilePaths.Abuse, "banshi003@gmail.com");
            if (result == Enums.ResultType.Success)
            {
                result = await NotificationProcessor.SendEmail(hashTable, Enums.EmailFilePaths.AbuseAknowledgement, model.EmailAddress);
            }
            return new Result() { Type = Enums.ResultType.Success, Description = "Your abuse report has been sent for review, we will look into matter shortly." };
        }

        public async Task<Result> Feeback(FeedbackViewModel model)
        {
            var result = await SendFeedback(model.Feedback);
            if (result == Enums.ResultType.Success)
            {
                return new Result() { Type = Enums.ResultType.Success, Description = string.Format("We received your feedback, and we will try to follow your suggestions, advice all the way.") };
            }
            return new Result() { Type = Enums.ResultType.Error, Description = "There is an error on server while sending feed email, please give it another try." };
        }
    }
}