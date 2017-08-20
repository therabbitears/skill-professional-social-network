using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Types;
using Wrly.Data.Models;
using Wrly.Data.Models.Extended;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Infrastructure.Utils;
using Wrly.Models;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class SearchProcessor : BaseProcessor, ISearchProcessor
    {
        public async Task<List<EntitySearchViewModel>> Execute(string keyword)
        {
            using (var repository = new SearchRepository())
            {
                using (var data = await repository.Search(keyword, UserHashObject.EntityID))
                {
                    return data.Tables[0].FromDataTable<EntitySearchViewModel>();
                }
            }
        }


        public async Task<Result> RecordSearch(EntitySearchViewModel model)
        {
            using (var repository = new SearchRepository())
            {
                var search = Mapper.Map<EntitySearch>(model);
                search.SourceEntity = UserHashObject.EntityID;
                var result = await repository.Record(search);
                if (result > 0)
                {
                    UserCacheManager.RefreshSearches();
                    return new Result() { Type = Types.Enums.ResultType.Success };
                }
                return new Result() { Type = Types.Enums.ResultType.Error };
            }
        }


        public async Task<List<EntitySearchViewModel>> GetResults(string q, string type)
        {
            using (var repository = new SearchRepository())
            {
                if (!string.IsNullOrEmpty(type))
                {
                    if (type.Equals("people"))
                    {
                        using (var data = await repository.IndividualSearch(q, UserHashObject.EntityID))
                        {
                            return data.Tables[0].FromDataTable<EntitySearchViewModel>();
                        }
                    }
                    if (type.Equals("companies"))
                    {
                        using (var data = await repository.OrganizationSearch(q, UserHashObject.EntityID, (int)Enums.OrganizationType.Company))
                        {
                            return data.Tables[0].FromDataTable<EntitySearchViewModel>();
                        }
                    }
                    if (type.Equals("groups"))
                    {
                        using (var data = await repository.OrganizationSearch(q, UserHashObject.EntityID, (int)Enums.OrganizationType.Group))
                        {
                            return data.Tables[0].FromDataTable<EntitySearchViewModel>();
                        }
                    }
                    if (type.Equals("connections"))
                    {
                        using (var data = await repository.ConnectionSearch(q, UserHashObject.EntityID))
                        {
                            return data.Tables[0].FromDataTable<EntitySearchViewModel>();
                        }
                    }
                }
                using (var data = await repository.MixedSearch(q, UserHashObject.EntityID))
                {
                    return data.Tables[0].FromDataTable<EntitySearchViewModel>();
                }
            }
        }


        public async Task<List<Data.Models.Extended.LuceneObject>> GetLuceneIndexableData()
        {
            var documents = new List<LuceneObject>();
            LuceneObject document = null;
            using (var repository = new SearchRepository())
            {
                using (var data = await repository.GetLuceneIndexableData())
                {
                    var result = data.Tables[0].FromDataTable<LuceneEntitySearchViewModel>();
                    foreach (var item in result)
                    {
                        if (item.EntityType == (int)Enums.EntityTypes.Person)
                        {
                            document = new LuceneObject()
                            {
                                DisplayName = item.FormatedName,
                                EducationHistoryText = item.EducationHistoryText ?? string.Empty,
                                EntityID = item.EntityID,
                                EntityType = item.EntityType,
                                Headiing = item.ProfileHeading ?? string.Empty,
                                LastModified = Now,
                                ProfilePicUrl = item.ProfilePath ?? string.Empty,
                                SkillText = item.SkillText ?? string.Empty,
                                Url = item.ProfileName,
                                WorkHistoryText = item.WorkHistorytext ?? string.Empty
                            };
                        }
                        else if (item.EntityType == (int)Enums.EntityTypes.Organization)
                        {
                            document = new LuceneObject()
                            {
                                DisplayName = item.Name,
                                EducationHistoryText = string.Empty,
                                EntityID = item.EntityID,
                                EntityType = item.EntityType,
                                Headiing = item.Category,
                                LastModified = Now,
                                ProfilePicUrl = item.LogoPath ?? string.Empty,
                                SkillText = string.Empty,
                                Url = item.Url,
                                WorkHistoryText = string.Empty
                            };
                        }
                        else if (item.EntityType == (int)Enums.EntityTypes.Group)
                        {
                            document = new LuceneObject()
                            {
                                DisplayName = item.Name,
                                EducationHistoryText = string.Empty,
                                EntityID = item.EntityID,
                                EntityType = item.EntityType,
                                Headiing = ((Types.Enums.GroupType)item.SubType).GetDescription(),
                                LastModified = Now,
                                ProfilePicUrl = item.LogoPath ?? string.Empty,
                                SkillText = string.Empty,
                                Url = item.Url,
                                WorkHistoryText = string.Empty,
                                SubType = item.SubType
                            };
                        }
                        documents.Add(document);
                    }
                }
            }
            return documents;
        }
    }
}