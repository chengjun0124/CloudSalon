using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Common
{
    public class AppNotification
    {
        private const string APPMASTERSECRET = "lqn09rxxmmutbn6dzjp45pfw1wezwdin";
        private const string APPKEY = "597f621ce88bad6d48001e5d";
        private const string ALIASTYPE = "jiyunorg";


        public static void SendAppNotification(int employeeId, string ticker, string title, string text, string description, string url, DateTime expireTime)
        {   
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                appkey = APPKEY,
                timestamp = "" + GetTimestamp(DateTime.Now).ToString() + "",
                type = "customizedcast",//customizedcast,broadcast
                alias_type = ALIASTYPE,
                alias = "" + employeeId + "",
                payload = new
                {
                    display_type = "notification",
                    body = new
                    {
                        ticker = ticker,
                        title = title,
                        text = text,
                        icon = "notification_s.jpg",
                        largeIcon = "notification_l.jpg",
                        sound = "appointment",
                        play_vibrate = "true",
                        play_lights = "true",
                        play_sound = "true",
                        after_open = "go_custom",
                        custom = url,
                    }
                },
                policy = new
                {
                    expire_time = "" + expireTime.ToString("yyyy-MM-dd HH:mm:ss") + "",  //yyyy-MM-dd HH:mm:ss
                },
                production_mode = "true",
                description = description
            });
            string method = "POST";
            string apiUrl = "http://msg.umeng.com/api/send";//此接口参数，参见http://dev.umeng.com/push/android/api-doc#2
            string sign = EncodingHelper.MD5(method + apiUrl + body + APPMASTERSECRET);


            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.CreateHttp(apiUrl + "?sign=" + sign);
            request.Method = method;
            byte[] b = System.Text.Encoding.UTF8.GetBytes(body);

            using (Stream s = request.GetRequestStream())
            {
                s.Write(b, 0, b.Length);
            }


            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                using (Stream s = e.Response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(s, Encoding.UTF8))
                    {
                        string str = reader.ReadToEnd();
                        response.Dispose();
                        response = null;
                        request = null;
                        throw new Exception(str + "Request Body:" + body);
                    }
                }
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                using (Stream s = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(s, Encoding.UTF8))
                    {
                        string str = reader.ReadToEnd();
                        response.Dispose();
                        response = null;
                        request = null;
                        throw new Exception(reader.ReadToEnd() + "Request Body:" + body);
                    }
                }
            }
            
            response.Dispose();
            response = null;
            request = null;
        }

        private static int GetTimestamp(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}
