using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Common
{
    public class Message
    {
        private static CCPRestSDK.CCPRestSDK api = null;

        private static void Init()
        {
            api = new CCPRestSDK.CCPRestSDK();
            bool isInit = api.init(ConfigurationManager.AppSettings["MobileMsgApiDomain"], ConfigurationManager.AppSettings["MobileMsgApiPort"]);
            api.setAccount(ConfigurationManager.AppSettings["MobileMsgApiAccountId"], ConfigurationManager.AppSettings["MobileMsgApiAccountToken"]);
            api.setAppId(ConfigurationManager.AppSettings["MobileMsgApiAppId"]);
        }

        public static void SendMoblieMessage(string mobile, string tempId, string[] Variables, bool isDebug)
        {
            if (!isDebug)
            {
                if (api == null)
                    Init();

                Dictionary<string, object> retData = api.SendTemplateSMS(mobile, tempId, Variables);

                if (retData["statusCode"].ToString() != "000000")
                {
                    string ret = getDictionaryData(retData);

                    throw new Exception(ret);
                }
            }
        }

        private static string getDictionaryData(Dictionary<string, object> data)
        {
            string ret = null;
            foreach (KeyValuePair<string, object> item in data)
            {
                if (item.Value != null && item.Value.GetType() == typeof(Dictionary<string, object>))
                {
                    ret += item.Key.ToString() + "={";
                    ret += getDictionaryData((Dictionary<string, object>)item.Value);
                    ret += "};";
                }
                else
                {
                    ret += item.Key.ToString() + "=" + (item.Value == null ? "null" : item.Value.ToString()) + ";";
                }
            }
            return ret;
        }

    }


}
