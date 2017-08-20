using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wrly.Models.Listing
{
    public class PersonFacehead
    {
        public string FormatedName { get; set; }
        public string ProfilePath { get; set; }
        public string ProfileName { get; set; }
        public string ProfileHeading { get; set; }
        public long EntityID { get; set; }

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
    }
}