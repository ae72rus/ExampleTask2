namespace TranCons.EventEmitter.TCP;

internal interface ITcpServer
{
    Task SendAsync(byte[] message);
}