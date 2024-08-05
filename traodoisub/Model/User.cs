using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace traodoisub.Model
{
    public class UserInfo
    {
        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("xu")]
        public string Xu { get; set; }

        [JsonProperty("xudie")]
        public string XuDie { get; set; }

        [JsonProperty("nameFB")]
        public string NameFB { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
    }
}
