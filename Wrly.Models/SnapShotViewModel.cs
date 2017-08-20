using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wrly.Models
{
    public class SnapShotViewModel
    {
        public int ProfileLevel { get; set; }
        
        public bool IsEndDefined { get; set; }
        public int WizardStep { get; set; }

        public string FormattedName { get; set; }
        public string ProfilePath { get; set; }
        public string ProfileName { get; set; }
        public string ProfileHeading { get; set; }


        public string Name { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public string LogoPath { get; set; }
        
        public int EntityType { get; set; }
    }
    public class SkillSnapShotViewModel:BaseViewModel
    {
        public SnapShotViewModel Stats { get; set; }
        public List<SkillViewModel> Skills { get; set; }
        public ProfileViewModel ProfileSnap { get; set; }
    }
}