using System.Net.WebSockets;

namespace Server.Services;

public interface IService
{
  Task Emit(WebSocket socket);
}

public class Service : IService
{
  public async Task Emit(WebSocket socket)
  {
    try
    {
      byte[] buffer = new byte[1024 * 4];
      DateTime killMeOn = DateTime.Now.AddSeconds(10);
      bool killMeNow = false;
      while (killMeNow || DateTime.Now < killMeOn)
      {
        await socket.SendAsync(
          new(buffer, 0, 4096),
          WebSocketMessageType.Text,
          true,
          CancellationToken.None);

        Thread.Sleep(2000);

        WebSocketReceiveResult result = await socket.ReceiveAsync(new(buffer), CancellationToken.None);
        killMeNow = result.MessageType == WebSocketMessageType.Close;
      }

      await socket.CloseAsync(
        WebSocketCloseStatus.NormalClosure,
        "Good bye",
        CancellationToken.None);
    }
    catch (Exception exception)
    {
      Console.WriteLine(exception);
    }
  }

  public async Task EmitByMs(WebSocket socket)
  {
    try
    {
      byte[] buffer = new byte[1024 * 4];

      WebSocketReceiveResult result = await socket.ReceiveAsync(new(buffer), CancellationToken.None);

      while (!result.CloseStatus.HasValue)
      {
        await socket.SendAsync(
          new(buffer, 0, result.Count),
          result.MessageType,
          result.EndOfMessage,
          CancellationToken.None);
      }

      await socket.CloseAsync(
        result.CloseStatus.Value,
        result.CloseStatusDescription,
        CancellationToken.None);
    }
    catch (Exception exception)
    {
      Console.WriteLine(exception);
    }
  }
}
