using Newtonsoft.Json;
using System;

namespace AutoWrapper.Wrappers
{
    public class ApiResponse
    {
        public string Version { get; set; }

        public bool ResponseStatus { get; set; }

        public int ResponseCode { get; set; }

        public DateTime ResponseTime { get; set; }

        public string ResponseMessage { get; set; }

        public bool? IsError { get; set; } 

        public object ResponseException { get; set; }

        public object Data { get; set; }


        [JsonConstructor]
        public ApiResponse(string message, object result = null, int statusCode = 200, string apiVersion = "1.0.0.0")
        {
            this.Version = apiVersion;
            this.ResponseStatus = true;
            this.ResponseCode = statusCode;
            this.ResponseTime = DateTime.Now;
            this.ResponseMessage = message;
            this.Data = result;
          
        }

        public ApiResponse(object result, int statusCode = 200)
        {
            this.ResponseCode = statusCode;
            this.Data = result;
        }

        public ApiResponse(int statusCode, object apiError)
        {
            this.ResponseStatus = true;
            this.ResponseCode = statusCode;
            this.ResponseTime = DateTime.Now;
            this.ResponseException = apiError;
            this.IsError = true;
        }

        public ApiResponse() { }
    }
}


