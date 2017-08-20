using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrly.Data.Models.Extended
{
    public class LuceneObject
    {
        public long? EntityID { get; set; }
        public int EntityType { get; set; }
        public string Url { get; set; }
        public string ProfilePicUrl { get; set; }
        public DateTime LastModified { get; set; }

        public string DisplayName { get; set; }
        public string SkillText { get; set; }
        public string WorkHistoryText { get; set; }
        public string EducationHistoryText { get; set; }
        public string Headiing { get; set; }
        public byte? SubType { get; set; }
    }
}
