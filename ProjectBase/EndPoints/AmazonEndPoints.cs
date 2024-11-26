using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Exceptions;
using ProjectBase.Insfracstructure.Services.Message.SNS;
using ProjectBase.Insfracstructure.Services.Message.SQS;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.EndPoints
{
    [ExcludeFromCodeCoverage]
    public static class AmazonEndPoints
    {
        public static void MapAmazonPoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/amazon");

            group.MapPost("create-topic", CreateTopic);

            group.MapPost("queue-list", GetQueueList);
        }

        public static async Task<IResult> CreateTopic(string topicName, ISnsMessage _snsMessage)
        {
            try
            {
                var res = await _snsMessage.CreateTopic(topicName);
                if (res != null)
                {
                    return Results.Ok();
                }
                return Results.BadRequest();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        //[HttpPost("create-queue")]
        public static async Task<IResult> CreateQueue(string queueName, ISqsMessage _sqsMessage)
        {
            // await _sqsMessage.CreateQueue(queueName);
            var deadLetterQueueUrl = await _sqsMessage.CreateQueue(queueName + "__dlq");
            await _sqsMessage.CreateQueue(queueName, deadLetterQueueUrl, "1", "20");

            return Results.Ok();
        }

        //[HttpPost("publish-message")]
        public static async Task<IResult> PublishMessage(SNSPublishMessageDTO data, ISnsMessage _snsMessage)
        {
            if (data.TopicName is null || data.message is null)
            {
                throw new NullException("Topic or Message is missing");
            }

            await _snsMessage.PublishMessage(data.TopicName, data.message);
            return Results.Ok();
        }

        //[HttpPost("poll-message")]
        public static async Task<IResult> PollSQSMessage(string queueName, ISqsMessage _sqsMessage)
        {
            try
            {
                var list = await _sqsMessage.ReceiveSQSMessage(queueName, default);
                return Results.Ok(list);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        //[HttpPost("restore-dead-letter-queue")]
        public static async Task<IResult> RestoreDeadLetterQueueSQSMessage(string queueName, ISqsMessage _sqsMessage)
        {
            try
            {
                await _sqsMessage.RestoreFromDeadLetterQueueAsync(queueName, default);
                return Results.Ok("Restore messages from dead-letter queue successfully");
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        //[HttpPost("toppic-list")]
        public static async Task<IResult> GetTopicList(ISnsMessage _snsMessage)
        {
            var res = await _snsMessage.ListTopicArns();
            return Results.Ok(res);
        }

        //[HttpPost("queue-list")]
        public static async Task<IResult> GetQueueList(ISqsMessage _sqsMessage)
        {
            var res = await _sqsMessage.GetSQSURls();
            return Results.Ok(res);
        }

        //[HttpPost("subscribe-topic")]
        public static async Task<IResult> SubscribeEmail(SNSSubscribeDTO data, ISnsMessage _snsMessage)
        {
            if (data.TopicName is null || data.QueueName is null)
            {
                throw new NullException("Topic or Queue's Name is missing");
            }

            try
            {
                await _snsMessage.SubscribeQueueToTopic(data.TopicName, data.QueueName);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
