using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Data.Models;
using Wrly.Data.Repositories.Signatures;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class CommonRepository : BaseRepository, ICommonRepository
    {
        public System.Data.DataSet Countries()
        {
            var objCommand = _Database.GetStoredProcCommand("GetCountries");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Countries");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<System.Data.DataSet> Industries(string key = "")
        {
            var objCommand = _Database.GetStoredProcCommand("GetIndustries");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Key", DbType.String, key);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Industries");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public long AddEntity(Entity entity)
        {
            var objCommand = _Database.GetStoredProcCommand("AddEntity");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddOutParameter(objCommand, "@EntityID", DbType.String, int.MaxValue);
                _Database.AddInParameter(objCommand, "@EntityType", DbType.Int64, entity.EntityType);
                if (_Database.ExecuteNonQuery(objCommand) > 0)
                {
                    var entityID = Convert.ToInt64(objCommand.Parameters["@EntityID"].Value);
                    return entityID;
                }
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddEntity");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return -1;
        }

        public long AddEntity(Entity entity, DbTransaction transaction)
        {
            var objCommand = _Database.GetStoredProcCommand("AddEntity");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddOutParameter(objCommand, "@EntityID", DbType.String, int.MaxValue);
                _Database.AddInParameter(objCommand, "@EntityType", DbType.Int64, entity.EntityType);
                if (_Database.ExecuteNonQuery(objCommand, transaction) > 0)
                {
                    var entityID = Convert.ToInt64(objCommand.Parameters["@EntityID"].Value);
                    return entityID;
                }
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddEntity");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return -1;
        }

        public async Task<long> AddActivity(NetworkActivity activity)
        {
            var objCommand = _Database.GetStoredProcCommand("AddNetworkActivity");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, activity.Type);
                _Database.AddInParameter(objCommand, "@Years", DbType.Int32, activity.Years);
                _Database.AddInParameter(objCommand, "@AwardID", DbType.Int64, activity.AwardID);
                _Database.AddInParameter(objCommand, "@CareerHistoryID", DbType.Int64, activity.CareerHistoryID);
                _Database.AddInParameter(objCommand, "@SkillID", DbType.Int64, activity.SkillID);
                _Database.AddInParameter(objCommand, "@ActionTaken", DbType.Boolean, activity.ActionTaken);
                _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, activity.IpAddress);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, activity.EntityID);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, activity.CreatedOn);
                _Database.AddInParameter(objCommand, "@Identifier", DbType.String, activity.Identifier);
                _Database.AddInParameter(objCommand, "@ForEntity", DbType.Int64, activity.ForEntity);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddEntity");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}
