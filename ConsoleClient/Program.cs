using System.Net.WebSockets;
using System.Text;

namespace ConsoleClient;

class Program
{
  static async Task Main(string[] args)
  {
    Console.WriteLine("Hello, Socket World!");
    await ConnectHttp2();
  }

  static async Task ConnectHttp2()
  {
    using SocketsHttpHandler handler = new();
    using ClientWebSocket socket = new();

    Uri uri = new("wss://localhost:7001/server/messaging");
    HttpMessageInvoker invoker = new(handler);
    await socket.ConnectAsync(uri, invoker, default);

    byte[] buffer = new byte[1024 * 4];
    DateTime killMeOn = DateTime.Now.AddSeconds(30);
    bool killMeNow = false;
    while (killMeNow || DateTime.Now < killMeOn)
    {
      WebSocketReceiveResult result = await socket.ReceiveAsync(new(buffer), CancellationToken.None);
      string payload = Encoding.UTF8.GetString(buffer);
      string punchLine = result.CloseStatusDescription ?? "wall of silence...";

      Console.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
      Console.WriteLine(payload);
      Console.WriteLine(punchLine);

      Console.WriteLine("Client going to sleep...");
      Thread.Sleep(2000);
      
      killMeNow = result.MessageType == WebSocketMessageType.Close;
    }

    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "I'm done.", CancellationToken.None);
  }

  static async Task ConnectHttp1()
  {
    Uri uri = new("wss://localhost:7001/server/messages");
    ClientWebSocket client = new();
    await client.ConnectAsync(uri, default);

    byte[] buffer = new byte[1024];
    WebSocketReceiveResult result = await client.ReceiveAsync(buffer, default);
    string text = Encoding.UTF8.GetString(buffer, 0, result.Count);
    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Good bye", default);
  }
}