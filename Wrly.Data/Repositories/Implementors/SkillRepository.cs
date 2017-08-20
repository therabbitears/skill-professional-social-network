using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class SkillRepository : BaseRepository
    {
        internal int SaveOrGetSkill(Models.Skill skill)
        {
            var objCommand = _Database.GetStoredProcCommand("SaveSkill");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddOutParameter(objCommand, "@SkillID", DbType.Int64, int.MaxValue);
                _Database.AddInParameter(objCommand, "@Decription", DbType.String, skill.Decription);
                _Database.AddInParameter(objCommand, "@Name", DbType.String, skill.Name);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, skill.CreatedBy);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, skill.CreatedOn);
                _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, skill.IpAddress);
                _Database.ExecuteNonQuery(objCommand);
                var id = Convert.ToInt32(objCommand.Parameters["@SkillID"].Value);
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

        public Task<DataSet> GetRow(long skillID)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Remove(Models.EntitySkill skill)
        {
            var objCommand = _Database.GetStoredProcCommand("RemoveEntitySkill");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntitySkillID", DbType.String, skill.EntitySkillID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Remove");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return -1;
        }

        public async Task<int> Revert(Models.EntitySkill skill)
        {
            var objCommand = _Database.GetStoredProcCommand("RevertEntitySkill");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntitySkillID", DbType.String, skill.EntitySkillID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Revert");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return -1;
        }
    }
}
