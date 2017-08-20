using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;

namespace Wrly.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
       
        [NotMapped]
        public string FirstName { get; set; }
        [NotMapped]
        public string LastName { get; set; }

        string _FullName;
        [NotMapped]
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(LastName))
                {
                    _FullName = FirstName + " " + LastName;
                }
                else
                {
                    _FullName = FirstName;
                }
                return _FullName;
            }
            set
            {

                if (!string.IsNullOrEmpty(LastName))
                {
                    _FullName = FirstName + " " + LastName;
                }
                else
                {
                    _FullName = FirstName;
                }
            }
        }
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string EmailAddress { get; set; }
        public int Status { get; set; }
        [NotMapped]
        public string ProfileImage { get; set; }

        [NotMapped]
        public bool Verified { get; set; }

        [NotMapped]
        public byte? Gender { get; set; }

        public int Type { get; set; }
    }

    public class UserInfo
    {

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}