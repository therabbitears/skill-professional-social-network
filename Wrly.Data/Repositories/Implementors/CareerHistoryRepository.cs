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
    public class CareerHistoryRepository : BaseRepository, ICareerHistoryRepository
    {
        public async Task<long> Save(Models.CareerHistory careerHistory)
        {
            DbCommand objCommand = null;
            long careerHistoryID = 0;
            try
            {
                using (var repository = new JobTitleRepository())
                {
                    careerHistory.JobTitle.JobTitleID = repository.SaveOrGetJobTitle(careerHistory.JobTitle);
                }
                if (careerHistory.Organization != null)
                {
                    using (var repository = new BusinessRepository())
                    {
                        careerHistory.OrganizationID = repository.GetOrganizationID(careerHistory.OrganizationName);
                        if (careerHistory.OrganizationID == 0)
                        {
                            careerHistory.OrganizationID = await repository.CreateMinimal(careerHistory.Organization);
                        }
                    }
                }
                if (careerHistory.CareerHistoryID > 0)
                {
                    objCommand = _Database.GetStoredProcCommand("UpdateCareerHistory");
                    objCommand.CommandTimeout = Constants.TIMEOUT;
                    _Database.AddInParameter(objCommand, "@CareerHistoryID", DbType.String, careerHistory.CareerHistoryID);
                    _Database.AddInParameter(objCommand, "@JobTitleID", DbType.Int32, careerHistory.JobTitle.JobTitleID);
                    _Database.AddInParameter(objCommand, "@OrganizationID", DbType.String, careerHistory.OrganizationID);
                    _Database.AddInParameter(objCommand, "@OrganizationName", DbType.String, careerHistory.OrganizationName);
                    _Database.AddInParameter(objCommand, "@SubType", DbType.String, careerHistory.SubType);
                    _Database.AddInParameter(objCommand, "@About", DbType.String, careerHistory.About);
                    _Database.AddInParameter(objCommand, "@StartFromMonth", DbType.Int32, careerHistory.StartFromMonth);
                    _Database.AddInParameter(objCommand, "@StartFromYear", DbType.Int32, careerHistory.StartFromYear);
                    _Database.AddInParameter(objCommand, "@StartFromDay", DbType.Int32, careerHistory.StartFromDay);
                    _Database.AddInParameter(objCommand, "@EndFromMonth", DbType.Int32, careerHistory.EndFromMonth);
                    _Database.AddInParameter(objCommand, "@EndFromYear", DbType.Int32, careerHistory.EndFromYear);
                    _Database.AddInParameter(objCommand, "@EndFromDay", DbType.Int32, careerHistory.EndFromDay);
                    _Database.AddInParameter(objCommand, "@PottentialStartDate", DbType.DateTime, careerHistory.PottentialStartDate);
                    _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, careerHistory.EditedOn);
                    _Database.AddInParameter(objCommand, "@PottentialEndDate", DbType.DateTime, careerHistory.PottentialEndDate);
                    _Database.AddInParameter(objCommand, "@PottentialCurrent", DbType.Boolean, careerHistory.PottentialCurrent);
                    _Database.ExecuteNonQuery(objCommand);
                    careerHistoryID = careerHistory.CareerHistoryID;
                }
                else
                {
                    objCommand = _Database.GetStoredProcCommand("AddCareerHistory");
                    objCommand.CommandTimeout = Constants.TIMEOUT;
                    _Database.AddOutParameter(objCommand, "@CareerHistoryID", DbType.String, int.MaxValue);
                    _Database.AddInParameter(objCommand, "@EntityID", DbType.Int32, careerHistory.EntityID);
                    _Database.AddInParameter(objCommand, "@JobTitleID", DbType.Int32, careerHistory.JobTitle.JobTitleID);
                    _Database.AddInParameter(objCommand, "@OrganizationID", DbType.String, careerHistory.OrganizationID);
                    _Database.AddInParameter(objCommand, "@OrganizationName", DbType.String, careerHistory.OrganizationName);
                    _Database.AddInParameter(objCommand, "@SubType", DbType.String, careerHistory.SubType);
                    _Database.AddInParameter(objCommand, "@About", DbType.String, careerHistory.About);
                    _Database.AddInParameter(objCommand, "@Type", DbType.Int16, careerHistory.Type);
                    _Database.AddInParameter(objCommand, "@StartFromMonth", DbType.Int32, careerHistory.StartFromMonth);
                    _Database.AddInParameter(objCommand, "@StartFromYear", DbType.Int32, careerHistory.StartFromYear);
                    _Database.AddInParameter(objCommand, "@StartFromDay", DbType.Int32, careerHistory.StartFromDay);
                    _Database.AddInParameter(objCommand, "@EndFromMonth", DbType.Int32, careerHistory.EndFromMonth);
                    _Database.AddInParameter(objCommand, "@EndFromYear", DbType.Int32, careerHistory.EndFromYear);
                    _Database.AddInParameter(objCommand, "@EndFromDay", DbType.Int32, careerHistory.EndFromDay);
                    _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, careerHistory.CreatedOn);
                    _Database.AddInParameter(objCommand, "@PottentialStartDate", DbType.DateTime, careerHistory.PottentialStartDate);
                    _Database.AddInParameter(objCommand, "@PottentialEndDate", DbType.DateTime, careerHistory.PottentialEndDate);
                    _Database.AddInParameter(objCommand, "@PottentialCurrent", DbType.Boolean, careerHistory.PottentialCurrent);
                    _Database.ExecuteNonQuery(objCommand);
                    careerHistoryID = Convert.ToInt64(_Database.GetParameterValue(objCommand, "@CareerHistoryID"));
                }
                if (careerHistoryID > 0)
                {
                    if (careerHistory.CareerHistorySkills != null && careerHistory.CareerHistorySkills.Count > 0)
                    {
                        using (var repository = new CareerHistorySkillRepository())
                        {
                            foreach (var item in careerHistory.CareerHistorySkills)
                            {
                                repository.Save(item, careerHistoryID);
                            }
                        }
                    }
                }
                return careerHistoryID;
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

        public async Task<DataSet> ForUser(long entityID, bool isProfileName, short? type = null, string subType = null)
        {
            var objCommand = _Database.GetStoredProcCommand("GetHistory");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, type);
                _Database.AddInParameter(objCommand, "@SubType", DbType.String, subType);
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

        public async Task<DataSet> Single(long careerHistoryID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetSignleHistory");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@careerHistoryID", DbType.Int64, careerHistoryID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Single");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetImprovableCareerHistoryRow(int type, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_CareerHistoryRowAsOganizationNull");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Type", DbType.Int64, type);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Single");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetImprovableCareerHistoryForTime(int type, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_CareerHistoryRowAsStartTimeNull");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Type", DbType.Int64, type);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Single");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> LoadOptionsForCareerHistory(long id, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_OptionsForCareerHistory");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@CareerHistoryID", DbType.Int64, id);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "LoadOptionsForCareerHistory");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> TakeLastTwo(long entityID, int type, string subType)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_LastCareerHistories");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int32, type);
                _Database.AddInParameter(objCommand, "@SubType", DbType.String, subType);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "LoadOptionsForCareerHistory");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> Remove(long careerHistoryID, long entityID, DateTime now)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("CareerHistory_Remove");
            try
            {
                _Database.AddInParameter(objCommand, "@CareerHistoryID", DbType.Int64, careerHistoryID);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Now", DbType.DateTime, now);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "CareerHistory_Remove");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetIfAny(long entityID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("CareerHistory_GetAll");
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetIfAny");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return null;
        }

        public async Task<DataSet> GetDataForOpportunity(long? entityID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Opportunity_GetCandidateData");
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetDataForOpportunity");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return null;
        }
    }
}
