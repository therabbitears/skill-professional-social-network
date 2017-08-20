using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Wrly.Models
{
    public class PostViewModel : BaseViewModel
    {

        public PostViewModel()
        {
            this.AddNewReply = new ReplyViewModel();
        }
        public virtual long ID
        { get; set; }

        [Required(ErrorMessage = "Enter a blog title")]
        [DisplayName("Post Title")]
        public virtual string Title
        { get; set; }

        [Required(ErrorMessage = "Enter a blog short description")]
        [DisplayName("Short Description")]
        public virtual string ShortDescription
        { get; set; }

        [Required(ErrorMessage = "Enter a blog description")]
        [DisplayName("Description")]
        public virtual string Description
        { get; set; }

        [Required(ErrorMessage = "Enter a blog keyword")]
        [DisplayName("Keyword")]
        public virtual string Meta
        { get; set; }

        [Required(ErrorMessage = "Enter a blog url")]
        [DisplayName("Url")]
        public virtual string UrlSlug
        { get; set; }

        public virtual bool Published
        { get; set; }

        public virtual DateTime PostedOn
        { get; set; }

        public virtual DateTime? Modified
        { get; set; }

        [Required(ErrorMessage = "Enter a meta description")]
        [DisplayName("Meta Description")]
        public string MetaDesc { get; set; }
        public string FilePath { get; set; }

        public byte? PostSubType { get; set; }

        public int PostType { get; set; }

        public System.Web.Mvc.SelectList SubTypes { get; set; }

        public long TotalReplies { get; set; }

        public long UpVotes { get; set; }

        public long DownVotes { get; set; }

        public long TotalViews { get; set; }

        public List<ReplyViewModel> Replies { get; set; }

        public ReplyViewModel AddNewReply { get; set; }

        public int[] TagIDList { get; set; }

        public SelectList Tags { get; set; }

        public List<PostTagViewModel> PostTags { get; set; }

        public string FullName { get; set; }

        public string ProfileName { get; set; }

        public string Hash { get; set; }

        public string AskedTo { get; set; }

        public bool AllowPublic { get; set; }

        public string AuthorEmailAddress { get; set; }

        public string DraftID { get; set; }
    }

    public class Category
    {
        public virtual int Id
        { get; set; }

        public virtual string Name
        { get; set; }

        public virtual string UrlSlug
        { get; set; }

        public virtual string Description
        { get; set; }

        public virtual IList<PostViewModel> Posts
        { get; set; }
    }


    public class Tag
    {
        public virtual int Id
        { get; set; }

        public virtual string Name
        { get; set; }

        public virtual string UrlSlug
        { get; set; }

        public virtual string Description
        { get; set; }

        public virtual IList<PostViewModel> Posts
        { get; set; }
    }

    public class CommentViewModel : BaseViewModel
    {
        public string Comment { get; set; }        
    }
}