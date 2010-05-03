using NUnit.Framework;

namespace Snowcode.S3BuildPublisher.Test.S3
{
    [TestFixture]
    public class S3HelperTest
    {
        private const string Container = "MySecretContainer";

        [Test]
        [Ignore("Manual run test")]
        public void CreateBucket_Should_Succeed()
        {
            // Get the client details from the stored client details (rather than embed secret keys in the test).
            // Ensure that your AWS/Secret keys have been stored before running.
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            S3Helper ec2Helper = new S3Helper(clientDetails);

            ec2Helper.CreateBucket("ExampleTestBucket");
        }

        [Test]
        [Ignore("Manual run test")]
        public void DeleteBucket_Should_Succeed()
        {
            // Get the client details from the stored client details (rather than embed secret keys in the test).
            // Ensure that your AWS/Secret keys have been stored before running.
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            S3Helper ec2Helper = new S3Helper(clientDetails);

            ec2Helper.DeleteBucket("ExampleTestBucket");
        }
    }
}
