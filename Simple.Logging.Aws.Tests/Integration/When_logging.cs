using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Simple.Logging.Aws.Mappers;

namespace Simple.Logging.Aws.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class When_logging
    {

        [Test]
        public void Then_a_message_is_logged()
        {
            // Arrange
            var awsAccessKeyId = "<access key>";
            var awsSecretAccessKey = "<secret key>";

            var simpelDbClient = new Amazon.SimpleDB.AmazonSimpleDBClient(awsAccessKeyId, awsSecretAccessKey);
            var s3Client = new Amazon.S3.AmazonS3Client(awsAccessKeyId, awsSecretAccessKey);

            var domainName = "SimpleLog";
            var bucketName = "Log_Exception";
            var logName = "integration tests";
            var log = new AwsLog(simpelDbClient, s3Client, new LogMessageMapper(), new LogMessageExceptionMapper(), domainName, bucketName, logName);

            // Act
            log.Log(LogLevel.Info,"Hello world!");

            // Assert

        }

        [Test]
        public void Then_an_Exception_is_logged()
        {
            // Arrange
            var awsAccessKeyId = "<access key>";
            var awsSecretAccessKey = "<secret key>";

            var simpelDbClient = new Amazon.SimpleDB.AmazonSimpleDBClient(awsAccessKeyId, awsSecretAccessKey);
            var s3Client = new Amazon.S3.AmazonS3Client(awsAccessKeyId, awsSecretAccessKey);

            var domainName = "SimpleLog";
            var bucketName = "Log_Exception";
            var logName = "integration tests";
            var log = new AwsLog(simpelDbClient, s3Client, new LogMessageMapper(), new LogMessageExceptionMapper(), domainName, bucketName, logName);

            var exception = GetException();

            // Act
            log.Error("Uh oh!", exception);

            // Assert

        }

        public Exception GetException()
        {
            try
            {
                var numerator = 1;
                var divisor = 0;

                var result = numerator/divisor;

                return null;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }

    }
}