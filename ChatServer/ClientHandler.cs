using System.Net.Sockets;

namespace ChatServer
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        public string Username { get; set; } = "";
        public Action<Message.MessageType, string, string> broadcastMessage { get; set; }
        public Action<ClientHandler> removeClientHandler { get; set; }

        public ClientHandler(
            TcpClient client, 
            Action<Message.MessageType, string, string> broadcastMessage, 
            Action<ClientHandler> removeClientHandler)
        {
            _client = client;
            _stream = client.GetStream();
            this.broadcastMessage = broadcastMessage;
            this.removeClientHandler = removeClientHandler;
            GetUserInfo();

            Task.Run(() =>
            {
                Listen();
            });
        }

        public void Listen()
        {
            byte[] bytes = new byte[1024];
            Message message;
            int count;

            try
            {
                while ((count = _stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    message = Serializer.Deserialize(bytes);
                    Console.WriteLine($"Message from client ({Username}): " + message.Data[0]);

                    broadcastMessage(Message.MessageType.Message, message.Data[0], Username);
                }
            }
            catch (Exception)
            {
                Close();
            }
        }

        public void GetUserInfo()
        {
            byte[] bytes = new byte[1024];
            Message message;
            int count;

            try
            {
                while ((count = _stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    message = Serializer.Deserialize(bytes);
                    Username = message.Data[0];
                    break;
                }
            }
            catch (Exception)
            {
                Close();
            }
        }

        public void Close()
        {
            _client.Close();
            removeClientHandler(this);
        }

        public async Task BroadcastMessage(Message message)
        {
            try
            {
                if (_stream.CanWrite)
                {
                    byte[] replyMessage = Serializer.Serialize(message);
                    await _stream.WriteAsync(replyMessage);
                }
                else
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in BroadcastMessage: " + ex.Message);
            }
        }
    }
}
