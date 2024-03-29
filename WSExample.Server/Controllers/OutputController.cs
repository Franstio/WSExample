using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WSExample.Server.WSHandler;

namespace WSExample.Server.Controllers
{
    [Route("api/[controller]")]
    [Controller]
    public class OutputController : ControllerBase,IDisposable
    {
        private IWSHandler _outputWebSocketHandler;
        private OutputController(IWSHandler outputWebSocketHandler)
        {
            _outputWebSocketHandler = outputWebSocketHandler;
        }

        public void Dispose()
        {
            _outputWebSocketHandler.cts.Cancel();
        }

        public IActionResult Get()
        {
            return Ok("ok");
        }
        [Route("ws")]
        public async Task WS()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            using (var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync())
            {
                await _outputWebSocketHandler.AddWSClient(webSocket);

                HttpContext.Response.StatusCode = StatusCodes.Status200OK;
            }
        }
    }
}
