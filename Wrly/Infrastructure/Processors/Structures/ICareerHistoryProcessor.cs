using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Models;
using Wrly.Models.Listing;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface ICareerHistoryProcessor
    {
        Task<List<CareerHistoryViewModel>> GetCareerHisotry(short type, string subType = null);

        Task<CareerHistoryViewModel> GetOneCareerHisotry(string hash);

        Task<long> Save(CareerHistoryViewModel model, bool wizard = false);

        Task<Dictionary<long, string>> GetCareerHisotry();

        Task<long> RemoveSkill(string hash);

        Task<string> GenerateTokenForCareerHistoryForEntity(long result);

        Task<IntelligenceCareerHistoryViewModel> GetImprovableCareerHistory(int type);

        Task<IntelligenceCareerHistoryViewModel> GetImprovableCareerHistoryForTime(int type);

        Task<IntelligenceAwardViewModel> GetImprovableAward();

        Task<IntelligenceAwardViewModel> GetImprovableAssignment();

        Task<List<CareerHistoryViewModel>> GetCareerHisotryForProfile(short mode, string profileHash, string subType = null);

        Task<List<CareerHistoryOption>> LoadAssginementOptions(string q);

        Task<long> SaveWizard(CareerHistoryWizardViewModel model, bool ispopup);

        Task<Result> Remove(string mode, string q);

        Task<OpportunityDataViewModel> GetDataForOpportunity(long? entityID);
    }
}
