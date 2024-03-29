using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace WSExample.Server.WSHandler
{
    public class MockOutputWebSocketHandler : IWSHandler
    {
        public CancellationTokenSource cts { get; set; } = new CancellationTokenSource();
        public async Task AddWSClient(WebSocket ws)
        {
            var _payload = new { msg ="ok" };
            string payload = JsonSerializer.Serialize(_payload);
            byte[] buffer = Encoding.UTF8.GetBytes(payload);
            await ws.SendAsync(buffer, WebSocketMessageType.Text, true, default);
            await HandleRequest(cts.Token,ws);
        }

        protected async Task HandleRequest(CancellationToken stoppingToken,WebSocket ws)
        {
            Random rnd = new Random();
            WebSocketCloseStatus? status =null;
            do
            {
                var _payload = new { output = rnd.Next(0, 100) };
                string payload = JsonSerializer.Serialize(_payload);
                byte[] buffer = Encoding.UTF8.GetBytes(payload);
                byte[] bufferEnd = new byte[1024 * 10];
                if (stoppingToken.IsCancellationRequested)
                    return;
                if (ws is not null)
                {
                    var res = await ws.ReceiveAsync(new ArraySegment<byte>(bufferEnd), default);
                    status = res.CloseStatus;
                    string end = Encoding.UTF8.GetString(bufferEnd);
                    Console.WriteLine(end);
                    await ws.SendAsync(buffer, WebSocketMessageType.Text,true,default);
                }
                await Task.Delay(100);
            }
            while (!stoppingToken.IsCancellationRequested && !status.HasValue);

            await ws!.CloseAsync(ws.CloseStatus!.Value, ws.CloseStatusDescription, default);
        }
    }
}
