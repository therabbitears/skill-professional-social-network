using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Types;
using Wrly.Models.Listing;

namespace Wrly.Models.Chat
{
    public class IndividualChatViewModel
    {
        public string CurrentUser { get; set; }
        public string ToUser { get; set; }
    }


    public class GroupChatViewModel : BaseViewModel
    {
        public GroupViewModel GroupInfo { get; set; }
        public AuthorViewModel MemberInfo { get; set; }
        public bool HasReceiver { get { return GroupInfo != null; } }
    }

    public class ChatMemberViewModel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }

        public string ProfilePic { get; set; }

        public DateTime LastOnline { get; set; }

        public bool IsOnline { get { return this.LastOnline > DateTime.UtcNow.AddMinutes(-30); } }
    }

    public class ChatGroupMessageModel
    {
        public virtual long ID
        {
            get;
            set;
        }

        public virtual string Message
        {
            get;
            set;
        }

        public virtual long UserID
        {
            get;
            set;
        }

        public virtual long GroupID
        {
            get;
            set;
        }

        public virtual int MessageType
        {
            get;
            set;
        }

        public virtual System.DateTime RecordedDate
        {
            get;
            set;
        }

        public string Name { get; set; }
        public string UserName { get; set; }
        public bool IsFromCurrentUser { get; set; }
    }

    public class UserDetail
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
    }


    public class Message
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }

    public class MessageDetail
    {
        public string UserName { get; set; }
        public string Message { get; set; }
    }

    public class ChatFaceViewModel
    {
        public long ID { get; set; }
        public string Message { get; set; }
        public long EntityID { get; set; }
        public long GroupID { get; set; }
        public int MessageType { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string FormatedName { get; set; }
        public string ProfilePath { get; set; }
        public string ProfileName { get; set; }
        public string ProfileHeading { get; set; }
        public virtual string UserID { get; set; }
        public bool HasRead { get; set; }

        #region Author
        public string LogoPath { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }

        public int EntityType
        {
            get;
            set;
        }
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
        public string ProfilePhotoUrl
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

        public string ProfilePhotoIconUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(ProfilePhotoUrl))
                {
                    return ProfilePhotoUrl.ImagePath(ProfilePhotoUrl, 50);
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
                return null;
            }
        }
        #endregion
    }

    public class MessageViewModel : IAuthor
    {
        public long ID { get; set; }
        public long GroupID { get; set; }
        public bool HasRead { get; set; }

        public string FormatedName { get; set; }
        public string ProfilePath { get; set; }
        public string ProfileName { get; set; }
        public string ProfileHeading { get; set; }

        public virtual string UserID { get; set; }
        public DateTime CreatedOn { get; set; }

        public string ShortMessage
        {
            get
            {
                if (!string.IsNullOrEmpty(Message) && Message.Length > 50)
                    return string.Format("{0}...", Message.Substring(0, 50));
                return Message;
            }
        }

        [ScriptIgnore]
        public string Message { get; set; }
        public string CreatedOnText { get { return CreatedOn.ToChatTime().ToString(); } }

        #region Author
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }

        public int EntityType
        {
            get;
            set;
        }
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
        public string ProfilePhotoUrl
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

        public string ProfilePhotoIconUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(ProfilePhotoUrl))
                {
                    return ProfilePhotoUrl.ImagePath(ProfilePhotoUrl, 50);
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
                return null;
            }
        }
        #endregion
    }
}