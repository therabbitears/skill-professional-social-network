using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Types;
using Wrly.Data.Models;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Infrastuctures.Utils;
using Wrly.Models;
using Wrly.Models.Import;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class ContactProcessor : BaseProcessor, IContactProcessor
    {

        public async Task<Result> CreateContactList(List<Models.Import.ContactViewModel> contacts, Enums.ImportType type)
        {
            var import = new EntityImport()
            {
                CreatedOn = Now,
                EntityID = UserHashObject.EntityID,
                ImportType = (int)type,
                ImportID = Guid.NewGuid().ToString()
            };

            import.EntityImportContacts = Mapper.Map<ICollection<EntityImportContact>>(contacts);
            using (var importRepository = new ImportRepository())
            {
                await importRepository.SaveImport(import);
            }
            return new Result() { Type = Enums.ResultType.Success, Description = import.ImportID };
        }


        public async Task<List<Models.Import.ContactViewModel>> GetByImportID(string id)
        {
            using (var importRepository = new ImportRepository())
            {
                using (var data = await importRepository.Get(id, UserHashObject.EntityID))
                {
                    return data.Tables[0].FromDataTable<ContactViewModel>();
                }
            }
        }


        public int Priority(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return -1;

            if (emailAddress.ToLower().Contains("gmail"))
                return 10;

            if (emailAddress.ToLower().Contains("yahoo"))
                return 9;

            if (emailAddress.ToLower().Contains("outlook") || emailAddress.ToLower().Contains("live"))
                return 8;

            if (emailAddress.ToLower().Contains("rediff"))
                return 7;

            if (emailAddress.ToLower().Contains("aol.com"))
                return 6;

            if (emailAddress.ToLower().Contains("zoho.com"))
                return 4;

            if (emailAddress.ToLower().Contains("mail.com"))
                return 3;

            if (emailAddress.ToLower().Contains("mail.com"))
                return 2;

            return 1;
        }

        public async Task<Result> Invite(IEnumerable<ContactViewModel> enumerable, long inviteID)
        {
            List<EntityImportInvite> imports = Mapper.Map<List<EntityImportInvite>>(enumerable);
            using (var importRepository = new ImportRepository())
            {
                enumerable = enumerable.OrderByDescending(c => Priority(c.EmailAddresses)).Take(10);
                await SendEmails(enumerable);
                await importRepository.SaveInvites(imports);
                return new Result() { Type = Enums.ResultType.Success, Description = string.Format("Your {0} contacts has been queued.", enumerable.Count()) };
            }
        }

        private async Task SendEmails(IEnumerable<ContactViewModel> enumerable)
        {
            var userHashViewModel = new ProfileFaceViewModel();
            using (var account = new AccountRepository())
            {
                using (var dsData = await account.EntityFace(UserHashObject.EntityID))
                {
                    userHashViewModel = dsData.Tables[0].FromDataTable<ProfileFaceViewModel>()[0];
                }
            }

            var hashTable = new Hashtable();
            foreach (var item in enumerable)
            {
                hashTable = new Hashtable();
                hashTable.Add("**Name**", string.IsNullOrEmpty(item.Name) ? item.EmailAddresses.Split(',')[0] : item.Name);
                hashTable.Add("**CurrentProfilePic**", string.Format(userHashViewModel.AuthorPhoto, 50));
                hashTable.Add("**CurrentHeading**", userHashViewModel.Heading);
                hashTable.Add("**CurrentName**", userHashViewModel.AuthorName);
                hashTable.Add("**CurrentProfileUrl**", AppConfig.SiteUrl + "/" + userHashViewModel.ProfileUrl);
                await NotificationProcessor.SendEmail(hashTable, Enums.EmailFilePaths.InviteEmail, item.EmailAddresses.Split(',')[0], true);
            }
        }

        public async Task<Result> Update(long? id, string value)
        {
            using (var importRepository = new ImportRepository())
            {
                await importRepository.UpdateName(id, value, UserHashObject.EntityID);
                return new Result() { Type = Enums.ResultType.Success, Description = string.Format("{0} invitation has been sent.") };
            }
        }


        public async Task<Result> Invite(ContactImportViewModel model)
        {
            var import = new EntityImport()
            {
                CreatedOn = Now,
                EntityID = UserHashObject.EntityID,
                ImportType = (int)Enums.ImportType.Manual,
                ImportID = Guid.NewGuid().ToString()
            };

            import.EntityImportContacts = new List<EntityImportContact>() { new EntityImportContact() { Name = model.Name, EmailAddresses = model.EmailAddress } };
            using (var importRepository = new ImportRepository())
            {
                var resultID = await importRepository.SaveImport(import);
                await SendEmails(new List<ContactViewModel>() { new ContactViewModel() { EmailAddresses = model.EmailAddress, Name = model.Name, EntityImportID = resultID, Send = true } });
            }
            return new Result() { Type = Enums.ResultType.Success, Description = import.ImportID };
        }
    }
}