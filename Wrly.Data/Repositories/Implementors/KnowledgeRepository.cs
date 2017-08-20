using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrly.Data.Models.Extended;
using Wrly.Utils;

namespace Wrly.Data.Repositories.Implementors
{
    public class KnowledgeRepository : BaseRepository
    {
        public async Task<DataSet> GetAllCategories(string category, long? topicId)
        {
            var objCommand = _Database.GetStoredProcCommand("P_PageCategories_All");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Category", DbType.String, category);
                _Database.AddInParameter(objCommand, "@TopicId", DbType.Int64, topicId);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetAllCategories");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetCategoriesList(string category)
        {
            var objCommand = _Database.GetStoredProcCommand("P_PageCategories_List");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@Category", DbType.String, category);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetCategoriesList");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }

        public async Task<DataSet> GetTopics(long categoryID)
        {
            var objCommand = _Database.GetStoredProcCommand("P_PageTopics_GetForCategory");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@categoryID", DbType.Int64, categoryID);
                return _Database.ExecuteDataSet(objCommand);
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "GetTopics");
                return null;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }      

        public async Task<long> Save(PageTopic topic)
        {
            var objCommand = _Database.GetStoredProcCommand("P_PageTopics_Save");
            objCommand.CommandTimeout = Constants.TIMEOUT;
            try
            {
                _Database.AddInParameter(objCommand, "@CategoryID", DbType.Int64, topic.CategoryID);
                _Database.AddInParameter(objCommand, "@ParentTopicID", DbType.Int64, topic.ParentTopicID);
                _Database.AddInParameter(objCommand, "@Description", DbType.String, topic.Description);
                _Database.AddInParameter(objCommand, "@DisplayOrder", DbType.Int32, topic.DisplayOrder);
                _Database.AddInParameter(objCommand, "@Html", DbType.String, topic.Html);
                _Database.AddInParameter(objCommand, "@ImagePath", DbType.String, topic.ImagePath);
                _Database.AddInParameter(objCommand, "@IsActive", DbType.Boolean, topic.IsActive);
                _Database.AddInParameter(objCommand, "@Lable", DbType.String, topic.Lable);
                _Database.AddInParameter(objCommand, "@Title", DbType.String, topic.Title);
                _Database.AddInParameter(objCommand, "@MetaDescription", DbType.String, topic.MetaDescription);
                _Database.AddInParameter(objCommand, "@Keywords", DbType.String, topic.Keywords);
                _Database.AddInParameter(objCommand, "@ThumbnailPath", DbType.String, topic.ThumbnailPath);
                _Database.AddInParameter(objCommand, "@TopicID", DbType.Int64, topic.TopicID);
                _Database.AddInParameter(objCommand, "@TopicName", DbType.String, topic.TopicName);
                _Database.AddInParameter(objCommand, "@Url", DbType.String, topic.Url);
                _Database.ExecuteDataSet(objCommand);
                return 1;
            }
            catch (Exception ex)
            {
                ex.HandleDataLayerException("Website.Data.Infrastructures.Repositories", this.GetType().FullName, "Save");
                return -1;
            }
            finally
            {
                if (objCommand != null) { objCommand.Dispose(); }
            }
        }
    }
}
