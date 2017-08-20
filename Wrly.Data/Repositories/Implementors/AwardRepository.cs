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
    public class AwardRepository : BaseRepository
    {
        public async Task<long> Save(Models.EntityAwardAndCompletion award)
        {
            DbCommand objCommand = null;
            try
            {
                if (award.AwardID == 0)
                {
                    objCommand = _Database.GetStoredProcCommand("SaveEntityAward");
                    objCommand.CommandTimeout = Constants.TIMEOUT;
                    _Database.AddOutParameter(objCommand, "@AwardID", DbType.Int64, int.MaxValue);
                    _Database.AddInParameter(objCommand, "@ParentID", DbType.String, award.ParentID);
                    _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, award.CreatedBy);
                    _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, award.CreatedOn);
                    _Database.AddInParameter(objCommand, "@Status", DbType.Int16, award.Status);
                    _Database.AddInParameter(objCommand, "@Type", DbType.Int16, award.Type);
                }
                else
                {
                    objCommand = _Database.GetStoredProcCommand("UpdateEntityAward");
                    objCommand.CommandTimeout = Constants.TIMEOUT;
                    _Database.AddInParameter(objCommand, "@AwardID", DbType.Int64, award.AwardID);
                }
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, award.EntityID);
                _Database.AddInParameter(objCommand, "@Description", DbType.String, award.Description);
                _Database.AddInParameter(objCommand, "@Name", DbType.String, award.Name);
                _Database.AddInParameter(objCommand, "@Role", DbType.String, award.Role);
                _Database.AddInParameter(objCommand, "@Url", DbType.String, award.Url);
                _Database.AddInParameter(objCommand, "@SubType", DbType.String, award.SubType);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, award.EditedBy);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, award.EditedOn);
                _Database.AddInParameter(objCommand, "@CareerHistoryID", DbType.Int16, award.CareerHistoryID == -1 ? null : award.CareerHistoryID);
                _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, award.IpAddress);
                _Database.AddInParameter(objCommand, "@StartFromMonth", DbType.Int32, award.StartFromMonth);
                _Database.AddInParameter(objCommand, "@StartFromYear", DbType.Int32, award.StartFromYear);
                _Database.AddInParameter(objCommand, "@StartFromDay", DbType.Int32, award.StartFromDay);
                _Database.AddInParameter(objCommand, "@EndFromMonth", DbType.Int32, award.EndFromMonth);
                _Database.AddInParameter(objCommand, "@EndFromYear", DbType.Int32, award.EndFromYear);
                _Database.AddInParameter(objCommand, "@EndFromDay", DbType.Int32, award.EndFromDay);
                _Database.AddInParameter(objCommand, "@PottentialStartDate", DbType.DateTime, award.PottentialStartDate);
                _Database.AddInParameter(objCommand, "@PottentialEndDate", DbType.DateTime, award.PottentialEndDate);
                _Database.AddInParameter(objCommand, "@PottentialCurrent", DbType.Boolean, award.PottentialCurrent);
                _Database.ExecuteNonQuery(objCommand);
                var id = award.AwardID == 0 ? Convert.ToInt32(objCommand.Parameters["@AwardID"].Value) : award.AwardID;
                if (id > 0 && award.AwardSkills != null && award.AwardSkills.Count > 0)
                {
                    using (var skillRepository = new AwardSkillRepository())
                    {
                        // Deletes all the skills not exist in the list.
                        //if (award.AwardID > 0)
                        //{
                        //    skillRepository.DeleteNotExistSkills(award.AwardSkills, id);
                        //}
                        foreach (var item in award.AwardSkills)
                        {
                            skillRepository.Save(item, id);
                        }
                    }
                }

                if (id > 0 && award.EntityAwardParticipants != null && award.EntityAwardParticipants.Count > 0)
                {
                    using (var skillRepository = new AwardParticipantRepository())
                    {
                        // Deletes all the participants not exist in the list.
                        //if (award.AwardID > 0)
                        //{
                        //    skillRepository.DeleteNotExistParticipants(award.EntityAwardParticipants, id);
                        //}
                        foreach (var item in award.EntityAwardParticipants)
                        {
                            skillRepository.Save(item, id);
                        }
                    }
                }

                //if (id > 0 && award.ParentAwardParticipants != null && award.ParentAwardParticipants.Count > 0)
                //{
                //    using (var participantRepository = new AwardParticipantRepository())
                //    {
                //        // Deletes all the participants not exist in the list.
                //        //if (award.AwardID > 0)
                //        //{
                //        //    skillRepository.DeleteNotExistParticipants(award.EntityAwardParticipants, id);
                //        //}
                //        foreach (var item in award.ParentAwardParticipants)
                //        {
                //            participantRepository.Save(item, item.EntityAwardID);
                //        }
                //    }
                //}

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

        public async Task<DataSet> ForUser(long entityID, bool isProfileName, short type)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAwards");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, type);
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

        public async Task<DataSet> BasicForUser(long entityID, int[] type)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAwards_Basic");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@Type", DbType.String, string.Join(",", type));
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




        public async Task<DataSet> Single(long awardID, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetSignleAward");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@AwardID", DbType.Int64, awardID);
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

        public async Task<DataSet> GetImprovableAwardForTeam(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_AwardRowAsZeroParticipant");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetImprovableAwardForTeam");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetImprovableAssignment(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_AssignmentRowAsZeroParticipant");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetImprovableAssignment");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetParticipantHeads(long awardID, string commaSaperatedEntities)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_ParticipantHeadWithDefaultCurrent");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@AwardID", DbType.Int64, awardID);
                _Database.AddInParameter(objCommand, "@Entities", DbType.String, commaSaperatedEntities);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetImprovableAssignment");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> LoadOptionsForAssignment(long awardID, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_OptionsForAssignment");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@AwardID", DbType.Int64, awardID);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetImprovableAssignment");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> Basic(long awardID, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetSignleBasicAward");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@AwardID", DbType.Int64, awardID);
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

        public async Task<int> RecordState(Models.AccomplishmentState accomplishmentState)
        {
            var objCommand = _Database.GetStoredProcCommand("RecordAccomplishmentState");
            try
            {
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, accomplishmentState.Type);
                _Database.AddInParameter(objCommand, "@SubType", DbType.Int16, accomplishmentState.SubType);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, accomplishmentState.EntityID);
                _Database.AddInParameter(objCommand, "@AccomplishmentID", DbType.Int16, accomplishmentState.AccomplishmentID);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, accomplishmentState.CreatedBy);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, accomplishmentState.CreatedOn);
                _Database.AddInParameter(objCommand, "@Description", DbType.String, accomplishmentState.Description);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, accomplishmentState.EditedBy);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, accomplishmentState.EditedOn);
                _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, accomplishmentState.IpAddress);
                _Database.AddInParameter(objCommand, "@Status", DbType.String, accomplishmentState.Status);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Single");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> AccomplitionForProfile(long entityID, long currentEntityID, short type)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_AccomplitionForProfile");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Int64, currentEntityID);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, type);
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

        public async Task<int> Remove(long awardID, long entityID, DateTime now)
        {
            var objCommand = _Database.GetStoredProcCommand("Remove_Award");
            try
            {
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@Now", DbType.DateTime, now);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@AwardID", DbType.String, awardID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Remove_Award");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}
