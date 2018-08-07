using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

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

                LogHandler.Error("requestid: "+requestid+"\tMessage："+message+"\tData："+datastr.Replace("\\",""));
            }
            else
            {
                message = string.Format("{0}请求执行成功,Data:{1}",requestid, datastr.Replace("\\", ""));
                LogHandler.Info(message);
            }


        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            var httpcontext = actionContext.Request.Properties["MS_HttpContext"] as System.Web.HttpContextWrapper;

            var ip = httpcontext.Request.UserHostAddress;

            var controller = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var action = actionContext.ActionDescriptor.ActionName;
            var data = actionContext.ActionArguments["obj"].ToString();

            string log = string.Format("IP:{0}\tControll:{1}\tAction:{2}\tData:{3}", ip, controller, action, data);

            log4net.ILog logger = log4net.LogManager.GetLogger("RequestLog");

            logger.Info(log);

            base.OnActionExecuting(actionContext);
            
        }
    }
}