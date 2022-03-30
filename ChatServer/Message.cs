namespace ChatServer
{
    public class Message
    {
        public MessageType Type { get; set; }
        public enum MessageType
        {
            Message,
            UserList,
            Info,
            File,
            ReceiveFile
        }

        public List<string> Data { get; set; } = new List<string>();
        public byte[] File { get; set; } = new byte[] { };
    }
}
