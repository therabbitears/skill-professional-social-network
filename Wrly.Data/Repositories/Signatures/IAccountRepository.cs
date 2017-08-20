using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Data.Models;

namespace Wrly.Data.Repositories.Signatures
{
    public interface IAccountRepository
    {
        Task<DataSet> GetProfileWithStates(string userName, bool isProfileName, long entityID);
        Task<DataSet> GetProfile(string userName, bool userID=false);
        bool IsUserProfileNameExist(string profileName);
        long AddExtendedInfo(Person extended);
        int UpdateProfile(string query);

        Task<DataSet> GetOpenProfile(string profileName);
    }
}
