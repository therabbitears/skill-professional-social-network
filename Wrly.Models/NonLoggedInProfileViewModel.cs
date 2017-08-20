using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wrly.Models
{
    public class NonLoggedInProfileViewModel : ProfileViewModel
    {
        public long? TotalAwards { get; set; }
        public long? TotalSkills { get; set; }
        public long? TotalCareerHistories { get; set; }
        public long? TotalCertifications { get; set; }
        public long? TotalAppriciations { get; set; }
        public long? TotalRecomedations { get; set; }


        public long? TotalProjects { get; set; }
        public long? TotalPublications { get; set; }
        public long? TotalResearches { get; set; }
        public long? TotalFindings { get; set; }
        public long? TotalCompositions { get; set; }
    }
}