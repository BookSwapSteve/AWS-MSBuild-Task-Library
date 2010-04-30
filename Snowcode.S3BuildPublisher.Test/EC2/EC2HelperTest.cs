using NUnit.Framework;
using Snowcode.S3BuildPublisher.EC2;

namespace Snowcode.S3BuildPublisher.Test.EC2
{
    /// <summary>
    /// Test ficture used to test the EC2 Helper class.  These tests run against AWS and as such will incure charges when run.
    /// </summary>
    [TestFixture]
    [Category("IntegrationTest")]
    public class EC2HelperTest
    {
        private const string Container = "MySecretContainer";
        private const string DefaultInstanceId = "i-6de0a406";
        private const string DefaultIpAddress = "184.73.176.41";

        [Test]
        [Ignore("Manual run test")]
        public void StartInstances_Should_Start_Instance()
        {
            // Get the client details from the stored client details (rather than embed secret keys in the test).
            // Ensure that your AWS/Secret keys have been stored before running.
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            ec2Helper.StartInstances(new string[] { DefaultInstanceId });

        }

        [Test]
        [Ignore("Manual run test")]
        public void StopInstances_Should_Stop_Instance()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            ec2Helper.StopInstances(new string[] { DefaultInstanceId });
        }

        [Test]
        [Ignore("Manual run test")]
        public void AssociateIpAddressWithInstance_Should_Succeed ()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            ec2Helper.AssociateIpAddress(DefaultInstanceId, DefaultIpAddress);
        }

        [Test]
        [Ignore("Manual run test")]
        public void DisassociateIpAddressWithInstance_Should_Succeed()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            ec2Helper.DisassociateIpAddress(DefaultIpAddress);
        }
    }
}
