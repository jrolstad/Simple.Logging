using System;
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
    public class When_logging_an_error
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
            log.Error("something bad happened",new Exception("foo"));
            
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
        public void Then_the_exception_is_saved_to_s3()
        {
            // Assert
            Assert.That(_s3Request, Is.Not.Null);
        }

        [Test]
        public void Then_the_item_is_logged_to_the_simpledb_domain()
        {
            // Assert
            Assert.That(_simpleDbRequest.DomainName, Is.EqualTo("my log domain"));
        }

        [Test]
        public void Then_the_exception_is_saved_to_the_correct_s3_bucket()
        {
            // Assert
            Assert.That(_s3Request.BucketName, Is.EqualTo("exceptions"));
        }

        [Test]
        public void Then_the_exception_details_are_saved()
        {
            // Assert
            Assert.That(_s3Request.ContentBody, Is.StringContaining("foo"));
        }

        [Test]
        public void Then_the_id_is_logged_to_the_simpledb_domain()
        {
            // Assert
            Assert.That(_simpleDbRequest.ItemName, Is.Not.Null);
        }

    }
}