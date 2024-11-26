using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using ProjectBase.Jobs.ApplicationLogic.Utils;
using ProjectBase.Jobs.Core.Configuration;
using ProjectBase.Jobs.Core.DTOs.Requests;
using ProjectBase.Jobs.Core.Interfaces.IServices;

namespace ProjectBase.Jobs.ApplicationLogic.Services
{
    public class SqsMessage : ISqsMessage
    {
        private readonly IAmazonSQS _sqs;
        private readonly AppSetting _appSetting;
        public SqsMessage(IAmazonSQS sqs, AppSetting appSetting)
        {
            _sqs = sqs;
            _appSetting = appSetting;
        }

        public async Task<List<string>> GetSQSURls()
        {
            // confirming queue exists
            ListQueuesRequest listQueuesRequest = new ListQueuesRequest();
            ListQueuesResponse listQueuesResponse = await _sqs.ListQueuesAsync(listQueuesRequest);

            return listQueuesResponse.QueueUrls;
        }

        public async Task<string?> GetSqsByName(string queueName)
        {
            var list = await GetSQSURls();
            var item = list.FirstOrDefault(t => t.EndsWith($"/{queueName}"));

            return item;
        }

        public async Task<string> GetSqsArnByName(string queueName)
        {
            var queueUrlExists = await GetSqsByName(queueName);
            if (queueUrlExists == null)
            {
                queueUrlExists = await CreateQueueAsync(queueName);
            }

            var getQueueArn = new GetQueueAttributesRequest(queueUrlExists, ["QueueArn"]);
            var queueArn = await _sqs.GetQueueAttributesAsync(getQueueArn);

            return queueArn.QueueARN;
        }

        public async Task SendSqsMessage(string queueName, MessageDTO content)
        {
            var queueUrl = await GetSqsByName(queueName);

            // send message
            Console.WriteLine("Sending a message to My first queue.\n");
            string json = JsonConvert.SerializeObject(content);

            SendMessageRequest sendMessageRequest = new SendMessageRequest()
            {
                QueueUrl = queueUrl,
                MessageBody = json
            };

            await _sqs.SendMessageAsync(sendMessageRequest);
        }

        public async Task SendSqsMessage(string queueName, string content)
        {
            try
            {
                var queueUrl = await GetSqsByName(queueName);

                // send message
                SendMessageRequest sendMessageRequest = new SendMessageRequest()
                {
                    QueueUrl = queueUrl,
                    MessageBody = content
                };

                await _sqs.SendMessageAsync(sendMessageRequest);
            }
            catch (AmazonSQSException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
            }
            return;
        }

        public async Task DeleteSqsMessage(string queueUrl, string messageRecieptHandle)
        {
            Console.WriteLine("Delete queue My first queue.\n");
            DeleteMessageRequest deleteSqsQueueRequest = new DeleteMessageRequest()
            {
                QueueUrl = queueUrl,
                ReceiptHandle = messageRecieptHandle
            };

            await _sqs.DeleteMessageAsync(deleteSqsQueueRequest);
        }

        public async Task<string> CreateQueueAsync(string queueName)
        {
            if (_appSetting.AWSSection is null)
            {
                throw new Exception("AWS setting section is missing");
            }

            const string arnAttribute = "QueueArn";

            try
            {
                var createQueueRequest = new CreateQueueRequest
                {
                    QueueName = queueName
                };

                if (_appSetting.AWSSection.AwsQueueIsFifo)
                {
                    createQueueRequest.Attributes.Add("FifoQueue", "true");
                }

                var createQueueResponse = await _sqs.CreateQueueAsync(createQueueRequest);
                createQueueRequest.QueueName = _appSetting.AWSSection.AwsDeadLetterQueueName;
                var createDeadLetterQueueResponse = await _sqs.CreateQueueAsync(createQueueRequest);

                // get the ARN of dead letter queue and configure main queue to deliver message to it
                var attributes = await _sqs.GetQueueAttributesAsync(new GetQueueAttributesRequest
                {
                    QueueUrl = createDeadLetterQueueResponse.QueueUrl,
                    AttributeNames = new List<string> { arnAttribute }
                });
                var deadLettterQueueArn = attributes.Attributes[arnAttribute];

                // Redrivepolicy on main queue to deliver messages to dead letter queue
                // if they fail processing after 3 times
                var redrivePolicy = new
                {
                    maxReceiveCount = "3",
                    deadLetterTargetArn = deadLettterQueueArn
                };

                await _sqs.SetQueueAttributesAsync(new SetQueueAttributesRequest
                {
                    QueueUrl = createQueueResponse.QueueUrl,
                    Attributes = new Dictionary<string, string>
                    {
                        {"RedrivePolicy", JsonConvert.SerializeObject(redrivePolicy) },
                        {"ReceiveMessageWaitTimeSeconds", _appSetting.AWSSection.AwsQueueLongPollTimeSeconds.ToString()}
                    }
                });

                await Console.Out.WriteLineAsync($"Queue created with URL: {createQueueResponse.QueueUrl}");
                return createQueueResponse.QueueUrl;
            }
            catch (AmazonSQSException ex)
            {
                await Console.Out.WriteLineAsync($"Error creating queue: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetQueueArn(string qUrl)
        {
            GetQueueAttributesResponse responseGetAtt = await _sqs.GetQueueAttributesAsync(
              qUrl, new List<string> { QueueAttributeName.QueueArn });
            return responseGetAtt.QueueARN;
        }

        private async Task ShowAllAttributes(string qUrl)
        {
            var attributes = new List<string> { QueueAttributeName.All };
            GetQueueAttributesResponse responseGetAtt =
              await _sqs.GetQueueAttributesAsync(qUrl, attributes);

            foreach (var att in responseGetAtt.Attributes)
            {
                Console.WriteLine($"\t{att.Key}: {att.Value}");
            }
        }

        public async Task<string> CreateQueue(
          string qName, string? deadLetterQueueUrl = null,
          string? maxReceiveCount = null, string? receiveWaitTime = null)
        {
            var attrs = new Dictionary<string, string>();

            // If a dead-letter queue is given, create a message queue
            if (!string.IsNullOrEmpty(deadLetterQueueUrl))
            {
                attrs.Add(QueueAttributeName.ReceiveMessageWaitTimeSeconds, receiveWaitTime ?? "20");
                attrs.Add(QueueAttributeName.RedrivePolicy,
                  $"{{\"deadLetterTargetArn\":\"{await GetQueueArn(deadLetterQueueUrl)}\"," +
                  $"\"maxReceiveCount\":\"{maxReceiveCount}\"}}");
                // Add other attributes for the message queue such as VisibilityTimeout
            }

            // If no dead-letter queue is given, create one of those instead
            //else
            //{
            //  // Add attributes for the dead-letter queue as needed
            //  attrs.Add();
            //}

            // Create the queue
            CreateQueueResponse responseCreate = await _sqs.CreateQueueAsync(
                new CreateQueueRequest { QueueName = qName, Attributes = attrs });

            return responseCreate.QueueUrl;
        }
        public async Task<List<Message>> ReceiveSQSMessage(
            string queueName,
            CancellationToken cancelToken,
            bool isRestore = false)
        {
            var queueUrl = await GetSqsByName(queueName);
            var request = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20
            };

            List<Message> resReturn = [];
            while (true)
            {
                var response = await _sqs.ReceiveMessageAsync(request, cancelToken);

                if (response.Messages.Count > 0)
                {
                    foreach (var message in response.Messages)
                    {
                        resReturn.Add(message);
                        Console.WriteLine($"Message received: {message.Body}");

                        // Delete the message after processing it
                        var deleteRequest = new DeleteMessageRequest
                        {
                            QueueUrl = queueUrl,
                            ReceiptHandle = message.ReceiptHandle
                        };

                        await _sqs.DeleteMessageAsync(deleteRequest);
                        Console.WriteLine($"Message deleted: {message.MessageId}");
                    }
                }
                else
                {
                    Console.WriteLine("No messages to process.");
                    break;
                }

                await Task.Delay(5000); // Wait 5 seconds before polling again
            }
            return resReturn;
        }

        public async Task RemoveSQSMessage(string queueName, Message message)
        {
            var queueUrl = await GetSqsByName(queueName);

            var deleteRequest = new DeleteMessageRequest
            {
                QueueUrl = queueUrl,
                ReceiptHandle = message.ReceiptHandle
            };

            await _sqs.DeleteMessageAsync(deleteRequest);
        }

        public async Task<Message?> ReceiveSingleSQSMessage(
            string queueName,
            CancellationToken cancelToken,
            bool isRestore = false)
        {
            var queueUrl = await GetSqsByName(queueName);
            var request = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20
            };
            var response = await _sqs.ReceiveMessageAsync(request, cancelToken);

            if (response.Messages.Count > 0)
            {
                foreach (var message in response.Messages)
                {
                    return message;
                }
            }
            else
            {
                Console.WriteLine("No messages to process.");
            }
            return null;
        }

        public async Task RestoreFromDeadLetterQueueAsync(
        string sourceQueueName,
        CancellationToken cancellationToken = default)
        {
            var deadLetterQueueName = sourceQueueName + "__dlq";

            var sourceQueueUrl = await GetSqsByName(sourceQueueName);
            if (sourceQueueUrl is null)
            {
                throw new Exception("Source queue not found!");
            }

            var token = new CancellationTokenSource();
            while (!token.Token.IsCancellationRequested)
            {
                var messages = await ReceiveSQSMessage(deadLetterQueueName, cancellationToken, true);
                if (!messages.Any())
                {
                    token.Cancel(); continue;
                }

                messages.ForEach(
                    async message =>
                    {
                        var messageType = message.MessageAttributes.GetMessageTypeAttributeValue();

                        if (messageType != null)
                        {
                            await SendSqsMessage(sourceQueueUrl, message.Body);
                            await DeleteSqsMessage(deadLetterQueueName, message.ReceiptHandle);
                        }
                    });
            }
        }
    }
}
