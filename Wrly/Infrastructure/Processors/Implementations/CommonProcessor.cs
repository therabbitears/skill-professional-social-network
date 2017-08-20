using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Wrly.Data.Repositories.Implementors;
using Wrly.Data.Repositories.Signatures;
using Wrly.Infrastructure.Processors.Structures;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class CommonProcessor : ICommonDataProcessor
    {
        private ICommonRepository _CommonRepository;

        public ICommonRepository CommonRepository
        {
            get
            {
                if (_CommonRepository == null)
                {
                    _CommonRepository = new CommonRepository();
                }
                return _CommonRepository;
            }
        }

        public Dictionary<string, string> Countries()
        {
            DataSet dsCountrie = CommonRepository.Countries();
            return (from country in dsCountrie.Tables[0].AsEnumerable() select new { Key = Convert.ToString(country["Name"]), Value = Convert.ToString(country["Name"]) }).ToDictionary(Key => Key.Key, Value => Value.Value);
        }

        public async Task<Dictionary<int, string>> Industries()
        {
            DataSet dsIndustries=await CommonRepository.Industries();
            return (from country in dsIndustries.Tables[0].AsEnumerable() select new { Key = Convert.ToInt32(country["IndustryID"]), Value = Convert.ToString(country["IndustryName"]) }).ToDictionary(Key => Key.Key, Value => Value.Value);
        }
    }
}