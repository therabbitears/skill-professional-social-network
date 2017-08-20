using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Types;

namespace Wrly.Models.Business
{
    public class BusinessProfileViewModel : BaseViewModel
    {
        public string ProfileHash { get; set; }
        public long EntityID { get; set; }
        public long OrganizationID { get; set; }
        public string Name { get; set; }
        public string LegalName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string ProfileCoverPath { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ProfileImagePath { get; set; }
        public string EmployeeStrength { get; set; }
        public int TotalFollowers { get; set; }
        public int TotalConnections { get; set; }
        public string ProfileName { get; set; }

        public bool AllowEdit { get; set; }
        public string UserName { get; set; }
        public Feeds.HomeFeedViewModel Feed { get; set; }

        public int ConnectionStatus { get; set; }
        public int FollowerStatus { get; set; }
        public int BlockStatus { get; set; }
        public int AllowUnblock { get; set; }

        public string NetworkHash { get; set; }


        /// Address

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string County { get; set; }

        // Phone
        public int? PhoneType { get; set; }
        public string Phone { get; set; }

        // Phone
        public int? EmailType { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string EmailAddress { get; set; }

        public string Website { get; set; }

        public System.Web.Mvc.SelectList IndustryList { get; set; }

        public System.Web.Mvc.SelectList EmployeeStrengths { get; set; }

        public int EstablishedYear { get; set; }

        public Types.Enums.ProfileMode ProfileMode { get; set; }

        public List<Listing.AssociateProfileViewModel> Connections { get; set; }

        public List<Listing.AssociateProfileViewModel> Followers { get; set; }

        public BusinessStatisticsViewModel Statistics { get; set; }
    }

    public class GroupViewModel : BaseViewModel
    {
        [Required(ErrorMessage="Group name cannot be left blank.")]
        [StringLength(50, ErrorMessage = "Group name cannot exceeded by max 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Group name cannot be left blank.")]
        [StringLength(500, ErrorMessage = "Group description cannot be exceeded by 500 characters.")]
        public string Description { get; set; }

        public int IntialFollowers { get; set; }

        public HttpPostedFileBase Logo { get; set; }

        public System.Web.Mvc.SelectList Types { get; set; }

        public byte SubType { get; set; }

        public string ProfileName { get; set; }

        public decimal ImgX1 { get; set; }

        public decimal ImgY1 { get; set; }

        public decimal ImgWidth { get; set; }

        public decimal ImgHeight { get; set; }

        public bool RequirePermission { get; set; }

        public bool Discoverable { get; set; }

        public string Slogan { get; set; }

        public int? IndustryID { get; set; }

        public int TotalFollowers { get; set; }

        public List<AssociationViewModel> Associations { get; set; }

        public string ProfileCoverPath { get; set; }

        public object ProfileHash { get; set; }

        public Feeds.HomeFeedViewModel Feed { get; set; }

        public string ProfileImagePath { get; set; }

        public long EntityID { get; set; }

        public long OrganizationID { get; set; }

        public string NetworkHash { get; set; }

        public AssociationViewModel Member
        {
            get
            {
                if (UserHashObject != null)
                {
                    return Associations.FirstOrDefault(c => c.EntityID2 == UserHashObject.EntityID && c.AssociationType == (int)Enums.AssociationType.Follow);
                }
                return null;
            }
        }

        public AssociationViewModel Owner
        {
            get
            {
                if (UserHashObject != null)
                {
                    return Associations.FirstOrDefault(c => c.EntityID2 == UserHashObject.EntityID && c.AssociationType == (int)Enums.AssociationType.GroupOwner);
                }
                return null;
            }
        }

        public long MemberRequests { get; set; }

        public Enums.GroupMode Mode { get; set; }

        public byte? FollowStatus { get; set; }
    }
}