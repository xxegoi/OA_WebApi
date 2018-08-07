using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OA_WebApi.Models
{
    public class OADBContext
    {
        public static OAContainer1 GetDBContext()
        {
            OAContainer1 result = HttpContext.Current.Items["DbContext"] as OAContainer1;

            if (result == null)
            {
                result = new OA_WebApi.Models.OAContainer1();
                HttpContext.Current.Items["DbContexxt"] = result;
            }

            return result;
        }
    }
}