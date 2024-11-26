using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.Data
{
    [ExcludeFromCodeCoverage]
    public class AppDynamoDBContext : DynamoDBContext
    {
        public AppDynamoDBContext(IAmazonDynamoDB client, DynamoDBContextConfig config) : base(client, config)
        {

        }
    }
}
