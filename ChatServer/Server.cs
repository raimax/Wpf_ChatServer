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
                    ClientHandler clientHandler = new(client);

                    BroadcastMessage("New user joined the chat");

                    _clientHandlers.Add(clientHandler);

                    Task.Run(() =>
                    {
                        clientHandler.Listen();
                    });
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error accepting client: " + ex.Message);
            }
        }

        private async void BroadcastMessage(string message)
        {
            foreach (ClientHandler handler in _clientHandlers)
            {
                await handler.BroadcastMessage(new Message() { Data = message });
            }
        }
    }
}
