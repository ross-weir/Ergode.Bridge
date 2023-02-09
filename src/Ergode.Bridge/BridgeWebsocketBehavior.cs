using Ergode.Bridge.Rpc;
using Newtonsoft.Json;
using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Ergode.Bridge
{
    internal class DialRequestEventArgs : EventArgs
    {
        public WebSocket WebSocket { get; set; }

        public Guid RequestId { get; set; }

        public DialRequest Request { get; set; }
    }

    internal class WriteDataRequestEventArgs : EventArgs
    {
        public WebSocket WebSocket { get; set; }

        public Guid RequestId { get; set; }

        public WriteDataRequest Request { get; set; }
    }

    internal class BridgeWebsocketBehavior : WebSocketBehavior
    {

        public event EventHandler<DialRequestEventArgs> DialRequestEvent;

        public event EventHandler<WriteDataRequestEventArgs> WriteDataRequestEvent;

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);

            dynamic json = JsonConvert.DeserializeObject(e.Data);

            if (json.method == null)
            {
                return;
            }

            string method = json.method;
            var requestId = Guid.Parse(json.id.ToString());

            switch (method)
            {
                case RpcMethod.DialRequest:
                    var dialRequest = json.@params.ToObject<DialRequest>();

                    DialRequestEvent?.Invoke(this, new DialRequestEventArgs { Request = dialRequest, WebSocket = Context.WebSocket, RequestId = requestId });
                    break;
                case RpcMethod.WriteDataRequest:
                    var writeDataRequest = json.@params.ToObject<WriteDataRequest>();

                    WriteDataRequestEvent?.Invoke(this, new WriteDataRequestEventArgs { Request = writeDataRequest, WebSocket = Context.WebSocket, RequestId = requestId });
                    break;
                default:
                    break;
            }
        }
    }
}
