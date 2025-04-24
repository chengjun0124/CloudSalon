using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace CloudSalon.Common
{
    public class EncodingHelper
    {
        public static string DecodeBase64(string base64)
        {

            byte[] bpath = Convert.FromBase64String(base64);
            return System.Text.UTF8Encoding.Default.GetString(bpath);
        }

        public static string EncodeBase64(string str)
        {
            System.Text.Encoding encode = System.Text.Encoding.UTF8;
            byte[] bytedata = encode.GetBytes(str);
            return Convert.ToBase64String(bytedata, 0, bytedata.Length);
        }

        public static string MD5(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                pwd = pwd + s[i].ToString("x2");

            }
            return pwd;
        }

        public static string HMACMD5(string str,string key)
        {
            string cl = str;
            string pwd = "";
            HMACMD5 md5 = new HMACMD5();
            md5.Key = Encoding.UTF8.GetBytes(key);
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                pwd = pwd + s[i].ToString("x2");

            }
            return pwd;
        }

        public static string FilterXSS(string str)
        {
            #region white list
            string[] whiteList = {"a","abbr","acronym","article","b","bdi","bdo","big","blockquote","br","caption","code","col","colgroup","dd","del",
                                "div","dl","dt","em","header","footer","h1", "h2","h3","h4","h5","h6","hr","i","img","font","ins","li","mark","ol",
                                "p","pre","q","section","small","span","strike","strong","sub","sup","table","tbody","td","tfoot","th","thead","tr",
                                "u","ul","wbr"};
            #endregion

            MatchCollection matches = Regex.Matches(str, @"<[^>]+>");
            

            //html encode and remove <a> the tags which are not in the white list
            string tagName;
            foreach (Match m in matches)
            {
                tagName = Regex.Match(m.Value, @"<\s*/?\s*([^\s>]+)").Groups[1].Value;

                /*
                 * it's hard to remove href which link to outside, such as 'href="http://www.xxx.com/assa.html"'. 
                 * So now remove all <a>,</a>, not remove the content in the <a>
                 * HTML encode the other tags which are not in the whitelist
                 */
                if (tagName.ToLower() == "a")
                    str = str.Replace(m.Value, "");
                else if (whiteList.FirstOrDefault(i => i == tagName.ToLower()) == null)
                    str = str.Replace(m.Value, System.Web.HttpUtility.HtmlEncode(m.Value));

            }

            //remove the js event in the tag
            matches = Regex.Matches(str, @"<[^/][^>]+>");

            foreach (Match m in matches)
            {
                str = str.Replace(m.Value, Regex.Replace(m.Value, @"on\w+\s*=", ""));
            }
            return str;
        }
    }
}
