using ProjectBase.Domain.DTOs.Message;

namespace ProjectBase.Insfracstructure.Services.Message.SQS
{
    public interface ISqsMessage
    {
        Task SendSqsMessage(string queueUrl, MessageDTO content);
        Task<string> GetSqsArnByName(string queueName);
        Task DeleteSqsMessage(string queueUrl, string messageRecieptHandle);
        Task<string> CreateQueueAsync(string queueName);
        Task<string> CreateQueue(string qName, string? deadLetterQueueUrl = null,
          string? maxReceiveCount = null, string? receiveWaitTime = null);
        Task<List<string>> GetSQSURls();
        Task<string?> GetSqsByName(string queueName);
        Task<List<Amazon.SQS.Model.Message>> ReceiveSQSMessage(string queueName,
            CancellationToken cancelToken,
            bool isRestore = false);
        Task<Amazon.SQS.Model.Message?> ReceiveSingleSQSMessage(
            string queueName,
            CancellationToken cancelToken,
            bool isRestore = false);
        Task RemoveSQSMessage(string queueUrl, Amazon.SQS.Model.Message message);
        Task RestoreFromDeadLetterQueueAsync(string sourceQueueName, CancellationToken cancellationToken = default);
    }
}