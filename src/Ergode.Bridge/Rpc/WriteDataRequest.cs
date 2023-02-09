using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ergode.Bridge.Rpc
{
    internal class WriteDataRequest : RpcParams
    {
        [JsonProperty("connId")]
        public Guid ConnectionId { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
