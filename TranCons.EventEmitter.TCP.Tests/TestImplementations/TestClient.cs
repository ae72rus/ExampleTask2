using System.Net;
using System.Net.Sockets;

namespace TranCons.EventEmitter.TCP.Tests.TestImplementations;

internal class TestClient : IAsyncDisposable
{
    private readonly TcpClient _client = new();
    private const int _bufferSize = 4096;
    private readonly int _port;
    private readonly Action<byte[]> _messageHandler;
    private readonly Task _listeningTask;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private bool _disposed;

    public TestClient(int port, Action<byte[]> onIncomingMessage)
    {
        _port = port;
        _messageHandler = onIncomingMessage;
        _listeningTask = StartListeningForMessages();
    }

    private async Task StartListeningForMessages()
    {
        await _client.ConnectAsync(
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port)
        );
        await Task.Run(async () =>
        {
            var buffer = new byte[_bufferSize];
            while (!_disposed && _client.Connected)
            {
                try
                {
                    var readCount = await _client.GetStream().ReadAsync(buffer, _cancellationTokenSource.Token);
                    var messageLength = BitConverter.ToInt32(buffer[..4]);
                    var message = buffer[4..readCount].ToList();
                    var messageReadLength = readCount - 4;//4 bytes takes payload length
                    while (messageReadLength < messageLength)//if message larger than buffer
                    {
                        readCount = await _client.GetStream().ReadAsync(buffer, _cancellationTokenSource.Token);
                        message.AddRange(buffer[..(readCount)]);
                        messageReadLength += readCount;
                    }

                    _messageHandler?.Invoke(message.ToArray());
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }

        });
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        _cancellationTokenSource.Cancel();
        await _listeningTask;
        if (_client.Connected)
        {
            _client.GetStream().Close();
            _client.Close();
        }

        _client.Dispose();
    }
}