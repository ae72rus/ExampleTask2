namespace TranCons.EventListener.TCP;

internal interface ITcpClient
{
    public Action<byte[]> ServerMessageHandler { set; }
}