using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wrly.Infrastructure.Processors.Implementations
{
    class EmailServerSetting
    {

        public string EmailAddress { get; set; }

        public string SMTP { get; set; }

        public bool EnableSSL { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }
    }
}
