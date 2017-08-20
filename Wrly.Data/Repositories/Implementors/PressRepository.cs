using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Data.Models;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class PressRepository : BaseRepository
    {

        public async Task<long> Save(Post post)
        {
            DbCommand insert = null;
            if (post.ID == 0)
            {
                insert = _Database.GetStoredProcCommand("P_Post_Insert");
                _Database.AddOutParameter(insert, "@ID", DbType.String, int.MaxValue);
                _Database.AddInParameter(insert, "@PostType", DbType.String, post.PostType);
            }
            else
            {
                insert = _Database.GetStoredProcCommand("P_Post_Update");
                _Database.AddInParameter(insert, "@ID", DbType.String, post.ID);
            }
            insert.CommandTimeout = Constants.TIMEOUT;
            _Database.AddInParameter(insert, "@Title", DbType.String, post.Title);
            _Database.AddInParameter(insert, "@ShortDescription", DbType.String, post.ShortDescription);
            _Database.AddInParameter(insert, "@Description", DbType.String, post.Description);
            _Database.AddInParameter(insert, "@EntityID", DbType.String, post.EntityID);
            _Database.AddInParameter(insert, "@Meta", DbType.String, post.Meta);
            _Database.AddInParameter(insert, "@UrlSlug", DbType.String, post.UrlSlug);
            _Database.AddInParameter(insert, "@Published", DbType.Boolean, post.Published);
            _Database.AddInParameter(insert, "@CreatedOn", DbType.DateTime, post.CreatedOn);
            _Database.AddInParameter(insert, "@EditedBy", DbType.String, post.EditedBy);
            _Database.AddInParameter(insert, "@EditedOn", DbType.DateTime, post.EditedOn);
            _Database.AddInParameter(insert, "@PrivacyLevel", DbType.Int16, post.PrivacyLevel);
            _Database.AddInParameter(insert, "@PublishOn", DbType.DateTime, post.PublishOn);
            _Database.AddInParameter(insert, "@SharingType", DbType.Int16, post.SharingType);
            _Database.AddInParameter(insert, "@CreatedBy", DbType.String, post.CreatedBy);
            _Database.AddInParameter(insert, "@FilePath", DbType.String, post.FilePath);
            _Database.AddInParameter(insert, "@DraftID", DbType.String, post.DraftID);
            _Database.AddInParameter(insert, "@AllowPublic", DbType.Boolean, post.AllowPublic);
            _Database.AddInParameter(insert, "@IPAddress", DbType.String, post.IPAddress);
            _Database.AddInParameter(insert, "@PostID", DbType.Int64, post.PostID);
            _Database.AddInParameter(insert, "@WritterID", DbType.Int64, post.WritterID);
            _Database.AddInParameter(insert, "@GroupEntityID", DbType.Int64, post.GroupEntityID);
            _Database.AddInParameter(insert, "@Status", DbType.Int64, post.Status);
            try
            {
                _Database.ExecuteNonQuery(insert);
                var postID = post.ID == 0 ? Convert.ToInt64(insert.Parameters["@ID"].Value) : post.ID;
                if (post.PostTags != null && post.PostTags.Count > 0)
                {
                    insert = _Database.GetStoredProcCommand("P_Post_DeleteTags");
                    _Database.AddInParameter(insert, "@PostID", DbType.String, postID);
                    _Database.ExecuteNonQuery(insert);
                    foreach (var item in post.PostTags)
                    {
                        insert = _Database.GetStoredProcCommand("P_Post_InsertTag");
                        _Database.AddInParameter(insert, "@PostID", DbType.String, postID);
                        _Database.AddInParameter(insert, "@TagID", DbType.String, item.TagID);
                        _Database.ExecuteNonQuery(insert);
                    }
                }

                if (post.Skills != null && post.Skills.Count > 0)
                {
                    insert = _Database.GetStoredProcCommand("P_Post_DeleteSkills");
                    _Database.AddInParameter(insert, "@PostID", DbType.String, postID);
                    _Database.ExecuteNonQuery(insert);
                    foreach (var item in post.Skills)
                    {
                        insert = _Database.GetStoredProcCommand("P_Post_InsertSkill");
                        _Database.AddInParameter(insert, "@PostID", DbType.String, postID);
                        _Database.AddInParameter(insert, "@SkillID", DbType.String, item.SkillID);
                        _Database.ExecuteNonQuery(insert);
                    }
                }

                if (post.JobTitles != null && post.JobTitles.Count > 0)
                {
                    insert = _Database.GetStoredProcCommand("P_Post_DeleteJobTitles");
                    _Database.AddInParameter(insert, "@PostID", DbType.String, postID);
                    _Database.ExecuteNonQuery(insert);
                    foreach (var item in post.JobTitles)
                    {
                        insert = _Database.GetStoredProcCommand("P_Post_InsertJobTitle");
                        _Database.AddInParameter(insert, "@PostID", DbType.String, postID);
                        _Database.AddInParameter(insert, "@JobTitleID", DbType.String, item.JobTitleID);
                        _Database.ExecuteNonQuery(insert);
                    }
                }
                return postID;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetCommercialProperties");
                return -1;
            }
            finally
            {
                if (insert != null) { insert.Dispose(); }
            }
        }

        public DataSet GetAllByType(int type)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("P_Post_GetAll");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@PostType", DbType.String, type);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetAllByType");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }


        public int RecordStatistics(PostInteraction interaction)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("RecordPostInteractions");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddOutParameter(objCommand, "@Result", DbType.String, int.MaxValue);
                _Database.AddInParameter(objCommand, "@UserName", DbType.String, interaction.CreatedBy);
                _Database.AddInParameter(objCommand, "@PostID", DbType.Int64, interaction.PostID);
                _Database.AddInParameter(objCommand, "@RecordedOn", DbType.DateTime, interaction.CreatedOn);
                _Database.AddInParameter(objCommand, "@InteractionType", DbType.Int64, interaction.InteractionType);
                _Database.ExecuteNonQuery(objCommand);
                return Convert.ToInt32(objCommand.Parameters["@Result"].Value);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "RecordStatistics");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }


        public DataSet GetQuestionOptions(int id, string user)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("P_Post_UserOptions");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@UserName", DbType.String, user);
                _Database.AddInParameter(objCommand, "@ID", DbType.Int64, id);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetQuestionOptions");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }


        public DataSet GetRepliesForPost(long? postID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("P_PostReplies_GetAllForPostID");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@PostID", DbType.String, postID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetRepliesForPost");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }


        public async Task<DataSet> GetDetails(long id, long entityID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("P_Post_GetOne");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ID", DbType.Int64, id);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetDetails");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }


        public async Task<long> InsertReply(PostReply reply)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("P_PostReplies_Insert");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddOutParameter(objCommand, "@ID", DbType.Int64, int.MaxValue);
                _Database.AddInParameter(objCommand, "@Active", DbType.Boolean, 1);
                _Database.AddInParameter(objCommand, "@CreatedOn", DbType.DateTime, reply.CreatedOn);
                _Database.AddInParameter(objCommand, "@CreatedBy", DbType.String, reply.CreatedBy);
                _Database.AddInParameter(objCommand, "@PostID", DbType.Int64, reply.PostID);
                _Database.AddInParameter(objCommand, "@Reply", DbType.String, reply.Reply);
                _Database.AddInParameter(objCommand, "@ReplyType", DbType.Int16, reply.ReplyType);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, reply.EntityID);
                _Database.AddInParameter(objCommand, "@ReplyID", DbType.Int64, reply.ReplyID);
                _Database.ExecuteNonQuery(objCommand);
                return Convert.ToInt64(_Database.GetParameterValue(objCommand, "@ID"));
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "InsertReply");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }

        }


        public async Task<DataSet> GetDetails(string url)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("P_Post_GetOne_ByUrl");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Url", DbType.String, url);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetDetails");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }


        public DataSet GetUserAnsweredQuestions(string userName)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("P_Post_RepliesGetForUser");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ProfileName", DbType.String, userName);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetUserAnsweredQuestions");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }

        }


        public DataSet GetUserAskedQuestions(string userName)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("P_Post_GetForUser");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ProfileName", DbType.String, userName);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetUserAskedQuestions");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> Feeds(long entityID, int pageNumber, int pageSize, DateTime? stamp)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_Feeds");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                _Database.AddInParameter(objCommand, "@PageNumber", DbType.Int64, pageNumber);
                _Database.AddInParameter(objCommand, "@PageSize", DbType.Int64, pageSize);
                _Database.AddInParameter(objCommand, "@TimeStamp", DbType.DateTime, stamp);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "Feeds");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> SingleReply(long replyID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_SingleReply");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ReplyID", DbType.String, replyID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "SingleReply");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> SingleFeed(long postID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_SinglePost");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@PostID", DbType.String, postID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "SingleReply");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> React(PostInteraction interaction)
        {
            DbCommand insert = null;
            insert = _Database.GetStoredProcCommand("P_Post_Intereact");
            insert.CommandTimeout = Constants.TIMEOUT;
            _Database.AddInParameter(insert, "@CreatedBy", DbType.String, interaction.CreatedBy);
            _Database.AddInParameter(insert, "@CreatedOn", DbType.DateTime, interaction.CreatedOn);
            _Database.AddInParameter(insert, "@Description", DbType.String, interaction.Description);
            _Database.AddInParameter(insert, "@EntityID", DbType.Int64, interaction.EntityID);
            _Database.AddInParameter(insert, "@EntityID2", DbType.Int64, interaction.EntityID2);
            _Database.AddInParameter(insert, "@InteractionType", DbType.Int16, interaction.InteractionType);
            _Database.AddInParameter(insert, "@PostID", DbType.Int64, interaction.PostID);
            _Database.AddInParameter(insert, "@IpAddress", DbType.String, interaction.IpAddress);
            try
            {
                _Database.ExecuteNonQuery(insert);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetCommercialProperties");
                return -1;
            }
            finally
            {
                if (insert != null) { insert.Dispose(); }
            }
        }

        public async Task<long> React(PostReplyInteraction interaction)
        {
            DbCommand insert = null;
            insert = _Database.GetStoredProcCommand("P_PostReply_Intereact");
            insert.CommandTimeout = Constants.TIMEOUT;
            _Database.AddInParameter(insert, "@CreatedBy", DbType.String, interaction.CreatedBy);
            _Database.AddInParameter(insert, "@CreatedOn", DbType.DateTime, interaction.CreatedOn);
            _Database.AddInParameter(insert, "@Description", DbType.String, interaction.Description);
            _Database.AddInParameter(insert, "@EntityID", DbType.Int64, interaction.EntityID);
            _Database.AddInParameter(insert, "@InteractionType", DbType.Int16, interaction.InteractionType);
            _Database.AddInParameter(insert, "@ReplyID", DbType.Int64, interaction.ReplyID);
            _Database.AddInParameter(insert, "@IpAddress", DbType.String, interaction.IpAddress);
            try
            {
                _Database.ExecuteNonQuery(insert);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetCommercialProperties");
                return -1;
            }
            finally
            {
                if (insert != null) { insert.Dispose(); }
            }
        }

        public async Task<DataSet> Replies(long? postID, long? replyID, int stock, int pageNumber, int pageSize, long entityID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_Replies");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@PostID", DbType.String, postID);
                _Database.AddInParameter(objCommand, "@ReplyID", DbType.String, replyID);
                _Database.AddInParameter(objCommand, "@stock", DbType.Int64, stock);
                _Database.AddInParameter(objCommand, "@pageNumber", DbType.Int64, pageNumber);
                _Database.AddInParameter(objCommand, "@pageSize", DbType.Int64, pageSize);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "SingleReply");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> TimeLine(long entityID, long feedID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_TimeLineFeed");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                _Database.AddInParameter(objCommand, "@feedID", DbType.Int64, feedID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "TimeLine");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> TimeLine(long entityID, int pageNo, int pageSize, bool group)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_TimeLineFeeds");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                _Database.AddInParameter(objCommand, "@PageNo", DbType.Int32, pageNo);
                _Database.AddInParameter(objCommand, "@PageSize", DbType.Int32, pageSize);
                _Database.AddInParameter(objCommand, "@Group", DbType.Boolean, group);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "Feeds");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> LoadOptionsForPost(long postID, long currentEntityID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_FeedOptions");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, currentEntityID);
                _Database.AddInParameter(objCommand, "@PostID", DbType.Int32, postID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "LoadOptionsForPost");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> SingleFeedRow(long postID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_SingleFeedAsRow");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@PostID", DbType.Int32, postID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "LoadOptionsForPost");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<bool> AllowOwnerAction(long entityID, long? postID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_PostOwnership");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@PostID", DbType.Int64, postID);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                var reader = _Database.ExecuteScalar(objCommand);
                return Convert.ToInt32(reader) == entityID;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "LoadOptionsForPost");
                return false;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> UpdateStatus(long? postID, int status)
        {
            DbCommand insert = null;
            insert = _Database.GetStoredProcCommand("P_PostUpdate_Status");
            insert.CommandTimeout = Constants.TIMEOUT;
            _Database.AddInParameter(insert, "@PostID", DbType.Int64, postID);
            _Database.AddInParameter(insert, "@Status", DbType.Int32, status);
            try
            {
                _Database.ExecuteNonQuery(insert);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetCommercialProperties");
                return -1;
            }
            finally
            {
                if (insert != null) { insert.Dispose(); }
            }
        }

        public async Task<DataSet> FromDraft(string id)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_SinglePostFromDraft");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@PostID", DbType.String, id);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "FromDraft");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> Single(long entityID, long postID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("SinglePost");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ID", DbType.Int64, postID);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "Single");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> ReshareMap(long entityID, long postID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Reshare_Map");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ID", DbType.Int64, postID);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "Single");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> GetLikes(long? postID, byte type)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("GetLikes_Count");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ID", DbType.Int64, postID);
                _Database.AddInParameter(objCommand, "@Type", DbType.Int16, type);
                return Convert.ToInt64(_Database.ExecuteScalar(objCommand));
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetLikes_Count");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<long> GetReplyLikes(long? replyID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("GetReplyLikes_Count");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@ID", DbType.Int64, replyID);
                return Convert.ToInt64(_Database.ExecuteScalar(objCommand));
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "GetReplyLikes");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> MyUpdateFeeds(long entityID, int pageNumber, int pageSize, DateTime? stamp)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_FeedUpdates");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                _Database.AddInParameter(objCommand, "@PageNumber", DbType.Int64, pageNumber);
                _Database.AddInParameter(objCommand, "@PageSize", DbType.Int64, pageSize);
                _Database.AddInParameter(objCommand, "@TimeStamp", DbType.DateTime, stamp);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "MyUpdateFeeds");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> MySavedFeeds(long entityID, int pageNumber, int pageSize, DateTime? stamp)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_FeedSaved");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@EntityID", DbType.String, entityID);
                _Database.AddInParameter(objCommand, "@PageNumber", DbType.Int64, pageNumber);
                _Database.AddInParameter(objCommand, "@PageSize", DbType.Int64, pageSize);
                _Database.AddInParameter(objCommand, "@TimeStamp", DbType.DateTime, stamp);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "MyUpdateFeeds");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> RemoveReply(long? replyID, long entityID)
        {
            DbCommand command = null;
            command = _Database.GetStoredProcCommand("P_PostReply_Remove");
            command.CommandTimeout = Constants.TIMEOUT;
            _Database.AddInParameter(command, "@EntityID", DbType.Int64, entityID);
            _Database.AddInParameter(command, "@ReplyID", DbType.Int64, replyID);
            try
            {
                _Database.ExecuteNonQuery(command);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "P_PostReply_Remove");
                return -1;
            }
            finally
            {
                if (command != null) { command.Dispose(); }
            }
        }

        public async Task<int> RemoveInteractionForNews(long? postID, long entityID, byte interactionType)
        {
            DbCommand command = null;
            command = _Database.GetStoredProcCommand("P_PostInteraction_Remove");
            command.CommandTimeout = Constants.TIMEOUT;
            _Database.AddInParameter(command, "@postID", DbType.Int64, postID);
            _Database.AddInParameter(command, "@entityID", DbType.Int64, entityID);
            _Database.AddInParameter(command, "@interactionType", DbType.Int16, interactionType);
            try
            {
                _Database.ExecuteNonQuery(command);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "P_PostReply_Remove");
                return -1;
            }
            finally
            {
                if (command != null) { command.Dispose(); }
            }
        }

        public async Task<DataSet> Insights(long postID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_FeedInsights");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@PostID", DbType.String, postID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "Insights");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> InsightDetails(long postID, int insightType)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_FeedInsightDetails");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@PostID", DbType.String, postID);
                _Database.AddInParameter(objCommand, "@InteractionType", DbType.String, insightType);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "InsightDetails");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<int> React(PostInteraction interaction, string posts)
        {
            DbCommand insert = null;
            insert = _Database.GetStoredProcCommand("P_Post_IntereactBulk");
            insert.CommandTimeout = Constants.TIMEOUT;
            _Database.AddInParameter(insert, "@CreatedBy", DbType.String, interaction.CreatedBy);
            _Database.AddInParameter(insert, "@CreatedOn", DbType.DateTime, interaction.CreatedOn);
            _Database.AddInParameter(insert, "@Description", DbType.String, interaction.Description);
            _Database.AddInParameter(insert, "@EntityID", DbType.Int64, interaction.EntityID);
            _Database.AddInParameter(insert, "@EntityID2", DbType.Int64, interaction.EntityID2);
            _Database.AddInParameter(insert, "@InteractionType", DbType.Int16, interaction.InteractionType);
            _Database.AddInParameter(insert, "@PostIDs", DbType.String, posts);
            _Database.AddInParameter(insert, "@IpAddress", DbType.String, interaction.IpAddress);
            try
            {
                _Database.ExecuteNonQuery(insert);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "React");
                return -1;
            }
            finally
            {
                if (insert != null) { insert.Dispose(); }
            }
        }


        public async Task<DataSet> MyInsights(long postID, long entityID)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_FeedMyInsightDetails");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@PostID", DbType.Int64, postID);
                _Database.AddInParameter(objCommand, "@EntityID", DbType.Int64, entityID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "MyInsights");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> FeedsByType(int type, int pageNumber, int pageSize, DateTime stamp)
        {
            DbCommand objCommand = _Database.GetStoredProcCommand("Get_Feeds_ByType");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@type", DbType.Int64, type);
                _Database.AddInParameter(objCommand, "@pageNumber", DbType.Int64, pageNumber);
                _Database.AddInParameter(objCommand, "@pageSize", DbType.Int64, pageSize);
                _Database.AddInParameter(objCommand, "@stamp", DbType.DateTime, stamp);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Processors.Repositories", this.GetType().FullName, "FeedsByType");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}