using System;
using Amazon.S3.Model;

namespace Simple.Logging.Aws.Mappers
{
    public class LogMessageExceptionMapper : IMapper<LogMessage, PutObjectRequest>, IMapper<Exception, string>
    {
        public PutObjectRequest Map(LogMessage toMap)
        {
            var exceptionBody = Map(toMap.Exception);

            var request = new PutObjectRequest()
                .WithBucketName(toMap.ExceptionBucketName)
                .WithContentBody(exceptionBody)
                .WithKey(toMap.Identifier);

            return request;

        }

        public string Map(Exception exception)
        {
            var result = exception.ToString();

            return result;
        }
    }
}