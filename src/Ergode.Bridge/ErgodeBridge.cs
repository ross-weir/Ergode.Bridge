using Ergode.Bridge.Network;
using Ergode.Bridge.Rpc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using WebSocketSharp.Server;

namespace Ergode.Bridge
{
    public class ErgodeBridge : IDisposable
    {
        private readonly WebSocketServer wsServer;
        private readonly List<ManagedStream> streams = new List<ManagedStream>();
        private TcpServer tcpServer;

        public ErgodeBridge(string listenUrl)
        {
            wsServer = new WebSocketServer(listenUrl);

            wsServer.AddWebSocketService("/", () => {
                var bridgeBehavior = new BridgeWebsocketBehavior();

                bridgeBehavior.DialRequestEvent += OnDialRequest;
                bridgeBehavior.WriteDataRequestEvent += OnWriteDataRequest;

                return bridgeBehavior;
            });
        }

        public void Start()
        {
            wsServer.Start();
        }

        public void Dispose()
        {
            foreach (var stream in streams)
            {
                stream.Dispose();
            }

            if (wsServer.IsListening)
            {
                wsServer.Stop();
            }
        }

        private void AcceptNewStream(ManagedStream stream)
        {
            stream.DataReceivedEvent += OnStreamDataReceived;
            streams.Add(stream);
        }

        private void OnDialRequest(object sender, DialRequestEventArgs e)
        {
            // could use client.ConnectAsync with a cancellation token and store the task/token in a map
            // so then the node could send a "abortDial" message that we could handle cancelling the connection
            var client = new TcpClient(e.Request.Host, e.Request.Port);
            var stream = new ManagedStream(e.Request.ConnectionId, client.GetStream());

            AcceptNewStream(stream);

            var responseParams = new DialResponse { ConnectionId = e.Request.ConnectionId, LocalAddress = client.Client.LocalEndPoint.ToString(), RemoteAddress = client.Client.RemoteEndPoint.ToString() };
            var response = new RpcMessage { Id = e.RequestId, Method = RpcMethod.DialResponse, Params = responseParams };

            string jsonResponse = JsonConvert.SerializeObject(response);

            e.WebSocket.Send(jsonResponse);
        }

        private void OnWriteDataRequest(object sender, WriteDataRequestEventArgs e)
        {
            var stream = streams.Find(s => s.Id == e.Request.ConnectionId);

            if (stream == null)
            {
                // TODO: return error response indicating stream doesn't exist
                return;
            }

            // TODO: check stream is in a valid state

            var data = Convert.FromBase64String(e.Request.Data);
            stream.WriteAsync(data).Wait();

            // TODO: return amount of bytes written in WriteDataResponse message
        }

        private void OnStreamDataReceived(object sender, ManagedStreamDataEventArgs e)
        {
            var receiveDataParams = new ReceiveData { ConnectionId = e.ConnectionId, Data = Convert.ToBase64String(e.Data) };
            var rpcMsg = new RpcMessage { Id = Guid.NewGuid(), Method = RpcMethod.ReceiveData, Params = receiveDataParams };
            string jsonMsg = JsonConvert.SerializeObject(rpcMsg);
            
            wsServer.WebSocketServices["/"].Sessions.Broadcast(jsonMsg);
        }

        private void OnTcpClientConnected(object sender, TcpServerClientConnectedEventArgs e)
        {
            var stream = new ManagedStream(e.Client.GetStream());

            AcceptNewStream(stream);

            // TODO: notify websocket
        }
    }
}
