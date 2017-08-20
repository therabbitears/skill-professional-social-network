using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class AwardSkillRepository : BaseRepository
    {
        internal long Save(Models.AwardSkill item, long awardID)
        {
            var objCommand = _Database.GetStoredProcCommand("SaveAwardSkill");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddOutParameter(objCommand, "@AwardSkillID", DbType.Int64, int.MaxValue);
                _Database.AddInParameter(objCommand, "@AwardID", DbType.Int64, awardID);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int16, item.Status);
                _Database.AddInParameter(objCommand, "@EntitySkillID", DbType.Int64, item.EntitySkillID);
                _Database.ExecuteNonQuery(objCommand);
                var id = Convert.ToInt32(objCommand.Parameters["@AwardSkillID"].Value);
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

        internal void DeleteNotExistSkills(ICollection<Models.AwardSkill> collection, long awardID)
        {
            var data = GetForAward(awardID);
            if (data != null && data.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in data.Tables[0].Rows)
                {
                    var entitySkillID = Convert.ToInt64(item["EntityskillID"]);
                    if (!collection.Any(c => c.EntitySkillID.Equals(entitySkillID)))
                    {
                        Delete(Convert.ToInt64(item["AwardSkillID"]));
                    }
                }
            }
        }

        public int Delete(long awardSkillID)
        {
            var objCommand = _Database.GetStoredProcCommand("DeleteAwardSkillAssociation");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@awardSkillID", DbType.String, awardSkillID);
                _Database.ExecuteDataSet(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetProjects");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private DataSet GetForAward(long awardID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetSkillsForAward");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@AwardID", DbType.String, awardID);
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
    }
}
