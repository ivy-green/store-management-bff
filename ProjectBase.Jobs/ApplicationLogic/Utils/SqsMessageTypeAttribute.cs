using Amazon.SQS.Model;

namespace ProjectBase.Jobs.ApplicationLogic.Utils
{
    public static class SqsMessageTypeAttribute
    {
        private const string AttributeName = "MessageType";
        public static string GetMessageTypeAttributeValue(this Dictionary<string,
            MessageAttributeValue> attributes)
        {
            return attributes.SingleOrDefault(x => x.Key == AttributeName).Value?.StringValue ?? "";
        }

        public static Dictionary<string, MessageAttributeValue> CreateAttributes<T>(string messageType)
        {
            return new Dictionary<string, MessageAttributeValue>
            {
                {
                    AttributeName, new MessageAttributeValue
                    {
                        DataType = nameof(String),
                        StringValue = messageType
                    }
                }
            };
        }
    }
}
