using Amazon.SimpleNotificationService.Model;

namespace ProjectBase.Insfracstructure.Services.Message.SQS
{
    public static class SqsMessageTypeAttribute
    {
        private const string AttributeName = "MessageType";
        public static string GetMessageTypeAttributeValue(this Dictionary<string,
            Amazon.SQS.Model.MessageAttributeValue> attributes)
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
