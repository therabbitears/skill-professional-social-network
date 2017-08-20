using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class CareerHistorySkillRepository : BaseRepository
    {
        internal long Save(Models.CareerHistorySkill item, long careerHistoryID)
        {
            var objCommand = _Database.GetStoredProcCommand("SaveCareerHistorySkill");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddOutParameter(objCommand, "@ID", DbType.Int64, int.MaxValue);
                _Database.AddInParameter(objCommand, "@CareerHistoryID", DbType.Int64, careerHistoryID);
                _Database.AddInParameter(objCommand, "@EntitySkillID", DbType.Int64, item.EntitySkillID);
                _Database.ExecuteNonQuery(objCommand);
                var id = Convert.ToInt32(objCommand.Parameters["@ID"].Value);
                return id;
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

        public async System.Threading.Tasks.Task<long> Delete(long ID)
        {
            var objCommand = _Database.GetStoredProcCommand("DeleteCareerHistorySkill");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ID", DbType.String, ID);
                _Database.ExecuteDataSet(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Delete");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}
