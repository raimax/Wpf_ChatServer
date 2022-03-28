using System.Net.Sockets;

namespace ChatServer
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        public string Username { get; set; } = "";

        public ClientHandler(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public async void Listen()
        {
            bool firstTime = true;
            byte[] bytes = new byte[1024];
            Message message;
            int count;

            try
            {
                while ((count = _stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    if (firstTime)
                    {
                        message = Serializer.Deserialize(bytes);
                        Username = message.Data;
                        firstTime = false;
                    }
                    else
                    {
                        message = Serializer.Deserialize(bytes);
                        Console.WriteLine($"Message from client ({Username}): " + message.Data);
                        await BroadcastMessage(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Close();
                Console.WriteLine(ex.Message);
            }
        }

        public void Close()
        {
            _client.Close();
            Console.WriteLine("Client disconnected");
        }

        public async Task BroadcastMessage(Message message)
        {
            try
            {
                byte[] replyMessage = Serializer.Serialize(message);
                await _stream.WriteAsync(replyMessage, 0, replyMessage.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in BroadcastMessage: " + ex.Message);
            }
        }
    }
}
