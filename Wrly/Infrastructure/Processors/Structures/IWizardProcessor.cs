using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using Wrly.Models;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface IWizardProcessor
    {
        Task<bool> IsValidStep(Enums.WizardStep step, string hash);
        Task<Enums.WizardStep> GetStepToComplete();
        Task<WizardResultViewModel> GetWizardResultData(Enums.WizardStep step);
        Task<string> GetHashForWizard(Enums.WizardStep step);

        Task<bool> IsValidVarification(string hash);

        Task<Enums.VarificationResult> VarifyEmail(string hash);

        Task<Enums.VarificationStatus> VarificationStatus(string hash);

        Task<Result> FinishWizard();

        Task<Result> Feeback(string feedback);

        Task<Result> SendAbuse(AbuseViewModel model);

        Task<Result> Feeback(FeedbackViewModel model);
    }
}
