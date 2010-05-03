using NUnit.Framework;
using Snowcode.S3BuildPublisher.Client;
using Snowcode.S3BuildPublisher.S3;

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

            S3Helper helper = new S3Helper(clientDetails);

            helper.CreateBucket("ExampleTestBucket");
        }

        [Test]
        [Ignore("Manual run test")]
        public void DeleteBucket_Should_Succeed()
        {
            // Get the client details from the stored client details (rather than embed secret keys in the test).
            // Ensure that your AWS/Secret keys have been stored before running.
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            S3Helper helper = new S3Helper(clientDetails);

            helper.DeleteBucket("ExampleTestBucket");
        }

        [Test]
        [Ignore("Manual run test")]
        public void DeleteObject_Should_Succeed()
        {
            // Get the client details from the stored client details (rather than embed secret keys in the test).
            // Ensure that your AWS/Secret keys have been stored before running.
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            S3Helper helper = new S3Helper(clientDetails);

            const string bucketName = "ExampleTestBucket";
            const string key = "ExampleObject";

            // Put a simple text object into the bucket to delete.
            helper.CreateBucket(bucketName);
            helper.PutTextObject(bucketName, key, "Example text to store in the object");

            try
            {
                helper.DeleteObject(bucketName, key);
            }
            finally
            {
                helper.DeleteBucket(bucketName);
            }
        }

        [Test]
        [Ignore("Manual run test")]
        public void SetAcl_Should_Succeed()
        {
            // Get the client details from the stored client details (rather than embed secret keys in the test).
            // Ensure that your AWS/Secret keys have been stored before running.
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            S3Helper helper = new S3Helper(clientDetails);

            const string bucketName = "ExampleTestBucket";
            const string key = "ExampleObject";

            // Put a simple text object into the bucket to delete.
            helper.CreateBucket(bucketName);
            helper.PutTextObject(bucketName, key, "Example text to store in the object");

            try
            {
                helper.SetAcl(bucketName, "AuthenticatedRead", key);
            }
            finally
            {
                helper.DeleteObject(bucketName, key);
                helper.DeleteBucket(bucketName);
            }   
        }
    }
}
