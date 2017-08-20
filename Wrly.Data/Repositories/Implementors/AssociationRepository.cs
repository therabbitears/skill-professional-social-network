using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using Wrly.Data.Models;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class AssociationRepository : BaseRepository
    {
        public async Task<DataSet> GetSuggestions(long entityID, int pageNumber, int pageSize, int? algorithm = null)
        {
            DbCommand objCommand = null;
            if (true)
            {
                if (algorithm == (int)Enums.NetworkCoverageLevel.BestMatch)
                {
                    objCommand = _Database.GetStoredProcCommand("GetSuggestionsByEntity_Score");
                }
                else if (algorithm == (int)Enums.NetworkCoverageLevel.ToSkill)
                {
                    objCommand = _Database.GetStoredProcCommand("GetSuggestionsByEntity_Skill");
                }
                else if (algorithm == (int)Enums.NetworkCoverageLevel.ToJobFunction)
                {
                    objCommand = _Database.GetStoredProcCommand("GetSuggestionsByEntity_Role");
                }
                else if (algorithm == (int)Enums.NetworkCoverageLevel.ToIndustry)
                {
                    objCommand = _Database.GetStoredProcCommand("GetSuggestionsByEntity_Industry");
                }
                else
                {
                    objCommand = _Database.GetStoredProcCommand("GetSuggestionsByEntityDefault");
                }
            }

            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@PageNumber", DbType.Int64, pageNumber);
                _Database.AddInParameter(objCommand, "@PageSize", DbType.Int64, pageSize);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetReference");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> SaveAssociation(Models.Association association, bool keepTransactionContinue = false)
        {
            var objCommand = _Database.GetStoredProcCommand("SaveAssociationRequest");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                if (_SqlTransaction == null)
                    StartTransaction();

                _Database.AddOutParameter(objCommand, "@AssociationID", DbType.Int64, int.MaxValue);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, association.EntityID);
                _Database.AddInParameter(objCommand, "@EntityID2", DbType.Int64, association.EntityID2);
                _Database.AddInParameter(objCommand, "@AssociationType", DbType.Int16, association.AssociationType);
                _Database.AddInParameter(objCommand, "@ObjectStatus", DbType.Int16, association.ObjectStatus);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, association.CreatedBy);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, association.CreatedOn);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, association.EditedBy);
                _Database.AddInParameter(objCommand, "@OppositeRowID", DbType.String, association.OppositeRowID);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, association.EditedOn);
                _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, association.IpAddress);
                _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                var id = Convert.ToInt32(objCommand.Parameters["@AssociationID"].Value);
                if (!keepTransactionContinue && id > 0)
                {
                    CommitTransaction();
                    _SqlTransaction = null;
                }
                return id;
            }
            catch (Exception ex)
            {
                if (!keepTransactionContinue)
                    RollbackTransaction();

                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ForUser");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }



        public async Task<long> UpdateAssociation(Association association, bool keepTransactionContinue = false)
        {
            if (_SqlTransaction == null)
                StartTransaction();

            var objCommand = _Database.GetStoredProcCommand("UpdateAssociationRequest");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@AssociationID", DbType.Int64, association.AssociationID);
                _Database.AddInParameter(objCommand, "@ObjectStatus", DbType.Int16, association.ObjectStatus);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, association.EditedBy);
                _Database.AddInParameter(objCommand, "@OppositeRowID", DbType.Int64, association.OppositeRowID);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, association.EditedOn);
                var result = _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                if (!keepTransactionContinue && result > 0)
                    CommitTransaction();
                return result;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ForUser");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> ApproveRequest(Models.Association request, Association response)
        {
            StartTransaction();
            var result = await SaveAssociation(response, true);
            if (result > 0)
            {
                request.OppositeRowID = result;
                if (await UpdateAssociation(request, true) > 0)
                {
                    CommitTransaction();
                    return request.AssociationID;
                }
                else
                    RollbackTransaction();
            }
            else
                RollbackTransaction();
            return -1;
        }

        public async Task<long> RejectRequest(Models.Association request, Association response)
        {
            StartTransaction();
            var result = await SaveAssociation(response, true);
            if (result > 0)
            {
                request.OppositeRowID = result;
                if (await UpdateAssociation(request, true) > 0)
                {
                    CommitTransaction();
                    return request.AssociationID;
                }
                else
                    RollbackTransaction();
            }
            else
                RollbackTransaction();
            return -1;
        }

        public async Task<DataSet> GetSingleAsObject(long id)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAssociationAsObject");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@AssociationID", DbType.Int64, id);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetSingleAsObject");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetRequests(long entityID, int pageNumber, int pageSize, byte status, byte direction)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAssociation_Requests");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@PageNumber", DbType.Int64, pageNumber);
                _Database.AddInParameter(objCommand, "@PapgeSize", DbType.Int16, pageSize);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int16, status);
                _Database.AddInParameter(objCommand, "@Direction", DbType.Int16, direction);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ForUser");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetExisitingAssociations(long entityID, int pageNumber, int pageSize, long? currentEntityID = null)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAssociations");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@PageNumber", DbType.Int64, pageNumber);
                _Database.AddInParameter(objCommand, "@PapgeSize", DbType.Int16, pageSize);
                _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Int64, currentEntityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ForUser");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetFollowers(long entityID, int pageNumber, int pageSize, byte direction, long? currentEntityID = null)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAssociation_Followers");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@PageNumber", DbType.Int64, pageNumber);
                _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Int64, currentEntityID);
                _Database.AddInParameter(objCommand, "@PapgeSize", DbType.Int16, pageSize);
                _Database.AddInParameter(objCommand, "@Direction", DbType.Int16, direction);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ForUser");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }



        public async Task<DataSet> GetSingleOppositeAsObject(long id)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAssociation_Opposite");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@AssociationID", DbType.Int64, id);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ForUser");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> RemoveAssociations(Association associationObject, Association oppositeAssociationObject)
        {
            try
            {
                StartTransaction();
                var result = await UpdateAssociation(associationObject, true);
                if (result > 0)
                {
                    result = await UpdateAssociation(oppositeAssociationObject, true);
                    if (result > 0)
                    {
                        CommitTransaction();
                        return 1;
                    }
                }
                RollbackTransaction();
                return -1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ForUser");
                RollbackTransaction();
                return -1;
            }
        }

        public async Task<long> RemoveStock(long? entity, long? entity2)
        {
            var objCommand = _Database.GetStoredProcCommand("RemoveFromStock");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@entity", DbType.Int64, entity);
                _Database.AddInParameter(objCommand, "@entity2", DbType.Int64, entity2);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "RemoveStock");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> ShuffleStock(long? entityID)
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
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ShuffleStock");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> Block(Association association)
        {
            var objCommand = _Database.GetStoredProcCommand("BlockAssociation");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                if (_SqlTransaction == null)
                    StartTransaction();

                _Database.AddOutParameter(objCommand, "@AssociationID", DbType.Int64, int.MaxValue);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, association.EntityID);
                _Database.AddInParameter(objCommand, "@EntityID2", DbType.Int64, association.EntityID2);
                _Database.AddInParameter(objCommand, "@AssociationType", DbType.Int16, association.AssociationType);
                _Database.AddInParameter(objCommand, "@ObjectStatus", DbType.Int16, association.ObjectStatus);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, association.CreatedBy);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, association.CreatedOn);
                _Database.AddInParameter(objCommand, "@EditedBy", DbType.String, association.EditedBy);
                _Database.AddInParameter(objCommand, "@OppositeRowID", DbType.String, association.OppositeRowID);
                _Database.AddInParameter(objCommand, "@EditedOn", DbType.DateTime, association.EditedOn);
                _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, association.IpAddress);
                _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                var id = Convert.ToInt32(objCommand.Parameters["@AssociationID"].Value);
                CommitTransaction();
                return id;
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Block");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetConnectionActions(long currentEntityID, long entityID2)
        {
            var objCommand = _Database.GetSqlStringCommand("Select * from ConnectionStatus_Fn(@EntityID,@EntityID2)");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, currentEntityID);
                _Database.AddInParameter(objCommand, "@EntityID2", DbType.Int64, entityID2);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetConnectionActions");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> UnBlock(long currentEntityID, long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("UnBlockAssociation");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                if (_SqlTransaction == null)
                    StartTransaction();

                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, currentEntityID);
                _Database.AddInParameter(objCommand, "@EntityID2", DbType.Int64, entityID);
                _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                CommitTransaction();
                return 1;
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "UnBlock");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetSingleAsObject(long entityID, long entityID2, int status)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAssociationAsObject_ByEntity");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@EntityID2", DbType.Int64, entityID2);
                _Database.AddInParameter(objCommand, "@Status", DbType.Int64, status);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetSingleAsObject");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> UnFollow(long currentEntity, long mainEntity)
        {
            var objCommand = _Database.GetStoredProcCommand("UnFollowAssociation");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                if (_SqlTransaction == null)
                    StartTransaction();

                _Database.AddInParameter(objCommand, "@EntityID2", DbType.Int64, currentEntity);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, mainEntity);
                _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                CommitTransaction();
                return 1;
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "UnBlock");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetNetworkHappenings(long entityID, int pageNumber, int pageSize)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_NetworkHappenings");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@pageSize", DbType.Int64, pageSize);
                _Database.AddInParameter(objCommand, "@pageNumber", DbType.Int64, pageNumber);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetNetworkHappenings");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> ActivityAction(ActivityAction action)
        {
            var objCommand = _Database.GetStoredProcCommand("Save_ActivityAction");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, action.EntityID);
                _Database.AddInParameter(objCommand, "@Action", DbType.Int64, action.Action);
                _Database.AddInParameter(objCommand, "@ID", DbType.Int64, action.ID);
                _Database.AddInParameter(objCommand, "@IpAddress", DbType.String, action.IpAddress);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, action.CreatedOn);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, action.CreatedBy);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ActivityAction");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> InsertSeedRequests(long entityID)
        {
            var objCommand = _Database.GetStoredProcCommand("Insert_SeedRequests");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.ExecuteNonQuery(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "InsertSeedRequests");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetMyHappenings(long entityID, int pageNumber, int pageSize)
        {
            var objCommand = _Database.GetStoredProcCommand("Get_MyNetworkHappenings");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@pageSize", DbType.Int64, pageSize);
                _Database.AddInParameter(objCommand, "@pageNumber", DbType.Int64, pageNumber);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetMyHappenings");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetExisitingAssociationsToInvite(string keyword, long groupEntityID, long currentEntityID, int pageNumber, int pageSize)
        {
            var objCommand = _Database.GetStoredProcCommand("GetAssociations_ForReference");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@GroupEntityID", DbType.Int64, groupEntityID);
                _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Int64, currentEntityID);
                _Database.AddInParameter(objCommand, "@PageNumber", DbType.Int64, pageNumber);
                _Database.AddInParameter(objCommand, "@Keyword", DbType.String, keyword);
                _Database.AddInParameter(objCommand, "@PapgeSize", DbType.Int16, pageSize);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ForUser");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> ApproveAll(long groupID, long currentEntityID)
        {
            if (_SqlTransaction == null)
                StartTransaction();

            var objCommand = _Database.GetStoredProcCommand("Association_ApproveAll");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@GroupEntityID", DbType.Int64, groupID);
                _Database.AddInParameter(objCommand, "@CurrentEntityID", DbType.Int64, currentEntityID);
                _Database.ExecuteNonQuery(objCommand, _SqlTransaction);
                CommitTransaction();
                return 1;
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "ApproveAll");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}
