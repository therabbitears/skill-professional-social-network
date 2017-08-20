using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class LookUpRepository : BaseRepository
    {
        public DataSet GetProjects(string userName, bool isProfileName)
        {
            var objCommand = _Database.GetStoredProcCommand("ProjectLookup");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@UserName", DbType.String, userName);
                _Database.AddInParameter(objCommand, "@IsProfileName", DbType.Boolean, isProfileName);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetProjects");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetSkills(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("SkillLookup");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetProjects");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetSkills(string keyword)
        {
            var objCommand = _Database.GetStoredProcCommand("GenericSkillLookup");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Keyword", DbType.String, keyword);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetSkills");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetSkills(string keyword,long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("EntitySkillLookup");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Keyword", DbType.String, keyword);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetSkills");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetConnections(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("ConnectionLookupByEntity");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetProjects");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetConnections(long entityID,string keyWord)
        {
            var objCommand = _Database.GetStoredProcCommand("ConnectionLookup");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                _Database.AddInParameter(objCommand, "@keyWord", DbType.String, keyWord);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetProjects");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetJobTitles(string keyword, int type)
        {
            var objCommand = _Database.GetStoredProcCommand("GenericJobTitleLookup");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Keyword", DbType.String, keyword);
                _Database.AddInParameter(objCommand, "@Type", DbType.String, type);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetJobTitles");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetOrganization(string keyword, int type)
        {
            var objCommand = _Database.GetStoredProcCommand("OrganizationLookup");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Keyword", DbType.String, keyword);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int32, type);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetJobTitles");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}
