using System.Net;
using Microsoft.Extensions.Logging;

namespace TranCons.EventListener.TCP
{
    internal class TcpClient : ITcpClient, IAsyncDisposable
    {
        private readonly ILogger _logger;
        private readonly TcpClientConfiguration _config;
        private readonly System.Net.Sockets.TcpClient _client = new();
        private readonly Task _listeningTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _disposed;

        public Action<byte[]>? ServerMessageHandler { get; set; }

        public TcpClient(ILogger logger,
            TcpClientConfiguration config)
        {
            _logger = logger;
            _config = config;
            _listeningTask = StartListeningForMessages();
        }

        private async Task StartListeningForMessages()
        {
            await _client.ConnectAsync(
                new IPEndPoint(IPAddress.Parse(_config.ServerAddress!), _config.ServerPort)
            );

            _logger.LogDebug("Connected to server");

            var hasSeenWelcomeMessage = false;

            await Task.Factory.StartNew(async () =>
            {
                var buffer = new byte[_config.BufferSize];
                while (!_disposed && _client.Connected)
                {
                    if (ServerMessageHandler == null)
                    {
                        await Task.Delay(100);
                        continue;
                    }

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


                        _logger.LogDebug($"Got message from server: {Convert.ToBase64String(message.ToArray())}");

                        //on first time we're getting server welcome message
                        if (hasSeenWelcomeMessage)
                            ServerMessageHandler.Invoke(message.ToArray());
                        else
                            hasSeenWelcomeMessage = true;
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }

            }, TaskCreationOptions.LongRunning);
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
}