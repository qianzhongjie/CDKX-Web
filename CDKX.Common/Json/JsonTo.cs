using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CDKX.Common.Json
{
    public class JsonTo
    {
        /// <summary>
        /// 将实体对象转换成json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ObjectToJsonString<T>(T t)
        {
            try
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer json = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
                using (MemoryStream stream = new MemoryStream())
                {
                    json.WriteObject(stream, t);
                    return System.Text.Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch { }
            return null;
        }
        /// <summary>
        /// 将json对象转换成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonobj"></param>
        /// <returns></returns>
        public static T JsonStringToObject<T>(string jsonobj)
        {
            T t = default(T);
            try
            {
                JavaScriptSerializer convertModel = new JavaScriptSerializer();
                t = convertModel.Deserialize<T>(jsonobj);
            }
            catch { }
            return t;
        }

        public static string CustomToJson(string nums)
        {

            string[] sss = nums.Substring(1, nums.Length - 1).Split(',');
            string zjson = "{";
            foreach (var s in sss)
            {
                zjson += "\"" + s.Split('=').First().Replace(" ", "") + "\":\"" + s.Split('=').Last().Replace(" ", "") + "\",";
            }
            return zjson.Substring(0, zjson.Length - 1) + "}";
            //DataContractJsonSerializer json = new DataContractJsonSerializer(nums.GetType());

            //string szJson = "";

            ////序列化

            //using (MemoryStream stream = new MemoryStream())
            //{

            //    json.WriteObject(stream, nums);

            //    szJson = Encoding.UTF8.GetString(stream.ToArray());

            //}
            //return szJson;
        }

    }
}
