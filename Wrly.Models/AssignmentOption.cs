using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wrly.Models
{
    public class AssignmentOption : Option
    {
    }

    public class SkillHistoryOption : Option
    {
        public bool NoAction { get; set; }

        public bool Strong { get; set; }
    }
}