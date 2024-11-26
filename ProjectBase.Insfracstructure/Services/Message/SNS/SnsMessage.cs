using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;
using ProjectBase.Domain.DTOs.Message;
using ProjectBase.Insfracstructure.Services.Message.SQS;

namespace ProjectBase.Insfracstructure.Services.Message.SNS
{
    public class SnsMessage : ISnsMessage
    {
        private readonly IAmazonSimpleNotificationService _sns;
        private readonly ISqsMessage _sqsMessage;
        public SnsMessage(IAmazonSimpleNotificationService sns, ISqsMessage sqsMessage)
        {
            _sns = sns;
            _sqsMessage = sqsMessage;
        }

        public async Task<string> CreateTopic(string topicName)
        {
            CreateTopicResponse createTopicResponse = await _sns.CreateTopicAsync(new CreateTopicRequest()
            {
                Name = topicName,
            });

            return createTopicResponse.TopicArn;
        }

        public async Task<bool> PublishMessage(string topicName, MessageDTO message)
        {
            var topicArnExists = await GetTopicArnByName(topicName);
            if (topicArnExists == null)
            {
                topicArnExists = await CreateTopic(topicName);
            }

            string json = JsonConvert.SerializeObject(message);
            PublishRequest req = new PublishRequest()
            {
                TopicArn = topicArnExists,
                Message = json,
            };

            try
            {
                var res = await _sns.PublishAsync(req);

                await Console.Out.WriteLineAsync($"Message sent: {res.MessageId}");
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                await Console.Out.WriteLineAsync($"SNS Error on Publish: {ex.Message}");
                throw;
            }
            return true;
        }

        public async Task SubscribeEmail(string topicArn, string mail)
        {
            SubscribeRequest subscribeRequest = new SubscribeRequest(topicArn, "email", mail);
            SubscribeResponse subscribeResponse = await _sns.SubscribeAsync(subscribeRequest);

            await Console.Out.WriteLineAsync($"Subscribe request Id: " +
                $"{subscribeResponse.ResponseMetadata.RequestId}");
            await Console.Out.WriteLineAsync("Check email for subscription");
        }

        public async Task DeleteTopic(string topicArn)
        {
            DeleteTopicRequest deleteTopicRequest = new DeleteTopicRequest(topicArn);
            await _sns.DeleteTopicAsync(deleteTopicRequest);
        }

        public async Task SubscribeQueueToTopic(string topicName, string queueName)
        {
            var topicArnExists = await GetTopicArnByName(topicName);
            if (topicArnExists == null)
            {
                topicArnExists = await CreateTopic(topicName);
            }

            var queueArn = await _sqsMessage.GetSqsArnByName(queueName);

            SubscribeRequest subscribeRequest = new SubscribeRequest(topicArnExists, "sqs", queueArn);
            SubscribeResponse subscribeResponse = await _sns.SubscribeAsync(subscribeRequest);

            await Console.Out.WriteLineAsync($"Subscribe request Id: " +
                $"{subscribeResponse.ResponseMetadata.RequestId}");
            await Console.Out.WriteLineAsync("Check email for subscription");
        }

        public async Task<IEnumerable<string>> ListTopicArns()
        {
            ListTopicsResponse listTopicsResponse = await _sns.ListTopicsAsync(new ListTopicsRequest());

            return listTopicsResponse.Topics.Select(x => x.TopicArn).ToList();
        }

        /*public async Task<IEnumerable<string>> ListTopicArns(string topinArn)
        {
            ListTopicsResponse listTopicsResponse = await _sns.ListSubscriptionsByTopicAsync(topinArn);
            return listTopicsResponse.Topics.Select(x => x.TopicArn).ToList();
        }*/

        public async Task<IEnumerable<Topic>> ListTopic()
        {
            ListTopicsResponse listTopicsResponse =
                await _sns.ListTopicsAsync(new ListTopicsRequest());

            return listTopicsResponse.Topics;
        }

        public async Task<string?> GetTopicArnByName(string topicName)
        {
            var list = await ListTopic();
            var item = list.FirstOrDefault(t => t.TopicArn.EndsWith($":{topicName}"))?
                           .TopicArn;

            return item;
        }
    }
}
