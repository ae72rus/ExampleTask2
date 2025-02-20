namespace TranCons.EventListener.TCP;

public class TcpClientConfiguration
{
    public string? ServerAddress { get; set; }
    public int ServerPort { get; set; }
    public int BufferSize { get; set; } = 4096;

    internal TcpClientConfiguration()
    {

    }
}