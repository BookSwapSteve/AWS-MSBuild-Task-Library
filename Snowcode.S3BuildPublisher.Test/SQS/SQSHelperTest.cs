using System.Threading;
using NUnit.Framework;
using Snowcode.S3BuildPublisher.SQS;

namespace Snowcode.S3BuildPublisher.Test.SQS
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class SQSHelperTest
    {
        private const string Container = "MySecretContainer";

        [Test]
        [Ignore("Manual run test")]
        public void CreateQueue_Should_CreateQueue()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SQSHelper helper = new SQSHelper(clientDetails);

            string queueUrl = helper.CreateQueue("TestQ");

            System.Diagnostics.Debug.WriteLine(queueUrl, "queueUrl:");

            Assert.IsNotEmpty(queueUrl);
        }

        [Test]
        [Ignore("Manual run test")]
        public void CreateQueueTwice_Should_Succeed()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SQSHelper helper = new SQSHelper(clientDetails);

            string url1 = helper.CreateQueue("TestQ2");
            string url2 = helper.CreateQueue("TestQ2");

            Assert.AreEqual(url1, url2, "Url's should match");
        }

        [Test]
        [Ignore("Manual run test")]
        public void SendMessage_Should_ReturnMessageId()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SQSHelper helper = new SQSHelper(clientDetails);


            const string messageBody = "TestMessageBody";
            string queueUrl = helper.CreateQueue("TestQ");

            string messageId = helper.SendMessage(messageBody, queueUrl);

            Assert.IsNotEmpty(messageId);
        }

        [Test]
        [Ignore("Manual run test")]
        public void ReceiveMessage_Should_ReceiveMessage()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SQSHelper helper = new SQSHelper(clientDetails);

            string queueUrl = helper.CreateQueue("TestQ");

            // Add a message to the queue to ensure that their is one and wait for 2 seconds to allow
            // the message to propogate.
            helper.SendMessage("Sample test message", queueUrl);
            Thread.Sleep(2000);

            string[] messageBodies = helper.ReceiveMessage(10M, queueUrl, true);

            Assert.IsNotEmpty(messageBodies, "No messages");

            string[] messageBodiesReRead = helper.ReceiveMessage(10M, queueUrl, true);
            Assert.IsEmpty(messageBodiesReRead, "Should not have messages on second read");
        }
    }
}
