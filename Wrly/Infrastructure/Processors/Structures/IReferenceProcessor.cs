using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Models;

namespace Wrly.Infrastructure.Processors.Structures
{
   public interface IReferenceProcessor
    {
        Models.AppreciationAndRecommendationViewModel GetAppriciation(string hash);

        Models.AppreciationAndRecommendationViewModel GetRecommendation(string hash);

        Task<Result> Save(Models.AppreciationAndRecommendationViewModel model);

        Task<List<AppreciationAndRecommendationViewModel>> GetRecommendations(string userName);

        Task<List<AppreciationAndRecommendationViewModel>> GetAppriciations(string userName);

        Task<ReferenceViewModel> GetRecommendationsForProfile(string profileHash);

        Task<List<AppreciationAndRecommendationViewModel>> GetAppriciationsForProfile(string profileHash);

        Task<AccompishmentAppriciation> GetAppriciationModelForAccomplishment(string q);

        Task<CareerHistoryReferenceViewModel> GetRecommendForRole(string q);

        Task<Result> SaveForRole(CareerHistoryReferenceViewModel model);

        Task<Result> SaveForSkill(SkillReferenceViewModel model);

        Task<Result> Ask(AskRecommendationViewModel model);

        Task<List<ReferenceRequestViewModel>> GetRequests(string dir, long? id, int status);

        Task<Result> Execute(string hash, string actn);

        Task<List<ReferenceRequestViewModel>> GetReferences(string dir, long? id, int status);
    }
}
