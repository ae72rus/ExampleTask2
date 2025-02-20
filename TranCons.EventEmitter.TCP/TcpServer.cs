using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace TranCons.EventEmitter.TCP;

internal class TcpServer : ITcpServer, IAsyncDisposable
{
    private readonly ILogger _logger;
    private readonly TcpListener _server;
    private readonly Task _listeningTask;
    private readonly List<TcpClient> _clients = new();
    private readonly CancellationTokenSource _onDisposeCancellationToken = new();
    private volatile object _clientsLock = new();
    private bool _disposed;

    public TcpServer(ILogger logger,
        TcpServerConfiguration config)
    {
        _logger = logger;
        _server = new TcpListener(IPAddress.Any, config.ServerPort);
        _server.Start();
        _listeningTask = ListenPort();
    }

    public async Task SendAsync(byte[] message)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(TcpServer));

        IReadOnlyCollection<TcpClient> recipients;
        lock (_clientsLock)
            recipients = _clients.Where(x => x.Connected).ToArray();

        foreach (var recipient in recipients)
            await SendToClientAsync(recipient, message);
    }

    private async Task ListenPort()
    {
        while (!_disposed)
        {
            TcpClient client;
            try
            {
                client = await _server.AcceptTcpClientAsync(_onDisposeCancellationToken.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            lock (_clientsLock)
                _clients.Add(client);

            await SendToClientAsync(client, Encoding.Default.GetBytes("You are connected to TranCons Input Module"));
        }
    }

    private async Task SendToClientAsync(TcpClient client, IReadOnlyCollection<byte> message)
    {
        try
        {
            if (!client.Connected)
                return;

            var length = message.Count;
            var lengthBytes = BitConverter.GetBytes(length);

            var messageBytes = lengthBytes.ToList();
            messageBytes.AddRange(message);
            var package = messageBytes.ToArray();

            _logger.LogDebug($"Sending message to {client.Client.RemoteEndPoint}. Message:\n {Convert.ToBase64String(package)}");

            await client.GetStream().WriteAsync(package);
        }
        catch (Exception e)
        {
            if (!client.Connected)
                return;

            _logger.LogError(e, $"Failed to send TCP message. Client: {client.Client.RemoteEndPoint}");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        _onDisposeCancellationToken.Cancel();
        _server.Stop();
        await _listeningTask;

        lock (_clientsLock)
            foreach (var client in _clients)
                client.Dispose();

        _onDisposeCancellationToken.Dispose();
    }

    internal int GetClientsCount()
    {
        lock (_clientsLock)
            return _clients.Count;
    }
}