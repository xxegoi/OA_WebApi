using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace OA_WebApi.Common
{
    public class LogAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var datastr = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;

            var data = JObject.Parse(datastr);

            var msg = data.GetValue("Message");
            var procmsg = data.GetValue("ProcMsg");

            string message = "";

            var requestid = data["RequestID"];

                       

            if (!string.IsNullOrEmpty(msg.ToString())  ||!string.IsNullOrEmpty( procmsg.ToString()))
            {
                message = string.IsNullOrEmpty(msg.ToString()) ? procmsg.ToString() : msg.ToString();

                LogHandler.Error("requestid: "+requestid+"\t"+message);
            }
            else
            {
                message = string.Format("{0}请求执行成功",requestid);
                LogHandler.Info(message);
            }


        }
    }
}