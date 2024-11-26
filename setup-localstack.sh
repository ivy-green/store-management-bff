#!/bin/bash

# Create Dead-letter queue
echo "Creating Dead-letter queue..."
DEADLETTER_QUEUE_NAME=StatisticBillQueue__dlq
awslocal sqs create-queue --queue-name $DEADLETTER_QUEUE_NAME

# Create Main SQS queue with RedrivePolicy
echo "Creating SQS queue..."
MAIN_QUEUE_NAME=StatisticBillQueue
awslocal sqs create-queue --queue-name $MAIN_QUEUE_NAME

# Retrieve DLQ URL and ARN
DEADLETTER_QUEUE_URL=$(awslocal sqs get-queue-url --queue-name $DEADLETTER_QUEUE_NAME --query QueueUrl --output text)
DEADLETTER_QUEUE_ARN=$(awslocal sqs get-queue-attributes --queue-url $DEADLETTER_QUEUE_URL 
    --attribute-names QueueArn --query "Attributes.QueueArn" --output text)
echo "DLQ URL: $DEADLETTER_QUEUE_URL"
echo "DLQ ARN: $DEADLETTER_QUEUE_ARN"

# Retrieve Main Queue URL
MAIN_QUEUE_URL=$(awslocal sqs get-queue-url --queue-name $MAIN_QUEUE_NAME --query QueueUrl --output text)
echo "Main Queue URL: $MAIN_QUEUE_URL"

awslocal sqs set-queue-attributes \
--queue-url $MAIN_QUEUE_URL \
--attributes '{
    "RedrivePolicy": "{\"deadLetterTargetArn\":\"'"$DEADLETTER_QUEUE_URL"'\",\"maxReceiveCount\":\"1\"}"
}'

# Create SNS topic
echo "Creating SNS topic..."
TOPIC_NAME=BillTopic
awslocal sns create-topic --name $TOPIC_NAME

# Retrieve ARNs
TOPIC_ARN=$(awslocal sns list-topics --query "Topics[?contains(TopicArn, '$TOPIC_NAME')].TopicArn" --output text)
QUEUE_ARN=$(awslocal sqs get-queue-attributes --queue-url $MAIN_QUEUE_URL --attribute-names QueueArn --query "Attributes.QueueArn" --output text)

# Subscribe SQS queue to SNS topic
echo "Subscribing SQS queue '$MAIN_QUEUE_NAME' to SNS topic '$TOPIC_NAME'..."
awslocal sns subscribe --topic-arn $TOPIC_ARN --protocol sqs --notification-endpoint $QUEUE_ARN

# Create table of DynamoDB
LOGGER_TABLE=LoggerTable
awslocal dynamodb create-table \
   --table-name $LOGGER_TABLE \
   --attribute-definitions AttributeName=Driver,AttributeType=S AttributeName=Team,AttributeType=S \
   --key-schema AttributeName=Driver,KeyType=HASH AttributeName=Team,KeyType=RANGE \
   --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5
   
   
LOGGER_TABLE=GeneralStatisticTable
awslocal dynamodb create-table \
   --table-name $LOGGER_TABLE \
   --attribute-definitions AttributeName=Driver,AttributeType=S AttributeName=Team,AttributeType=S \
   --key-schema AttributeName=Driver,KeyType=HASH AttributeName=Team,KeyType=RANGE \
   --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5

# Create S3 bucket
awslocal s3 mb s3://coffee-logger-bucket
awslocal s3 mb s3://coffee-user-file

echo "Setup completed."
