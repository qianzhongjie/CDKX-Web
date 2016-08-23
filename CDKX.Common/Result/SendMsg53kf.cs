using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CDKX.Common.Json;

namespace CDKX.Common.Result
{
    public class SendMsg53kf
    {
        /// <summary>
        /// 发起一个HTTP请求（以GET方式）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <param name="phoneNo"></param>
        /// <returns></returns>
        public static ResultData HttpGet(int type, string code, string phoneNo)
        {
            string url = "";
            if (type == 1 || type == 3 || type == 2)
            {
                url =
                    "http://sms6.53kf.com/interface/index.php?controller=sendmsgsignature&user=72128165&pwd=BRSB82p2fcFTb5ate2iZ8abWQ3CyQXXc&id6d=10176120&numbers=" +
                    phoneNo + "&content=" + code + "&template_id=4059";
            }
            else
            {
                url =
                    "http://sms6.53kf.com/interface/index.php?controller=sendmsgsignature&user=72128165&pwd=BRSB82p2fcFTb5ate2iZ8abWQ3CyQXXc&id6d=10176120&numbers=" + phoneNo + "&content=" + code + "&template_id=4159";
            }
            WebRequest myWebRequest = WebRequest.Create(url);

            WebResponse myWebResponse = myWebRequest.GetResponse();
            Stream ReceiveStream = myWebResponse.GetResponseStream();
            string responseStr = "";
            if (ReceiveStream != null)
            {
                StreamReader reader = new StreamReader(ReceiveStream, Encoding.UTF8);
                responseStr = reader.ReadToEnd();
                reader.Close();
            }
            myWebResponse.Close();
            return JsonTo.JsonStringToObject<ResultData>(responseStr);
        }

        public class ResultData
        {
            public int code { get; set; }
            public string status { get; set; }

            public string msg { get; set; }
        }
    }
}
