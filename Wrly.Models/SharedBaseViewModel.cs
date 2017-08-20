using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Wrly.Models
{
    public class SharedBaseViewModel
    {
        DateTime _CreatedOn;
        [JsonIgnore]
        public DateTime CreatedOn
        {
            get
            {
                if (_CreatedOn == DateTime.MinValue)
                {
                    return DateTime.UtcNow;
                }
                else
                {
                    return _CreatedOn;
                }
            }
            set
            {
                _CreatedOn = value;
            }
        }
        [JsonIgnore]
        public DateTime EditedOn { get { return DateTime.UtcNow; } }
        [JsonIgnore]
        public string CreatedBy { get { return HttpContext.Current.User.Identity.Name; } }
        [JsonIgnore]
        public string EditedBy { get { return HttpContext.Current.User.Identity.Name; } }

        [JsonIgnore]
        public string Hash { get; set; }

    }
}
