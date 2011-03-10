using NUnit.Framework;
using Snowcode.S3BuildPublisher.Logging;
using Snowcode.S3BuildPublisher.SQS;

namespace Snowcode.S3BuildPublisher.Test.SQS
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class CreateSQSQueueTaskTest
    {
        [Test]
        [Ignore("Manual run test")]
        public void CreateQueue_Should_CreateQueue()
        {
            // Setup
            // TODO: Replace this with a mocked factory.
            IAwsClientFactory awsClientFactory = new AwsClientFactory();
            ITaskLogger logger = new NullLogger();

            var task = new CreateSQSQueueTask(awsClientFactory, logger)
                           {
                               EncryptionContainerName = TestHelper.EncryptionContainerName,
                               QueueName = "TestQ"
                           };

            // Execute
            bool suceeded = task.Execute();
            
            // Test
            Assert.IsTrue(suceeded, "Did not suceed");
            Assert.IsNotNullOrEmpty(task.QueueUrl, "QueueUrl");
        }

        [Test]
        [Ignore("Manual run test")]
        public void CreateQueueTwice_Should_Succeed()
        {
            // Setup
            // TODO: Replace this with a mocked factory.
            IAwsClientFactory awsClientFactory = new AwsClientFactory();
            ITaskLogger logger = new NullLogger();

            var task = new CreateSQSQueueTask(awsClientFactory, logger)
            {
                EncryptionContainerName = TestHelper.EncryptionContainerName,
                QueueName = "TestQ"
            };

            var task2 = new CreateSQSQueueTask(awsClientFactory, logger)
            {
                EncryptionContainerName = TestHelper.EncryptionContainerName,
                QueueName = "TestQ"
            };

            // Execute
            bool suceeded = task.Execute();
            bool suceeded2 = task2.Execute();

            // Test
            Assert.IsTrue(suceeded, "Did not suceed");
            Assert.IsTrue(suceeded2, "Task 2 did not suceed");

            Assert.IsNotNullOrEmpty(task.QueueUrl, "QueueUrl");
            Assert.AreEqual(task.QueueUrl, task2.QueueUrl, "Second queue url did not match first");
        }
    }
}
