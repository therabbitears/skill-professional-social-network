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
    public class EntitySkillRepository : BaseRepository
    {
        public async Task<DataSet> ForUser(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetSkills");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
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

        public async Task<long> Save(Models.EntitySkill entitySkill)
        {
            DbCommand objCommand = null;
            try
            {
                using (var repository = new SkillRepository())
                {
                    entitySkill.Skill.SkillID = repository.SaveOrGetSkill(entitySkill.Skill);
                }
                if (entitySkill.EntitySkillID > 0)
                {
                    objCommand = _Database.GetStoredProcCommand("UpdateEntitySkill");
                    objCommand.CommandTimeout = Constants.TIMEOUT;
                    _Database.AddInParameter(objCommand, "@EntitySkillID", DbType.String, entitySkill.EntitySkillID);
                    _Database.AddInParameter(objCommand, "@SkillID", DbType.String, entitySkill.Skill.SkillID);
                    _Database.AddInParameter(objCommand, "@FromMonth", DbType.String, entitySkill.FromMonth);
                    _Database.AddInParameter(objCommand, "@FromYear", DbType.Int32, entitySkill.FromYear);
                    _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, entitySkill.IpAddress);
                    _Database.AddInParameter(objCommand, "@ExpertiseLevel", DbType.String, entitySkill.ExpertiseLevel);
                    _Database.ExecuteNonQuery(objCommand);
                    return entitySkill.EntitySkillID;
                }
                else
                {
                    objCommand = _Database.GetStoredProcCommand("AddEntitySkill");
                    objCommand.CommandTimeout = Constants.TIMEOUT;
                    _Database.AddOutParameter(objCommand, "@EntitySkillID", DbType.String, Int32.MaxValue);
                    _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, entitySkill.CreatedBy);
                    _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, entitySkill.CreatedOn);
                    _Database.AddInParameter(objCommand, "@SkillID", DbType.String, entitySkill.Skill.SkillID);
                    _Database.AddInParameter(objCommand, "@Score", DbType.String, entitySkill.Score);
                    _Database.AddInParameter(objCommand, "@FromMonth", DbType.String, entitySkill.FromMonth);
                    _Database.AddInParameter(objCommand, "@FromYear", DbType.Int32, entitySkill.FromYear);
                    _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, entitySkill.IpAddress);
                    _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entitySkill.EntityID);
                    _Database.AddInParameter(objCommand, "@ExpertiseLevel", DbType.String, entitySkill.ExpertiseLevel);
                    _Database.ExecuteNonQuery(objCommand);
                    return Convert.ToInt64(_Database.GetParameterValue(objCommand, "@EntitySkillID"));
                }
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Save");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return -1;
        }

        public async Task<DataSet> Single(long entitySkillID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetOneEntitySkill");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntitySkillID", DbType.String, entitySkillID);
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

        public async Task<int> CalculateAndSetSkillScore(long skillID, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_SkillStatisticsForRank");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntitySkillID", DbType.Int64, skillID);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                var statistics = _Database.ExecuteDataSet(objCommand);
                var score = GetScore(statistics);
                if (score > 0)
                    UpdateScoreForSkill(skillID, score);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "CalculateAndSetSkillScore");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private int UpdateScoreForSkill(long skillID, long score)
        {
            var query = string.Format("Update entitySkill Set Score = {0} where entitySkillID ={1}", score, skillID);
            var objCommand = _Database.GetSqlStringCommand(query);
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                return _Database.ExecuteNonQuery(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "UpdateScoreForSkill");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private long GetScore(DataSet statistics)
        {
            // Initial score is 5 to add the skill on the profile.
            var score = 5;
            var countData = statistics.Tables[0].Rows[0];
            var accomplishments = statistics.Tables[1];
            var awards = statistics.Tables[2];
            var workHistories = statistics.Tables[3];
            var educationAndCertification = statistics.Tables[4];

            #region Based on skill level states

            var expertiseLevel = Convert.ToInt16(countData["ExpertiseLevel"]);
            var totalEndorsement = Convert.ToInt16(countData["TotalEndorsement"]);
            var totalRecommendation = Convert.ToInt16(countData["TotalRecommendation"]);

            if (expertiseLevel > 0)
            {
                // Adding profeciancy level to your skill gives you 5 points.
                score += 5;
                // The expertise level * 2 means if you are having an expert level skill it should be given 10 or if it is beginner then 2;
                score += (expertiseLevel * 2);
            }
            if (totalRecommendation > 0)
            {
                // Per recommedation gives you 20 marks.
                score += (totalRecommendation * 20);
            }

            // Endorsement are markable only if there is:
            //      #1 expertise level defined
            //      #2 at least having the skill in at least one role in work history.
            //      #3 having at least one accomplishment
            //      
            if (totalEndorsement > 0 && expertiseLevel > 0 && workHistories.Rows.Count > 0 && accomplishments.Rows.Count > 0)
            {
                // if total endorsement are more than 50 then it requires more calculations based on work experiece on that skill.
                if (totalEndorsement <= 50)
                {
                    // Per endorsement we will give two points.
                    score += (totalEndorsement * 2);
                }
                // Calculate the year of work histories.
                else
                {
                    var averageEndorsementPerAccomplishment = totalEndorsement / accomplishments.Rows.Count;
                    // The most important is we consider there would be one accomplishment in one year of work, that is vary in diffrent job titles but that would be a base for calculations
                    // and in comparison we could have compare with similar industry so the renk would be same in that case.
                    var days = 0;
                    foreach (DataRow item in workHistories.Rows)
                    {
                        if (item["PottentialStartDate"] != DBNull.Value)
                        {
                            if (item["PottentialEndDate"] != null)
                                days = Convert.ToDateTime(item["PottentialEndDate"]).Subtract(Convert.ToDateTime(item["PottentialStartDate"])).Days;
                            else
                                days = Convert.ToDateTime(item["PottentialEndDate"]).Subtract(DateTime.UtcNow).Days;
                        }
                    }
                    var year = (days / 365);
                    var pottentialNumberOfAccomplishments = year * 1;
                    // Total point must be multiply with number of years worked, and average is required in case.
                    score += ((averageEndorsementPerAccomplishment * accomplishments.Rows.Count) * (2 * year));
                }
            }

            #endregion

            #region Based on accomplishments

            score += accomplishments.Rows.Count * 20;

            #endregion

            #region Based on careerline

            // At least having the start date.
            var validNumberOfCareerLine = workHistories.AsEnumerable().Where(c => c["PottentialStartDate"] != DBNull.Value).Count();
            score += validNumberOfCareerLine * 10;

            #endregion

            #region Based on awards

            score += awards.Rows.Count * 30;

            #endregion

            #region Based on education

            var totalEducationDefined = educationAndCertification.Select("SubType='Course'").Length;
            score += totalEducationDefined * 5;

            #endregion

            #region Based on certification

            var totalCertificationDefined = educationAndCertification.Select("SubType='Certification'").Length;
            score += totalCertificationDefined * 50;

            #endregion

            return score;
        }

        public async Task<DataSet> LoadOptionsForSkill(long entitySkillID, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_OptionsForSkill");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntitySkillID", DbType.Int64, entitySkillID);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "LoadOptionsForSkill");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        internal long SaveRecomedationForSkill(Models.AppreciationAndRecommendationSkill item, int id)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("AddRecomedationEntitySkill");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddOutParameter(objCommand, "@ID", DbType.String, Int32.MaxValue);
                _Database.AddInParameter(objCommand, "@EntitySkillID", DbType.String, item.EntitySkillID);
                _Database.AddInParameter(objCommand, "@ReferenceID", DbType.Int64, id);
                _Database.ExecuteNonQuery(objCommand);
                return Convert.ToInt64(_Database.GetParameterValue(objCommand, "@ID"));
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SaveRecomedationForSkill");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return -1;
        }

        public async Task<long> RecordState(Models.EntitySkillState state)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("RecordEntitySkillState");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddOutParameter(objCommand, "@ID", DbType.String, Int32.MaxValue);
                _Database.AddInParameter(objCommand, "@EntitySkillID", DbType.Int64, state.EntitySkillID);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, state.CreatedBy);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, state.CreatedOn);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, state.EntityID);
                _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, state.IpAddress);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int32, state.Status);
                _Database.AddInParameter(objCommand, "@SubType", DbType.Int32, state.SubType);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int32, state.Type);
                _Database.ExecuteNonQuery(objCommand);
                return Convert.ToInt64(_Database.GetParameterValue(objCommand, "@ID"));
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SaveRecomedationForSkill");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return -1;
        }

        public async Task<long> ChangeSkillStateStatus(Models.EntitySkillState state)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("RemoveEntitySkillState");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@EntitySkillID", DbType.Int64, state.EntitySkillID);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, state.EntityID);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int64, state.Status);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SaveRecomedationForSkill");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return -1;
        }

        public async Task<DataSet> PublicSkills(long entityID,long currentEntityID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetSkillsPublic");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Int64, currentEntityID);
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
    }
}
