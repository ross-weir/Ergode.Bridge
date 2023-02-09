using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Ergode.Bridge.Network
{
    internal class TcpServerClientConnectedEventArgs : EventArgs
    {
        public TcpClient Client { get; set; }
    }

    internal class TcpServer : IDisposable
    {
        private readonly TcpListener listener;
        private readonly List<TcpClient> clients = new List<TcpClient>();

        public TcpServer(IPAddress listenAddress, int port)
        {
            listener = new TcpListener(listenAddress, port);

            AcceptConnectionsAsync().ContinueWith(OnAcceptConnectionsFailed, TaskContinuationOptions.OnlyOnFaulted);
        }

        public event EventHandler<TcpServerClientConnectedEventArgs> ClientConnectedEvent;

        public void CloseClientByStream(Stream stream)
        {
            var client = clients.Find(c => c.GetStream() == stream);

            client?.Close();
        }

        public void Dispose()
        {
            foreach (var client in clients)
            {
                client.Close();
            }

            listener.Stop();
        }

        private async Task AcceptConnectionsAsync()
        {
            var client = await listener.AcceptTcpClientAsync();
            clients.Add(client);

            ClientConnectedEvent?.Invoke(this, new TcpServerClientConnectedEventArgs { Client = client });

            await AcceptConnectionsAsync();
        }

        private void OnAcceptConnectionsFailed(Task task)
        {

        }
    }
}
