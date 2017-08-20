using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wrly.Models.Listing
{
    public class OrganizationFaceViewModel
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public string LogoPath { get; set; }
        public string LogoIconPath
        {
            get
            {
                if (!string.IsNullOrEmpty(LogoPath))
                {
                    return LogoPath.ImagePath(LogoPath, 50);
                }
                return null;
            }
        }
        public int TotalEmployees { get; set; }

        public string AddressLine1 { get; set; }
        public int AddressType { get; set; }

        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }

        public long AddressId { get; set; }

    }
}