using NUnit.Framework;
using Snowcode.S3BuildPublisher.Logging;
using Snowcode.S3BuildPublisher.SQS;

namespace Snowcode.S3BuildPublisher.Test.SQS
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class SendSQSMessageTaskTest
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
        public void SendMessage_Should_ReturnMessageId()
        {
            // Setup
            // TODO: Replace this with a mocked factory.
            IAwsClientFactory awsClientFactory = new AwsClientFactory();
            ITaskLogger logger = new NullLogger();

            const string messageBody = "TestMessageBody";

            var task = new SendSQSMessageTask(awsClientFactory, logger)
            {
                QueueUrl = _queueUrl,
                MessageBody = messageBody,
                EncryptionContainerName = TestHelper.EncryptionContainerName
            };

            // Execute
            bool suceeded = task.Execute();

            // Test
            Assert.IsTrue(suceeded, "Did not suceed");
            Assert.IsNotNullOrEmpty(task.MessageId, "MessageId");
        }
    }
}
