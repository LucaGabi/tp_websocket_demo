using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading.Tasks;
using WebSocketManager;

namespace ChatApplication
{
    public class ChatMessageHandler : WebSocketHandler
    {
        public ChatMessageHandler(WebSocketConnectionManager socketManager)
            : base(socketManager)
        {
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var socketId = WebSocketConnectionManager.GetSocketId(socket);
            await SendMessageToAllAsync($"{socketId} is now joined chat room.");
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetSocketId(socket);

            var str = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count); // base64 string
            Debug.Print(str);
            var b64stringDecode = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(str));
            Debug.Print(b64stringDecode);

            var message = $"{socketId} said: {b64stringDecode}";
            await SendMessageToAllAsync(message);
        }
    }
}
