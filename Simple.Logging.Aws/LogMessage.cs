using System;

namespace Simple.Logging.Aws
{
    public class LogMessage
    {
        public string Identifier { get; set; }

        public LogLevel Level { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public string LogDomainName { get; set; }

        public string ExceptionBucketName { get; set; }

        public DateTime CreatedAt { get; set; }

        public string MachineName { get; set; }

        public string UserName { get; set; }

        public string LogName { get; set; }
    }
}