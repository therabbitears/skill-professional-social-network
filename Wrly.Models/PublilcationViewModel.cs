using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wrly.Models
{
    public class PublilcationViewModel : AwardViewModel
    {
        public SelectList PublicationType { get; set; }

        [JsonIgnore]
        public bool ShowExtendeFindingOptions
        {
            get
            {
                return !((StartFromMonth == null || StartFromMonth == -1) && (StartFromYear == null || StartFromYear == -1) && (ParticipantIncluded == null || ParticipantIncluded.Count == 0));
            }
        }

        [JsonIgnore]
        public bool ShowExtendeResearchOptions
        {
            get
            {
                return ParticipantIncluded != null && ParticipantIncluded.Count > 0;
            }
        }

        [JsonIgnore]
        public bool ShowExtendedPublicationOptions
        {
            get
            {
                return !((StartFromMonth == null || StartFromMonth == -1) && (StartFromYear == null || StartFromYear == -1) && (ParticipantIncluded == null || ParticipantIncluded.Count == 0));
            }
        }

        [JsonIgnore]
        public bool ShowExtendedCompositionOptions
        {
            get
            {
                return !((StartFromMonth == null || StartFromMonth == -1) && (StartFromYear == null || StartFromYear == -1) && (ParticipantIncluded == null || ParticipantIncluded.Count == 0));
            }
        }

        public bool IsPeriodMode
        {
            get
            {
                return StartFromMonth > 0 && EndFromMonth > 0;
            }
        }
    }
}