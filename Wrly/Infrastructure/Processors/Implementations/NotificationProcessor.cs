using Dialer.Mail.Infrastructure.Implementors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mail;
using Types;
using Wrly.Infrastuctures.Utils;
using Wrly.Utils;
using System.Threading;
using System.Threading.Tasks;
using SendGrid;
using Wrly.Models;
using System.Xml.Serialization;
using Wrly.infrastuctures.Utils;

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class NotificationProcessor
    {
        internal async static Task<Enums.ResultType> SendEmail(Hashtable objHashtable, Enums.EmailFilePaths path, string email, bool processSubject = false)
        {
            if (AppConfig.IsLiveMode)
            {
                try
                {
                    var emailData = GetDate(path);
                    AdStaticValues(objHashtable, path, email);
                    var emailStructure = File.ReadAllText(HttpContext.Current.Server.MapPath(emailData.FilePath));
                    var objMailInfo = new MailInfo();
                    emailStructure = objMailInfo.ProcessEmailBody(objHashtable, emailStructure);
                    if (processSubject)
                    {
                        emailData.Subject = objMailInfo.ProcessEmailBody(objHashtable, emailData.Subject);
                    }
                    await SendEmail(email, emailStructure, emailData.Subject, emailData.FromName, emailData.FromEmail);
                    return Enums.ResultType.Success;
                }
                catch
                {
                    return Enums.ResultType.Error;
                }
            }
            return Enums.ResultType.Success;
        }

        private static string GetUnsubscribeLink(Enums.EmailFilePaths path, string email)
        {
            var hashTable = new Hashtable();
            hashTable.Add("Type", (int)path);
            hashTable.Add("Email", email);
            hashTable.Add("GeneratedOn", DateTime.Now);
            return QueryStringHelper.Encrypt(hashTable);
        }

        private static EmailParserSetting GetDate(Enums.EmailFilePaths path)
        {
            var filePath = HttpContext.Current.Server.MapPath(path.GetDescription());
            var serializer = new XmlSerializer(typeof(EmailParserSetting));
            using (StreamReader reader = new StreamReader(filePath))
            {
                var model = (EmailParserSetting)serializer.Deserialize(reader);
                reader.Close();
                serializer = null;
                return model;
            }
        }

        private static void AdStaticValues(Hashtable objHashtable, Enums.EmailFilePaths path, string email)
        {
            if (!objHashtable.ContainsKey("**Year**"))
                objHashtable.Add("**Year**", (DateTime.UtcNow.Year - 1).ToString() + " - " + DateTime.UtcNow.Year.ToString());

            string unsubsctibeLink = GetUnsubscribeLink(path, email);
            objHashtable.Add("**UnsubscribeUrl**", unsubsctibeLink);
        }

        static async Task SendEmail(string emailAddress, string emailStructure, string subject, string senderName, string senderEmailAddress)
        {
            if (AppConfig.UseSendGrid)
            {
                using (var mail = new System.Net.Mail.MailMessage())
                {
                    mail.From = new MailAddress(senderEmailAddress, senderName);
                    mail.To.Add(new MailAddress(emailAddress));
                    mail.Subject = subject;
                    mail.Body = emailStructure;
                    mail.IsBodyHtml = true;
                    // Can set to false, if you are sending pure text.

                    using (SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", 25))
                    {
                        smtp.Credentials = new NetworkCredential("banshi003", "Admin@123");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            else
            {
                var emailSettings = GetEmailSettings();
                string senderID = emailSettings.EmailAddress;// use sender’s email id here..
                string senderPassword = ValueEncryptionHelper.Decrypt(emailSettings.Password, string.Empty); // sender password here…
                try
                {
                    using (var mail = new System.Net.Mail.MailMessage())
                    {
                        mail.From = new MailAddress(senderID, senderName);
                        mail.To.Add(emailAddress);
                        mail.Subject = subject;
                        mail.Body = emailStructure;
                        mail.IsBodyHtml = true;
                        // Can set to false, if you are sending pure text.

                        using (SmtpClient smtp = new SmtpClient(emailSettings.SMTP, emailSettings.Port))
                        {
                            smtp.Credentials = new NetworkCredential(senderID, senderPassword);
                            smtp.EnableSsl = emailSettings.EnableSSL;
                            smtp.SendAsync(mail, null);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        static EmailServerSetting GetEmailSettings()
        {
            return new EmailServerSetting()
            {
                EmailAddress = "babygomz.web@gmail.com",
                SMTP = "smtp.gmail.com",
                EnableSSL = true,
                Password = "LH6spuQCmAIPRudVzFjbZJ0T6PvpbnTu",
                Port = 587,
            };
        }
    }
}