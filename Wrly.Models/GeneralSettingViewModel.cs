using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wrly.Infrastructure.Utils;
using Wrly.Infrastuctures.Utils;

namespace Wrly.Models
{
    public class GeneralSettingViewModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9_.-]*$",ErrorMessage = "Invalid characters in profile name, special characters other than [-] and [_] and spaces are not allowed.")]
        public string ProfileName { get; set; }
        public string ProfileNameHash { get; set; }
    }
    public class NetworkSettingViewModel
    {
        public NetworkSettingViewModel()
        {
            this.NetworkCoverageOptions = new SelectList(CommonData.NetworkLevel(SessionInfo.UserHash.EntityType), "Key", "Value");
            this.RequestCapabilityOptions = new SelectList(CommonData.RequesterCapabilityLevel(), "Key", "Value");
        }
        public string NetworkCoverageHash { get; set; }
        public int NetworkCoverageLevel { get; set; }
        public SelectList NetworkCoverageOptions { get; set; }
        public string RequestCapabilityHash { get; set; }
        public int RequestCapability { get; set; }
        public bool AllowIndividualToConnect { get; set; }
        public SelectList RequestCapabilityOptions { get; set; }
        [Required]
        public string IndustryName { get; set; }
        [Range(1, 10000,ErrorMessage="Please select a valid industry to prefer connection from.")]
        public int? IndustryID { get; set; }
    }

    public class PrivacySettingViewModel
    {
        public bool SearchEngineVisible { get; set; }


        public bool HasPassword { get; set; }
    }

    public class WidgetSettingViewModel : BaseViewModel
    {
        public int EntityWidgetCategoryID { get; set; }
        public long WidgetID { get; set; }
        public string WidgetName { get; set; }
        public string Description { get; set; }
        public string Help { get; set; }
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or Sets if entity choosen this widget
        /// </summary>
        public long? ID { get; set; }

        /// <summary>
        /// Gets or Sets if the widget is added to profile
        /// </summary>
        public bool IsSubscribed { get { return ID > 0; } }

        public string Icon { get; set; }
    }
    public class JobSearchViewModel
    {
        public JobSearchViewModel()
        {
            this.OppurtunityLevelOptions = new SelectList(CommonData.OppurtunitiesLevel(), "Key", "Value");
        }
        public string OppurtunityLevelHash { get; set; }
        public int JobInterestLevel { get; set; }
        public SelectList OppurtunityLevelOptions { get; set; }
        public bool AllowToRefer { get; set; }
        public bool AllowOppurtunities { get; set; }
    }
}