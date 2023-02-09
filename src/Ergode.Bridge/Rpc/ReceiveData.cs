using Newtonsoft.Json;
using System;

namespace Ergode.Bridge.Rpc
{
    internal class ReceiveData : RpcParams
    {
        [JsonProperty("connId")]
        public Guid ConnectionId { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
