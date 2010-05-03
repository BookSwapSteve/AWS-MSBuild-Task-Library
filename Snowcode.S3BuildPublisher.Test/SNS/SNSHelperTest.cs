using NUnit.Framework;
using Snowcode.S3BuildPublisher.Client;
using Snowcode.S3BuildPublisher.SNS;

namespace Snowcode.S3BuildPublisher.Test.SNS
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class SNSHelperTest
    {
        private const string Container = "MySecretContainer";

        [Test]
        [Ignore("Manual run test")]
        public void CreateTopic_Should_CreateTopic()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SNSHelper helper = new SNSHelper(clientDetails);

            string topicId = helper.CreateTopic("TestTopic");

            System.Diagnostics.Debug.WriteLine("Topic Id: " + topicId);

            Assert.IsNotEmpty(topicId);
        }

        [Test]
        [Ignore("Manual run test")]
        public void CreateTopicTwice_Should_Succeed_WithMatchingArns()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SNSHelper helper = new SNSHelper(clientDetails);

            string topicArn = helper.CreateTopic("TestTopic2");
            string topicArn2 = helper.CreateTopic("TestTopic2");

            Assert.AreEqual(topicArn, topicArn2, "Topic Arn's should match");
        }

        [Test]
        [Ignore("Manual run test")]
        public void ListTopics_Should_ReturnTopics()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SNSHelper helper = new SNSHelper(clientDetails);

            string[] topics = helper.ListTopics("");

            Assert.AreEqual(2, topics.Length);
        }

        [Test]
        [Ignore("Manual run test")]
        public void Publish_Should_PublishANotification()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SNSHelper helper = new SNSHelper(clientDetails);

            const string topicArn = "arn:aws:sns:us-east-1:167532394791:TestTopic";

            string messageId = helper.Publish(topicArn, "Test", "This is a test of Publish...");

            Assert.IsNotEmpty(messageId);
        }

        [Test]
        [Ignore("Manual run test")]
        public void TestSubscrive_Should_Subscribe()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SNSHelper helper = new SNSHelper(clientDetails);

            const string topicArn = "arn:aws:sns:us-east-1:167532394791:TestTopic";
            const string protocol = "email";
            const string endpoint = "Email.Address@Example.com";

            string subscriptionArn = helper.Subscribe(topicArn, protocol, endpoint);

            Assert.IsNotEmpty(subscriptionArn);
        }

        [Test]
        [Ignore("Manual run test")]
        public void TestUnSubscribe_Doesnt_Fail()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SNSHelper helper = new SNSHelper(clientDetails);

            // TODO: Replace this with the valid subscription Arn
            const string subscriptionArn = "arn:aws:sns:us-east-1:167532394791:TestTopic:37be7a47-7e3f-418f-961e-2ef68af1749c";

            helper.Unsubscribe(subscriptionArn);
        }

        [Test]
        [Ignore("Manual run test - expected to fail due to AWS SDK validation issue.")]
        public void TestAddPermission_Sets_PermissionsOnTopic()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SNSHelper helper = new SNSHelper(clientDetails);
            string topicArn = helper.CreateTopic("TestSetPermissionsTopic");

            // TODO: Replace this with a valid AWS account Id.
            string[] awsAccountIds = new[] { "123456789012" };
            string[] actionNames = new[] { "*" };
            const string label = "Test Topic Permissions";

            // There appears to be an issue with the AWS SDK 1.0.8.1 which failes validation
            // for action names and aws account ids whilst they are valid.
            helper.AddPermission(actionNames, awsAccountIds, label, topicArn);

            // Now clean up and delete the topic.
            helper.DeleteTopic(topicArn);
        }
    }
}
