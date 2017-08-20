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
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public async Task<DataSet> GetProfileWithStates(string userName, bool isProfileName, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetProfileWithStates");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@UserName", DbType.String, userName);
                _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@IsProfileName", DbType.Boolean, isProfileName);
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

        public async Task<DataSet> GetProfile(string userName, bool isUserID = false)
        {
            var objCommand = _Database.GetStoredProcCommand("GetProfile");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Identifier", DbType.String, userName);
                _Database.AddInParameter(objCommand, "@IsID", DbType.Boolean, isUserID);
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

        public bool IsUserProfileNameExist(string profileName)
        {
            var objCommand = _Database.GetStoredProcCommand("IsProfileNameExist");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ProfileName", DbType.String, profileName);
                var reader = _Database.ExecuteReader(objCommand);
                while (reader.Read())
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetFollowers");
                return false;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public long AddExtendedInfo(Person extended)
        {
            var objCommand = _Database.GetStoredProcCommand("AddExtendedInfo");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            if (_SqlTransaction == null)
                StartTransaction();
            try
            {
                using (var commonRepository = new CommonRepository())
                {
                    var entityID = commonRepository.AddEntity(extended.Entity, _SqlTransaction);
                    long personID = 0;
                    if (entityID > 0)
                    {
                        _Database.AddOutParameter(objCommand, "@PersonID", DbType.String, int.MaxValue);
                        _Database.AddInParameter(objCommand, "@DateOfBirth", DbType.DateTime, extended.DateOfBirth);
                        _Database.AddInParameter(objCommand, "@Gender", DbType.Int16, extended.Gender);
                        _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                        _Database.AddInParameter(objCommand, "@UserID", DbType.String, extended.UserID);
                        _Database.AddInParameter(objCommand, "@FirstName", DbType.String, extended.FirstName);
                        _Database.AddInParameter(objCommand, "@LastName", DbType.String, extended.LastName);
                        if (_Database.ExecuteNonQuery(objCommand, _SqlTransaction) > 0)
                        {
                            personID = Convert.ToInt64(objCommand.Parameters["@PersonID"].Value);
                            var result = AddProfileInfo(extended.PersonProfiles, personID, _SqlTransaction);
                            if (result < 0)
                            {
                                RollbackTransaction();
                                return -1;
                            }
                        }
                        else
                        {
                            RollbackTransaction();
                            return -1;
                        }

                        if (AddWidgets(_SqlTransaction, entityID) < 0)
                        {
                            RollbackTransaction();
                            return -1;
                        }
                        if (AddGeneralSettings(_SqlTransaction, entityID, personID) < 0)
                        {
                            RollbackTransaction();
                            return -1;
                        }
                        if (AddSeedData(_SqlTransaction, entityID) < 0)
                        {
                            RollbackTransaction();
                            return -1;
                        }
                        if (AddPrivacySettings(_SqlTransaction, entityID, personID) < 0)
                        {
                            RollbackTransaction();
                            return -1;
                        }
                        if (AddNetworkSettings(_SqlTransaction, entityID, personID) < 0)
                        {
                            RollbackTransaction();
                            return -1;
                        }
                        if (AddJobSearchSettings(_SqlTransaction, entityID, personID) < 0)
                        {
                            RollbackTransaction();
                            return -1;
                        }
                        if (AddBasicSubscription(_SqlTransaction, entityID, personID) < 0)
                        {
                            RollbackTransaction();
                            return -1;
                        }
                        if (AddDefaultAssociations(_SqlTransaction, entityID) < 0)
                        {
                            RollbackTransaction();
                            return -1;
                        }
                    }
                    else
                    {
                        RollbackTransaction();
                        return -1;
                    }
                    CommitTransaction();
                    return entityID;
                }
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddExtendedInfo");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private int AddSeedData(DbTransaction _SqlTransaction, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("AddSeedData");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                if (_Database.ExecuteNonQuery(objCommand, _SqlTransaction) > 0)
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

        private int AddDefaultAssociations(DbTransaction _SqlTransaction, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("AddDefaultAssociations");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                if (_Database.ExecuteNonQuery(objCommand, _SqlTransaction) > 0)
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

        private int AddBasicSubscription(DbTransaction _SqlTransaction, long entityID, long personID)
        {
            var objCommand = _Database.GetStoredProcCommand("AddIndividualBasicSubscription");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                if (_Database.ExecuteNonQuery(objCommand, _SqlTransaction) > 0)
                    return 1;
                return -1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddBasicSubscription");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> AddWidgets(long entityID, List<EntityWidget> widgets)
        {
            DbCommand objCommand = null;
            if (_SqlTransaction == null)
                StartTransaction();
            try
            {
                foreach (var item in widgets)
                {
                    objCommand = _Database.GetStoredProcCommand("AddPersonWidget");
                    objCommand.CommandTimeout = Constants.TIMEOUT;
                    _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                    _Database.AddInParameter(objCommand, "@WidgetID", DbType.Int64, item.WidgetID);
                    _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                }
                CommitTransaction();
                return 1;
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddWidgets");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private int AddWidgets(DbTransaction _SqlTransaction, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("AddIndividualWidgets");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                if (_Database.ExecuteNonQuery(objCommand, _SqlTransaction) > 0)
                    return 1;
                return -1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddWidgets");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private int AddJobSearchSettings(DbTransaction _SqlTransaction, long entityID, long personID)
        {
            var objCommand = _Database.GetStoredProcCommand("AddJobSearchSettings");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                if (_Database.ExecuteNonQuery(objCommand, _SqlTransaction) > 0)
                    return 1;
                return -1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddJobSearchSettings");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        private int AddNetworkSettings(DbTransaction _SqlTransaction, long entityID, long personID)
        {
            var objCommand = _Database.GetStoredProcCommand("AddNetworkSettings");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@EntityType", DbType.Int64, (int)Enums.EntityTypes.Person);
                if (_Database.ExecuteNonQuery(objCommand, _SqlTransaction) > 0)
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

        private int AddPrivacySettings(DbTransaction _SqlTransaction, long entityID, long personID)
        {
            var objCommand = _Database.GetStoredProcCommand("AddPrivacySettings");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@EntityType", DbType.Int64, (int)Enums.EntityTypes.Person);
                if (_Database.ExecuteNonQuery(objCommand, _SqlTransaction) > 0)
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

        private int AddGeneralSettings(DbTransaction _SqlTransaction, long entityID, long personID)
        {
            var objCommand = _Database.GetStoredProcCommand("AddGeneralSettings");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@EntityType", DbType.Int64, (int)Enums.EntityTypes.Person);
                if (_Database.ExecuteNonQuery(objCommand, _SqlTransaction) > 0)
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



        private int AddProfileInfo(ICollection<PersonProfile> collection, long personID, DbTransaction transaction)
        {
            if (collection != null && collection.Count > 0)
            {
                DbCommand objCommand = null;
                try
                {
                    var item = collection.FirstOrDefault();
                    objCommand = _Database.GetStoredProcCommand("AddProfileInfo");
                    objCommand.CommandTimeout = Constants.TIMEOUT;
                    _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, item.IpAddress);
                    _Database.AddInParameter(objCommand, "@LastModified", DbType.DateTime, DateTime.UtcNow);
                    _Database.AddInParameter(objCommand, "@PersonID", DbType.String, personID);
                    _Database.AddInParameter(objCommand, "@ProfileSummary", DbType.String, item.ProfileSummary);
                    _Database.AddInParameter(objCommand, "@ProfileName", DbType.String, item.ProfileName);
                    _Database.AddInParameter(objCommand, "@FormatedName", DbType.String, item.FormatedName);
                    _Database.AddInParameter(objCommand, "@NameFormat", DbType.String, item.NameFormat);
                    _Database.AddInParameter(objCommand, "@EmailVarified", DbType.String, item.EmailVarified);
                    _Database.ExecuteNonQuery(objCommand, transaction);
                    return 1;
                }
                catch (Exception ex)
                {
                    ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddProfileInfo");
                    return -1;
                }
                finally
                {
                    if (objCommand != null) { objCommand.Dispose(); }
                }
            }
            return 1;
        }

        public int UpdateProfile(string query)
        {
            var objCommand = _Database.GetSqlStringCommand(query);
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                return _Database.ExecuteNonQuery(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetFollowers");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public DataSet GetUserHashData(string userName)
        {
            var objCommand = _Database.GetStoredProcCommand("UserHash");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@UserName", DbType.String, userName);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetUserHashData");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetStates(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetStates");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetStates");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetBasicUserProfile(long personID, string column)
        {
            var objCommand = _Database.GetSqlStringCommand(string.Format("Select {0} from personProfile where personID={1}", column, personID));
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetStates");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> SaveProfileInfo(string colunName, string value, long personID)
        {
            var objCommand = _Database.GetSqlStringCommand(string.Format("Update  personProfile set {0}='{1}' where personID={2}", colunName, value, personID));
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                return _Database.ExecuteNonQuery(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetStates");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetAdvancedCareerLine(long entityID, bool includeS = true, bool includeA = true)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAdvancedCareerLine");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@IncludeAward", DbType.Boolean, includeA);
                _Database.AddInParameter(objCommand, "@IncludeStudy", DbType.Boolean, includeS);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetCareerLine");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> Intelligence(long personID, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_Intelligence");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@PersonID", DbType.Int64, personID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Intelligence");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<bool> SkipIntelligence(long entityID, short type, int action)
        {
            DbCommand objCommand = null;
            try
            {
                objCommand = _Database.GetStoredProcCommand("AddIntelligenceAction");
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@entityID", DbType.String, entityID);
                _Database.AddInParameter(objCommand, "@Type", DbType.String, type);
                _Database.AddInParameter(objCommand, "@Action", DbType.String, action);
                _Database.ExecuteNonQuery(objCommand);
                return true;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "AddProfileInfo");
                return false;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public void UpdateHeading(string profileSummary, long personID)
        {
            var query = string.Format("update personprofile set profileheading='{0}' where personID={1}", profileSummary, personID);
            UpdateProfile(query);
        }

        public async Task<int> UpdateProfileItems(Dictionary<string, string> profileItems, long personID, Dictionary<string, string> personItems = null)
        {
            DbCommand objCommand = null;
            try
            {
                if (_SqlTransaction == null)
                    StartTransaction();
                string sql = "Update PersonProfile set";
                int count = 1;
                foreach (var item in profileItems)
                {
                    if (count == profileItems.Count)
                        sql += string.Format(" {0}=@{1}", item.Key, item.Key);
                    else
                        sql += string.Format(" {0}=@{1},", item.Key, item.Key);
                    count++;
                }

                sql += " where PersonID=@PersonID";
                objCommand = _Database.GetSqlStringCommand(sql);
                _Database.AddInParameter(objCommand, "@PersonID", DbType.String, personID);
                foreach (var item in profileItems)
                {
                    _Database.AddInParameter(objCommand, "@" + item.Key, DbType.String, item.Value);
                }
                _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                if (personItems != null && personItems.Count > 0)
                {
                    sql = "Update Person set";
                    count = 1;
                    foreach (var item in personItems)
                    {
                        if (count == personItems.Count)
                            sql += string.Format(" {0}=@{1}", item.Key, item.Key);
                        else
                            sql += string.Format(" {0}=@{1},", item.Key, item.Key);
                        count++;
                    }

                    sql += " where PersonID=@PersonID";
                    objCommand = _Database.GetSqlStringCommand(sql);
                    _Database.AddInParameter(objCommand, "@PersonID", DbType.String, personID);
                    foreach (var item in personItems)
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

        public async Task<DataSet> GetCommonSkills(long firstEntityID, long secondEntityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_CommonEntitySkill");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@FirstEntityID", DbType.Int64, firstEntityID);
                _Database.AddInParameter(objCommand, "@SecondEntityID", DbType.Int64, secondEntityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetCommonSkills");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetCommonCompanies(long firstEntityID, long secondEntityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_CommonCompanies");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@FirstEntityID", DbType.Int64, firstEntityID);
                _Database.AddInParameter(objCommand, "@SecondEntityID", DbType.Int64, secondEntityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetCommonCompanies");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> GetCurrentStepOfAward(long personID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_WizardStep");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@FirstEntityID", DbType.Int64, personID);
                var reader = _Database.ExecuteScalar(objCommand);
                return Convert.ToInt32(reader);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetCommonCompanies");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<bool> IsValidWizardStep(long personID, int step)
        {
            return await GetCurrentStepOfAward(personID) == step;
        }

        public async Task<DataSet> SnapShot(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_WizardSnapShot");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetCommonCompanies");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetProfile(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetEntityProfile");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
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

        public async Task<DataSet> FindPersonDetailByVarification(long personID, string id, string emailAddress)
        {
            var query = "Select * from PersonProfile JOIN Person on Person.PersonID = PersonProfile.PersonID JOIN aspnetusers on aspnetusers.Id = Person.UserID where Person.PersonID = @PersonID AND aspnetusers.UserName=@EmailAddress";
            var objCommand = _Database.GetSqlStringCommand(query);
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@PersonID", DbType.Int64, personID);
                _Database.AddInParameter(objCommand, "@EmailAddress", DbType.String, emailAddress);
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

        public async Task<DataSet> HoverCard(string id, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Hovercard");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, id);
                _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "HoverCard");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetUserByEmailAddress(string emailAddress)
        {
            var objCommand = _Database.GetStoredProcCommand("GetUserByEmailAddress");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@emailAddress", DbType.String, emailAddress);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetUserByEmailAddress");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> SetForgotpasswordData(string emailAddress, long entityID, string hash)
        {
            var objCommand = _Database.GetStoredProcCommand("SetProfile_ForgottenPassword");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EmailAddress", DbType.String, emailAddress);
                _Database.AddInParameter(objCommand, "@Hash", DbType.String, hash);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "PrepareForForgottenPassword");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetUserByForgottPassword(string userHash, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetUserByForgottPassword");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Hash", DbType.String, userHash);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetUserByForgottPassword");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }



        public async Task<int> PrepareSuggestions(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("PrepareStockForEntity");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "PrepareSuggestions");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> PrepareWhatsOn(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Create_NetworkActivites");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "PrepareWhatsOn");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public DataSet GetValidationContext(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetValidationContext");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "PrepareWhatsOn");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> SetLastMessageSeenData(DateTime now, long entityID)
        {
            string sql = "Update EntityProfile set LastMessagesSeen=@LastMessagesSeen where EntityID=@EntityID";
            DbCommand objCommand = _Database.GetSqlStringCommand(sql);
            try
            {
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@LastMessagesSeen", DbType.DateTime, now);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SetLastLoginData");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> SetLastNotificationSeenData(DateTime now, long entityID)
        {
            string sql = "Update EntityProfile set LastNotificationSeen=@LastNotificationSeen where EntityID=@EntityID";
            DbCommand objCommand = _Database.GetSqlStringCommand(sql);
            try
            {
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@LastNotificationSeen", DbType.DateTime, now);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SetLastLoginData");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> SetLastLoginData(DateTime now, long entityID)
        {
            string sql = "Update EntityProfile set LastLoggedIn=@LastLoggedIn where EntityID=@EntityID";
            DbCommand objCommand = _Database.GetSqlStringCommand(sql);
            try
            {
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@LastLoggedIn", DbType.DateTime, now);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SetLastLoginData");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> SetLastRequestSeenData(DateTime now, long entityID)
        {
            string sql = "Update EntityProfile set LastRequestSeen=@LastRequestSeen where EntityID=@EntityID";
            DbCommand objCommand = _Database.GetSqlStringCommand(sql);
            try
            {
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@LastRequestSeen", DbType.DateTime, now);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "SetLastLoginData");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> UpdateProfileScore(long entityID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("UpdateProfileScore");
            try
            {
                objCommand.CommandTimeout = Constants.TIMEOUT;
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "UpdateProfileScore");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }


        public async Task<DataSet> GetOpenProfile(string profileName)
        {
            var objCommand = _Database.GetStoredProcCommand("GetProfileWithStates_Public");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@UserName", DbType.String, profileName);
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

        public async Task<DataSet> EntityFace(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("EntityFace");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "EntityFace");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<bool> ShowContactImport(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("ShowContactImport");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                var reader = _Database.ExecuteReader(objCommand);
                while (reader.Read())
                    return reader.GetBoolean(0);
                return false;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Intelligence");
                return true;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> States(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Person_EntityStatistics");
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

        public async Task<int> VarificationSent(long entityID)
        {
            var objCommand = _Database.GetSqlStringCommand("select count(1) from personprofile join person on person.personID = personprofile.personID where entityID = @EntityID and lastvarificationsent is not null");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                var reader = _Database.ExecuteScalar(objCommand);
                return Convert.ToInt32(reader);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "VarificationSent");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetChatProfile(string userID)
        {
            var objCommand = _Database.GetStoredProcCommand("GetChatProfile");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Identifier", DbType.String, userID);
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


        public async Task<bool> VarificationDone(long entityID)
        {
            var objCommand = _Database.GetSqlStringCommand("select count(1) from personprofile join person on person.personID = personprofile.personID where entityID = @EntityID and EmailVarified=1");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                var reader = _Database.ExecuteScalar(objCommand);
                return Convert.ToInt32(reader) > 0;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "VarificationDone");
                return false;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetBasicCareerLine(long entityID, bool includeS)
        {
            var objCommand = _Database.GetStoredProcCommand("GetBasicCareerLine");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@IncludeStudy", DbType.Boolean, includeS);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetBasicCareerLine");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> EntityTrack(Models.EntityState interaction)
        {
            DbCommand insert = null;
            insert = _Database.GetStoredProcCommand("P_EntityState_Insert");
            insert.CommandTimeout = Constants.TIMEOUT;
            _Database.AddInParameter(insert, "@CreatedOn", DbType.DateTime, interaction.CreatedOn);
            _Database.AddInParameter(insert, "@Description", DbType.String, interaction.Description);
            _Database.AddInParameter(insert, "@EntityID", DbType.Int64, interaction.EntityID);
            _Database.AddInParameter(insert, "@EntityID2", DbType.Int64, interaction.EntityID2);
            _Database.AddInParameter(insert, "@ReferenceEntityID", DbType.Int64, interaction.ReferenceEntityID);
            _Database.AddInParameter(insert, "@Type", DbType.Int16, interaction.Type);
            _Database.AddInParameter(insert, "@IpAddress", DbType.String, interaction.IpAddress);
            _Database.AddInParameter(insert, "@Status", DbType.Int16, interaction.Status);
            try
            {
                _Database.ExecuteNonQuery(insert);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "EntityTrack");
                return -1;
            }
            finally
            {
                if (insert != null) { insert.Dispose(); }
            }
        }

        public async Task<DataSet> Insights(int pageNumber, byte insighType, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("P_EntityState_Get");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@pageNumber", DbType.Int32, pageNumber);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, insighType);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Insights");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> Unsubscribe
                        (
                            string email,
                            long? entityID,
                            int emailType,
                            DateTime updatedOn,
                            bool subscribed,
                            string ipAddress
                        )
        {
            DbCommand insert = null;
            insert = _Database.GetStoredProcCommand("EmailSettings_Save");
            insert.CommandTimeout = Constants.TIMEOUT;
            _Database.AddInParameter(insert, "@UpdatedOn", DbType.DateTime, updatedOn);
            _Database.AddInParameter(insert, "@Email", DbType.String, email);
            _Database.AddInParameter(insert, "@IpAddress", DbType.String, ipAddress);
            _Database.AddInParameter(insert, "@EntityID", DbType.Int64, entityID);
            _Database.AddInParameter(insert, "@EmailType", DbType.Int32, emailType);
            _Database.AddInParameter(insert, "@Subscribed", DbType.Boolean, subscribed);
            try
            {
                _Database.ExecuteNonQuery(insert);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "Unsubscribe");
                return -1;
            }
            finally
            {
                if (insert != null) { insert.Dispose(); }
            }
        }

        public async Task<DataSet> GetProfileDataToImprove(long entityID)
        {
            DbCommand insert = null;
            insert = _Database.GetSqlStringCommand("Select * from dbo.[ProfilePercentage](@EntityID)");
            insert.CommandTimeout = Constants.TIMEOUT;
            _Database.AddInParameter(insert, "@EntityID", DbType.Int64, entityID);
            try
            {
                return _Database.ExecuteDataSet(insert);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetProfileDataToImprove");
                return null;
            }
            finally
            {
                if (insert != null) { insert.Dispose(); }
            }
        }
    }
}
