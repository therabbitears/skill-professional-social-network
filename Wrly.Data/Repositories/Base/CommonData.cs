#region--Includes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Wrly.Utils;

#endregion
namespace Wrly.Data
{
    public class CommonData
    {
        #region ' ---- Members ---- '
        public static string MainDbConnectionString
        {
            get
            {
                string strConnctionString = ConfigurationManager.ConnectionStrings["LiveDbConnectionString"].ConnectionString;
                return ValueEncryption.Decrypt(strConnctionString, string.Empty);
            }
        }
        
        #endregion

       
    }
}

