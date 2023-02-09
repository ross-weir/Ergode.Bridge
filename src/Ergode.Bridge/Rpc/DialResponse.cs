using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;

namespace Ergode.Bridge.Rpc
{
    internal class DialResponse : RpcParams
    {
        [JsonProperty("connId")]
        public Guid ConnectionId { get; set; }

        [JsonProperty("localAddr")]
        public string LocalAddress { get; set; }

        [JsonProperty("remoteAddr")]
        public string RemoteAddress { get; set; }
    }
}
