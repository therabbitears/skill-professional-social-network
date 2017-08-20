using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Types;

namespace Wrly.Models.Share
{
    public class FeedViewModel : BaseViewModel
    {
        public virtual byte Feedtype { get; set; }
    }

    public class NewsViewModel : FeedViewModel
    {
        [MaxLength(1000,ErrorMessage="The story should be no longer that 1000 characters.")]
        [Required(ErrorMessage = "Share something great")]
        public string Text { get; set; }
        public string Title { get; set; }
        public override byte Feedtype
        {
            get
            {
                return (byte)Enums.ShareType.News;
            }
        }

        public byte DistributionLevel { get; set; }
        public long PostID { get; set; }
        public HttpPostedFileBase PostImage { get; set; }

        public string FilePath { get; set; }

        public string DraftID { get; set; }

        public bool IsDiscussion { get; set; }
    }
}