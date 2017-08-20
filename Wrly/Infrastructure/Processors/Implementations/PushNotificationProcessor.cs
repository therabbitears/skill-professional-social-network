using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Types;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models;
using Wrly.Models.Listing;  

namespace Wrly.Infrastructure.Processors.Implementations
{
    public class PushNotificationProcessor : BaseProcessor, IPushNotificationProcessor
    {
        public async Task<List<NotificationViewModel>> Get(int pageNo, int pageSize)
        {
            if (pageNo == 0)
            {
                using (AccountRepository repository = new AccountRepository())
                {
                    await repository.SetLastNotificationSeenData(Now, UserHashObject.EntityID);
                }
            }
            using (var repository = new NotificationRepository())
            {
                using (var dsNotifications = await repository.Get(null, UserHashObject.EntityID, pageNo, pageSize))
                {
                    return dsNotifications.Tables[0].FromDataTable<NotificationViewModel>();
                }
            }
        }


        public async Task<Models.Result> Acknowledge(long? notificationID = null)
        {
            using (var repository = new NotificationRepository())
            {
                var result = await repository.Acknowledge(notificationID, UserHashObject.EntityID);
                if (result > 0)
                    return new Models.Result() { Type = Enums.ResultType.Success, Description = "All the notification has been marked as Acknowledged.", ReferenceID = notificationID };
                else
                    return new Models.Result() { Type = Enums.ResultType.Error, Description = "Error while processing the requuest, please make another try.", ReferenceID = notificationID };
            }
        }
    }
}