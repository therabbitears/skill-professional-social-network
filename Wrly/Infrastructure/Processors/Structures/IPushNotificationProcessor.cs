using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Models;
using Wrly.Models.Listing;

namespace Wrly.Infrastructure.Processors.Structures
{
    public interface IPushNotificationProcessor
    {
        Task<List<NotificationViewModel>> Get(int pageNo, int pageSize);
        Task<Wrly.Models.Result> Acknowledge(long? notificationID=null);
    }
}
