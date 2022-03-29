namespace ChatServer
{
    public class Message
    {
        public MessageType Type { get; set; }
        public enum MessageType
        {
            Message,
            UserList,
            Info
        }

        public List<string> Data { get; set; } = new List<string>();
    }
}
