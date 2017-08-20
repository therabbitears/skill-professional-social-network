using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class SearchRepository : BaseRepository
    {
        public System.Data.DataSet GetSearches(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("List_EntitySearches");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetSearches");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> Search(string keyword, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Execute_EntitySearch");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@keyword", DbType.String, keyword);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetSearches");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> Record(Models.EntitySearch search)
        {
            var objCommand = _Database.GetStoredProcCommand("Insert_EntitySearch");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, search.EntityID > 0 ? search.EntityID : null);
                _Database.AddInParameter(objCommand, "@keyword", DbType.String, search.Keyword);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, search.CreatedBy);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, search.CreatedOn);
                _Database.AddInParameter(objCommand, "@SourceEntity", DbType.Int64, search.SourceEntity);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, search.EditedOn);
                _Database.AddInParameter(objCommand, "@Url", DbType.String, search.Url);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Record");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> MixedSearch(string q, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Search_All");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Keyword", DbType.String, q);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "MixedSearch");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> IndividualSearch(string q, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Search_Individual");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Keyword", DbType.String, q);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "IndividualSearch");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> OrganizationSearch(string q, long entityID, int type)
        {
            var objCommand = _Database.GetStoredProcCommand("Search_Organization");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Keyword", DbType.String, q);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, type);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "OrganizationSearch");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> ConnectionSearch(string q, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Search_Connections");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Keyword", DbType.String, q);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ConnectionSearch");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetLuceneIndexableData()
        {
            var objCommand = _Database.GetStoredProcCommand("Search_LuceneIndexableData");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetLuceneIndexableData");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}
