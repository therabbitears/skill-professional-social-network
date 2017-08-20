using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Models.Knowledge;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface IKnowledgeProcessor
    {
        Task<HelpViewModel> Categories(string category = null, long? topicId = null);

        Task<List<TopicViewModel>> GetTopics(long categoryID);

        Task<List<CategoryViewModel>> GetCategories(string category = null);

        Task<long> Save(HelpViewModel model);
    }
}
