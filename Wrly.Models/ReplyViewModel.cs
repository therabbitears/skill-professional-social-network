using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Types;

namespace Wrly.Models
{
    public class ReplyViewModel : NewsAndReplyAuthorInfo
    {
        #region Primitive Properties

        public long ID
        {
            get;
            set;
        }
        [Required(ErrorMessage = "Enter a reply/message.")]
        public string Reply
        {
            get;
            set;
        }



        public Nullable<System.DateTime> PostedOn
        {
            get;
            set;
        }

        public Nullable<long> PostID
        {
            get;
            set;
        }

        public Nullable<long> ReplyID
        {
            get { return _replyID; }
            set
            {
                _replyID = value;
            }
        }
        private Nullable<long> _replyID;

        public Nullable<bool> Active
        {
            get;
            set;
        }

        public bool IsAcceptedAnswer { get; set; }

        public int TotalDownvotes { get; set; }
        public int TotalUpvotes { get; set; }
        public int TotalVotes { get; set; }

        #endregion

        public string UserID { get; set; }

        public long EntityID { get; set; }

        public bool Liked { get; set; }

        public int TotalChildReplies { get; set; }

        public int TotalLikes { get; set; }

        public bool AllowDelete { get { return UserHashObject != null && EntityID == UserHashObject.EntityID; } }
    }

    public abstract class NewsAndReplyAuthorInfo : BaseViewModel
    {
        public Int32 EntityType { get; set; }
        public string FormatedName { get; set; }

        public string AuthorName
        {
            get
            {
                if (EntityType == (byte)Enums.EntityTypes.Person)
                {
                    return FormatedName;
                }
                if (EntityType == (byte)Enums.EntityTypes.Organization)
                {
                    return Name;
                }
                if (EntityType == (byte)Enums.EntityTypes.Topic)
                {
                    return Name1;
                }
                return null;
            }
        }

        public string AuthorPhoto
        {
            get
            {
                if (EntityType == (byte)Enums.EntityTypes.Person)
                {
                    if (!string.IsNullOrEmpty(ProfilePath))
                    {
                        return ProfilePath;
                    }
                    return "/content/images/no-image.png";
                }
                if (EntityType == (byte)Enums.EntityTypes.Organization)
                {
                    if (!string.IsNullOrEmpty(LogoPath))
                    {
                        return LogoPath;
                    }
                    return "/content/images/o/no-image.png";
                }
                return null;
            }
        }

        public string Heading
        {
            get
            {
                if (EntityType == (byte)Enums.EntityTypes.Person)
                {
                    return ProfileHeading;
                }
                if (EntityType == (byte)Enums.EntityTypes.Organization)
                {
                    return Category;
                }
                return null;
            }
        }

        public string ProfileUrl
        {
            get
            {
                if (EntityType == (byte)Enums.EntityTypes.Person)
                {
                    return ProfileName.ToLower();
                }
                if (EntityType == (byte)Enums.EntityTypes.Organization)
                {
                    return string.Format("fou/{0}", Url);
                }
                if (EntityType == (byte)Enums.EntityTypes.Topic)
                {
                    return string.Format("tags/{0}", Name1);
                }
                return null;
            }
        }

        public string Category { get; set; }
        public string Name { get; set; }
        public string Name1 { get; set; }
        public string ProfilePath { get; set; }
        public string LogoPath { get; set; }
        public string ProfileHeading { get; set; }
        public string ProfileName { get; set; }
        public string Url { get; set; }
    }
}