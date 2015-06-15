using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gdyzTransform.Common
{
    public class FormationResult
    {
        public JToken FormationJToken(ResponseResultCode resultCode, string errMsg, params JToken[] results)
        {
            JObject resultJson = new JObject();
            JObject resultHeadContext = new JObject();
            resultHeadContext.Add("resultCode", resultCode.ToString());
            resultHeadContext.Add("errMsg", errMsg);
            resultJson.Add("head", resultHeadContext);
            if (results != null)
            {
                foreach (JToken item in results)
                {
                    resultJson.Add("body", item);
                }

            }
            return resultJson;
        }


        public JToken FormationJToken(ResponseResultCode resultCode, String errMsg, String token, params JToken[] results)
        {
            JObject resultJson = new JObject();
            JObject resultHeadContext = new JObject();
            resultHeadContext.Add("resultCode", resultCode.ToString());
            resultHeadContext.Add("errMsg", errMsg);
            resultHeadContext.Add("token", token);
            resultJson.Add("head", resultHeadContext);
            if (results != null)
            {
                foreach (JToken item in results)
                {
                    resultJson.Add("body", item);
                }
            }
            return resultJson;
        }
    }
}