using System.Text.Json.Serialization;

#nullable disable
namespace Infrastructure.Messaging
{
    public class Message
    {
        [JsonInclude]
        public Guid MessageId;
        [JsonInclude]
        public string MessageType;
        [JsonInclude]
        public object Data;

        // Empty constructor required for Json Serializing
        public Message() { }

        public Message(object data) : this(Guid.NewGuid(), data.GetType().Name.Replace("Event", ""), data) { }
        public Message(Guid messageId, object data) : this(messageId, data.GetType().Name.Replace("Event", ""), data) { }
        public Message(string messageType, object data) : this(Guid.NewGuid(), messageType, data) { }
        public Message(Guid messageId, string messageType, object data)
        {
            Console.WriteLine(messageType);
            MessageId = messageId;
            MessageType = messageType;
            Data = data;
        }
    }
}
