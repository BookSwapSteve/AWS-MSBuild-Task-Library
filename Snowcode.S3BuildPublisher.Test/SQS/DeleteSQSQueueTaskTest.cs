using NUnit.Framework;
using Snowcode.S3BuildPublisher.Logging;
using Snowcode.S3BuildPublisher.SQS;

namespace Snowcode.S3BuildPublisher.Test.SQS
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class DeleteSQSQueueTaskTest
    {
        [Test]
        [Ignore("Manual run test")]
        public void DeleteQueue_Should_DeleteQueue()
        {
            // Setup
            // TODO: Replace this with a mocked factory.
            IAwsClientFactory awsClientFactory = new AwsClientFactory();
            ITaskLogger logger = new NullLogger();

            // Create a queue to delete.
            string queueUrl = TestHelper.CreateQueue("TestQ", TestHelper.EncryptionContainerName);

            var task = new DeleteSQSQueueTask(awsClientFactory, logger)
                           {
                               QueueUrl = queueUrl,
                               EncryptionContainerName = TestHelper.EncryptionContainerName
                           };

            // Execute
            bool suceeded = task.Execute();

            // Test
            Assert.IsTrue(suceeded, "Did not suceed");
        }
    }
}
