using System;
using Amazon.EC2.Model;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to get details of an EC2 Instance.
    /// </summary>
    public class DescribeEC2InstanceTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Instance Id
        /// </summary>
        [Required]
        public string InstanceId { get; set; }

        #endregion

        #region Output Properties

        [Output]
        public string Architecture { get; set; }

        [Output]
        public string ImageId { get; set; }

        [Output]
        public string InstanceLifecycle { get; set; }

        [Output]
        public decimal InstanceStateCode { get; set; }

        [Output]
        public string InstanceStateName { get; set; }

        [Output]
        public string InstanceType { get; set; }

        [Output]
        public string IpAddress { get; set; }

        [Output]
        public string KernelId { get; set; }

        [Output]
        public string KeyName { get; set; }

        [Output]
        public string LaunchTime { get; set; }

        [Output]
        public string MonitoringState { get; set; }

        [Output]
        public string AvailabilityZone { get; set; }

        [Output]
        public string Platform { get; set; }

        [Output]
        public string PrivateDnsName { get; set; }

        [Output]
        public string PublicDnsName { get; set; }

        [Output]
        public string RamdiskId { get; set; }

        [Output]
        public string RootDeviceName { get; set; }

        [Output]
        public string RootDeviceType { get; set; }

        [Output]
        public string SpotInstanceRequestId { get; set; }

        [Output]
        public string StateReason { get; set; }

        [Output]
        public string StateTransitionReason { get; set; }

        [Output]
        public string SubnetId { get; set; }

        [Output]
        public string VpcId { get; set; }

        [Output]
        public string AmiLaunchIndex { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Getting details of EC2 instance {0}", InstanceId);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                StartInstances(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        #region Private methods

        private void StartInstances(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                RunningInstance instance = helper.DescribeInstance(InstanceId);
                Log.LogMessage(MessageImportance.Normal, "Got Instance {0} details", InstanceId);

                SetOutputProperties(instance);
            }
        }

        private void SetOutputProperties(RunningInstance instance)
        {
            AmiLaunchIndex = instance.AmiLaunchIndex;
            Architecture = instance.Architecture;
            ImageId = instance.ImageId;
            InstanceLifecycle = instance.InstanceLifecycle;
            if (instance.InstanceState != null)
            {
                InstanceStateCode = instance.InstanceState.Code;
                InstanceStateName = instance.InstanceState.Name;
            }
            InstanceType = instance.InstanceType;
            IpAddress = instance.IpAddress;
            KernelId = instance.KernelId;
            KeyName = instance.KeyName;
            LaunchTime = instance.LaunchTime;
            if (instance.Monitoring!=null)
            {
                MonitoringState = instance.Monitoring.MonitoringState;
            }
            if (instance.Placement!=null)
            {
                AvailabilityZone = instance.Placement.AvailabilityZone;
            }
            Platform = instance.Platform;
            PrivateDnsName = instance.PrivateDnsName;
            PublicDnsName = instance.PublicDnsName;
            RamdiskId = instance.RamdiskId;
            RootDeviceName = instance.RootDeviceName;
            RootDeviceType = instance.RootDeviceType;
            SpotInstanceRequestId = instance.SpotInstanceRequestId;
            if (instance.StateReason != null)
            {
                StateReason = instance.StateReason.Message;
            }
            StateTransitionReason = instance.StateTransitionReason;
            SubnetId = instance.SubnetId;
            VpcId = instance.VpcId;
        }

        #endregion
    }
}
