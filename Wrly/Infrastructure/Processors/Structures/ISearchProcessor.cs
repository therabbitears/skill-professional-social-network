using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Data.Models.Extended;
using Wrly.Models;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface ISearchProcessor
    {
        Task<List<EntitySearchViewModel>> Execute(string keyword);

        Task<Result> RecordSearch(EntitySearchViewModel model);

        Task<List<EntitySearchViewModel>> GetResults(string q, string type);

        Task<List<LuceneObject>> GetLuceneIndexableData();
    }
}
