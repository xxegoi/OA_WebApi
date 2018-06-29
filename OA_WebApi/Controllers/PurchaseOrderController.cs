using Newtonsoft.Json.Linq;
using OA_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OA_WebApi.Common;



namespace OA_WebApi.Controllers
{
    [Log]
    [Auth]
    public class PurchaseOrderController : ApiController
    {
        OAContainer1 db = new OAContainer1();

        [HttpPost]
        public ResponseModel Put(JObject obj)
        {
            string message = "";
            var data = obj.GetValue("data");
            string datastr = data == null ? "" : data.ToString();

            var requestId =obj.GetValue("requestid").ToString();


            uf_zh_PurchaseOrder header;
            List<uf_zh_PurchaseOrder_dt1> rows;

            try
            {

                if (data == null)
                {
                    return new FailResponseModel(requestId, "", "data值为空");
                }

                
                if (requestId == string.Empty)
                {
                    return new FailResponseModel("", datastr, "requestid为空");
                }

                if (!string.IsNullOrEmpty(datastr))
                {
                    var datacontext = JObject.Parse(datastr);
                    if (uf_zh_PurchaseOrderValiderModel.Valid(datacontext,out header,out rows, out message))
                    {

                        db.uf_zh_PurchaseOrder.Add(header);
                        db.SaveChanges();
                        //插入明细
                        rows.ForEach(p => {p.mainid=header.id; db.uf_zh_PurchaseOrder_dt1.Add(p); });

                        //插入权限
                        InsertRights(header.id);

                        db.SaveChanges();
                        return new SuccessResponseModel(requestId, datastr);
                    }
                    else
                    {
                        return new FailResponseModel(requestId, datastr, message);
                    }
                }

                return new FailResponseModel(requestId, datastr, "data值为空");
            }
            catch (Exception ex)
            {
                LogHandler.Error(string.Format("requestid:{0} \t data:{1} \t Exception:{2}", requestId, datastr, ex.Message));
                return new FailResponseModel(requestId, datastr, "发生异常");
            }
        }

        //插入权限数据
        private void InsertRights(int sourceid)
        {
            //过滤条件，跟踪出来系统就是这些，不知道是什么意思
            /*
             原查询语句是：
                SELECT * FROM    moderightinfo
                WHERE   modeid = 5
                AND ( sharetype IN ( 80, 90 ) OR sharetype NOT IN ( 80, 81, 84, 85, 89, 90 ) )
                AND righttype IN ( 1, 2, 3 )
             */
            int?[] shareType = new int?[] { 80, 90 };
            int?[] shareTypeNotIn = new int?[] { 80, 81, 84, 85, 89, 90 };
            int?[] rightType = new int?[] { 1, 2, 3 };

            //权限数量，对应模块中的默认共享权限
            var count = (from m in db.moderightinfo
                        where m.modeid == 5 &&
                        (shareType.Contains(m.sharetype) || !shareTypeNotIn.Contains(m.sharetype)) &&
                        rightType.Contains(m.righttype)
                        select m).Count();

            //先按sourceid排序，然后取单个souceid所有记录，为复制准备
            var modedatashareList = db.modeDataShare_5.OrderBy(p => p.sourceid).Take(count).ToList();
            //复制权限数据
            modedatashareList.ForEach(p =>
            {
                var item = CopyObject(p);
                //sourceid值为新插入订单ID
                item.sourceid = sourceid;
                db.modeDataShare_5.Add(item);
            });

            var modedatashareSetList = db.modeDataShare_5_set.OrderBy(p => p.sourceid).Take(count).ToList();
            modedatashareSetList.ForEach(p =>
            {
                var item = CopyObject(p);
                item.sourceid = sourceid;
                db.modeDataShare_5_set.Add(item);
            });

            db.SaveChanges();
        }

        /// <summary>
        /// 通过反射复制数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private T CopyObject<T>(T obj) where T: class ,new()
        {
            var t = typeof(T);
            var pis = t.GetProperties().ToList();
            T result = new T();

            pis.ForEach(p =>
            {
                var value = p.GetValue(obj);
                p.SetValue(result, value);
            });
            return result;
        }
    }
}
