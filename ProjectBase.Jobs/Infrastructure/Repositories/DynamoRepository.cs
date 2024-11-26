using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using ProjectBase.Jobs.Core.Interfaces.IRepositories;

namespace ProjectBase.Jobs.Insfrastructure.Repositories
{
    public class DynamoRepository : IDynamoRepository
    {
        private readonly IAmazonDynamoDB _context;
        public DynamoRepository(IAmazonDynamoDB context)
        {
            _context = context;
        }

        public async Task<List<TOut>?> GetAll<TOut>(string tableName)
        {
            var scanRequest = new ScanRequest()
            {
                TableName = tableName,
            };
            var scanResponse = await _context.ScanAsync(scanRequest);

            List<TOut> resultList = [];
            foreach (var item in scanResponse.Items)
            {
                var json = Document.FromAttributeMap(item);
                var obj = JsonConvert.DeserializeObject<TOut>(json.ToJson());
                if (obj is null)
                {
                    throw new Exception("Cannot convert item in LoggerTable");
                }
                resultList.Add(obj);
            }

            return resultList;
        }

        public async Task<TOut?> GetById<TOut>(string tableName, string driverName, string teamName)
        {
            var getItemRequest = new GetItemRequest()
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Driver", new AttributeValue(driverName) },
                    { "Team", new AttributeValue(teamName) },
                }
            };

            var getItemResponse = await _context.GetItemAsync(getItemRequest);
            if (getItemResponse is null)
            {
                throw new Exception("No item was found");
            }

            var getItemJson = Document.FromAttributeMap(getItemResponse.Item);
            return JsonConvert.DeserializeObject<TOut>(getItemJson.ToJson());
        }

        public async Task<bool> Add<TOut>(string tableName, TOut data)
        {
            // convert input data
            var json = JsonConvert.SerializeObject(data);
            var obj = Document.FromJson(json).ToAttributeMap();

            var putItemRequest = new PutItemRequest
            {
                TableName = tableName,
                Item = obj,
            };

            var putItemResponse = await _context.PutItemAsync(putItemRequest);
            return putItemResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> Delete(string tableName, string driverName, string teamName)
        {
            var deleteItemRequest = new DeleteItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "DriverName", new AttributeValue(driverName) },
                    { "TeamName", new AttributeValue(teamName) },
                }
            };

            var deleteItemResponse = await _context.DeleteItemAsync(deleteItemRequest);
            return deleteItemResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> Update<TOut>(string driverName, TOut data)
        {
            // convert input data
            var json = JsonConvert.SerializeObject(data);
            var obj = Document.FromJson(json).ToAttributeMap();

            var putItemRequest = new PutItemRequest
            {
                TableName = driverName,
                Item = obj,
            };

            var putItemResponse = await _context.PutItemAsync(putItemRequest);
            return putItemResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> RecreateTable(string tableName, List<AttributeDefinition> attrs, List<KeySchemaElement> keys)
        {
            try
            {
                // check if table exists
                var describeTable = new DescribeTableRequest
                {
                    TableName = tableName
                };
                await _context.DescribeTableAsync(describeTable);

                var deleteTableRequest = new DeleteTableRequest { TableName = tableName };
                await _context.DeleteTableAsync(deleteTableRequest);

                // check if table exists to create new one
                await _context.DescribeTableAsync(describeTable);

                return false; // cannot create new one
            }
            catch (ResourceNotFoundException)
            {
                var createTableRequest = new CreateTableRequest
                {
                    TableName = tableName,
                    AttributeDefinitions = attrs,
                    KeySchema = keys,
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    }
                };

                var createResponse = await _context.CreateTableAsync(createTableRequest);
                return createResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
        }
    }
}
