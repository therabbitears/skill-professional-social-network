using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Models;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface IAwardProcessor
    {
        Task<List<AwardViewModel>> GetAwards(string userName);

        Task<AwardViewModel> GetOneAward(string hash);

        Task<int> Save(AwardViewModel model);

        Task<int> SavePublication(PublilcationViewModel model);

        Task<List<AwardViewModel>> GetAssignments(string userName);

        int RemoveParticipant(string hash);

        int RemoveSkill(string hash);

        Task<PublilcationViewModel> GetOnePublication(string hash);

        Task<List<PublilcationViewModel>> GetPublication(string userName);

        Task<List<PublilcationViewModel>> GetCompositions(string userName);

        Task<List<PublilcationViewModel>> GetResearches(string userName);

        Task<List<PublilcationViewModel>> GetFindings(string userName);

        Task<AwardViewModel> GetProjectToRequestParticipant(string hash);

        Task<long> SaveAddParticipantRequest(AwardViewModel model);

        Task<List<AssignmentOption>> LoadAssginementOptions(string hash);

        Task<List<PublicAwardViewModel>> GetAwardsForProfile(string profileHash);

        Task<List<AwardViewModel>> GetAssignmentsForProfile(string profileHash);

        Task<AwardViewModel> GetBasicDetailsToAdd(string hash);

        Task<List<PublilcationViewModel>> GetFindingsForProfile(string profileHash);

        Task<List<PublilcationViewModel>> GetResearchesForProfile(string profileHash);

        Task<List<PublilcationViewModel>> GetPublicationForProfile(string profileHash);

        Task<List<PublilcationViewModel>> GetCompositionsForProfile(string profileHash);

        Task<AccomplishmentReportViewModel> GetReportableAccomplishment(string hash);

        Task<Result> Report(AccomplishmentReportViewModel model);

        Task<Result> Congratulate(string q);

        Task<List<PublilcationViewModel>> GetServices();

        Task<List<PublilcationViewModel>> GetProducts();

        Task<List<PublilcationViewModel>> GetServicesForProfile(string q);

        Task<List<PublilcationViewModel>> GetProductsForProfile(string q);

        Task<List<AwardViewModel>> GetServicesAndProductBasic(string hash);

        Task<Result> Remove(string mode, string hash);
    }
}
