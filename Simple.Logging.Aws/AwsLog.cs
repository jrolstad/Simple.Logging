using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Simple.Logging.Aws.Mappers;

namespace Simple.Logging.Aws
{
    public class AwsLog:ILog
    {
        private readonly AmazonSimpleDB _simpleDbClient;
        private readonly AmazonS3 _s3Client;
        private readonly IMapper<LogMessage, PutAttributesRequest> _logMessageSimpleDbMapper;
        private readonly IMapper<LogMessage, PutObjectRequest> _logMessageS3Mapper;
        private readonly string _logDomainName;
        private readonly string _exceptionBucketName;
        private readonly string _logName;

        public AwsLog(AmazonSimpleDB simpleDbClient,
            Amazon.S3.AmazonS3 s3Client,
            IMapper<LogMessage,PutAttributesRequest> logMessageSimpleDbMapper, 
            IMapper<LogMessage,PutObjectRequest> logMessageS3Mapper,
            string logDomainName, 
            string exceptionBucketName,
            string logName)
        {
            _simpleDbClient = simpleDbClient;
            _s3Client = s3Client;
            _logMessageSimpleDbMapper = logMessageSimpleDbMapper;
            _logMessageS3Mapper = logMessageS3Mapper;
            _logDomainName = logDomainName;
            _exceptionBucketName = exceptionBucketName;
            _logName = logName;
        }

        public void Log(LogLevel level, string message)
        {
            LogMessageAsync(level, message);
        }

        public void Error(string message, Exception exception)
        {
           LogMessageAsync(LogLevel.Error, message,exception);
        }

        public void Fatal(string message, Exception exception)
        {
            LogMessageAsync(LogLevel.Fatal, message, exception);
        }

        private void LogMessageAsync(LogLevel level, string message, Exception exception = null)
        {
            var logMessage = new LogMessage
            {
                Level = level,
                Message = message,
                Exception = exception,
                LogDomainName = _logDomainName,
                ExceptionBucketName = _exceptionBucketName,
                Identifier = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                MachineName = Environment.MachineName,
                UserName = Environment.UserName,
                LogName = _logName
            };

            SaveToS3(logMessage);

            SaveToSimpleDb(logMessage);
        }

        private void SaveToSimpleDb(LogMessage logMessage)
        {
            Action saveToSimpleDbAction = () =>
                {
                    var simpleDbRequest = _logMessageSimpleDbMapper.Map(logMessage);
                    _simpleDbClient.PutAttributes(simpleDbRequest);
                };


            var saveToSimpleDbTask = new Task(saveToSimpleDbAction);
            saveToSimpleDbTask.Start();
        }

        private void SaveToS3(LogMessage logMessage)
        {
            if (logMessage.Exception != null)
            {
                Action saveToS3Action = () =>
                    {
                        var s3Request = _logMessageS3Mapper.Map(logMessage);
                        _s3Client.PutObject(s3Request);
                    };

                var saveToS3Task = new Task(saveToS3Action);
                saveToS3Task.Start();
            }
        }
    }
}