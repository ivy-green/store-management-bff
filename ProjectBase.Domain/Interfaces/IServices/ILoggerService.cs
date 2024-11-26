using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Entities;

namespace ProjectBase.Domain.Interfaces.IServices
{
    public interface ILoggerService
    {
        Task<bool> Add(LoggerCreateRequestDTO data, string loggerTableName);
        Task<List<LoggerItem>?> GetAll(string loggerTableName);
        Task<bool> Remove(string loggerTableName);
    }
}