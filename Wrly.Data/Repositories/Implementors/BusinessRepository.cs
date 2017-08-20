using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using Wrly.Data.Models;
using Wrly.Data.Repositories.Signatures;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class BusinessRepository : BaseRepository, IBusinessRepository
    {

        public Types.Enums.OrganizationSaveStatus Save(Models.Organization organization, out long organizationId)
        {
            var objCommand = _Database.GetStoredProcCommand("AddOrganization");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            long entityID = 0;
            long organizationID = 0;
            try
            {
                using (var commonRepository = new CommonRepository())
                {
                    entityID = commonRepository.AddEntity(organization.Entity);
                    if (entityID > 0)
                    {
                        _Database.AddOutParameter(objCommand, "@OrganizationID", DbType.String, int.MaxValue);
                        _Database.AddInParameter(objCommand, "@Name", DbType.String, organization.Name);
                        _Database.AddInParameter(objCommand, "@EstablishedYear", DbType.Int16, organization.EstablishedYear);
                        _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                        _Database.AddInParameter(objCommand, "@CategoryID", DbType.String, organization.CategoryID);
                        _Database.AddInParameter(objCommand, "@EmployeeStrength", DbType.String, organization.EmployeeStrength);
                        _Database.AddInParameter(objCommand, "@Type", DbType.Int32, organization.Type);
                        _Database.AddInParameter(objCommand, "@SubType", DbType.Int32, organization.SubType);
                        _Database.AddInParameter(objCommand, "@UserID", DbType.String, organization.UserID);
                        if (_Database.ExecuteNonQuery(objCommand) > 0)
                        {
                            organizationID = Convert.ToInt64(objCommand.Parameters["@OrganizationID"].Value);
                            AddProfile(organization, organizationID);
                        }
                    }
                    else
                    {
                        organizationId = -1;
                        return Types.Enums.OrganizationSaveStatus.Error;
                    }

                }
                if (organization.Entity.Addresses != null && organization.Entity.Addresses.Count > 0)
                {
                    using (var addressRepository = new AddressRepository())
                    {
                        var address = organization.Entity.Addresses.FirstOrDefault();
                        address.EntityID = entityID;
                        addressRepository.Save(address);
                    }
                }
                if (organization.Entity.Associations != null && organization.Entity.Associations.Count > 0)
                {
                    using (var repository = new AssociationRepository())
                    {
                        foreach (var item in organization.Entity.Associations)
                        {
                            item.EntityID = entityID;
                            repository.SaveAssociation(item);
                        }
                    }
                }
                if (organization.Entity.Phones != null && organization.Entity.Phones.Count > 0)
                {
                    using (var phoneRepository = new PhoneRespository())
                    {
                        var phone = organization.Entity.Phones.FirstOrDefault();
                        phone.EntityID = entityID;
                        phoneRepository.Save(phone);
                    }
                }
                if (organization.Entity.Emails != null && organization.Entity.Emails.Count > 0)
                {
                    using (var emailRepository = new EmailRepository())
                    {
                        var email = organization.Entity.Emails.FirstOrDefault();
                        email.EntityID = entityID;
                        emailRepository.Save(email);
                    }
                }

                if (organization.Entity.EntityMedias != null && organization.Entity.EntityMedias.Count > 0)
                {
                    using (var mediaRepository = new EntityMediaRepository())
                    {
                        var media = organization.Entity.EntityMedias.FirstOrDefault();
                        media.EntityID = entityID;
                        mediaRepository.Save(media);
                    }
                }

                if (organization.Entity.EntityType == (int)Enums.EntityTypes.Organization)
                {
                    if (AddGeneralSettings(entityID) < 0)
                    {
                        organizationId = -1;
                        return Types.Enums.OrganizationSaveStatus.Error;
                    }
                    if (AddSeedData(entityID) < 0)
                    {
                        organizationId = -1;
                        return Types.Enums.OrganizationSaveStatus.Error;
                    }
                    if (AddPrivacySettings(_SqlTransaction, entityID) < 0)
                    {
                        organizationId = -1;
                        return Types.Enums.OrganizationSaveStatus.Error;
                    }
                    if (AddNetworkSettings(_SqlTransaction, entityID) < 0)
                    {
                        organizationId = -1;
                        return Types.Enums.OrganizationSaveStatus.Error;
                    }
                }
                organizationId = organizationID;
                return Types.Enums.OrganizationSaveStatus.Success;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SaveOrganization");
                organizationId = -1;
                return Types.Enums.OrganizationSaveStatus.Error;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }


        private int AddSeedData(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("AddSeedData");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                if (_Database.ExecuteNonQuery(objCommand) > 0)
                    return 1;
                return -1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddDefaultAssociations");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private int AddNetworkSettings(DbTransaction _SqlTransaction, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("AddNetworkSettings");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@EntityType", DbType.Int64, (int)Enums.EntityTypes.Organization);
                if (_Database.ExecuteNonQuery(objCommand) > 0)
                    return 1;
                return -1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddNetworkSettings");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private int AddPrivacySettings(DbTransaction _SqlTransaction, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("AddPrivacySettings");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@EntityType", DbType.Int64, (int)Enums.EntityTypes.Organization);
                if (_Database.ExecuteNonQuery(objCommand) > 0)
                    return 1;
                return -1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddPrivacySettings");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private int AddGeneralSettings(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("AddGeneralSettings");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@EntityType", DbType.Int64, (int)Enums.EntityTypes.Organization);
                if (_Database.ExecuteNonQuery(objCommand) > 0)
                    return 1;
                return -1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddGeneralSettings");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private void AddProfile(Models.Organization organization, long organizationID)
        {
            var profile = organization.OrganizationProfiles.FirstOrDefault();
            var objCommand = _Database.GetStoredProcCommand("AddOrganizationProfile");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@OrganizationID", DbType.Int64, organizationID);
                _Database.AddInParameter(objCommand, "@Website", DbType.String, profile.Website);
                _Database.AddInParameter(objCommand, "@ProfileName", DbType.String, profile.ProfileName);
                _Database.AddInParameter(objCommand, "@Description", DbType.String, profile.Description);
                _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, profile.IpAddress);
                _Database.AddInParameter(objCommand, "@TagLine", DbType.String, profile.TagLine);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, profile.CreatedOn);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, profile.CreatedBy);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, profile.EditedOn);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, profile.EditedBy);
                _Database.AddInParameter(objCommand, "@Discoverable", DbType.Boolean, profile.Discoverable);
                _Database.AddInParameter(objCommand, "@ProfileImagePath", DbType.String, profile.ProfileImagePath);
                _Database.AddInParameter(objCommand, "@RequirePermission", DbType.Boolean, profile.RequirePermission);
                _Database.ExecuteNonQuery(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddProfile");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        internal long? GetOrganizationID(string name)
        {
            var objCommand = _Database.GetStoredProcCommand("GetOrganizationID");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Name", DbType.String, name);
                var reader = _Database.ExecuteReader(objCommand);
                while (reader.Read())
                {
                    return Convert.ToInt64(reader["OrganizationID"]);
                }
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetOrganizationID");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return default(long?);
        }

        internal async Task<long> CreateMinimal(Models.Organization organization)
        {
            var objCommand = _Database.GetStoredProcCommand("AddOrganization");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            long entityID = 0;
            long organizationID = 0;
            try
            {
                using (var commonRepository = new CommonRepository())
                {
                    entityID = commonRepository.AddEntity(organization.Entity);
                    if (entityID > 0)
                    {
                        _Database.AddOutParameter(objCommand, "@OrganizationID", DbType.String, int.MaxValue);
                        _Database.AddInParameter(objCommand, "@Name", DbType.String, organization.Name);
                        _Database.AddInParameter(objCommand, "@EstablishedYear", DbType.Int16, organization.EstablishedYear);
                        _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                        _Database.AddInParameter(objCommand, "@CategoryID", DbType.String, organization.CategoryID);
                        _Database.AddInParameter(objCommand, "@EmployeeStrength", DbType.String, organization.EmployeeStrength);
                        _Database.AddInParameter(objCommand, "@Type", DbType.Int32, organization.Type);
                        if (_Database.ExecuteNonQuery(objCommand) > 0)
                        {
                            organizationID = Convert.ToInt64(objCommand.Parameters["@OrganizationID"].Value);

                            AddProfile(organization, organizationID);
                        }
                    }
                    return organizationID;
                }
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SaveOrganization");
                organizationID = -1;
                return organizationID;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> IndustryByOrganization(long organizationID)
        {
            var objCommand = _Database.GetSqlStringCommand("Select CategoryID from Organization where OrganizationID = @OrganizationID");
            _Database.AddInParameter(objCommand, "@OrganizationID", DbType.String, organizationID);
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                var reader = _Database.ExecuteReader(objCommand);
                while (reader.Read())
                {
                    return Convert.ToInt64(reader["CategoryID"]);
                }
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "IndustryByOrganization");
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return default(long);
        }

        public async Task<DataSet> GetProfileWithStates(string profilename, long currentEntityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Organization_GetProfileWithStates");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ProfileName", DbType.String, profilename);
                _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Boolean, currentEntityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetProfileWithStates");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetAddress(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Entity_GetAddress");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetAddress");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> UpdateOrganizationItems(Dictionary<string, string> items, Dictionary<string, string> profileItems, long organizationID)
        {
            DbCommand objCommand = null;
            try
            {
                if (_SqlTransaction == null)
                    StartTransaction();
                string sql = "Update Organization set";
                int count = 1;
                if (items != null && items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        if (count == items.Count)
                            sql += string.Format(" {0}=@{1}", item.Key, item.Key);
                        else
                            sql += string.Format(" {0}=@{1},", item.Key, item.Key);
                        count++;
                    }

                    sql += " where OrganizationID=@OrganizationID";
                    objCommand = _Database.GetSqlStringCommand(sql);
                    _Database.AddInParameter(objCommand, "@OrganizationID", DbType.String, organizationID);
                    foreach (var item in items)
                    {
                        _Database.AddInParameter(objCommand, "@" + item.Key, DbType.String, item.Value);
                    }
                    _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                }
                if (profileItems != null && profileItems.Count > 0)
                {
                    sql = "Update OrganizationProfile set";
                    count = 1;
                    foreach (var item in profileItems)
                    {
                        if (count == profileItems.Count)
                            sql += string.Format(" {0}=@{1}", item.Key, item.Key);
                        else
                            sql += string.Format(" {0}=@{1},", item.Key, item.Key);
                        count++;
                    }

                    sql += " where OrganizationID=@OrganizationID";
                    objCommand = _Database.GetSqlStringCommand(sql);
                    _Database.AddInParameter(objCommand, "@OrganizationID", DbType.String, organizationID);
                    foreach (var item in profileItems)
                    {
                        _Database.AddInParameter(objCommand, "@" + item.Key, DbType.String, item.Value);
                    }
                    _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                }
                CommitTransaction();
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetCareerLine");
                RollbackTransaction();
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetProfile(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Organization_GetProfile");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetProfile");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetProfileWithStates(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Organization_GetProfileWithStatesByEntity");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetProfileWithStates");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> SavePhone(Phone phoneModel, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("UpdatePhone");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@entityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@PhoneID", DbType.Int64, phoneModel.PhoneID);
                _Database.AddInParameter(objCommand, "@Phone1", DbType.String, phoneModel.Phone1);
                _Database.AddInParameter(objCommand, "@PhoneType", DbType.String, phoneModel.PhoneType);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, phoneModel.EditedOn);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, phoneModel.EditedBy);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SavePhone");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetSimilar(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Organization_Similar");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetSimilar");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetOpenProfile(string profileName)
        {
            var objCommand = _Database.GetStoredProcCommand("GetBusinessProfileWithStates_Public");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ProfileName", DbType.String, profileName);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetOpenProfile");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> States(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Business_EntityStatistics");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "States");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> EmailConfiguration(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("EmailConfiguration");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "EmailConfiguration");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> Groups(string keyword, long currentEntityID, string id = null, long? groupID = null)
        {
            DbCommand objCommand = null;
            try
            {
                if (groupID > 0)
                {
                    objCommand = _Database.GetStoredProcCommand("Groups_OneBasic");
                    objCommand.CommandTimeout = Constants.TIMEOUT;
                    _Database.AddInParameter(objCommand, "@GroupID", DbType.Int64, groupID);
                    _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Int64, currentEntityID);
                    return _Database.ExecuteDataSet(objCommand);
                }
                else if (!string.IsNullOrEmpty(id))
                {
                    objCommand = _Database.GetStoredProcCommand("Groups_One");
                    objCommand.CommandTimeout = Constants.TIMEOUT;
                    _Database.AddInParameter(objCommand, "@Name", DbType.String, id);
                    _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Int64, currentEntityID);
                    return _Database.ExecuteDataSet(objCommand);
                }
                else
                {
                    objCommand = _Database.GetStoredProcCommand("Groups_All");
                    _Database.AddInParameter(objCommand, "@Keyword", DbType.String, keyword);
                    _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Int64, currentEntityID);
                    objCommand.CommandTimeout = Constants.TIMEOUT;
                    return _Database.ExecuteDataSet(objCommand);
                }
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Groups");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> Members(long currentEntityID, long groupEntityID, int status, int pageNumber, int pageSize, int type)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("Association_Members");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, groupEntityID);
                _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Int64, currentEntityID);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int32, status);
                _Database.AddInParameter(objCommand, "@PageNumber", DbType.Int32, pageNumber);
                _Database.AddInParameter(objCommand, "@PageSize", DbType.Int32, pageSize);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int32, type);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Members");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}
