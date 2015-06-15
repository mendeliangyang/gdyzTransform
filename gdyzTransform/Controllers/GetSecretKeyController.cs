using gdyzTransform.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace gdyzTransform.Controllers
{
    public class GetSecretKeyController : ApiController
    {
        [HttpPost]
        public JToken GetSecretKey(dynamic strParam)
        {
            NLogHelper.DefalutInfo(string.Format("GetSecretKey: {0} .", strParam.ToString()));
            JObject jobject = JObject.Parse(strParam.ToString());

            return JObject.Parse("{\"head\":{\"resultCode\":\"Success\",\"errMsg\":\"\",\"token\":\"\"},\"body\":{\"primaryKey\":\"1111111111111111\"}}");

        }


    }
}
