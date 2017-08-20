using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Wrly.Data.Models.Extended;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models.Knowledge;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class KnowledgeProcessor : IKnowledgeProcessor
    {
        public async Task<HelpViewModel> Categories(string category = null, long? topicId = null)
        {
            var model = new HelpViewModel();
            using (var repository = new KnowledgeRepository())
            {
                using (var dataSet = await repository.GetAllCategories(category, topicId))
                {
                    model.Categories = dataSet.Tables[0].FromDataTable<CategoryViewModel>(null);
                    if (dataSet.Tables.Count > 1)
                    {
                        model.ActiveTopic = dataSet.Tables[1].FromDataTable<TopicViewModel>(null).FirstOrDefault();

                        if (model.ActiveTopic.ParentTopicID > 0)
                        {
                            foreach (var item in model.Categories)
                            {
                                if (item.CategoryID == model.ActiveTopic.CategoryID)
                                {
                                    item.Topics = dataSet.Tables[3].FromDataTable<TopicViewModel>(null);
                                    item.IsActive = item.Url.Equals(category, StringComparison.InvariantCultureIgnoreCase);
                                    foreach (var itemTopic in item.Topics)
                                    {
                                        if (itemTopic.TopicID == model.ActiveTopic.ParentTopicID)
                                        {
                                            itemTopic.Childs = dataSet.Tables[2].FromDataTable<TopicViewModel>(null);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in model.Categories)
                            {
                                item.Topics = dataSet.Tables[2].FromDataTable<TopicViewModel>("CategoryID=" + item.CategoryID);
                                item.IsActive = item.Url.Equals(category, StringComparison.InvariantCultureIgnoreCase);
                                if (topicId > 0)
                                {
                                    foreach (var itemTopic in item.Topics)
                                    {
                                        itemTopic.Childs = dataSet.Tables[3].FromDataTable<TopicViewModel>("ParentTopicID=" + itemTopic.TopicID);
                                    }
                                }
                            }

                            //if (topicId > 0)
                            //{
                            //    model.ActiveTopic.Childs = dataSet.Tables[3].FromDataTable<TopicViewModel>(null);
                            //}
                        }
                    }
                }
            }
            model.ActiveTopicID = topicId;
            if (!string.IsNullOrEmpty(category))
            {
                model.ActiveCategoryID = model.Categories.Any(c => c.Url.Equals(category, StringComparison.InvariantCultureIgnoreCase)) ? model.Categories.FirstOrDefault(c => c.Url.Equals(category, StringComparison.InvariantCultureIgnoreCase)).CategoryID : default(long?);
            }
            return model;
        }


        public async Task<long> Save(HelpViewModel model)
        {
            using (var repository = new KnowledgeRepository())
            {
                var topic = AutoMapper.Mapper.Map<PageTopic>(model.ActiveTopic);
                return await repository.Save(topic);
            }
        }


        public async Task<List<TopicViewModel>> GetTopics(long categoryID)
        {
            using (var repository = new KnowledgeRepository())
            {
                using (var dataSet = await repository.GetTopics(categoryID))
                {
                    return dataSet.Tables[0].FromDataTable<TopicViewModel>(null);
                }
            }
        }


        public async Task<List<CategoryViewModel>> GetCategories(string category = null)
        {
            using (var repository = new KnowledgeRepository())
            {
                using (var dataSet = await repository.GetCategoriesList(category))
                {
                    return dataSet.Tables[0].FromDataTable<CategoryViewModel>(null);
                }
            }
        }
    }
}