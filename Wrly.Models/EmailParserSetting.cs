using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wrly.Models
{
    public class EmailParserSetting
    {
        public string Subject { get; set; }
        public string FilePath { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
    }
}