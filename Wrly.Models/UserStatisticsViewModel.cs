using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wrly.Models
{
    public class UserStatisticsViewModel
    {
        public int PendingMessages { get; set; }
        public int PendingNotifications { get; set; }
        public int PendingRequests { get; set; }
    }
}