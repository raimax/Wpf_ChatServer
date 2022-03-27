using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    public class Server
    {
        private readonly TcpListener _server;

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
    }
}
