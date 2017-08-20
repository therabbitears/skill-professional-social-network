using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Wrly.Models.Knowledge
{
    public class HelpViewModel
    {
        public List<CategoryViewModel> Categories { get; set; }
        public SelectList CategoriesSelect
        {
            get
            {
                if (Categories!=null)
                {
                    return new SelectList(Categories, "CategoryID", "Label");
                }
                return null;
            }
        }
        public long? ActiveTopicID { get; set; }
        public long? ActiveCategoryID { get; set; }

        public TopicViewModel ActiveTopic { get; set; }
    }

    public class CategoryViewModel
    {
        public int CategoryID { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string ThumbnailPath { get; set; }
        public string ImagePath { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public List<TopicViewModel> Topics { get; set; }
    }

    public class TopicViewModel
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
        public List<TopicViewModel> Childs { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string MetaDescription { get; set; }
    }
}
