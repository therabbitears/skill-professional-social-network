using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class ChatRepository : BaseRepository
    {
        public async Task<DataSet> GetGroupChatHistory(long group, int page, int pageSize)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("ChatGroupMessages");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@GroupID", DbType.Int64, group);
                _Database.AddInParameter(objCommand, "@Page", DbType.Int64, page);
                _Database.AddInParameter(objCommand, "@PageSize", DbType.Int64, pageSize);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetGroupChatHistory");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> FindOrCreateGroupByUsers(long currentEntity, long anotherEntity)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("FindOrCreateGroup");
            try
            {
                _Database.AddOutParameter(objCommand, "@GroupID", DbType.Int64, int.MaxValue);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, currentEntity);
                _Database.AddInParameter(objCommand, "@EntityID2", DbType.Int64, anotherEntity);
                _Database.ExecuteNonQuery(objCommand);
                var groupID = Convert.ToInt64(_Database.GetParameterValue(objCommand, "@GroupID"));
                return await GroupDetail(groupID);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "FindOrCreateGroupByUsers");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private async Task<DataSet> GroupDetail(long groupID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_GroupDetail");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@GroupID", DbType.Int64, groupID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ForUser");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> InsertMessageToGroup(Models.ChatGroupMessage groupMessage)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("InsertMessage");
            try
            {
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, groupMessage.CreatedOn);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, groupMessage.EntityID);
                _Database.AddInParameter(objCommand, "@GroupID", DbType.Int64, groupMessage.GroupID);
                _Database.AddInParameter(objCommand, "@Message", DbType.String, groupMessage.Message);
                _Database.AddInParameter(objCommand, "@MessageType", DbType.Int16, groupMessage.MessageType);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int16, groupMessage.Status);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "InsertMessageToGroup");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetChatFace(long entityID, int page, int pageSize, long? groupID = null)
        {
            if (groupID > 0)
            {
                var objCommand = _Database.GetStoredProcCommand("Get_GroupMessageFaceForGroup");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                try
                {
                    _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                    _Database.AddInParameter(objCommand, "@Page", DbType.Int16, page);
                    _Database.AddInParameter(objCommand, "@PageSize", DbType.Int16, pageSize);
                    _Database.AddInParameter(objCommand, "@groupID", DbType.Int64, groupID);
                    return _Database.ExecuteDataSet(objCommand);
                }
                catch (Exception ex)
                {
                    ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetChatFace");
                    return null;
                }
                finally
                {
                    if (objCommand != null) { objCommand.Dispose(); }
                }
            }
            else
            {
                var objCommand = _Database.GetStoredProcCommand("Get_GroupMessageFace");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                try
                {
                    _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                    _Database.AddInParameter(objCommand, "@Page", DbType.Int16, page);
                    _Database.AddInParameter(objCommand, "@PageSize", DbType.Int16, pageSize);
                    return _Database.ExecuteDataSet(objCommand);
                }
                catch (Exception ex)
                {
                    ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetChatFace");
                    return null;
                }
                finally
                {
                    if (objCommand != null) { objCommand.Dispose(); }
                }
            }
        }


        public async Task<int> UpdateReadStatus(long groupID, int status, long? entityID=null)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("UpdateGroupChatMessageStatus");
            try
            {
                _Database.AddInParameter(objCommand, "@GroupID", DbType.Int64, groupID);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int16, status);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "InsertMessageToGroup");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> Acknowledge(long? groupID, long entityID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Acknowledge_Messages");
            try
            {
                _Database.AddInParameter(objCommand, "@GroupID", DbType.Int64, groupID);
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
