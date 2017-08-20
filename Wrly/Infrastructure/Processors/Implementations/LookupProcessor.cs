using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Structures;
using System.Data;
using System.Threading.Tasks;
using Wrly.Models.Listing;
using Types;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class LookupProcessor : BaseProcessor, ILookupProcessor
    {
        public Dictionary<string, string> Projects(string userName, bool isProfileName)
        {
            using (var repository = new LookUpRepository())
            {
                using (var dataSet = repository.GetProjects(userName, isProfileName))
                {
                    return (from country in dataSet.Tables[0].AsEnumerable()
                            select new
                                {
                                    Key = Convert.ToString(country["AwardID"]),
                                    Value = Convert.ToString(country["Name"])
                                }).ToDictionary(Key => Key.Key, Value => Value.Value);
                }
            }
        }


        public async Task<List<KeyValue>> Skills(long entityID)
        {
            using (var repository = new LookUpRepository())
            {
                using (var dataSet = await repository.GetSkills(entityID))
                {
                    return (from country in dataSet.Tables[0].AsEnumerable()
                            select new KeyValue
                            {
                                Key = Convert.ToString(country["EntitySkillID"]),
                                Value = Convert.ToString(country["Name"])
                            }).ToList();
                }
            }
        }

        public async Task<List<KeyValue>> MySkills(string keyword)
        {
            using (var repository = new LookUpRepository())
            {
                using (var dataSet = await repository.GetSkills(keyword, UserHashObject.EntityID))
                {
                    return (from country in dataSet.Tables[0].AsEnumerable()
                            select new KeyValue
                            {
                                Key = Convert.ToString(country["SkillID"]),
                                Value = Convert.ToString(country["Name"])
                            }).ToList();
                }
            }
        }

        public async Task<List<KeyValue>> AllSkills(string keyword)
        {
            using (var repository = new LookUpRepository())
            {
                using (var dataSet = await repository.GetSkills(keyword))
                {
                    return (from country in dataSet.Tables[0].AsEnumerable()
                            select new KeyValue
                            {
                                Key = Convert.ToString(country["SkillID"]),
                                Value = Convert.ToString(country["Name"])
                            }).ToList();
                }
            }
        }



        public async Task<List<KeyValue>> Connections(long entityID)
        {
            using (var repository = new LookUpRepository())
            {
                using (var dataSet = await repository.GetConnections(entityID))
                {
                    return (from country in dataSet.Tables[0].AsEnumerable()
                            select new KeyValue
                            {
                                Key = Convert.ToString(country["EntityID"]),
                                Value = Convert.ToString(country["Name"])
                            }).ToList();
                }
            }
        }


        public async Task<List<PersonFacehead>> Connections(long entityID, string keyword)
        {
            using (var repository = new LookUpRepository())
            {
                using (var dataSet = await repository.GetConnections(entityID, keyword))
                {
                    return dataSet.Tables[0].FromDataTable<PersonFacehead>();
                }
            }
        }


        public async Task<List<KeyValue>> JobTitles(string keyword, Enums.JobtTitleType jobTitleType = Enums.JobtTitleType.Professional)
        {
            using (var repository = new LookUpRepository())
            {
                using (var dataSet = await repository.GetJobTitles(keyword, (int)jobTitleType))
                {
                    return (from country in dataSet.Tables[0].AsEnumerable()
                            select new KeyValue
                            {
                                Key = Convert.ToString(country["JobTitleID"]),
                                Value = Convert.ToString(country["Name"]),
                                Total = Convert.ToInt64(country["Total"]),
                            }).ToList();
                }
            }
        }


        public async Task<List<KeyValue>> Organizations(string key, Enums.OrganizationType type = Enums.OrganizationType.Company)
        {
            using (var repository = new LookUpRepository())
            {
                using (var dataSet = await repository.GetOrganization(key, (int)type))
                {
                    return (from country in dataSet.Tables[0].AsEnumerable()
                            select new KeyValue
                            {
                                Key = Convert.ToString(country["OrganizationID"]),
                                Value = Convert.ToString(country["Name"]),
                            }).ToList();
                }
            }
        }


        public async Task<List<KeyValue>> Universities(string key)
        {
            return await Organizations(key, Enums.OrganizationType.University);
        }


        public async Task<List<KeyValue>> Courses(string keyword)
        {
            return await JobTitles(keyword, Enums.JobtTitleType.Educational);
        }


        public async Task<Dictionary<string, string>> CareerHistoryList(long entityID)
        {
            var result = new Dictionary<string, string>() { { "-1", "Select" } };
            using (var repository = new CareerHistoryRepository())
            {
                var dataSet = await repository.ForUser(entityID, false);
                foreach (DataRow item in dataSet.Tables[0].Rows)
                {
                    string name = Convert.ToInt16(item["Type"]) == (short)Enums.CareerHistoryMode.Education ?
                        string.Format("Student at {0} while {1}", item["OrganizationName"], item["JobTitleName"]) :
                        string.Format("Working at {0} as {1}", item["OrganizationName"], item["JobTitleName"]);
                    result.Add(Convert.ToString(item["CareerHistoryID"]), name);
                }
            }
            return result;
        }


        public async Task<List<KeyValue>> Industries(string keyWord)
        {
            using (var CommonRepository = new CommonRepository())
            {
                using (var dataSet = await CommonRepository.Industries())
                {
                    return (from country in dataSet.Tables[0].AsEnumerable()
                            select new KeyValue
                            {
                                Key = Convert.ToString(country["IndustryID"]),
                                Value = Convert.ToString(country["IndustryName"]),
                            }).ToList();
                }
            }
        }
    }
}