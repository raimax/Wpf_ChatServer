using System.Net.Sockets;

namespace ChatServer
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;

        public ClientHandler(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public void Listen()
        {
            byte[] bytes = new byte[1024];
            string? message;
            int count;

            try
            {
                while ((count = _stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    message = System.Text.Encoding.ASCII.GetString(bytes, 0, count);
                    Console.WriteLine("Message from client: " + message);
                    Reply(message);
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

        private void Reply(string message)
        {
            byte[] replyMessage = System.Text.Encoding.ASCII.GetBytes(message);
            _stream.Write(replyMessage, 0, replyMessage.Length);
        }
    }
}
