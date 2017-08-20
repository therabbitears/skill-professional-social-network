using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models;
using Wrly.Models.Import;

namespace Wrly.Controllers
{
    //[Authorize]
    public class GoogleController : BaseController
    {
        private IContactProcessor _ContactProcessor;

        public IContactProcessor ContactProcessor
        {
            get
            {
                if (_ContactProcessor == null)
                {
                    _ContactProcessor = new ContactProcessor();
                }
                return _ContactProcessor;

            }
        }


        // GET: Import
        public Task<ActionResult> Index()
        {
            if (Request.QueryString["code"] != null)
                GetAccessToken();

            return null;
        }


        public async Task<ActionResult> GetContacts()
        {
            if (Request.QueryString["code"] != null)
            {
                var result = await GetAccessToken();
                return RedirectToAction("Import", "Import", new { id = result.Description });
            }

            /*https://developers.google.com/google-apps/contacts/v3/
              https://developers.google.com/accounts/docs/OAuth2WebServer
              https://developers.google.com/oauthplayground/
            */
            string clientId = "1008964632872-q1h8h70nlg74v5jpuiflpjm8be52i6b1.apps.googleusercontent.com";
            string redirectUrl = "https://www.sklative.com/Google/GetContacts";
            Response.Redirect("https://accounts.google.com/o/oauth2/auth?redirect_uri=" + redirectUrl + "&response_type=code&client_id=" + clientId + "&scope=https://www.google.com/m8/feeds/&approval_prompt=force&access_type=offline");

            return null;
        }
        public async Task<Result> GetAccessToken()
        {
            string code = Request.QueryString["code"];
            string google_client_id = "1008964632872-q1h8h70nlg74v5jpuiflpjm8be52i6b1.apps.googleusercontent.com";
            string google_client_sceret = "Fg4CJFtXYuCN-DpflCu2Zdzb";
            string google_redirect_url = "https://www.sklative.com/Google/GetContacts";

            /*Get Access Token and Refresh Token*/
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/o/oauth2/token");
            webRequest.Method = "POST";
            string parameters = "code=" + code + "&client_id=" + google_client_id + "&client_secret=" + google_client_sceret + "&redirect_uri=" + google_redirect_url + "&grant_type=authorization_code";
            byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = byteArray.Length;
            Stream postStream = webRequest.GetRequestStream();
            // Add the post data to the web request
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();
            WebResponse response = webRequest.GetResponse();
            postStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(postStream);
            string responseFromServer = reader.ReadToEnd();
            GooglePlusAccessToken serStatus = JsonConvert.DeserializeObject<GooglePlusAccessToken>(responseFromServer);
            /*End*/
            return await RetContact(serStatus);
        }

        public async Task<Result> RetContact(GooglePlusAccessToken serStatus)
        {
            string google_client_id = "1008964632872-q1h8h70nlg74v5jpuiflpjm8be52i6b1.apps.googleusercontent.com";
            string google_client_sceret = "Fg4CJFtXYuCN-DpflCu2Zdzb";
            /*Get Google Contacts From Access Token and Refresh Token*/
            string refreshToken = serStatus.refresh_token;
            string accessToken = serStatus.access_token;
            string scopes = "https://www.google.com/m8/feeds/contacts/default/full/";
            OAuth2Parameters oAuthparameters = new OAuth2Parameters()
            {
                ClientId = google_client_id,
                ClientSecret = google_client_sceret,
                RedirectUri = "https://www.sklative.com/Google/GetContacts",
                Scope = scopes,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };


            RequestSettings settings = new RequestSettings("<var>BTree</var>", oAuthparameters);
            ContactsRequest cr = new ContactsRequest(settings);
            ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri("default"));
            query.NumberToRetrieve = 5000;
            Feed<Contact> feed = cr.Get<Contact>(query);

            var listCotacts = new List<ContactViewModel>();
            StringBuilder sb = new StringBuilder();
            int i = 1;
            ContactViewModel contact = null;
            foreach (Contact entry in feed.Entries.Where(c => c.Emails != null && c.Emails.Count > 0))
            {
                contact = new ContactViewModel()
                {
                    Name = entry.Name != null && entry.Name.FullName != null ? entry.Name.FullName : entry.Name != null && entry.Name.FamilyName != null ? entry.Name.FamilyName : string.Empty,
                    EmailList = new List<string>()
                };

                foreach (EMail email in entry.Emails)
                {
                    contact.EmailList.Add(email.Address);
                }

                if (!string.IsNullOrEmpty(entry.PhotoEtag))
                {
                    // DownloadPhoto(cr, new Uri(Url.Encode(entry.PhotoUri.ToString())));
                }
                listCotacts.Add(contact);
            }

            var result = await ContactProcessor.CreateContactList(listCotacts, Enums.ImportType.Google);
            return result;
        }


        public static void DownloadPhoto(ContactsRequest cr, Uri contactURL)
        {
            Contact contact = cr.Retrieve<Contact>(contactURL);

            Stream photoStream = cr.GetPhoto(contact);
            FileStream outStream = System.IO.File.OpenWrite("test.jpg");
            byte[] buffer = new byte[photoStream.Length];

            photoStream.Read(buffer, 0, (int)photoStream.Length);
            outStream.Write(buffer, 0, (int)photoStream.Length);
            photoStream.Close();
            outStream.Close();
        }
    }


    /// <summary>
    /// Summary description for GoogleContactsApi
    /// </summary>
    public class GoogleContactsApi
    {
        public GoogleContactsApi()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
    public class GooglePlusAccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }
    //public class GoogleUserOutputData
    //{
    //    public string id { get; set; }
    //    public string name { get; set; }
    //    public string given_name { get; set; }
    //    public string email { get; set; }
    //    public string picture { get; set; }
    //}
}