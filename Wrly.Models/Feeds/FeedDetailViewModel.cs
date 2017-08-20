using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Types;
using Wrly.Models.Listing;

namespace Wrly.Models.Feeds
{
    public class FeedDetailViewModel : NewsAndReplyAuthorInfo
    {
        public long ID { get; set; }
        public int PostType { get; set; }
        public int PostSubType { get; set; }
        public Nullable<byte> SharingType { get; set; }
        public long EntityID { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Meta { get; set; }
        public string UrlSlug { get; set; }
        public Nullable<bool> Published { get; set; }
        public string Description { get; set; }
        public System.DateTime PostedOn { get; set; }
        public Nullable<System.DateTime> PublishOn { get; set; }
        public Nullable<bool> AllowPublic { get; set; }
        public int TotalReplies { get; set; }
        public List<ReplyViewModel> Replies { get; set; }
        public long PostID { get; set; }
        public int TotalLikes { get; set; }
        public bool Liked { get; set; }
        public string FilePath { get; set; }



        public int OriginalPostType { get; set; }
        public int OriginalPostSubType { get; set; }
        public Nullable<byte> OriginalSharingType { get; set; }
        public long OriginalEntityID { get; set; }
        public string OriginalTitle { get; set; }
        public string OriginalShortDescription { get; set; }
        public string OriginalMeta { get; set; }
        public string OriginalUrlSlug { get; set; }
        public Nullable<bool> OriginalPublished { get; set; }
        public string OriginalDescription { get; set; }
        public System.DateTime OriginalPostedOn { get; set; }
        public Nullable<System.DateTime> OriginalPublishOn { get; set; }
        public Nullable<bool> OriginalAllowPublic { get; set; }
        public long OriginalPostID { get; set; }
        public int OriginalTotalLikes { get; set; }
        public bool OriginalLiked { get; set; }
        public string OriginalFilePath { get; set; }
        
        public long OriginalWritterID { get; set; }

        public string TopicName { get; set; }
        public string TopicDescription { get; set; }

        public long? ParentPostID { get; set; }

        public int? SecondEntityType { get; set; }

        public string SecondProfileHeading { get; set; }
        public string SecondProfileName { get; set; }
        public string SecondFormatedName { get; set; }
        public string SecondProfilePath { get; set; }

        public string SecondAuthorName
        {
            get
            {
                if (SecondEntityType == (byte)Enums.EntityTypes.Person)
                {
                    return SecondFormatedName;
                }
                if (SecondEntityType == (byte)Enums.EntityTypes.Organization)
                {
                    return SecondName;
                }
                return null;
            }
        }

        public string SecondAuthorPhoto
        {
            get
            {
                if (SecondEntityType == (byte)Enums.EntityTypes.Person)
                {
                    if (!string.IsNullOrEmpty(SecondProfilePath))
                    {
                        return SecondProfilePath;
                    }
                    return "/content/images/no-image.png";
                }
                if (SecondEntityType == (byte)Enums.EntityTypes.Organization)
                {
                    if (!string.IsNullOrEmpty(SecondLogoPath))
                    {
                        return SecondLogoPath;
                    }
                    return "/content/images/o/no-image.png";
                }
                return null;
            }
        }

        public string SecondHeading
        {
            get
            {
                if (SecondEntityType == (byte)Enums.EntityTypes.Person)
                {
                    return SecondProfileHeading;
                }
                if (SecondEntityType == (byte)Enums.EntityTypes.Organization)
                {
                    return SecondCategory;
                }
                return null;
            }
        }

        public string SecondProfileUrl
        {
            get
            {
                if (SecondEntityType == (byte)Enums.EntityTypes.Person)
                {
                    return SecondProfileName.ToLower();
                }
                if (SecondEntityType == (byte)Enums.EntityTypes.Organization)
                {
                    return string.Format("fou/{0}", SecondUrl);
                }
                return null;
            }
        }

        public string SecondName { get; set; }

        public string SecondLogoPath { get; set; }

        public string SecondCategory { get; set; }

        public string SecondUrl { get; set; }

        public List<SkillViewModel> Skills { get; set; }
        public List<CareerHistoryViewModel> JobTitles { get; set; }
        public long WritterID { get; set; }

        public int TotalReferrals { get; set; }

        public int TotalMyReferals { get; set; }

        public bool Referred { get; set; }

        public bool Applied { get; set; }

        public FeedInsightsViewModel Insgihts { get; set; }
    }
}
