using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SagecomRouterAPI.Model
{
    public class Response
    {
        [JsonPropertyName("reply")]
        public Reply Reply { get; set; }

    }

    public class ReplyError
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class Reply
    {
        public readonly int Success_Code = 16777216;

        [JsonPropertyName("uid")]
        public int UID { get; set; }

        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("error")]
        public ReplyError Error { get; set; }

        [JsonPropertyName("actions")]
        public IEnumerable<Action> Actions { get; set; }
    }
}

