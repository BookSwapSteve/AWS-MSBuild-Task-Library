using Microsoft.Win32;
using NUnit.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.Test
{
    [TestFixture]
    public class ClientDetailsStoreTests
    {
        private const string DefaultTestRegistrySubKey = "Software\\SnowCode\\S3BuildPublisher\\Tests";

        [SetUp]
        public void Setup()
        {
            if (Registry.CurrentUser.OpenSubKey(DefaultTestRegistrySubKey) != null)
            {
                Registry.CurrentUser.DeleteSubKey(DefaultTestRegistrySubKey);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (Registry.CurrentUser.OpenSubKey(DefaultTestRegistrySubKey) != null)
            {
                Registry.CurrentUser.DeleteSubKey(DefaultTestRegistrySubKey);
            }
        }

        [Test]
        public void SaveAndLoadClientDetails_AreCorrectlyStored()
        {
            const string containerName = "S3BuildPublisher.TestContainer.ClientDetailsStore";
            var clientDetails = new AwsClientDetails
                                    {
                                        AwsAccessKeyId = "AwsAccessKeyId",
                                        AwsSecretAccessKey = "AwsSecretAccessKey"
                                    };

            var store = new ClientDetailsStore(DefaultTestRegistrySubKey);

            store.Save(containerName, clientDetails);

            AwsClientDetails actual = store.Load(containerName);

            Assert.AreEqual(clientDetails.AwsAccessKeyId, actual.AwsAccessKeyId, "AwsAccessKeyId");
            Assert.AreEqual(clientDetails.AwsSecretAccessKey, actual.AwsSecretAccessKey, "AwsSecretAccessKey");
        }
    }
}
