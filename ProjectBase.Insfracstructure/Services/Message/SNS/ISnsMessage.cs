
using ProjectBase.Domain.DTOs.Message;

namespace ProjectBase.Insfracstructure.Services.Message.SNS
{
    public interface ISnsMessage
    {
        Task<string?> GetTopicArnByName(string topicName);
        Task<string> CreateTopic(string topicName);
        Task DeleteTopic(string topicArn);
        Task<IEnumerable<string>> ListTopicArns();
        Task<bool> PublishMessage(string topicArn, MessageDTO message);
        Task SubscribeEmail(string topicArn, string mail);
        Task SubscribeQueueToTopic(string topicArn, string queueName);
    }
}