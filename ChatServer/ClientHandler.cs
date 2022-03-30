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
        public Action<Message.MessageType, string, string> broadcastFile { get; set; }

        public ClientHandler(
            TcpClient client,
            Action<Message.MessageType, string, string> broadcastMessage,
            Action<ClientHandler> removeClientHandler,
            Action<Message.MessageType, string, string> broadcastFile)
        {
            _client = client;
            _stream = client.GetStream();
            this.broadcastMessage = broadcastMessage;
            this.removeClientHandler = removeClientHandler;
            this.broadcastFile = broadcastFile;
            GetUserInfo();

            Task.Run(() =>
            {
                Listen();
            });
        }

        public async void Listen()
        {
            byte[] bytes = new byte[1024];
            Message message;
            int count;

            try
            {
                while ((count = _stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    message = Serializer.Deserialize(bytes);

                    if (message.Type == Message.MessageType.Message)
                    {
                        broadcastMessage(Message.MessageType.Message, message.Data[0], Username);
                    }
                    else if (message.Type == Message.MessageType.File)
                    {
                        SaveFileToDisk(message.Data[0], message.File);
                        broadcastFile(Message.MessageType.File, message.Data[0], Username);
                    }
                    else if (message.Type == Message.MessageType.ReceiveFile)
                    {
                        await SendFileToClient(message.Data[0]);
                    }
                }
            }
            catch (Exception)
            {
                Close();
            }
        }

        private static void SaveFileToDisk(string fileName, byte[] file)
        {
            try
            {
                File.WriteAllBytes(@$"files/{fileName}", file);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't save file: " + ex.Message);
            }
        }

        private async Task SendFileToClient(string fileName)
        {
            try
            {
                Message message = new();
                message.Type = Message.MessageType.ReceiveFile;
                message.Data.Add(fileName);
                message.File = File.ReadAllBytes(@$"files/{fileName}");

                byte[] replyMessage = Serializer.Serialize(message);
                await _stream.WriteAsync(replyMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't send file to client: " + ex.Message);
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
                Close();
            }
        }
    }
}
