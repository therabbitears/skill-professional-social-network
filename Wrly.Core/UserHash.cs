using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wrly.Models
{
    public class UserHash
    {
        public long PersonID { get; set; }

        public long OrganizationID { get; set; }

        public int EntityType { get; set; }

        public long EntityID { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }
    }
}