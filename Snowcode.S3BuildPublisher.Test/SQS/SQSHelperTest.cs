using System;
using System.Threading;
using NUnit.Framework;
using Snowcode.S3BuildPublisher.Client;
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
        public void DeleteQueue_Should_DeleteQueue()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SQSHelper helper = new SQSHelper(clientDetails);

            // Qreate a queue to delete.
            string queueUrl = TestHelper.CreateQueue("TestQ", Container);

            helper.DeleteQueue(queueUrl);
        }

        [Test]
        [Ignore("Manual run test")]
        public void SendMessage_Should_ReturnMessageId()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SQSHelper helper = new SQSHelper(clientDetails);


            const string messageBody = "TestMessageBody";
            string queueUrl = TestHelper.CreateQueue("TestQ", Container);

            string messageId = helper.SendMessage(messageBody, queueUrl);

            Assert.IsNotEmpty(messageId);
        }

        [Test]
        [Ignore("Manual run test")]
        public void ReceiveMessage_Should_ReceiveMessageAndNotRemoveIt()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SQSHelper helper = new SQSHelper(clientDetails);

            string queueUrl = TestHelper.CreateQueue("TestQ", Container);

            // Add a message to the queue to ensure that their is one and wait for 2 seconds to allow
            // the message to propogate.
            // Add the time on to ensure the correct message is received.
            string expectedMessage = "Sample test message " + DateTime.Now.ToLongTimeString(); 
            helper.SendMessage(expectedMessage, queueUrl);

            // Messages can be very slow to appear on the queue.
            Thread.Sleep(60000);

            // Get the sent message.
            Amazon.SQS.Model.Message message = helper.ReceiveMessage(queueUrl);
            Assert.IsNotNull(message, "No messages");
            
            try
            {
                Assert.AreEqual(expectedMessage, message.Body, "Expected message body first time");

                // Ensure that we can get the message a second time.
                Amazon.SQS.Model.Message message2 = helper.ReceiveMessage(queueUrl);
                Assert.IsNotNull(message2, "Message should not have been removed.");
                Assert.AreEqual(expectedMessage, message2.Body, "Expected message body second time");
            } 
            finally
            {
                // Delete the message.
                helper.DeleteMessage(queueUrl, message.ReceiptHandle);    
            }
        }

        [Test]
        [Ignore("Manual run test")]
        [ExpectedException(typeof(TimeoutException))]
        public void WaitForMessage_Should_ThrowTimeoutException()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SQSHelper helper = new SQSHelper(clientDetails);

            string queueUrl = TestHelper.CreateQueue("TestQ", Container);

            // Get the sent message.
            helper.WaitForMessage(queueUrl, 10, 5);
        }
    }
}
