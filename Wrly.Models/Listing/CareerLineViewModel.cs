using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wrly.Models.Listing
{
    public class CareerLineViewModel
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }

        public string Mode { get; set; }

        public string WhereTitle { get; set; }

        public string AsTitle { get; set; }

        public string Help { get; set; }

        public string CareelineTimeText
        {
            get
            {
                return Date.ToString("MMM, yyyy");
            }
        }


        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string CareelineStartTimeText
        {
            get
            {
                return StartDate.Value.ToString("MMM, yyyy");
            }
        }

        public string CareelineEndTimeText
        {
            get
            {
                if (EndDate != null)
                {
                    return EndDate.Value.ToString("MMM, yyyy");
                }
                return "Current";
            }
        }

    }
}