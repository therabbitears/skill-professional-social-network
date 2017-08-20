using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class SettingsRepository : BaseRepository
    {
        public async Task<long> SaveProfileName(string profileName, long personID)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("Setting_ProfileName");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddOutParameter(objCommand, "@Result", DbType.Int32, int.MaxValue);
                _Database.AddInParameter(objCommand, "@ProfileName", DbType.String, profileName);
                _Database.AddInParameter(objCommand, "@PersonID", DbType.Int64, personID);
                _Database.ExecuteNonQuery(objCommand);
                return Convert.ToInt64(objCommand.Parameters["@Result"].Value);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddProfileInfo");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }

        }

        public async Task<long> SaveBusinessProfileName(string profileName, long organizationID)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("Setting_BusinessProfileName");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddOutParameter(objCommand, "@Result", DbType.Int32, int.MaxValue);
                _Database.AddInParameter(objCommand, "@ProfileName", DbType.String, profileName);
                _Database.AddInParameter(objCommand, "@organizationID", DbType.Int64, organizationID);
                _Database.ExecuteNonQuery(objCommand);
                return Convert.ToInt64(objCommand.Parameters["@Result"].Value);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SaveBusinessProfileName");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }

        }

        public async Task<long> SaveSearchEngineVisibility(bool enabled, long entityID)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("Setting_SearchEngineVisibility");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@Enabled", DbType.Boolean, enabled);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SaveSearchEngineVisibility");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> SaveNetworkSettings(Dictionary<string, string> values, long entityID)
        {
            DbCommand objCommand = null;
            try
            {
                if (_SqlTransaction == null)
                    StartTransaction();
                string sql = "Update NetworkSettings set";
                int count = 1;
                foreach (var item in values)
                {
                    if (count == values.Count)
                        sql += string.Format(" {0}=@{1}", item.Key, item.Key);
                    else
                        sql += string.Format(" {0}=@{1},", item.Key, item.Key);
                    count++;
                }

                sql += " where EntityID=@EntityID";
                objCommand = _Database.GetSqlStringCommand(sql);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                foreach (var item in values)
                {
                    _Database.AddInParameter(objCommand, "@" + item.Key, DbType.String, item.Value);
                }
                _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                CommitTransaction();
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetCareerLine");
                RollbackTransaction();
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> SetAllowReference(bool enabled, long entityID)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("Setting_AllowReference");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@Enabled", DbType.Boolean, enabled);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SetAllowReference");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> SetJobAppurtunities(bool enabled, long entityID)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("Setting_SetOppurtunities");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@Enabled", DbType.Boolean, enabled);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SetJobAppurtunities");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> SetOppurtunityLevel(int level, long entityID)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("Setting_OppurtunityLevel");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@Value", DbType.Int32, level);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SetOppurtunityLevel");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetSetting(long entityID, Enums.SettingType settingType)
        {
            DbCommand objCommand = null;
            try
            {
                switch (settingType)
                {
                    case Enums.SettingType.JobSearch:
                        objCommand = _Database.GetStoredProcCommand("Setting_JobSearch");
                        break;
                    case Enums.SettingType.General:
                        objCommand = _Database.GetStoredProcCommand("Setting_General");
                        break;
                    case Enums.SettingType.Network:
                        objCommand = _Database.GetStoredProcCommand("Setting_Network");
                        break;
                    case Enums.SettingType.Privacy:
                        objCommand = _Database.GetStoredProcCommand("Setting_Privacy");
                        break;
                    case Enums.SettingType.Widget:
                        objCommand = _Database.GetStoredProcCommand("Get_EntityWidget");
                        break;
                    case Enums.SettingType.All:
                        objCommand = _Database.GetStoredProcCommand("Get_Settings");
                        break;
                }

                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetSetting");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> SetWidgetStatus(Models.EntityWidget entityWidget)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("Setting_WidgetSubscription");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@Active", DbType.Boolean, entityWidget.Active);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityWidget.EntityID);
                _Database.AddInParameter(objCommand, "@WidgetID", DbType.Int64, entityWidget.WidgetID);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, entityWidget.CreatedBy);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, entityWidget.CreatedOn);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, entityWidget.EditedBy);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, entityWidget.EditedOn);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SetWidgetStatus");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> SaveKeyValue(Dictionary<string, string> jobSearchKeyValue, long entityID)
        {
            DbCommand objCommand = null;
            try
            {
                if (_SqlTransaction == null)
                    StartTransaction();
                string sql = "Update JobSearchSettings set";
                int count = 1;
                foreach (var item in jobSearchKeyValue)
                {
                    if (count == jobSearchKeyValue.Count)
                        sql += string.Format(" {0}=@{1}", item.Key, item.Key);
                    else
                        sql += string.Format(" {0}=@{1},", item.Key, item.Key);
                    count++;
                }

                sql += " where EntityID=@EntityID";
                objCommand = _Database.GetSqlStringCommand(sql);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                foreach (var item in jobSearchKeyValue)
                {
                    _Database.AddInParameter(objCommand, "@" + item.Key, DbType.String, item.Value);
                }
                _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                CommitTransaction();
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetCareerLine");
                RollbackTransaction();
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}
