using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Data.Models;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class NotificationRepository : BaseRepository
    {
        public async Task<bool> Save(Notificaction notification)
        {
            var objCommand = _Database.GetStoredProcCommand("InsertNotificationSubscriber");
            if (_SqlTransaction == null)
                StartTransaction();
            try
            {
                var notificationID = await AddNotification(notification);
                if (notificationID > 0)
                {
                    objCommand.CommandTimeout = Constants.TIMEOUT;
                    foreach (var item in notification.NotificationSubscribers)
                    {
                        objCommand = _Database.GetStoredProcCommand("InsertNotificationSubscriber");
                        _Database.AddInParameter(objCommand, "@NotificationID", DbType.Int64, notificationID);
                        _Database.AddInParameter(objCommand, "@EntityID", DbType.Int16, item.EntityID);
                        _Database.AddInParameter(objCommand, "@Status", DbType.Int16, item.Status);
                        _Database.AddInParameter(objCommand, "@Subscribed", DbType.Boolean, item.Subscribed);
                        _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                    }
                }
                else
                {
                    RollbackTransaction();
                    return false;
                }
                CommitTransaction();
                return true;

            }
            catch (Exception ex)
            {
                RollbackTransaction();
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddExtendedInfo");
                return false;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private async Task<long> AddNotification(Notificaction notification)
        {
            var objCommand = _Database.GetStoredProcCommand("InsertNotification");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddOutParameter(objCommand, "@NotificationID", DbType.Int64, int.MaxValue);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, notification.CreatedBy);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, notification.CreatedOn);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, notification.EditedBy);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, notification.EditedOn);
                _Database.AddInParameter(objCommand, "@NewsID", DbType.Int64, notification.NewsID);
                _Database.AddInParameter(objCommand, "@EntitySkillID", DbType.Int64, notification.EntitySkillID);
                _Database.AddInParameter(objCommand, "@AwardID", DbType.Int64, notification.AwardID);
                _Database.AddInParameter(objCommand, "@ReferenceID", DbType.Int64, notification.ReferenceID);
                _Database.AddInParameter(objCommand, "@ReferenceEntityID", DbType.Int64, notification.ReferenceEntityID);
                _Database.AddInParameter(objCommand, "@ActivityID", DbType.Int64, notification.ActivityID);
                _Database.AddInParameter(objCommand, "@Text", DbType.String, notification.Text);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, notification.Type);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, notification.EntityID);
                _Database.AddInParameter(objCommand, "@Updatable", DbType.Boolean, notification.Updatable);
                _Database.AddInParameter(objCommand, "@Url", DbType.String, notification.Url);
                if (_Database.ExecuteNonQuery(objCommand, _SqlTransaction) > 0)
                    return Convert.ToInt64(objCommand.Parameters["@NotificationID"].Value);
                return -1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddBasicSubscription");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> Get(int? status, long entityID, int pageNo, int pageSize)
        {
            //var objCommand = _Database.GetStoredProcCommand("Get_Notifications");
            var objCommand = _Database.GetStoredProcCommand("Get_Notifications_1");
            //objCommand.CommandTimeout = Constants.TIMEOUT;
            //try
            //{
            //    _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
            //    _Database.AddInParameter(objCommand, "@Status", DbType.Int16, status);
            //    _Database.AddInParameter(objCommand, "@PageNo", DbType.Int16, pageNo);
            //    _Database.AddInParameter(objCommand, "@PageSize", DbType.Int16, pageSize);
            //    return _Database.ExecuteDataSet(objCommand);
            //}
            //catch (Exception ex)
            //{
            //    ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddBasicSubscription");
            //    return null;
            //}
            //finally
            //{
            //    if (objCommand != null) { objCommand.Dispose(); }
            //}

            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                //_Database.AddInParameter(objCommand, "@Status", DbType.Int16, status);
                _Database.AddInParameter(objCommand, "@PageNumber", DbType.Int16, pageNo);
                _Database.AddInParameter(objCommand, "@PageSize", DbType.Int16, pageSize);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddBasicSubscription");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> Acknowledge(long? notificationID, long entityID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Acknowledge_Notifications");
            try
            {
                _Database.AddInParameter(objCommand, "@NotificationID", DbType.Int64, notificationID);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Acknowledge");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}
