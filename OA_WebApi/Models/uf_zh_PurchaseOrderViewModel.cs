using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OA_WebApi.Models;

namespace OA_WebApi.Models
{
    public class uf_zh_PurchaseOrderViewModel
    {
        public string Cg_no { get; set; }
        public string company { get; set; }
        public string Sup_no { get; set; }
        public string Sup_name { get; set; }
        public string Pay_type { get; set; }
        public string Ship_addr { get; set; }

    }
}