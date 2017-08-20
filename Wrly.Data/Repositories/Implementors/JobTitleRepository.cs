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
    public class JobTitleRepository : BaseRepository, IJobTitleRepository
    {
        internal int SaveOrGetJobTitle(Models.JobTitle jobTitle)
        {
            var objCommand = _Database.GetStoredProcCommand("SaveJobTitle");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddOutParameter(objCommand, "@JobTitleID", DbType.String, int.MaxValue);
                _Database.AddInParameter(objCommand, "@Active", DbType.Boolean, jobTitle.Active);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, jobTitle.CreatedBy);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, jobTitle.CreatedOn);
                _Database.AddInParameter(objCommand, "@Description", DbType.String, jobTitle.Description);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, jobTitle.EditedBy);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int32, jobTitle.Type);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, jobTitle.EditedOn);
                _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, jobTitle.IpAddress);
                _Database.AddInParameter(objCommand, "@Name", DbType.String, jobTitle.Name);
                _Database.ExecuteNonQuery(objCommand);
                    var id = Convert.ToInt32(objCommand.Parameters["@JobTitleID"].Value);
                    return id;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddEntity");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return -1;
        }
    }
}
