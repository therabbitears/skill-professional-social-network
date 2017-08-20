using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Data.Repositories.Signatures;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
  public  class EntityMediaRepository:BaseRepository,IEntityMediaRepository
    {
        public int Save(Models.EntityMedia media)
        {
            var objCommand = _Database.GetStoredProcCommand("SaveMedia");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                //TODO: Make it bigint
                _Database.AddOutParameter(objCommand, "@ReturnValue", DbType.String, int.MaxValue);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int16, media.Status);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, media.EntityID);
                _Database.AddInParameter(objCommand, "@MediaType", DbType.Int32, media.MediaType);
                _Database.AddInParameter(objCommand, "@Path", DbType.String, media.Path);
                if (_Database.ExecuteNonQuery(objCommand) > 0)
                {
                    var addressID = Convert.ToInt32(objCommand.Parameters["@ReturnValue"].Value);
                    return addressID;
                }
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddAddress");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return -1;
        }
    }
}
