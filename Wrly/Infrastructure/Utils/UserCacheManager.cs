using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastuctures.Utils;
using Wrly.Models;

namespace Wrly.Infrastructure.Utils
{
    public static class UserCacheManager
    {
        public static SettingViewModel Settings
        {
            get
            {
                var entityID = SessionInfo.UserHash.EntityID;
                var model = CacheManager.GetValue<SettingViewModel>("UserSettings_" + entityID.ToString());
                if (model == null)
                {
                    using (var repository = new SettingsRepository())
                    {
                        using (DataSet dsSettings = repository.GetSetting(entityID, Types.Enums.SettingType.All).Result)
                        {
                            model = new SettingViewModel()
                            {
                                General = dsSettings.Tables[0].FromDataTable<GeneralSettingViewModel>(null).FirstOrDefault(),
                                Privacy = dsSettings.Tables[1].FromDataTable<PrivacySettingViewModel>(null).FirstOrDefault(),
                                Network = dsSettings.Tables[2].FromDataTable<NetworkSettingViewModel>(null).FirstOrDefault(),
                                JobSearch = dsSettings.Tables[3].FromDataTable<JobSearchViewModel>(null).FirstOrDefault()
                            };
                            CacheManager.Add("UserSettings_" + entityID.ToString(), model);
                        }
                    }
                }
                return model;
            }
        }

        public static List<EntitySearchViewModel> Searches
        {
            get
            {
                var entityID = SessionInfo.UserHash.EntityID;
                var model = CacheManager.GetValue<List<EntitySearchViewModel>>("UserSearches_" + entityID.ToString());
                if (model == null)
                {
                    model = CacheSearches(entityID);
                    return Searches;
                }
                return model;

            }
        }

        internal static void RefreshSearches()
        {
            var entityID = SessionInfo.UserHash.EntityID;
            CacheManager.Delete("UserSearches_" + entityID.ToString());
            CacheSearches(entityID);
        }

        private static List<EntitySearchViewModel> CacheSearches(long entityID)
        {
            using (var repository = new SearchRepository())
            {
                using (DataSet dsSettings = repository.GetSearches(entityID))
                {
                    var model = dsSettings.Tables[0].FromDataTable<EntitySearchViewModel>();
                    CacheManager.Add("UserSearches_" + entityID.ToString(), model, 2);
                    return model;
                }
            }
        }

        public static ProfileFaceViewModel Face
        {
            get
            {
                if (SessionInfo.UserHash != null)
                {
                    var entityID = SessionInfo.UserHash.EntityID;
                    var model = CacheManager.GetValue<ProfileFaceViewModel>("EntityFace_" + entityID.ToString());
                    if (model == null)
                    {
                        model = CacheFace(entityID);
                        return Face;
                    }
                    return model;
                }
                return null;
            }
        }

        public static ProfileFaceViewModel CacheFace(long entityID)
        {
            using (var repository = new AccountRepository())
            {
                using (var dsSettings = repository.EntityFace(entityID))
                {
                    var model = dsSettings.Result.Tables[0].FromDataTable<ProfileFaceViewModel>().FirstOrDefault();
                    CacheManager.Add("EntityFace_" + entityID.ToString(), model, 100);
                    return model;
                }
            }
        }

        public static void ClearAll(long entityID)
        {
            WebCache.Remove("UserSearches_" + entityID.ToString());
            WebCache.Remove("EntityFace_" + entityID.ToString());
            WebCache.Remove("UserSettings_" + entityID.ToString());
        }
    }
}