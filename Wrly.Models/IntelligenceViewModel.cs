using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Types;

namespace Wrly.Models
{
    public class IntelligenceViewModel
    {
        public Enums.InteligenceType IntelligenceType { get; set; }
        public long? ReferenceID { get; set; }
    }
}