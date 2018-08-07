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
        

        public static bool Valid(JToken obj,out uf_zh_PurchaseOrder header,out List<uf_zh_PurchaseOrder_dt1> details, out string message)
        {
            var H = typeof(uf_zh_PurchaseOrderViewModel);
            var D = typeof(uf_zh_PurchaseOrder_dt1ViewModel);
            message = "";

            var pis = H.GetProperties().ToList();
            var data = obj["cgmx"];

            foreach (var p in pis)
            {
                var item = obj[p.Name.ToLower()];
                
                if (item == null&&p.Name.ToLower()!="ship_addr")
                {
                    message += p.Name + "值为null";
                    header = null;
                    details = null;
                    return false;
                }

                if (p.Name.ToLower() == "cg_no")
                {
                    var cg_no = item.ToString();
                    message +="cg_no："+ cg_no+"\t";

                    var db = OADBContext.GetDBContext();

                    if (db.uf_zh_PurchaseOrder.Count(o => o.Cg_no == cg_no) > 0)
                    {
                        var order = db.uf_zh_PurchaseOrder.SingleOrDefault(a => a.Cg_no == cg_no);
                        var id = order.id.ToString();
                        if (db.formtable_main_84.Count(a => a.cght == id) > 0)
                        {
                            header = null;
                            details = null;
                            message +=  "已有审批流程在运转，不能重复插入";
                            return false;
                        }
                        else
                        {
                            var h = db.uf_zh_PurchaseOrder.SingleOrDefault(a => a.Cg_no == cg_no);
                            var detail = db.uf_zh_PurchaseOrder_dt1.Where(a => a.mainid == h.id).ToList();

                            db.Entry(h).State = System.Data.Entity.EntityState.Deleted;
                            detail.ForEach(a =>
                            {
                                db.Entry(a).State = System.Data.Entity.EntityState.Deleted;
                            });
                            db.SaveChanges();

                            message += "已存在：" + cg_no + "单据，但未有流程在运转，本次将删除旧数据插入新值";

                            LogHandler.Info(message);
                        }
                    }
                }
            }

            
            if (data == null)
            {
                message += "明细记录为null";
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
                        var item = row[p.Name.ToLower()];
                        if (item == null)
                        {
                            message += p.Name + "值为null";
                            header = null;
                            details = null;
                            return false;
                        }

                        if (p.PropertyType == typeof(double))
                        {
                            double value;
                            if (!double.TryParse(item.ToString(), out value))
                            {
                                message += p.Name + "值类型错误，必须为数字";
                                header = null;
                                details = null;
                                return false;
                            }

                            if (value == 0)
                            {
                                message += p.Name + "不能为零";
                                header = null;
                                details = null;
                                return false;
                            }

                        }
                    }
                }
            }
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
                var item1 = obj[p.Name.ToLower()];
                if(item!=null&&p.Name.ToLower()!="ship_addr")
                    p.SetValue(item, item1.ToString());
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
                modedatacreatertype = 0,
                Sup_fullname = item.Sup_fullname,
                DeliveryStart = Convert.ToDateTime(item.DeliveryStart).ToString("yyyy-MM-dd"),
                DeliveryEnd= Convert.ToDateTime(item.DeliveryEnd).ToString("yyyy-MM-dd"),
                JSFS =item.JSFS
            };
            return result;
        }

        private static List<uf_zh_PurchaseOrder_dt1> GetDetails(List<JToken> dataList)
        {
            var t = typeof(uf_zh_PurchaseOrder_dt1);
            var pis = t.GetProperties().Where(p=>p.PropertyType!=typeof(Nullable)&&p.Name!="id"&&p.Name!= "mainid"&&!p.Name.Contains("delivery")).ToList();
            var result =new List<uf_zh_PurchaseOrder_dt1>();

            dataList.ForEach(r =>
            {
                var item = new uf_zh_PurchaseOrder_dt1();
                var rowdata = JObject.Parse(r.ToString());

                pis.ForEach(p =>
                {
                    p.SetValue(item, rowdata[p.Name.ToLower()].ToString());
                });

                result.Add(item);
            });

            return result;
        }
    }
}