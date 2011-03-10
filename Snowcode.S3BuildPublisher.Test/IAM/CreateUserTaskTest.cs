using NUnit.Framework;
using Snowcode.S3BuildPublisher.IAM;
using Snowcode.S3BuildPublisher.Logging;

namespace Snowcode.S3BuildPublisher.Test.IAM
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class CreateUserTaskTest
    {
        private const string userName = "TestUserName4";

        [TearDown]
        public void TearDown()
        {
            // Delete the user so it can be reused.
        }

        [Test]
        [Ignore("Manual run test")]
        public void CreateQueue_Should_CreateQueue()
        {
            // Setup
            // TODO: Replace this with a mocked factory.
            IAwsClientFactory awsClientFactory = new AwsClientFactory();
            ITaskLogger logger = new NullLogger();

            var task = new CreateUserTask(awsClientFactory, logger)
                           {
                               EncryptionContainerName = TestHelper.EncryptionContainerName,
                               UserName = userName,
                           };

            // Execute
            bool suceeded = task.Execute();
           
            // Test
            Assert.IsTrue(suceeded, "Did not suceed");
            Assert.IsNotNullOrEmpty(task.UserId, "UserId");
            Assert.IsNotNullOrEmpty(task.Arn, "Arn");
        }
    }
}
