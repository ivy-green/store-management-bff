using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ProjectBase.Domain.Configuration;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Exceptions;
using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IServices;
using ProjectBase.Insfracstructure.Services.FileService;
using System.Text;

namespace ProjectBase.Application.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettingConfiguration _setting;
        private readonly IFileService _fileService;
        private readonly DynamoService.Repositories.Interfaces.IDynamoRepository _dynamoRepository;
        public LoggerService(IUnitOfWork unitOfWork,
            AppSettingConfiguration setting,
            IFileService fileService,
            DynamoService.Repositories.Interfaces.IDynamoRepository dynamoRepository)
        {
            _unitOfWork = unitOfWork;
            _setting = setting;
            _fileService = fileService;
            _dynamoRepository = dynamoRepository;
        }

        public async Task<List<LoggerItem>?> GetAll(string loggerTableName)
        {
            if (_setting.DynamoDBTables is null)
            {
                throw new NullException("DynamoDB setting is missing");
            }

            return await _dynamoRepository.GetAll<LoggerItem>(loggerTableName);
        }

        public async Task<bool> Add(LoggerCreateRequestDTO data,
            string loggerTableName)
        {
            if (_setting.DynamoDBTables is null)
            {
                throw new NullException("DynamoDB setting is missing");
            }

            data.Driver = Guid.NewGuid().ToString();
            data.Team = DateTime.UtcNow.ToString();

            //return await _dynamoRepository.Add(_setting.DynamoDBTables.LoggerTable, data);
            return await _dynamoRepository.Add(loggerTableName, data);
        }

        public async Task<bool> Remove(string loggerTableName)
        {
            if (_setting.DynamoDBTables is null ||
                _setting.AWSSection is null ||
                _setting.AWSSection.LoggerBucket is null)
            {
                throw new NullException("DynamoDB or AWSSection setting is missing");
            }

            // Get all data & save it to S3 bucket
            var datas = await GetAll(loggerTableName);

            var json = JsonConvert.SerializeObject(datas);
            var memoryStream = new MemoryStream(Encoding.Default.GetBytes(json));
            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "test", "test.json");

            await _fileService.UploadFileS3(_setting.AWSSection.LoggerBucket, formFile);

            // Drop and recreate table in dynamodb
            var res = await _dynamoRepository.RecreateTable(
                loggerTableName,
                new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Driver",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "Team",
                        AttributeType = "S"
                    }
                },
                new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Driver",
                        KeyType = "HASH"
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "Team",
                        KeyType = "RANGE"
                    },
                });

            return res;
        }
    }
}
