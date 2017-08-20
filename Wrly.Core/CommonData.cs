using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Types;

namespace Wrly.Infrastructure.Utils
{
    public static class CommonData
    {

        public static Dictionary<string, string> EmployeeStrengths()
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var item in Enum.GetValues(typeof(Enums.EmployeeStrengths)))
            {
                dictionary.Add(item.ToString(), item.GetDescription());
            }
            return dictionary;
        }

        public static Dictionary<int?, string> Months()
        {
            var dictionary = new Dictionary<int?, string>();
            dictionary.Add(-1, "Month");
            foreach (var item in Enum.GetValues(typeof(Enums.Months)))
            {
                dictionary.Add((int)item, item.GetDescription());
            }
            return dictionary;
        }

        public static Dictionary<int?, string> Years()
        {
            var dictionary = new Dictionary<int?, string>();
            dictionary.Add(-1, "Year");
            for (int i = DateTime.UtcNow.Year; i >= 1950; i--)
            {
                dictionary.Add(i, i.ToString());
            }
            return dictionary;
        }

        public static Dictionary<int?, string> ExpertiseLevel()
        {
            var dictionary = new Dictionary<int?, string>();
            dictionary.Add(-1, "Select");
            foreach (var item in Enum.GetValues(typeof(Enums.ExpertiseLevel)))
            {
                dictionary.Add((int)item, item.GetDescription());
            }
            return dictionary;
        }

        public static Dictionary<int, string> OppurtunitiesLevel()
        {
            var dictionary = new Dictionary<int, string>();
            dictionary.Add(-1, "Select");
            foreach (var item in Enum.GetValues(typeof(Enums.OppurtunityLevel)))
            {
                dictionary.Add((int)item, item.GetDescription());
            }
            return dictionary;
        }

        public static Dictionary<int, string> NetworkLevel(int entityType)
        {
            if (entityType == (int)Enums.EntityTypes.Person)
            {
                var dictionary = new Dictionary<int, string>();
                dictionary.Add(-1, "Select");
                foreach (var item in Enum.GetValues(typeof(Enums.NetworkCoverageLevel)))
                {
                    dictionary.Add((int)item, item.GetDescription());
                }
                return dictionary;
            }
            else
            {
                var dictionary = new Dictionary<int, string>();
                dictionary.Add(-1, "Select");
                foreach (var item in Enum.GetValues(typeof(Enums.BusinessNetworkCoverageLevel)))
                {
                    dictionary.Add((int)item, item.GetDescription());
                }
                return dictionary;
            }
        }

        public static Dictionary<int, string> RequesterCapabilityLevel()
        {
            var dictionary = new Dictionary<int, string>();
            dictionary.Add(-1, "Select");
            foreach (var item in Enum.GetValues(typeof(Enums.RequestSenderCapabilities)))
            {
                dictionary.Add((int)item, item.GetDescription());
            }
            return dictionary;
        }


        public static Dictionary<string, string> EnumToDictionary(Type type, string value = "", string key = "", bool isStringDefault = true)
        {
            var dictionary = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(value))
                dictionary.Add(key, value);
            foreach (var item in Enum.GetValues(type))
            {
                if (!isStringDefault)
                    dictionary.Add(((int)item).ToString(), item.GetDescription());
                else
                    dictionary.Add(item.ToString(), item.GetDescription());
            }
            return dictionary;
        }

        public static Dictionary<string, string> GetQuestionTags()
        {
            throw new NotImplementedException();
        }



        public static Dictionary<int?, string> Coming30Years()
        {
            var dictionary = new Dictionary<int?, string>();
            dictionary.Add(-1, "Year");
            for (int i = DateTime.UtcNow.Year; i <= DateTime.UtcNow.AddYears(30).Year; i++)
            {
                dictionary.Add(i, i.ToString());
            }
            return dictionary;
        }

        public static Dictionary<int?, string> GetGroupTypes()
        {
            var dictionary = new Dictionary<int?, string>();
            dictionary.Add(-1, "Select");
            foreach (var item in Enum.GetValues(typeof(Enums.GroupType)))
            {
                dictionary.Add((int)item, item.GetDescription());
            }
            return dictionary;
        }
    }
}