using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrly.Models
{
    public class EmailConfigurationViewModel : BaseViewModel
    {
        public long? EmailSettingID { get; set; }
        public byte? EmailType { get; set; }
        public long? EntityID { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? Subscribed { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Optional { get; set; }
    }
}
