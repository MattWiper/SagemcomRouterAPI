using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SagecomRouterAPI.Model
{
    public class Result
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class Callback
    {
        [JsonPropertyName("uid")]
        public int UID { get; set; }

        [JsonPropertyName("result")]
        public Result Result { get; set; }

        [JsonPropertyName("xpath")]
        public string XPath { get; set; }

        [JsonPropertyName("parameters")]
        public IDictionary<string, object> Parameters { get; set; }
    }

    public class Action //: Reply
    {
        [JsonPropertyName("callbacks")]
        public IEnumerable<Callback> Callbacks { get; set; }
    }
}
