using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using Wrly.Data.Repositories.Implementors;
using Wrly.Models.Listing;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface ILookupProcessor
    {
        Dictionary<string, string> Projects(string userName, bool isProfileName);

        Task<List<KeyValue>> Skills(long entityID);

        Task<List<KeyValue>> MySkills(string keyword);

        Task<List<KeyValue>> Connections(long entityID);

        Task<List<PersonFacehead>> Connections(long entityID, string keyWord);

        Task<List<KeyValue>> JobTitles(string keyword, Enums.JobtTitleType type = Enums.JobtTitleType.Professional);

        Task<List<KeyValue>> Organizations(string key, Enums.OrganizationType type = Enums.OrganizationType.Company);

        Task<List<KeyValue>> Universities(string key);

        Task<List<KeyValue>> Courses(string keyword);

        Task<Dictionary<string, string>> CareerHistoryList(long entityID);

        Task<List<KeyValue>> Industries(string keyWord);

        Task<List<KeyValue>> AllSkills(string keyword);
    }
}
