using System.Collections.Generic;
using System.Diagnostics;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// Helper class to control AWS EC2 instances.
    /// </summary>
    public class EC2Helper
    {
        #region Constructors

        public EC2Helper(string awsAccessKeyId, string awsSecretAccessKey)
        {
            Client = new AmazonEC2Client(awsAccessKeyId, awsSecretAccessKey);
        }

        public EC2Helper(AwsClientDetails clientDetails)
        {
            Client = new AmazonEC2Client(clientDetails.AwsAccessKeyId, clientDetails.AwsSecretAccessKey);
        }

        #endregion

        protected AmazonEC2Client Client
        {
            get;
            set;
        }

        /// <summary>
        /// Start's instances - these should be EBS block storage instances.
        /// </summary>
        /// <param name="instanceIds">The instance Id of an EC2 instance</param>
        /// <remarks>This uses EBS storage EC2 instances which can be stopped and started.  The instance should be stopped.</remarks>
        public void StartInstances(IEnumerable<string> instanceIds)
        {
            var request = new StartInstancesRequest { InstanceId = new List<string>(instanceIds) };
            StartInstancesResponse resonse = Client.StartInstances(request);

            if (resonse.IsSetStartInstancesResult())
            {
                foreach (InstanceStateChange instanceStateChange in resonse.StartInstancesResult.StartingInstances)
                {
                    Trace.WriteLine(string.Format("Starting instance {0}", instanceStateChange.InstanceId));
                }
            }
        }

        /// <summary>
        /// Stop Amazon EC2 instances.
        /// </summary>
        /// <param name="instances"></param>
        public void StopInstances(string[] instances)
        {
            var request = new StopInstancesRequest { InstanceId = new List<string>(instances) };
            StopInstancesResponse response = Client.StopInstances(request);

            if (response.IsSetStopInstancesResult())
            {
                foreach (InstanceStateChange instanceStateChange in response.StopInstancesResult.StoppingInstances)
                {
                    Trace.WriteLine(string.Format("Stopping instance {0}", instanceStateChange.InstanceId));
                }
            }
        }

        /// <summary>
        /// Associate a public IP Address with an EC2 instance
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="publicIpAddress"></param>
        public void AssociateIpAddress(string instanceId, string publicIpAddress)
        {
            var request = new AssociateAddressRequest { InstanceId = instanceId, PublicIp = publicIpAddress };
            Client.AssociateAddress(request);
        }

        /// <summary>
        /// Disassociate a public IP Address from it's current EC2 instance
        /// </summary>
        /// <param name="publicIpAddress"></param>
        public void DisassociateIpAddress(string publicIpAddress)
        {
            var request = new DisassociateAddressRequest { PublicIp = publicIpAddress };
            Client.DisassociateAddress(request);
        }


    }
}
