using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;

namespace Ergode.Bridge.Rpc
{
    internal abstract class RpcParams
    {

    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal class RpcMessage
    {
        public Guid Id { get; set; }

        public string Method { get; set; }

        public RpcParams Params { get; set; }
    }

    internal static class RpcMethod
    {
        public const string DialRequest = "dialRequest";

        public const string DialResponse = "dialResponse";

        public const string ReceiveData = "receiveData";

        public const string WriteDataRequest = "writeDataRequest";

        public const string WriteDataResponse = "writeDataResponse";
    }
}
