using LuceneSearch.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Types;
using Wrly.Data.Models.Extended;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.infrastuctures.Utils;
using Wrly.Models;

namespace Wrly.Infrastructure.Utils
{
    public static class CommonFunctions
    {
        internal static void SendWelcomeEmail(Models.RegisterViewModel model)
        {
            return;
        }

        internal static void SendWelcomeEmailToBusiness(Models.NewOrganizationViewModel model)
        {
            return;
        }

        internal static System.Web.Mvc.SelectList ReadFromType(Type type)
        {
            throw new NotImplementedException();
        }

        internal static void SendOrganizationWelcomeEmail(Models.OrganizationSignupViewModel model)
        {
            return;
        }

        internal static bool SendForgotPasswordEmail(Models.ProfileViewModel profile)
        {
            var hashTable = new Hashtable();
            hashTable.Add("FormatedName", profile.FormatedName);
            hashTable.Add("EmailAddress", profile.EmailAddress);
            hashTable.Add("EntityID", profile.EntityID);
            hashTable.Add("PersonID", profile.PersonID);
            hashTable.Add("UserID", profile.UserID);
            hashTable.Add("Ticks", DateTime.UtcNow.Ticks);
            hashTable.Add("Hash", profile.Hash);
            var varificationHash = QueryStringHelper.Encrypt(hashTable);
            hashTable.Clear();
            hashTable.Add("**Hash**", varificationHash);
            hashTable.Add("**UserName**", profile.FormatedName);
            var result = NotificationProcessor.SendEmail(hashTable, Enums.EmailFilePaths.ForgottenPassword, profile.EmailAddress);
            return result.Result == Enums.ResultType.Success;
        }

        internal static async Task<int> UpdateUserFaceIndex(long entityID, int type)
        {
            using (var repository = new AccountRepository())
            {
                using (var dsSettings = await repository.EntityFace(entityID))
                {
                    var model = dsSettings.Tables[0].FromDataTable<ProfileFaceViewModel>().FirstOrDefault();
                    var luecenIndex = new LuceneObject()
                    {
                        DisplayName = model.AuthorName,
                        Headiing = model.Heading ?? string.Empty,
                        EntityType = type,
                        EntityID = entityID,
                        ProfilePicUrl = model.AuthorPhoto ?? string.Empty,
                        Url = model.ProfileUrl ?? string.Empty,
                        WorkHistoryText = model.WorkHistoryText ?? string.Empty,
                        SkillText = model.SkillText ?? string.Empty,
                        LastModified = Now,
                        EducationHistoryText = model.EducationHistoryText ?? string.Empty
                    };
                    GoLucene.AddUpdateLuceneIndex(luecenIndex);
                }
            }
            return 1;
        }

        public static DateTime Now { get { return DateTime.UtcNow; } }

        public static byte[] CropImage(byte[] bytes, decimal x, decimal y, decimal width, decimal height)
        {
            Bitmap bmp = null;
            using (var ms = new System.IO.MemoryStream(bytes))
            {
                using (var img = Image.FromStream(ms))
                {
                    Rectangle crop = new Rectangle((int)x, (int)y, (int)width, (int)height);

                    bmp = new Bitmap(crop.Width, crop.Height);
                    using (var gr = Graphics.FromImage(bmp))
                    {
                        gr.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
                        using (var msResult = new System.IO.MemoryStream(bytes))
                        {
                            bmp.Save(msResult, img.RawFormat);
                            return msResult.ToArray();
                        }
                    }
                }
            }
        }
    }


}