using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OA_WebApi.Models
{
    public class uf_zh_PurchaseOrder_dt1ViewModel
    {
        public string Cg_no { get; set; }
        public string Mat_id { get; set; }
        public string Mat_name { get; set; }
        public string color { get; set; }
        public string Col_code { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public double quantity { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public double price { get; set; }
        /// <summary>
        /// 采购单价
        /// </summary>
        public double Pack_price { get; set; }
    }
}