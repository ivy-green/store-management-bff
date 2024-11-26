using ProjectBase.Domain.DTOs.Message;

namespace ProjectBase.Domain.DTOs.Requests
{
    public class SNSPublishMessageDTO
    {
        public string? TopicName { get; set; }
        public MessageDTO? message { get; set; }
    }

    public class SNSSubscribeDTO
    {
        public string? TopicName { get; set; }
        public string? QueueName { get; set; }
    }
}
