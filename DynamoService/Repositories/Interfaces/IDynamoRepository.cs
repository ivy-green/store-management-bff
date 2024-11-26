using Amazon.DynamoDBv2.Model;

namespace DynamoService.Repositories.Interfaces
{
    public interface IDynamoRepository
    {
        Task<List<TOut>?> GetAll<TOut>(string tableName);
        Task<TOut?> GetById<TOut>(string tableName, string driverName, string teamName);
        Task<bool> Add<TOut>(string tableName, TOut data);
        Task<bool> Update<TOut>(string tableName, TOut data);
        Task<bool> Delete(string tableName, string driverName, string teamName);
        Task<bool> RecreateTable(string tableName, List<AttributeDefinition> attrs, List<KeySchemaElement> keys);
    }
}
