using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    public class Server
    {
        private readonly TcpListener _server;
        private readonly List<ClientHandler> _clientHandlers = new();

        public Server(string ipAddress, short port)
        {
            _server = new TcpListener(IPAddress.Parse(ipAddress), port);
        }

        public void StartConnection()
        {
            try
            {
                _server.Start();
                Console.WriteLine("Server started");
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Connection error: " + ex.Message);
            }

            while (true)
            {
                AcceptNewConnection();
            }
        }

        public void CloseConnection()
        {
            try
            {
                _server.Stop();
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Can't stop connection: " + ex.Message);
            }
        }

        private void AcceptNewConnection()
        {
            try
            {
                while (true)
                {
                    TcpClient client = _server.AcceptTcpClient();
                    Console.WriteLine("Client connected");
                    ClientHandler clientHandler = new(client, BroadcastMessage);

                    BroadcastMessage(Message.MessageType.Info, $"{clientHandler.Username} joined the chat");

                    _clientHandlers.Add(clientHandler);

                    BroadcastOnlineUsers();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error accepting client: " + ex.Message);
            }
        }

        private async void BroadcastMessage(Message.MessageType messageType, string message, string author = "")
        {
            if (_clientHandlers.Count > 0)
            {
                Message msg = new();
                msg.Type = messageType;
                msg.Data.Add(message);

                foreach (ClientHandler handler in _clientHandlers)
                {
                    if (handler.Username != author)
                    {
                        await handler.BroadcastMessage(msg);
                    }
                }
            }
        }

        private async void BroadcastOnlineUsers()
        {
            if (_clientHandlers.Count > 0)
            {
                List<string> message = new List<string>();

                foreach (ClientHandler handler in _clientHandlers)
                {
                    message.Add(handler.Username);
                }

                Message msg = new();
                msg.Type = Message.MessageType.UserList;
                msg.Data = message;

                foreach (ClientHandler handler in _clientHandlers)
                {
                    await handler.BroadcastMessage(msg);
                }
            }
        }
    }
}
