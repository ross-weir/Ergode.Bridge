using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ergode.Bridge.Network
{
    internal class ManagedStreamDataEventArgs : EventArgs
    {
        public Guid ConnectionId { get; set; }

        public byte[] Data { get; set; }
    }

    internal class ManagedStream : IDisposable
    {
        private const int readBufferSize = 4096;
        private readonly Stream stream;
        private CancellationTokenSource readTokenSource;
        private CancellationTokenSource writeTokenSource;

        public ManagedStream(Guid id, Stream stream)
        {
            Id = id;
            this.stream = stream;

            Start();
        }

        public ManagedStream(Stream stream)
        {
            Id = Guid.NewGuid();
            this.stream = stream;
        }

        public event EventHandler<ManagedStreamDataEventArgs> DataReceivedEvent;

        public Guid Id { get; private set; }

        public async Task ReadStreamContinuationAsync()
        {
            readTokenSource = new CancellationTokenSource();

            var data = await ReadAsync(readTokenSource.Token);

            DataReceivedEvent?.Invoke(this, new ManagedStreamDataEventArgs { ConnectionId = Id, Data = data });

            await ReadStreamContinuationAsync();
        }

        public async Task<byte[]> ReadAsync(CancellationToken token)
        {
            var buffer = new byte[readBufferSize];

            int bytesRead = await stream.ReadAsync(buffer, 0, readBufferSize, token);

            if (bytesRead != readBufferSize)
            {
                Array.Resize(ref buffer, bytesRead);
            }

            return buffer;
        }

        public async Task WriteAsync(byte[] data)
        {
            writeTokenSource = new CancellationTokenSource();

            await stream.WriteAsync(data, 0, data.Length, writeTokenSource.Token);
        }

        private void Start()
        {
            _ = ReadStreamContinuationAsync().ContinueWith(OnReadFailure, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void OnReadFailure(Task task)
        {

        }

        public void Dispose()
        {
            readTokenSource?.Cancel();
            writeTokenSource?.Cancel();
            stream.Dispose();
        }
    }
}
