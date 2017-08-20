using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wrly.Models.Feeds
{
    public class FeedInsightsViewModel
    {
        public int TotalViews { get; set; }
        public int TotalImpressions { get; set; }
        public int TotalReferals { get; set; }
        public int TotalReplies { get; set; }
        public int TotalApplications { get; set; }
        public int TotalReferalsForJobLooking { get; set; }
    }
}
