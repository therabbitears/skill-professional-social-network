using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Models;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface ISkillHistoryProcessor
    {
        Task<List<SkillViewModel>> GetSkillHisotry();

        Task<SkillViewModel> GetOneSkill(string hash);

        Task<long> Save(SkillViewModel model);

        Task<SkillDetailViewModel> Details(string id);

        Task<List<PublicSkillViewModel>> GetSkillHisotryForProfile(string profileHash);

        Task<List<SkillHistoryOption>> LoadSkillOptions(string q);

        Task<Result> Endorse(string q);
        Task<Result> RemoveEndorse(string q);

        Task<long> SaveWizard(ListSkillViewModel model, bool ispopup);

        Task<Result> Remove(string q);

        Task<Result> RevertRemove(string q);
    }
}
