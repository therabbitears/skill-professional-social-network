using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class AwardParticipantRepository : BaseRepository
    {
        internal int Save(Models.EntityAwardParticipant item, long awardId)
        {
            var objCommand = _Database.GetStoredProcCommand("SaveAwardEntity");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddOutParameter(objCommand, "@AwardParticipantID", DbType.Int64, int.MaxValue);
                _Database.AddInParameter(objCommand, "@AwardID", DbType.Int64, awardId);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int64, item.Status);
                _Database.AddInParameter(objCommand, "@RefrenceEntity", DbType.Int64, item.RefrenceEntity);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, item.EntityID);
                _Database.AddInParameter(objCommand, "@Role", DbType.String, item.Role);
                _Database.AddInParameter(objCommand, "@GroupID", DbType.String, item.GroupID);
                _Database.ExecuteNonQuery(objCommand);
                var id = Convert.ToInt32(objCommand.Parameters["@AwardParticipantID"].Value);
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

        internal void DeleteNotExistParticipants(ICollection<Models.EntityAwardParticipant> collection, long awardId)
        {
            var data = GetForAward(awardId);
            if (data != null && data.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in data.Tables[0].Rows)
                {
                    var entityID = Convert.ToInt64(item["EntityID"]);
                    if (!collection.Any(c => c.EntityID.Equals(entityID)))
                    {
                        Delete(Convert.ToInt64(item["ID"]));
                    }
                }
            }
        }

        public int Delete(long Id)
        {
            var objCommand = _Database.GetStoredProcCommand("DeleteAwardEntityAssociation");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ID", DbType.String, Id);
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
            var objCommand = _Database.GetStoredProcCommand("GetParticipantForAward");
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
