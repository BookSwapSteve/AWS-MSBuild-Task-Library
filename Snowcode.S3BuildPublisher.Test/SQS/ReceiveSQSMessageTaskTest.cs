using System;
using System.Threading;
using NUnit.Framework;
using Snowcode.S3BuildPublisher.Logging;
using Snowcode.S3BuildPublisher.SQS;

namespace Snowcode.S3BuildPublisher.Test.SQS
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class ReceiveSQSMessageTaskTest
    {
        private string _queueUrl;

        [SetUp]
        public void Setup()
        {
            // Create a queue to use.
            _queueUrl = TestHelper.CreateQueue("TestQ", TestHelper.EncryptionContainerName);
        }

        [TearDown]
        public void TearDown()
        {
            TestHelper.DeleteQueue(_queueUrl, TestHelper.EncryptionContainerName);
        }

        [Test]
        [Ignore("Manual run test")]
        public void ReceiveMessage_Should_ReceiveMessage()
        {
            // Setup
            // TODO: Replace this with a mocked factory.
            IAwsClientFactory awsClientFactory = new AwsClientFactory();
            ITaskLogger logger = new NullLogger();
            
            // Add a message to the queue to ensure that their is one and wait for 2 seconds to allow
            // the message to propogate.
            // Add the time on to ensure the correct message is received.
            string expectedMessage = "Sample test message " + DateTime.Now.ToLongTimeString();
            var sendSqsMessageTask = new SendSQSMessageTask(awsClientFactory, logger)
                                         {
                                             QueueUrl = _queueUrl,
                                             MessageBody = expectedMessage,
                                             EncryptionContainerName = TestHelper.EncryptionContainerName,
                                         };

            sendSqsMessageTask.Execute();

            // Messages can be very slow to appear on the queue.
            Thread.Sleep(60000);

            var task = new ReceiveSQSMessageTask(awsClientFactory, logger)
                           {
                               QueueUrl = _queueUrl,
                               EncryptionContainerName = TestHelper.EncryptionContainerName,
                           };

            // Execute
            bool suceeded = task.Execute();

            // Test
            Assert.IsTrue(suceeded, "Did not suceed");
            Assert.IsTrue(task.HasMessage, "No message");
            Assert.IsNotNullOrEmpty(expectedMessage, task.MessageBody, "MessageId");
        }
    }
}
