using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Wrly.Models
{
    public class AboutViewModel
    {
        public string UserHash { get; set; }
        [StringLength(500, ErrorMessage="Your profile summary cannot be acceed total 500 characters.")]
        [Required(ErrorMessage="Your profile summary cannot be left blank.")]
        public string ProfileSummary { get; set; }

        public string FormatedProfileSummary
        {
            get
            {
                if (!string.IsNullOrEmpty(ProfileSummary))
                {
                    if (ProfileSummary.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length > 1)
                    {
                        return string.Format("<ul class='career-history-details'>{0}</ul>", string.Join(Environment.NewLine, ProfileSummary.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Select(x => string.Format("<li>{0}</li>", x)).ToList()));
                    }
                }
                return ProfileSummary;
            }
        }
    }


    public class BusinessAboutViewModel:BaseViewModel
    {
        [StringLength(500, ErrorMessage = "Your profile summary cannot be acceed total 500 characters.")]
        [Required(ErrorMessage = "Your profile summary cannot be left blank.")]
        public string Description { get; set; }
    }
}