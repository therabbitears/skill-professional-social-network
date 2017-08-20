using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrly.Data.Repositories.Signatures
{
    public interface ICommonRepository
    {
        System.Data.DataSet Countries();

        Task<System.Data.DataSet> Industries(string key = "");
    }
}
