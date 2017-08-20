using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface ICommonDataProcessor
    {
        Dictionary<string, string> Countries();
        Task<Dictionary<int, string>> Industries();
    }
}
