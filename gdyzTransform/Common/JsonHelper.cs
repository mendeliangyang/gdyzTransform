using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace gdyzTransform.Common
{
    public static class JsonHelper
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pJObj"></param>
        /// <param name="isNotContainAlert">如果没有存在 false表示异常，true表示返回空字符串empty</param>
        /// <returns></returns>
        internal static string GetStringFromJToken(this JToken jToken, string strProperty, bool isNotContainAlert = false)
        {
            if (jToken == null)
            {
                throw new Exception("GetStringFromJObject-JObject is null.");
            }
            try
            {
                JObject jObject = (JObject)jToken;
                JToken tempJToken = null;
                if (jObject.TryGetValue(strProperty, out tempJToken) || isNotContainAlert)
                {
                    if (tempJToken != null)
                    {
                        return tempJToken.ToString();
                    }
                    return string.Empty;
                }
                throw new Exception( string.Format("GetStringFromJObject-not find {0} .",strProperty));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetStringFromJObject-error .{0}", ex.Message));
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="strProperty"></param>
        /// <param name="isNotContainAlert">如果没有存在 false表示异常，true表示返回空JObject</param>
        /// <returns></returns>
        internal static JToken GetJTokenFromJToken(this JToken jToken, string strProperty, bool isNotContainAlert = false)
        {
            if (jToken == null)
            {
                throw new Exception("GetStringFromJObject-JObject is null.");
            }
            try
            {
                JObject jObject =(JObject)jToken;
                JToken tempJToken = null;
                if (jObject.TryGetValue(strProperty, out tempJToken) || isNotContainAlert)
                {
                    if (tempJToken != null)
                    {
                        return tempJToken;
                    }
                    return new JObject();
                }
                throw new Exception(string.Format("GetStringFromJObject-not find {0} .", strProperty));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetStringFromJObject-error .{0}", ex.Message));
            }


        }

        //protected JToken GetValueFromPostData(string key, bool isEmptyAlert = true, bool isNotContainAlert = true)
        //{
        //    if (!isNotContainAlert)
        //        isEmptyAlert = false;

        //    JToken value = null;
        //    if (!PostData.TryGetValue(key, out value) && isNotContainAlert)
        //        throw new InteralError("无法获取Post参数:'" + key + "'");

        //    if (value == null && isEmptyAlert)
        //        throw new InteralError("参数:" + key + "为空");
        //    if (value == null)
        //    {
        //        return JToken.Parse("");
        //    }

        //    return value;
        //}
        public static string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            //替换Json的Date字符串
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;

        }
        public static T JsonDeserialize<T>(string jsonString)
        {

            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            //MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            //T obj = (T)ser.ReadObject(ms);
            //return obj;
        }

        /// <summary>
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串
        /// </summary>
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        /// <summary>
        /// 将时间字符串转为Json时间

        /// </summary>
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }


    }
}