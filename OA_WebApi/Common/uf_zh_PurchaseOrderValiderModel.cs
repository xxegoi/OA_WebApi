using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OA_WebApi.Models;
using Newtonsoft.Json.Linq;

namespace OA_WebApi.Common
{
    public class uf_zh_PurchaseOrderValiderModel
    {
        static OAContainer1 db = new OAContainer1();

        public static bool Valid(JToken obj,out uf_zh_PurchaseOrder header,out List<uf_zh_PurchaseOrder_dt1> details, out string message)
        {
            var H = typeof(uf_zh_PurchaseOrderViewModel);
            var D = typeof(uf_zh_PurchaseOrder_dt1ViewModel);

            var pis = H.GetProperties().ToList();
            var data = obj["cgmx"];

            foreach (var p in pis)
            {
                var item = obj[p.Name];
                if (item == null)
                {
                    message = p.Name + "值为null";
                    header = null;
                    details = null;
                    return false;
                }

                if (p.Name == "Cg_no")
                {
                    var cg_no = item.ToString();

                    if (db.uf_zh_PurchaseOrder.Count(o => o.Cg_no == cg_no) > 0)
                    {
                        message = "已存在 " + cg_no + " 的单据信息";
                        header = null;
                        details = null;
                        return false;
                    }
                }
            }

            
            if (data == null)
            {
                message = "明细记录为null";
                header = null;
                details = null;
                return false;
            }
            else
            {
                pis = D.GetProperties().ToList();
                foreach (var p in pis)
                {
                    foreach (var row in data.ToList())
                    {
                        var item = row[p.Name];
                        if (item == null)
                        {
                            message = p.Name + "值为null";
                            header = null;
                            details = null;
                            return false;
                        }

                        if (p.PropertyType == typeof(double))
                        {
                            double value;
                            if (!double.TryParse(item.ToString(), out value))
                            {
                                message = p.Name + "值类型错误，必须为数字";
                                header = null;
                                details = null;
                                return false;
                            }
                        }
                    }
                }
            }
            message = "";
            header = GetHeader(obj);
            details = GetDetails(data.ToList());
            return true;
        }

        private static uf_zh_PurchaseOrder  GetHeader(JToken obj) 
        {
            var t = typeof(uf_zh_PurchaseOrderViewModel);
            var pis = t.GetProperties();

            uf_zh_PurchaseOrderViewModel item = new uf_zh_PurchaseOrderViewModel();

            foreach (var p in pis)
            {
                var value = obj[p.Name].ToString() ;
                p.SetValue(item, value);
            }

            uf_zh_PurchaseOrder result = new uf_zh_PurchaseOrder()
            {
                Cg_no = item.Cg_no,
                company = item.company,
                Pay_type = item.Pay_type,
                Ship_addr = item.Ship_addr,
                Sup_name = item.Sup_name,
                Sup_no = item.Sup_no,
                formmodeid = 5,
                modedatacreater = 1,
                modedatacreatedate = DateTime.Now.Date.ToString("yyyy-MM-dd"),
                modedatacreatetime = DateTime.Now.ToString("hh:mm:ss"),
                modedatacreatertype=0
            };
            return result;
        }

        private static List<uf_zh_PurchaseOrder_dt1> GetDetails(List<JToken> dataList)
        {
            var t = typeof(uf_zh_PurchaseOrder_dt1);
            var pis = t.GetProperties().Where(p=>p.PropertyType!=typeof(Nullable)&&p.Name!="id"&&p.Name!= "mainid").ToList();
            var result =new List<uf_zh_PurchaseOrder_dt1>();

            dataList.ForEach(r =>
            {
                var item = new uf_zh_PurchaseOrder_dt1();
                var rowdata = JObject.Parse(r.ToString());

                pis.ForEach(p =>
                {
                    p.SetValue(item, rowdata[p.Name].ToString());
                });

                result.Add(item);
            });

            return result;
        }
    }
}