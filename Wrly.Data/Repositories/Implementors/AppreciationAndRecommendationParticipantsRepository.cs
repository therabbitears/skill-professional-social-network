using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class AppreciationAndRecommendationParticipantsRepository:BaseRepository
    {
        internal long Save(Models.AppreciationAndRecommendationParticipant item, int appreciationAndRecommendationId)
        {
            var objCommand = _Database.GetStoredProcCommand("SaveAppreciationAndRecommendationParticipant");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddOutParameter(objCommand, "@ID", DbType.Int64, int.MaxValue);
                _Database.AddInParameter(objCommand, "@AppreciationAndRecommendationID", DbType.Int64, appreciationAndRecommendationId);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, item.EntityID);
                _Database.AddInParameter(objCommand, "@Status", DbType.Byte, item.Status);
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
    }
}
