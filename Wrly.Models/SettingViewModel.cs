using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wrly.Models
{
    public class SettingViewModel
    {
        public GeneralSettingViewModel General { get; set; }
        public NetworkSettingViewModel Network { get; set; }
        public PrivacySettingViewModel Privacy { get; set; }
        public JobSearchViewModel JobSearch { get; set; }
    }
}