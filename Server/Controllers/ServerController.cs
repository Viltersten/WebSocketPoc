using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController, Route("[controller]")]
public class ServerController : ControllerBase
{
  readonly IService _service;
  readonly ILogger<ServerController> _logger;

  public ServerController(IService service, ILogger<ServerController> logger)
    => (_service, _logger) = (service, logger);

  [HttpGet("ping")]
  public IActionResult Ping()
  {
    // todo Ensure pong being echoed through websocket.
    return Ok("HTML pong");
  }

  [Route("messaging")]
  public async Task Messaging()
  {
    if (HttpContext.WebSockets.IsWebSocketRequest)
      using (WebSocket socket = await HttpContext.WebSockets.AcceptWebSocketAsync())
        await _service.Emit(socket);
    else
      HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
  }
}