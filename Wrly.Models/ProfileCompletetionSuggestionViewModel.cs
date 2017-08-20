using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrly.Models
{
    public class ProfileCompletetionSuggestionViewModel
    {
        public long TotalSkills { get; set; }
        public long TotalCareerHistories { get; set; }
        public long TotalConnections { get; set; }
        public long TotalProjects { get; set; }
        public string ProfileCoverExist { get; set; }
        public string ProfilePicExist { get; set; }
        public string IsSummaryAdded { get; set; }
        public decimal Percentage { get; set; }
    }
}
