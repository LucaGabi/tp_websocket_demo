using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace EchoConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            WebSocketRunner().GetAwaiter().GetResult();
        }
        private static async Task WebSocketRunner()
        {
            var client = new ClientWebSocket();

            // leave it to configurations will be a great idea.
            var webSocketUri = "ws://localhost:5000/channel";
            // var webSocketUri = "wss://localhost:5001/channel";
            await client.ConnectAsync(new Uri(webSocketUri), CancellationToken.None);

            Console.WriteLine("Connected!");

            var sending = Task.Run(async () =>
            {
                string line;
                while ((line = Console.ReadLine()) != null && line != string.Empty)
                {
                    var bytes = Encoding.UTF8.GetBytes(line);

                    var b64Str = Convert.ToBase64String(bytes);
                    var b64bytes = Encoding.UTF8.GetBytes(b64Str);

                    await client.SendAsync(new ArraySegment<byte>(b64bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            });
            var receiving = Receiving(client);
            await Task.WhenAll(sending, receiving);
        }

        private static async Task Receiving(ClientWebSocket client)
        {
            const int BUFFER_LENGTH = 4096; // 4 * 1024
            var buffer = new byte[BUFFER_LENGTH];
            while (true)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine(str);
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    // do nothing
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }
            }
        }
    }
}
