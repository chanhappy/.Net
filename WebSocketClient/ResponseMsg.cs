using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketClient
{
    class ResponseMsg
    {
        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("pathInfoList")]
        public object PathInfoList { get; set; }

        [JsonProperty("msgId")]
        public string MsgId { get; set; }
    }
}
