using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace traodoisub.Model
{
    public class ApiResponse
    {
        [JsonProperty("success")]
        public int Success { get; set; }

        [JsonProperty("data")]
        public UserInfo Data { get; set; }
    }
    public class ApiResponseGetCoin
    {
        [JsonProperty("success")]
        public int Success { get; set; }

        [JsonProperty("data")]
        public DataCoin Data { get; set; }
    }
    public class DataCoin
    {
        public string TotalCoin { get; set; }
        public string ID { get; set; }
        public string msg { get; set; }
    }
}
