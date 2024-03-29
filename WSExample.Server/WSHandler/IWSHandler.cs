using System.Net.WebSockets;

namespace WSExample.Server.WSHandler
{
    public interface IWSHandler
    {
        Task AddWSClient(WebSocket ws);
        CancellationTokenSource cts { get; set; }
    }

}
