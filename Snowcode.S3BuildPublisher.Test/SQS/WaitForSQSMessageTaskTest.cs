using NUnit.Framework;
using Snowcode.S3BuildPublisher.Logging;
using Snowcode.S3BuildPublisher.SQS;

namespace Snowcode.S3BuildPublisher.Test.SQS
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class WaitForSQSMessageTaskTest
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
        public void WaitForMessage_WithNoMessage_ShouldFail()
        {
            // Setup
            // TODO: Replace this with a mocked factory.
            IAwsClientFactory awsClientFactory = new AwsClientFactory();
            ITaskLogger logger = new NullLogger();

            var task = new WaitForSQSMessageTask(awsClientFactory, logger)
            {
                QueueUrl = _queueUrl,
                TimeOutSeconds = 10,
                PollIntervalSeconds = 5,
                EncryptionContainerName = TestHelper.EncryptionContainerName,
            };

            // Execute
            bool suceeded = task.Execute();

            // Test
            Assert.IsFalse(suceeded, "Should not have suceeded");
        }



        [Test]
        [Ignore("Manual run test")]
        public void WaitForMessage_WithMessageInQueue_ShouldSucceed()
        {
            // Setup
            // TODO: Replace this with a mocked factory.
            IAwsClientFactory awsClientFactory = new AwsClientFactory();
            ITaskLogger logger = new NullLogger();

            var sendMessageTask = new SendSQSMessageTask(awsClientFactory, logger)
                                      {
                                          QueueUrl = _queueUrl,
                                          MessageBody = "Test Wait for Mesage",
                                          EncryptionContainerName = TestHelper.EncryptionContainerName,
                                      };
            Assert.IsTrue(sendMessageTask.Execute(), "Failed to send setup message");

            // Message should have appeared in the 60 seconds timeout
            var task = new WaitForSQSMessageTask(awsClientFactory, logger)
            {
                QueueUrl = _queueUrl,
                TimeOutSeconds = 60,
                PollIntervalSeconds = 1,
                EncryptionContainerName = TestHelper.EncryptionContainerName,
            };

            // Execute
            bool suceeded = task.Execute();

            // Test
            Assert.IsTrue(suceeded, "Did not suceeded");
            Assert.IsNotNullOrEmpty(task.MessageBody, "MessageBody");
            Assert.IsNotNullOrEmpty(task.MessageId, "MessageId");
            Assert.IsNotNullOrEmpty(task.ReceiptHandle, "ReceiptHandle");
        }
    }
}
