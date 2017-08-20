using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Types;

namespace Wrly.Infrastuctures.Utils
{
    public static class AppConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool IsLiveMode
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["IsLiveMode"]);
            }
        }

        public static bool UseLocalStorage
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["UseLocalStorage"]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool UseSendGrid
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["UseSendGrid"]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool OverrideExceptionScreen
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["OverrideExceptionScreen"]);
            }
        }

        public static string ImageUrl
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ImageUrl"]);
            }

        }

        public static object SpamUrl
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["SpamUrl"]);
            }
        }

        public static string SiteUrl
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["SiteUrl"]);
            }
        }


        public static Enums.StorageProvider StorageProvider
        {
            get
            {
                if (UseLocalStorage)
                {
                    return Enums.StorageProvider.Local;
                }
                return Enums.StorageProvider.Amazon;
            }
        }

        public static bool ForceSecure
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["ForceSecure"]);
            }
        }
    }
}
