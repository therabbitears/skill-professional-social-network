using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using Wrly.Models;
using Wrly.Models.Import;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface IContactProcessor
    {
        Task<Result> CreateContactList(List<ContactViewModel> contacts,Enums.ImportType importType);
        Task<List<ContactViewModel>> GetByImportID(string id);
        Task<Result> Invite(IEnumerable<ContactViewModel> enumerable, long inviteID);
        Task<Result> Update(long? id, string value);
        Task<Result> Invite(ContactImportViewModel model);
    }
}
