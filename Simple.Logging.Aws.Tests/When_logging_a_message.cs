using System.Threading;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using NUnit.Framework;
using Rhino.Mocks;
using Simple.Logging.Aws.Mappers;

namespace Simple.Logging.Aws.Tests
{
    [TestFixture]
    public class When_logging_a_message
    {
        private PutAttributesRequest _simpleDbRequest;
        private PutObjectRequest _s3Request;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            // Arrange
            var simpleDbClient = MockRepository.GenerateStub<AmazonSimpleDB>();
            simpleDbClient.Stub(c => c.PutAttributes(Arg<PutAttributesRequest>.Is.Anything)).WhenCalled(args =>
                {
                    var request = args.Arguments[0] as PutAttributesRequest;
                    _simpleDbRequest = request;
                });

            var s3Client = MockRepository.GenerateStub<AmazonS3>();
            s3Client.Stub(c => c.PutObject(Arg<PutObjectRequest>.Is.Anything)).WhenCalled(args =>
                {
                    var request = args.Arguments[0] as PutObjectRequest;
                    _s3Request = request;
                });

            var log = new AwsLog(simpleDbClient, s3Client, new LogMessageMapper(), new LogMessageExceptionMapper(), "my log domain", "exceptions", "test log");

            // Act
            log.Log(LogLevel.Warn,"some message");
            
            // Wait for things to complete
            Thread.Sleep(1000);
        }

        [Test]
        public void Then_the_item_is_logged()
        {
            // Assert
            Assert.That(_simpleDbRequest,Is.Not.Null);
        }


        [Test]
        public void Then_an_exception_is_not_logged()
        {
            // Assert
            Assert.That(_s3Request,Is.Null);
        }

        [Test]
        public void Then_the_item_is_logged_to_the_simpledb_domain()
        {
            // Assert
            Assert.That(_simpleDbRequest.DomainName, Is.EqualTo("my log domain"));
        }

        [Test]
        public void Then_the_id_is_logged_to_the_simpledb_domain()
        {
            // Assert
            Assert.That(_simpleDbRequest.ItemName, Is.Not.Null);
        }

    }
}