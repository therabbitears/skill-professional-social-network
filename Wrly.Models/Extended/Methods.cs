using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Wrly.Models.Listing;
using Wrly.Utils;
using Types;
using System.Collections;
using Wrly.infrastuctures.Utils;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Wrly.Models;
using System.Threading;
using Wrly.Infrastructure.Utils;

namespace Wrly
{
    public static class Methods
    {

        public static bool IsWWWRequest(this Uri URL)
        {
            return URL.ToString().ToLower().Contains("http://www.") || URL.ToString().ToLower().Contains("https://www.");
        }

        public static string ToJson(this SelectList list)
        {
            var resultList = new List<KeyValue>();
            foreach (var item in list)
            {
                resultList.Add(new KeyValue() { Key = item.Value, Value = item.Text });
            }
            var stringWithJson = JsonConvert.SerializeObject(resultList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
            return stringWithJson;
        }

        public static string AddWWW(this Uri URL)
        {

            string strNewUrl = URL.ToString();
            // Checks either URL with https.
            if (strNewUrl.ToLower().Contains("https://"))
            {
                //Replace https:// with https://www. .
                strNewUrl = strNewUrl.ToLower().Replace("https://", "https://www.");
            }
            else//URL with http.
            {
                //Replace http:// with http://www
                strNewUrl = strNewUrl.Replace("http://", "http://www.");
            }
            // Returns new URL.
            return strNewUrl;
        }

        public static bool IsImageFile(this HttpPostedFileBase image, int? maxContentLength = null)
        {
            string extension = Path.GetExtension(image.FileName);
            var extensions = new string[] { ".png", ".gif", ".jpeg", ".jpg", ".bmp", ".tiff" };
            if (!extension.Contains(extension))
                return false;
            if (maxContentLength != null)
                return image.ContentLength <= maxContentLength;

            return true;
        }

        public static string FormatedNumber(this int num)
        {
            num = MaxThreeSignificantDigits(num);

            if (num >= 100000000)
                return (num / 1000000D).ToString("0.#M");
            if (num >= 1000000)
                return (num / 1000000D).ToString("0.##M");
            if (num >= 100000)
                return (num / 1000D).ToString("0k");
            if (num >= 100000)
                return (num / 1000D).ToString("0.#k");
            if (num >= 1000)
                return (num / 1000D).ToString("0.##k");
            return num.ToString("#,0");
        }

        public static string FormatedNumber(this long num)
        {
            num = MaxThreeSignificantDigits(num);

            if (num >= 100000000)
                return (num / 1000000D).ToString("0.#M");
            if (num >= 1000000)
                return (num / 1000000D).ToString("0.##M");
            if (num >= 100000)
                return (num / 1000D).ToString("0k");
            if (num >= 100000)
                return (num / 1000D).ToString("0.#k");
            if (num >= 1000)
                return (num / 1000D).ToString("0.##k");
            return num.ToString("#,0");
        }

        static int MaxThreeSignificantDigits(int x)
        {
            int i = (int)Math.Log10(x);
            i = Math.Max(0, i - 2);
            i = (int)Math.Pow(10, i);
            return x / i * i;
        }

        static long MaxThreeSignificantDigits(long x)
        {
            int i = (int)Math.Log10(x);
            i = Math.Max(0, i - 2);
            i = (int)Math.Pow(10, i);
            return x / i * i;
        }

        public static bool IsImageFile(this string image, int? maxContentLength = null)
        {
            if (image == null || string.IsNullOrWhiteSpace(image))
                return true; // not concerned with whether or not this field is required
            var base64string = (image as string).Trim();

            // we are expecting a URL type string
            if (!base64string.StartsWith("data:image/png;base64,"))
                return false;

            base64string = base64string.Substring("data:image/png;base64,".Length);

            // match length and regular expression
            if (base64string.Length % 4 != 0 || !Regex.IsMatch(base64string, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
                return false;

            // finally, try to convert it to a byte array and catch exceptions
            try
            {
                byte[] converted = Convert.FromBase64String(base64string);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public static string GetExtension(this string input)
        {
            return Path.GetExtension(input).Replace(".", string.Empty);
        }


        public static string ImagePath(this HtmlHelper helper, string path, int w = int.MaxValue)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (w == int.MaxValue)
                {
                    return string.Format(path, "full");
                }
                return string.Format(path, w);
            }
            return null;
        }

        public static string ImagePath(this string helper, string path, int w = int.MaxValue)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (w == int.MaxValue)
                {
                    return string.Format(path, "full");
                }
                return string.Format(path, w);
            }
            return null;
        }

        public static string GetFileName(this string input)
        {
            return Path.GetFileName(input);
        }




        public static HtmlString HashForIntelligence(this HtmlHelper helper, Enums.InteligenceType type)
        {
            Hashtable table = new Hashtable();
            table.Add("___T___", (int)type);
            table.Add("___S___", DateTime.UtcNow);
            table.Add("___I___", Guid.NewGuid());
            return new HtmlString(QueryStringHelper.Encrypt(table));
        }

        private static readonly Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        public static bool IsMobile(this HttpRequestBase request)
        {
            string userAgent = request.UserAgent;
            if (request.Browser.IsMobileDevice || (b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
            {
                return true;
            }
            return false;

        }


        public static HtmlString ToFormattedTabAndNewLine(this string About)
        {
            if (!string.IsNullOrEmpty(About))
            {
                if (About.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length > 1)
                {
                    About = string.Format("<ul class='career-history-details'>{0}</ul>", string.Join(Environment.NewLine, About.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Select(x => string.Format("<li>{0}</li>", x)).ToList()));
                }
            }
            return new HtmlString(About);
        }

        public static HtmlString ToChatTime(this DateTime value)
        {
            if (value >= DateTime.UtcNow.AddMinutes(-1))
                return new HtmlString("Just now");

            if (value >= DateTime.UtcNow.AddHours(-1))
                return new HtmlString(string.Format("{0} minutes ago", DateTime.UtcNow.Subtract(value).Minutes));

            if (value.Date == DateTime.UtcNow.Date)
                return new HtmlString(string.Format("Today at {0} ", value.ToString()));

            if (value.Date == DateTime.UtcNow.AddDays(-1).Date)
                return new HtmlString(string.Format("Yesterday at {0} ", value.ToString()));

            return new HtmlString(string.Format("at {0} ", value.ToString()));

        }

        public static List<T> Randomize<T>(this IEnumerable<T> source)
        {
            Random rnd = new Random();
            return source.OrderBy<T, int>((item) => rnd.Next()).ToList();
        }

        public static async Task<string> GetCustomValue(this ClaimsIdentity identity, string key)
        {
            return identity.Claims.Where(c => c.Type == key).Single().Value;
        }




    }
}