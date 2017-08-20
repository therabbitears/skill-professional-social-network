using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using Wrly.Utils;

namespace Wrly.infrastuctures.Utils
{
    /// <summary>
    /// Contains common functions which are used to encrypt and decrypt query string.
    /// It also contains some functions which are used to get value in the specific type
    /// (like string,DateTime,Int64,Int32 and Int16) base on parameter name.
    /// </summary>

    /// <Remarks>
    /// </Remarks>
    public class QueryStringHelper
    {
        #region '---- Members ----'

        /// <summary>
        /// Default query string key that hold encrypted query string value.
        /// e.g. [URL]?Data="encrypted value"
        /// </summary>
        public const string DATA = "data";

        #endregion

        #region '----- Methods -----'

        /// <summary>
        /// Encrypts hash table data into query string
        /// 
        /// POSSIBLE PURPOSE       : Convert multiple parameter values into encrypted string so it can pass into query string.
        /// EXAMPLE APPEARANCE     : Convert ID into encrypted string to pass of it in query string of view page.
        /// INPUT                  : Parameter Name : "ID" and its Value : 1008 (Add parameter name as key and its vale as value in hash table).
        /// OUTPUT                 : YisyeCb4buQ=
        /// </summary>
        /// <param name="htParams">possible value of htParams like hash table with key/value pair of the query string parameter</param>
        /// <Author></Author>
        /// <Remarks>
        /// </Remarks>
        public static string Encrypt(Hashtable htParams)
        {
            // Store query string parameters.
            StringBuilder sbQueryString = new StringBuilder();
            // Encrypt decrypt key it will used to encrypt the query string.
            string strKey = "%&gt;s{;+#";

            // Iterate thru each key value pair from  hash table and convert into query string parameter format (ID=1008) 
            //and append to query string with "&".
            foreach (DictionaryEntry deKey in htParams)
            {
                // Append query string with & sign if parameter is already exist in the query string.
                if (sbQueryString.Length > 0) sbQueryString.Append("&");
                // Convert key/value pair of hash table into query string parameter format
                if (deKey.Value!=null)
                    sbQueryString.Append(string.Format("{0}={1}", deKey.Key.ToString(), deKey.Value.ToString()));
                else
                    sbQueryString.Append(string.Format("{0}={1}", deKey.Key.ToString(), string.Empty));
                
            }
            // Encrypt Query string and return it in string format.
            return ValueEncryptionHelper.Encrypt(sbQueryString.ToString(), strKey);

        }

        /// <summary>
        /// Decrypts the query string base on parameter name and return parameter value as string.
        /// 
        /// POSSIBLE PURPOSE       : Decrypt query string and get parameter value in string
        /// EXAMPLE APPEARANCE     : Encrypted query string :"YisyeCb4buQ=" and get value of parameter name "ID" from the query string.
        /// INPUT                  : ParamName : "ID" and EncryptedString : "YisyeCb4buQ="
        /// OUTPUT                 : 1008
        /// </summary>
        /// <param name="strParamName">possible value of ParamName like "ID", "Name" etc.</param>
        /// <param name="strEncryptedString">possible value of EncryptedString like any encrypted query string that encrypted using this class</param>
        /// <Remarks>
        /// </Remarks>
        public static string Decrypt(string strParamName, string strEncryptedString)
        {
            // Encrypt decrypt key it will used to decrypt encrypted query string.
            string strKey = "%&gt;s{;+#";

            // Decrypt full query string value
            string strDecryptedQueryString = ValueEncryptionHelper.Decrypt(strEncryptedString.Replace(" ", "+"), strKey);

            if (string.IsNullOrEmpty(strDecryptedQueryString))
            {
                // Note:- Earlier if query string tampered it was returning default value (means 0).
                // Now as over some pages we have conditions where we are checking that if value passed > 0 then do this else do that.
                // So to avoid conflict scenario we decided to return -1 if query string tampered.
                // MODIFIED DATE:- 10/10/2013
                return string.Empty;
            }

            string strDecryptedValue = string.Empty;

            // Split decrypted query string value as it may have multiple key value pair
            string[] strNameValuePairs = strDecryptedQueryString.Split('&');

            // Iterate thru each query string key value pair to get requested parameter value
            for (int i = 0; i < strNameValuePairs.Length; i++)
            {
                // Split parameter name and value and store in array of string
                string[] nameValue = strNameValuePairs[i].Split('=');
                // Process if parameter contains value.
                if (nameValue.Length == 2)
                {
                    // Compare query string parameter with name in pass parameter.
                    if (nameValue[0].ToUpper().Equals(strParamName.ToUpper()))
                    {
                        // Find out the value for passed parameter and return it.
                        strDecryptedValue = nameValue[1];
                        // Exist from loop because we find parameter value.
                        break;
                    }
                }
            }
            // return parameter value as string.
            return strDecryptedValue;
        }

        /// <summary>
        /// Decrypts the query string base on parameter name and return parameter value as string.
        /// 
        /// POSSIBLE PURPOSE       : Decrypt query string and get parameter value in string
        /// EXAMPLE APPEARANCE     : Encrypted query string :"YisyeCb4buQ=" and get value of parameter name "ID" from the query string.
        /// INPUT                  : ParamName : "ID" and EncryptedString : "YisyeCb4buQ="
        /// OUTPUT                 : 1008
        /// </summary>
        /// <param name="strEncryptedString">possible value of EncryptedString like any encrypted query string that encrypted using this class</param>

        /// <Remarks>
        /// </Remarks>
        public static string Decrypt(string strEncryptedString)
        {
            // Encrypt decrypt key it will used to decrypt encrypted query string.
            string strKey = "%&gt;s{;+#";

            // Decrypt full query string value
            string strDecryptedQueryString = ValueEncryptionHelper.Decrypt(strEncryptedString.Replace(" ", "+"), strKey);

            return strDecryptedQueryString;
        }

        /// <summary>
        /// Convert name value collection to query string.
        /// 
        /// POSSIBLE PURPOSE       : Convert name value collection into query string format to pass it as query string.
        /// EXAMPLE APPEARANCE     : Convert name value collection into query string format to pass it as query string.
        /// INPUT                  : Name : "ID" Value : "1008" and Name : "Name" Value : "Rajesh" 
        /// OUTPUT                 : ?ID=1008Name=Rajesh
        /// </summary>
        /// <param name="nvcValues">possible value of Values like collection with name and value that needed to convert in query string</param>

        /// <Remarks>
        /// </Remarks>
        public static string ConvertNameValueToQueryString(NameValueCollection nvcValues)
        {
            // Get all key value of name value collection and convert it in query string parameter format (ID=1008)
            // then join all string with & sign and return string with prefix "?".
            return "?" + string.Join("&", Array.ConvertAll(nvcValues.AllKeys, strKey => string.Format("{0}={1}", HttpUtility.UrlEncode(strKey), HttpUtility.UrlEncode(nvcValues[strKey]))));
        }

        /// <summary>
        /// Gets the string value.
        /// 
        /// POSSIBLE PURPOSE       : Get value of parameter by parameter name from decrypted query string of the current page URL.
        /// EXAMPLE APPEARANCE     : Get value of ID parameter from [URL]?ID=1008
        /// INPUT                  : "ID"
        /// OUTPUT                 : 1008
        /// </summary>
        /// <param name="strKey">possible value of Key like "ID"</param>

        /// <Remarks>
        /// </Remarks>
        public static string GetStringValue(string strKey)
        {
            string strQueryStringVal = HttpContext.Current.Request.QueryString[strKey];

            if (!string.IsNullOrEmpty(strQueryStringVal))
                strQueryStringVal = HttpUtility.UrlDecode(strQueryStringVal);

            return strQueryStringVal;
        }

        /// <summary>
        /// Get decrypted value as string.
        /// 
        /// POSSIBLE PURPOSE       : Get parameter value from encrypted query string.
        /// EXAMPLE APPEARANCE     : Get value of ID parameter from query string
        /// INPUT                  : "ID"
        /// OUTPUT                 : "1008"
        /// </summary>
        /// <param name="strParamName">possible value of ParamName like "ID"</param>

        /// <Remarks>
        /// </Remarks>
        public static string GetDecryptedValueAsString(string strParamName)
        {
            string strValue = string.Empty;

            // Check is value after decrypting query string is null?
            if (HttpContext.Current.Request.QueryString[DATA] == null)
            {
                strValue = string.Empty;
            }
            else
            {
                // Find parameter value from encrypted query string and assign value.
                // If parameter value is not found from encrypted query string then calling method return empty string as value.
                strValue = Decrypt(strParamName, HttpContext.Current.Request.QueryString[DATA].ToString());
            }

            return strValue;
        }


        /// <summary>
        /// Get decrypted value as string.
        /// 
        /// POSSIBLE PURPOSE       : Get parameter value from encrypted query string.
        /// EXAMPLE APPEARANCE     : Get value of ID parameter from query string
        /// INPUT                  : "ID"
        /// OUTPUT                 : "1008"
        /// </summary>
        /// <param name="strParamName">possible value of ParamName like "ID"</param>

        /// <Remarks>
        /// </Remarks>
        public static string GetDecryptedValueAsString(string strParamName, string strQueryStringName)
        {
            string strValue = string.Empty;

            // Check is value after decrypting query string is null?
            if (HttpContext.Current.Request.QueryString[strQueryStringName] == null)
            {
                strValue = string.Empty;
            }
            else
            {
                // Find parameter value from encrypted query string and assign value.
                // If parameter value is not found from encrypted query string then calling method return empty string as value.
                strValue = Decrypt(strParamName, HttpContext.Current.Request.QueryString[strQueryStringName].ToString());
            }

            return strValue;
        }

        /// <summary>
        /// Get decrypted value as datetime.
        /// 
        /// POSSIBLE PURPOSE       : Get parameter value from encrypted query string.
        /// EXAMPLE APPEARANCE     : Get value of CreatedDate parameter from query string
        /// INPUT                  : "CreatedDate"
        /// OUTPUT                 : 11/10/2012 4:39 PM
        /// </summary>
        /// <param name="strParamName">possible value of ParamName like "ID"</param>

        /// <Remarks>
        /// </Remarks>
        public static DateTime? GetDecryptedValueAsDateTime(string strParamName)
        {
            string strValue = string.Empty;

            // Check is query string have data?
            if (HttpContext.Current.Request.QueryString[DATA] == null)
            {
                strValue = string.Empty;
            }
            else
            {
                // As data exists, get the decrypted data.
                strValue = Decrypt(strParamName, HttpContext.Current.Request.QueryString[DATA].ToString());
            }

            // Due to tampering the value will be empty, so return DateTime.MinValue.
            if (string.IsNullOrEmpty(strValue))
            {
                return DateTime.MinValue;
            }

            if (!string.IsNullOrEmpty(strValue))
            {
                // As decrypted data exists, convert data into target format.
                // throw following exception if string value is not convertible in date and time.
                // System.FormatException: value is not a properly formatted date and time string.
                return Convert.ToDateTime(strValue);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get decrypted value as Int64.
        /// 
        /// POSSIBLE PURPOSE       : Get parameter value from encrypted query string.
        /// EXAMPLE APPEARANCE     : Get value of ID parameter from query string
        /// INPUT                  : "UD"
        /// OUTPUT                 : 1008 as Int64
        /// </summary>
        /// <param name="strParamName">possible value of ParamName like "ID"</param>

        /// <Remarks>
        /// </Remarks>
        public static Int64 GetDecryptedValueAsInt64(string strParamName)
        {
            Int64 intValue = 0;
            string strValue = string.Empty;

            // Check is query string have data?
            if (HttpContext.Current.Request.QueryString[DATA] == null)
            {
                strValue = string.Empty;
            }
            else
            {
                // As data exists, get the decrypted data.
                strValue = Decrypt(strParamName, HttpContext.Current.Request.QueryString[DATA].ToString());
            }

            // Due to tampering the value will be empty, so return -1.
            if (string.IsNullOrEmpty(strValue))
            {
                return -1;
            }

            if (!string.IsNullOrEmpty(strValue))
            {
                // As decrypted data exists, convert data into target format.
                // Throw following exception if string value is not convertible in Int64.
                // System.FormatException: value does not consist of an optional sign 
                // followed by a sequence of digits (0 through 9).
                // System.OverflowException: value represents a number that is less than 
                // System.Int64.MinValue or greater than System.Int64.MaxValue.
                intValue = Convert.ToInt64(strValue);
            }

            return intValue;
        }


        /// <summary>
        /// Get decrypted value as Int64.
        /// 
        /// POSSIBLE PURPOSE       : Get parameter value from encrypted query string.
        /// EXAMPLE APPEARANCE     : Get value of ID parameter from query string
        /// INPUT                  : "UD"
        /// OUTPUT                 : 1008 as Int64
        /// </summary>
        /// <param name="strParamName">possible value of ParamName like "ID"</param>

        /// <Remarks>
        /// </Remarks>
        public static Int64 GetDecryptedValueAsInt64(string strParamName, string strQueryStringName)
        {
            Int64 intValue = 0;
            string strValue = string.Empty;

            // Check is query string have data?
            if (HttpContext.Current.Request.QueryString[strQueryStringName] == null)
            {
                strValue = string.Empty;
            }
            else
            {
                // As data exists, get the decrypted data.
                strValue = Decrypt(strParamName, HttpContext.Current.Request.QueryString[strQueryStringName].ToString());
            }

            // Due to tampering the value will be empty, so return -1.
            if (string.IsNullOrEmpty(strValue))
            {
                return -1;
            }

            if (!string.IsNullOrEmpty(strValue))
            {
                // As decrypted data exists, convert data into target format.
                // Throw following exception if string value is not convertible in Int64.
                // System.FormatException: value does not consist of an optional sign 
                // followed by a sequence of digits (0 through 9).
                // System.OverflowException: value represents a number that is less than 
                // System.Int64.MinValue or greater than System.Int64.MaxValue.
                intValue = Convert.ToInt64(strValue);
            }

            return intValue;
        }

        /// <summary>
        /// Get decrypted value as Int32.
        /// 
        /// POSSIBLE PURPOSE       : Get parameter value from encrypted query string.
        /// EXAMPLE APPEARANCE     : Get value of ID parameter from query string
        /// INPUT                  : "ID"
        /// OUTPUT                 : 1008 as Int32
        /// </summary>
        /// <param name="strParamName">possible value of ParamName like "ID"</param>

        /// <Remarks>
        /// </Remarks>
        public static Int32 GetDecryptedValueAsInt32(string strParamName)
        {
            Int32 intValue = 0;
            string strValue = string.Empty;

            // Check is query string have data?
            if (HttpContext.Current.Request.QueryString[DATA] == null)
            {
                strValue = string.Empty;
            }
            else
            {
                // As data exists, get the decrypted data
                strValue = Decrypt(strParamName, HttpContext.Current.Request.QueryString[DATA].ToString());
            }

            // Due to tampering the value will be empty, so return -1.
            if (string.IsNullOrEmpty(strValue))
            {
                return -1;
            }

            if (!string.IsNullOrEmpty(strValue))
            {
                // As data exists, convert data into target format
                intValue = Convert.ToInt32(strValue);
            }

            return intValue;
        }

        /// <summary>
        /// Get parameter value as Int32 from encrypted query string by name of the parameter.
        ///
        /// POSSIBLE PURPOSE       : Get parameter value from encrypted query string.
        /// EXAMPLE APPEARANCE     : Get value of ID parameter from query string
        /// INPUT                  : "ID"
        /// OUTPUT                 : 1008 as Int16
        /// </summary>
        /// <param name="strParamName">possible value of ParamName like "ID"</param>

        /// <Remarks>
        /// </Remarks>
        public static Int16 GetDecryptedValueAsInt16(string strParamName)
        {
            Int16 intValue = 0;
            string strValue = string.Empty;

            // Check is query string have data?
            if (HttpContext.Current.Request.QueryString[DATA] == null)
            {
                strValue = string.Empty;
            }
            else
            {
                // As data exists, get the decrypted data.
                strValue = Decrypt(strParamName, HttpContext.Current.Request.QueryString[DATA].ToString());
            }

            // Due to tampering the value will be empty, so return -1.
            if (string.IsNullOrEmpty(strValue))
            {
                return -1;
            }

            if (!string.IsNullOrEmpty(strValue))
            {
                // As decrypted data exists, convert data into target format.
                // Throw following exception if string value is not convertible in Int16.
                // System.FormatException: value does not consist of an optional sign 
                // followed by a sequence of digits (0 through 9).
                // System.OverflowException: value represents a number that is less than 
                // System.Int16.MinValue or greater  than System.Int16.MaxValue.
                intValue = Convert.ToInt16(strValue);
            }

            return intValue;
        }

        #endregion

        public static long BusinessID
        {
            get
            {
                return GetDecryptedValueAsInt64("id", "data");
            }
        }

        public static string AppendQuesryString(string url, Hashtable table)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (DictionaryEntry item in table)
            {
                query[item.Key.ToString()] = item.Value.ToString();
            }
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        
    }
}