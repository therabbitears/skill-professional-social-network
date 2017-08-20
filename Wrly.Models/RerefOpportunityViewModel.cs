using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Models.Feeds;

namespace Wrly.Models
{
    public class RerefOpportunityViewModel : SharedBaseViewModel
    {
        [Required(ErrorMessage = "Select a connection name for this opportunity.")]
        public string ConnectionName { get; set; }
        public long? ConnectionID { get; set; }
        public FeedDetailViewModel Opportunity { get; set; }
        public string Text { get; set; }
    }
}
