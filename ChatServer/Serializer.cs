using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace ChatServer
{
    public static class Serializer
    {
        public static byte[] Serialize(object anySerializableObject)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                using (BsonWriter writer = new BsonWriter(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, anySerializableObject);
                }

                return ms.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Serialization error: " + ex.Message);
            }

            return new byte[1];
        }

        public static Message Deserialize(byte[] message)
        {
            try
            {
                MemoryStream ms = new MemoryStream(message);
                using (BsonReader reader = new BsonReader(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    return serializer.Deserialize<Message>(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Deserialization error: " + ex.Message);
            }

            return new Message();
        }
    }

}
