using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace OA_WebApi.Common
{
    public class LogHandler
    {
        static ILog logger = LogManager.GetLogger("EventLog");

        public static void Info(string message)
        {
            logger.Info(message);
        }

        public static void Error(string message)
        {
            LogManager.GetLogger("ExceptionLog").Error(message);
        }
    }
}