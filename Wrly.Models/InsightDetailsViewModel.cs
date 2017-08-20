using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;

namespace Wrly.Models
{
    public class InsightDetailsViewModel : NewsAndReplyAuthorInfo
    {
        public long ID { get; set; }
        public int InteractionType { get; set; }
        public long PostID { get; set; }
        public string Description { get; set; }
        public long EntityID { get; set; }
        public long EntityID2 { get; set; }

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

    }
}
