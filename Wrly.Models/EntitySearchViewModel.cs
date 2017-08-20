using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wrly.Models
{
    public class EntitySearchViewModel : BaseViewModel
    {
        public long ID { get; set; }
        public string Keyword { get; set; }
        public int EntityType { get; set; }
        public long? EntityID { get; set; }
        public string Url { get; set; }
        public short Status { get; set; }
        public long SourceEntity { get; set; }
        public long Score { get; set; }

        // Person entity type
        public string FormatedName { get; set; }
        public string ProfilePath { get; set; }
        public string ProfileHeading { get; set; }
        public string ProfileName { get; set; }
        public string ProfileIconPath
        {
            get
            {
                if (!string.IsNullOrEmpty(ProfilePath))
                {
                    return ProfilePath.ImagePath(ProfilePath, 50);
                }
                return null;
            }
        }

        // Organization entity type
        public string Name { get; set; }
        public string Category { get; set; }
        public string LogoPath { get; set; }
        public string LogoIconPath
        {
            get
            {
                if (!string.IsNullOrEmpty(LogoPath))
                {
                    return LogoPath.ImagePath(LogoPath, 50);
                }
                return null;
            }
        }

        public long TotalRows { get; set; }

        public byte? SubType { get; set; }
    }

    public class LuceneEntitySearchViewModel : EntitySearchViewModel
    {
        public string EducationHistoryText { get; set; }

        public string SkillText { get; set; }

        public string WorkHistorytext { get; set; }
    }
}