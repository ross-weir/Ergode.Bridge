using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Ergode.Bridge.Rpc
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal class DialRequest : RpcParams
    {
        [JsonProperty("connId")]
        public Guid ConnectionId { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }
    }
}
