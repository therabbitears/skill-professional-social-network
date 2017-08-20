using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrly.Data.Models.Extended
{
    public class PageTopic
    {
        public long TopicID { get; set; }
        public long? ParentTopicID { get; set; }
        public int CategoryID { get; set; }
        public string TopicName { get; set; }
        public string Lable { get; set; }
        public string Description { get; set; }
        public string ThumbnailPath { get; set; }
        public string ImagePath { get; set; }
        public string Url { get; set; }
        public string Html { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public string MetaDescription { get; set; }
        public string Keywords { get; set; }
    }
}
