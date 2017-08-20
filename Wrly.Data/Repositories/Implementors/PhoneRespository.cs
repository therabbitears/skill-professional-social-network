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
    public class PhoneRespository : BaseRepository, IPhoneRespository
    {
        public async Task<int> Save(Models.Phone phone)
        {
            var objCommand = _Database.GetStoredProcCommand("SavePhone");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                //TODO: Make it bigint
                _Database.AddOutParameter(objCommand, "@ReturnValue", DbType.String, int.MaxValue);
                _Database.AddInParameter(objCommand, "@Active", DbType.Boolean, phone.Active);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, phone.EntityID);
                _Database.AddInParameter(objCommand, "@PhoneType", DbType.Int32, phone.PhoneType);
                _Database.AddInParameter(objCommand, "@Varified", DbType.Boolean, phone.Varified);
                _Database.AddInParameter(objCommand, "@Phone", DbType.String, phone.Phone1);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, phone.CreatedBy);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, phone.CreatedOn);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, phone.EditedBy);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, phone.EditedOn);
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
