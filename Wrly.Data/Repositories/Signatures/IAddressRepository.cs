using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Data.Models;

namespace Wrly.Data.Repositories.Signatures
{
    public interface IAddressRepository
    {
        Task<int> Save(Address address);
    }
}
