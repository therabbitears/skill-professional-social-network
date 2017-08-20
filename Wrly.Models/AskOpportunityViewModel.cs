using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrly.Models
{
    public class AskOpportunityViewModel : SharedBaseViewModel
    {
        [Required(ErrorMessage="The text cannot be left blank.")]
        public string Text { get; set; }
        [Required(ErrorMessage = "Select a connection name for this opportunity.")]
        public string ConnectionName { get; set; }
        public long ConnectionID { get; set; }
        [Required(ErrorMessage = "Select at least one skill for this opportunity.")]
        public List<int> Skills { get; set; }
        [Required(ErrorMessage = "Select at least one job title for this opportunity.")]
        public List<int> JobTitles { get; set; }
        
        public int OpportunitySource { get; set; }
        
        public string AvailableInDays { get; set; }

        public string GroupHash { get; set; }
    }
}
