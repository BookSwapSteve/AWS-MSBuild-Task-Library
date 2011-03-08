using System.Collections.Generic;
using NUnit.Framework;
using Snowcode.S3BuildPublisher.Client;
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
        public void RunInstance_Should_RunInstance()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            // Modify these to match your own AWS settings.
            const string ami = "ami-bfab42d6";
            const string keyPair = "BookSwapPair1";
            const string securityGroup = "Basic";
            const string userData = "";
            const string availabilityZone = "us-east-1a";

            List<string> instances = ec2Helper.RunInstance(ami, 1, keyPair, userData, new string[] { securityGroup }, availabilityZone);

            Assert.IsNotEmpty(instances);
        }

        [Test]
        [Ignore("Manual run test")]
        public void Reboot_Should_RebootInstance()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            // TODO: this instanceId will change based on the Run Instance test.
            ec2Helper.RebootInstance(new string[] { "i-4501472e" });
        }

        [Test]
        [Ignore("Manual run test")]
        public void TerminateInstnace_Should_TerminateInstance()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            // TODO: this instanceId will change based on the Run Instance test.
            ec2Helper.TerminateInstance(new string[] { "i-6503450e" });
        }

        [Test]
        [Ignore("Manual run test")]
        public void AssociateIpAddressWithInstance_Should_Succeed()
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

        [Test]
        [Ignore("Manual run test")]
        public void CreateNewVolume_Should_CreateNewVolume()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            // Create a 2Gb volumne.
            string volumeId = ec2Helper.CreateNewVolume("us-east-1b", "2"); // "us-east-1b"

            Assert.IsNotEmpty(volumeId, "Expected VolumeId");
        }

        [Test]
        [Ignore("Manual run test")]
        public void CreateSnapShotFromVolume_Should_CreateSnapShot()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            const string volumeId = "vol-d1e15eb8";
            const string description = "Test SnapShot";

            // Create a 2Gb volumne.
            string snapShotId = ec2Helper.CreateSnapShot(volumeId, description);

            Assert.IsNotEmpty(snapShotId, "Expected SnapShot Id");
        }

        [Test]
        [Ignore("Manual run test")]
        public void DeleteVolume_Should_DeleteVolume()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            const string volumeId = "vol-d1e15eb8";

            // Delete the volume
            ec2Helper.DeleteVolume(volumeId);
        }

        [Test]
        [Ignore("Manual run test")]
        public void CreateVolumeFromSnapShot_Should_CreateNewVolume()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            const string availabilityZone = "us-east-1b";
            const string snapShotId = "snap-422c832a";

            // Create a 2Gb volumne.
            string volumeId = ec2Helper.CreateVolumeFromSnapshot(availabilityZone, snapShotId);

            Assert.IsNotEmpty(volumeId, "Expected VolumeId");
        }

        [Test]
        [Ignore("Manual run test")]
        public void DeleteSnapShot_Should_DeleteSnapShot()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            const string snapShotId = "snap-422c832a";

            ec2Helper.DeleteSnapShot(snapShotId);
        }

        [Test]
        [Ignore("Manual run test")]
        public void AttachVolumeToInstance_Should_AttachVolume()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            const string device = "xvdf";
            const string volumeId = "vol-6bd56a02";
            const string instanceId = "i-6de0a406";

            ec2Helper.AttachVolume(device, instanceId, volumeId);
        }

        [Test]
        [Ignore("Manual run test")]
        public void DetachVolume_Should_DetachVolume()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            const string device = "xvdf";
            const string volumeId = "vol-6bd56a02";
            const string instanceId = "i-6de0a406";

            ec2Helper.DetachVolume(device, instanceId, volumeId, true);
        }

        [Test]
        [Ignore("Manual run test")]
        public void DescribeInstance_Should_ReturnInstanceDetails()
        {
            // NB: This test requires an EC2 Instance to succeed.
            const string instanceId = "i-6de0a406";

             var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            EC2Helper ec2Helper = new EC2Helper(clientDetails);

            Amazon.EC2.Model.RunningInstance instance = ec2Helper.DescribeInstance(instanceId);

            Assert.IsNotNull(instance, "No instance");
        }
    }
}
