using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OA_WebApi.Models
{
    public class ResponseModel
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public string Data { get; set; }
        public string RequestID { get; set; }
        public string ProcCode { get; set; }
        public string ProcMsg { get; set; }
        public string ResponseDate { get; set; }

    }

    public class SuccessResponseModel : ResponseModel
    {
        public SuccessResponseModel(string requestid,string data)
        {
            this.Success = true;
            this.ResponseDate = DateTime.Now.ToString("yyyyMMddhhmmss");
            this.RequestID = requestid;
            this.ProcCode = "0000";
            this.Data = data;
        }
    }

    public class FailResponseModel : ResponseModel
    {
        public FailResponseModel(string requestid,string data,string message)
        {
            this.Success = false;
            this.ResponseDate = DateTime.Now.ToString("yyyyMMddhhmmss");
            this.RequestID = requestid;
            this.Data = data;
            this.ProcCode = "";
            this.Message = message;
        }
    }
}