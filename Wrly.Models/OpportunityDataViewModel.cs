using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrly.Models.Listing;

namespace Wrly.Models
{
    public class OpportunityDataViewModel
    {
        public List<CareerHistoryViewModel> Careers { get; set; }
        public List<SkillViewModel> Skills { get; set; }
    }
}