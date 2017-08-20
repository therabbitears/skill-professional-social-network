using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Wrly.Infrastuctures.Utils;
using Wrly.infrastuctures.Utils;

namespace Wrly.Models
{
    public partial class BaseViewModel : SharedBaseViewModel
    {      

        [ScriptIgnore]
        [JsonIgnore]
        public string UserHash
        {
            get
            {
                if (SessionInfo.UserHash != null)
                {
                    var userHash = SessionInfo.UserHash;
                    var table = new Hashtable();
                    table.Add("PersonID", userHash.PersonID);
                    table.Add("EntityID", userHash.EntityID);
                    table.Add("UserID", userHash.UserID);
                    return QueryStringHelper.Encrypt(table);
                }
                return string.Empty;
            }
        }
        [ScriptIgnore]
        [JsonIgnore]
        public UserHash UserHashObject
        {
            get
            {
                return SessionInfo.UserHash;
            }
        }
    }

    public class ProfileHashViewModel
    {
        public string FullName { get; set; }
        public long EntityID { get; set; }
        public long PersonID { get; set; }
    }
}