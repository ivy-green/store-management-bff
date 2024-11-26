using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Jobs.Core.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AppSetting
    {
        public Database? Database { get; set; }
        public AWSSection? AWSSection { get; set; }
        public RecurrentJobs? RecurrentJobs { get; set; }
        public DynamoDBTables? DynamoDBTables { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class RecurrentJobs
    {
        public List<BackgroundConfigsItem>? BackgroundConfigs { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class BackgroundConfigsItem
    {
        public string? JobId { get; set; }
        public string? Duration { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Database
    {
        public string DefaultConnection { get; set; } = string.Empty;
        public string DynamoConnection { get; set; } = string.Empty;
    }

    [ExcludeFromCodeCoverage]
    public class DynamoDBTables
    {
        public string LoggerTable { get; set; } = string.Empty;
        public string GeneralStatisticTable { get; set; } = string.Empty;
        public string Driver { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;
    }

    [ExcludeFromCodeCoverage]
    public class AWSSection
    {
        private string queueName = "";
        private const string FifoSuffix = ".fifo";
        public string AccessKey { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public string? S3Url { get; set; }
        public string? BillTopic { get; set; }
        public string? StatisticBillDLQ { get; set; }
        public string? StatisticBillQueue { get; set; }

        public string? LocalstackUrl { get; set; }
        public string? LoggerBucket { get; set; }
        public string? UserFileBucket { get; set; }
        public string? ProductFileBucket { get; set; }
        public int AwsQueueLongPollTimeSeconds { get; set; }
        public int AwsQueueMaxMessageResponse { get; set; }
        public bool AwsQueueIsFifo { get; set; }
        public string AwsDeadLetterQueueName
        {
            get
            {
                var deadLetter = queueName + "-exception";
                return AwsQueueIsFifo ? deadLetter + FifoSuffix : deadLetter;
            }
        }
    }
}
