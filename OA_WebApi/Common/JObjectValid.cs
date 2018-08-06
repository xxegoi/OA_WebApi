using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OA_WebApi.Common
{
    public class JObjectValid
    {
        public bool Valid<T>(JObject obj) where T : class, new()
        {
            if (!ValidProperties<T>(obj))
            {
                return false;
            }



            return false;
        }

        private bool ValidProperties<T>(JObject obj) where T : class, new()
        {
            var t = typeof(T).GetProperties().ToList();

            var t1 = obj.Properties().ToList();

            foreach (var o in t)
            {
                var flag = false;
                foreach (var p in t1)
                {
                    if (o.Name.ToLower() == p.Name.ToLower())
                        flag = true;
                    break;
                }
                if (!flag)
                {
                    return false;
                }
            }

            return true;
        }
    }
}