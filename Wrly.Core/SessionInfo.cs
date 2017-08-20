using System.Web;
using Wrly.Data.Repositories.Implementors;
using Wrly.Models;
namespace Wrly.Infrastuctures.Utils
{
    public static class SessionInfo
    {
        const string USER_HASH_KEY = "___UserHash";
        public static UserHash UserHash
        {
            get
            {
                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    if (HttpContext.Current.Session[USER_HASH_KEY] == null)
                    {
                        using (AccountRepository repository = new AccountRepository())
                        {
                            using (var objectDataSet = repository.GetUserHashData(HttpContext.Current.User.Identity.Name))
                            {
                                HttpContext.Current.Session[USER_HASH_KEY] = objectDataSet.Tables[0].FromDataTable<UserHash>()[0];
                            }
                        }
                    }
                    return HttpContext.Current.Session[USER_HASH_KEY] as UserHash;
                }
                return null;
            }
        }


        /// <summary>
        /// Sets the value into Current Session 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strKey"></param>
        /// <param name="objValue"></param>
        public static void Set<T>(string strKey, T objValue)
        {
            HttpContext.Current.Session[strKey] = objValue;
        }








        /// <summary>
        /// Returns a generic type from Current Session
        /// </summary>
        /// <param name="strKey">Key of the Session</param>
        /// <returns></returns>
        public static T Get<T>(string strKey)
        {
            return (T)HttpContext.Current.Session[strKey];
        }

        public static void RemoveAuthCookies()
        {
            HttpCookie cookies = HttpContext.Current.Response.Cookies["_AuthWebUser"];
            if (cookies != null && cookies.Value != null)
            {
                cookies.Value = null;
                HttpContext.Current.Response.Cookies.Add(cookies);
            }
        }      
    }
}


