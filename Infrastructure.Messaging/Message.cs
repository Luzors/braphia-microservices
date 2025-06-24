namespace Infrastructure.Messaging
{
    public class Message
    {
        public readonly Guid MessageId;
        public readonly string MessageType;

        public Message():  this(Guid.NewGuid())
        {
        }
        public Message(Guid messageId)
        {
            Console.WriteLine("1 param alleen messagid");
            MessageId = messageId;
            MessageType = GetType().Name;
        }
        public Message(string messageType): this(Guid.NewGuid())
        {
            MessageType = messageType;
        }
        public Message(Guid messageId, string messageType)
        {
            Console.WriteLine("2 param");
            MessageId = messageId;
            MessageType = messageType;
        }
    }
}
