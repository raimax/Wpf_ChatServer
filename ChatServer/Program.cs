using ChatServer;

Server server = new("127.0.0.1", 6666);

try
{
    server.StartConnection();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    server.CloseConnection();
}