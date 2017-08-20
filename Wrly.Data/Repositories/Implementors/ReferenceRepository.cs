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
    public class ReferenceRepository : BaseRepository
    {
        public async Task<DataSet> GetReference(long entityID, bool isProfileName, byte type)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAppreciationAndRecommendation");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int16, null);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, type);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetReference");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> Save(Models.AppreciationAndRecommendation appreciationAndRecommendation)
        {
            DbCommand objCommand=null;
            if (appreciationAndRecommendation.ReferenceID > 0)
            {
                objCommand = _Database.GetStoredProcCommand("UpdateAppreciationAndRecommendation");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                try
                {
                    _Database.AddInParameter(objCommand, "@ReferenceID", DbType.Int64, appreciationAndRecommendation.ReferenceID);
                    _Database.AddInParameter(objCommand, "@Description", DbType.String, appreciationAndRecommendation.Description);
                    _Database.AddInParameter(objCommand, "@Status", DbType.Int64, appreciationAndRecommendation.Status);
                    _Database.ExecuteNonQuery(objCommand);
                }
                catch (Exception ex)
                {
                    ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SaveOrGetSkill");
                    return -1;
                }
                finally
                {
                    if (objCommand != null) { objCommand.Dispose(); }
                }
                return 1;
            }

            objCommand = _Database.GetStoredProcCommand("SaveAppreciationAndRecommendation");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddOutParameter(objCommand, "@ReferenceID", DbType.Int64, int.MaxValue);
                _Database.AddInParameter(objCommand, "@Description", DbType.String, appreciationAndRecommendation.Description);
                _Database.AddInParameter(objCommand, "@Title", DbType.String, appreciationAndRecommendation.Title);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, appreciationAndRecommendation.EntityID);
                _Database.AddInParameter(objCommand, "@SourceEntityID", DbType.Int64, appreciationAndRecommendation.SourceEntityID);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, appreciationAndRecommendation.EditedBy);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, appreciationAndRecommendation.EditedOn);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, appreciationAndRecommendation.CreatedBy);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, appreciationAndRecommendation.CreatedOn);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int16, appreciationAndRecommendation.Status);
                _Database.AddInParameter(objCommand, "@AwardID", DbType.Int64, appreciationAndRecommendation.AwardID);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, appreciationAndRecommendation.Type);
                _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, appreciationAndRecommendation.IpAddress);
                _Database.AddInParameter(objCommand, "@CareerHistoryID", DbType.String, appreciationAndRecommendation.CareerHistoryID);
                _Database.AddInParameter(objCommand, "@RecomedationRelation", DbType.String, appreciationAndRecommendation.RecomedationRelation);
                _Database.ExecuteNonQuery(objCommand);
                var id = Convert.ToInt32(objCommand.Parameters["@ReferenceID"].Value);
                if (id > 0 && appreciationAndRecommendation.AppreciationAndRecommendationParticipants != null && appreciationAndRecommendation.AppreciationAndRecommendationParticipants.Count > 0)
                {
                    using (var participantRepository = new AppreciationAndRecommendationParticipantsRepository())
                    {
                        foreach (var item in appreciationAndRecommendation.AppreciationAndRecommendationParticipants)
                        {
                            participantRepository.Save(item, id);
                        }
                    }
                }

                if (id > 0 && appreciationAndRecommendation.AppreciationAndRecommendationSkills != null && appreciationAndRecommendation.AppreciationAndRecommendationSkills.Count > 0)
                {
                    using (var skillRepository = new EntitySkillRepository())
                    {
                        foreach (var item in appreciationAndRecommendation.AppreciationAndRecommendationSkills)
                        {
                            skillRepository.SaveRecomedationForSkill(item, id);
                        }
                    }
                }

                return id;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SaveOrGetSkill");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return -1;
        }

        public async Task<DataSet> SentRequests(long entityID, byte? type, int status)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAppreciationAndRecommendation_Requests");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, type);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int16, status);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetSentRequests");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> ReceivedRequests(long entityID, byte? type, int status)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAppreciationAndRecommendation_Requests_Received");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, type);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int16, status);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetReceivedRequests");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }


        public async Task<DataSet> Sent(long entityID, byte? type, int status)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAppreciationAndRecommendation_Sent");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, type);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int16, status);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetSentRequests");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> Received(long entityID, byte? type, int? status)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAppreciationAndRecommendation");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, type);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int16, status);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetReceivedRequests");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}
