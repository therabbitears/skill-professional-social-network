using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Types;

namespace Wrly.Models
{
    public class AccomplishmentReportViewModel:BaseViewModel
    {
        public AwardViewModel Accomplishment { get; set; }
        public byte Type { get; set; }
        public string Description { get; set; }
    }

    public class Result
    {
        public Enums.ResultType Type { get; set; }
        public string Description { get; set; }
        public object ReferenceID { get; set; }
    }
}