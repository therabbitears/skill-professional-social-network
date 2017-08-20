using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Data.Models;

namespace Wrly.Data.Repositories.Implementors
{
    public class ImportRepository : BaseRepository
    {
        public async Task<long> SaveImport(EntityImport import)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("SaveImport");
            try
            {
                _Database.AddOutParameter(objCommand, "@ID", DbType.Int64, int.MaxValue);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, import.CreatedOn);
                _Database.AddInParameter(objCommand, "@ImportID", DbType.String, import.ImportID);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, import.EntityID);
                _Database.AddInParameter(objCommand, "@ImportType", DbType.Int32, import.ImportType);
                _Database.ExecuteNonQuery(objCommand);
                var importID = Convert.ToInt64(objCommand.Parameters["@ID"].Value); ;
                return await SaveImportContact(import.EntityImportContacts, importID);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SaveImport");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private async Task<long> SaveImportContact(ICollection<EntityImportContact> collection, long importID)
        {
            DbCommand objCommand = null;
            try
            {
                long importContactID = 0;
                foreach (var item in collection)
                {
                    objCommand = _Database.GetStoredProcCommand("SaveImport_Contact");
                    _Database.AddInParameter(objCommand, "@EmailAddresses", DbType.String, item.EmailAddresses);
                    _Database.AddInParameter(objCommand, "@ImportID", DbType.Int64, importID);
                    _Database.AddOutParameter(objCommand, "@ID", DbType.Int64, int.MaxValue);
                    _Database.AddInParameter(objCommand, "@Name", DbType.String, item.Name);
                    _Database.ExecuteNonQuery(objCommand);
                    importContactID = Convert.ToInt64(_Database.GetParameterValue(objCommand, "@ID"));
                }
                return importContactID;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SaveImportContact");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> Get(string id, long entityID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("GetImportContacts");
            try
            {
                _Database.AddInParameter(objCommand, "@ImportID", DbType.String, id);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Get");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> SaveInvites(List<EntityImportInvite> imports)
        {
            DbCommand objCommand = null;
            try
            {
                foreach (var item in imports)
                {
                    objCommand = _Database.GetStoredProcCommand("SaveImportContact_Invite");
                    _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, item.CreatedOn);
                    _Database.AddInParameter(objCommand, "@EntityImportContactID", DbType.Int64, item.EntityImportContactID);
                    _Database.AddInParameter(objCommand, "@InviteType", DbType.Int32, item.InviteType);
                    _Database.ExecuteNonQuery(objCommand);
                }
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SaveImportContact");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            };
        }

        public async Task<int> UpdateName(long? id, string value, long entityID)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("SaveImport_ContactName");
                _Database.AddInParameter(objCommand, "@id", DbType.Int64, id);
                _Database.AddInParameter(objCommand, "@value", DbType.String, value);
                _Database.AddInParameter(objCommand, "@entityID", DbType.Int64, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "UpdateName");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}
