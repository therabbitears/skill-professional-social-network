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
    public class AddressRepository : BaseRepository, IAddressRepository
    {
        public async Task<int> Save(Models.Address address)
        {
            var objCommand = _Database.GetStoredProcCommand("SaveAddress");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                //TODO: Make it bigint
                _Database.AddOutParameter(objCommand, "@ReturnValue", DbType.String, int.MaxValue);
                _Database.AddInParameter(objCommand, "@Active", DbType.Boolean, address.Active);
                _Database.AddInParameter(objCommand, "@AddressId", DbType.Int64, address.AddressId);
                _Database.AddInParameter(objCommand, "@AddressLine1", DbType.String, address.AddressLine1);
                _Database.AddInParameter(objCommand, "@AddressLine2", DbType.String, address.AddressLine2);
                _Database.AddInParameter(objCommand, "@AddressLine3", DbType.String, address.AddressLine3);
                _Database.AddInParameter(objCommand, "@AddressLine4", DbType.String, address.AddressLine4);
                _Database.AddInParameter(objCommand, "@AddressType", DbType.String, address.AddressType);
                _Database.AddInParameter(objCommand, "@City", DbType.String, address.City);
                _Database.AddInParameter(objCommand, "@Varified", DbType.Boolean, address.Varified);
                _Database.AddInParameter(objCommand, "@State", DbType.String, address.State);
                _Database.AddInParameter(objCommand, "@Country", DbType.String, address.Country);
                _Database.AddInParameter(objCommand, "@County", DbType.String, address.County);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, address.EntityID);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, address.CreatedBy);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, address.CreatedOn);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, address.EditedBy);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, address.EditedOn);
                _Database.AddInParameter(objCommand, "@Lat", DbType.String, address.Lat);
                _Database.AddInParameter(objCommand, "@Long", DbType.String, address.Long);
                _Database.AddInParameter(objCommand, "@ZipCode", DbType.String, address.ZipCode);
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
