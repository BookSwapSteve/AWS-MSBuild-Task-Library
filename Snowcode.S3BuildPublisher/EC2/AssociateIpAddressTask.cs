using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to associate an AWS public IP Address with a EC2 instance.
    /// </summary>
    public class AssociateIpAddressTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Instance Id
        /// </summary>
        [Required]
        public string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the IpAddress to assign
        /// </summary>
        [Required]
        public string IpAddress { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Associating IP Address {0} with AWS EC2 instance {1}", IpAddress, InstanceId);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                AssociateIpAddress(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void AssociateIpAddress(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                helper.AssociateIpAddress(InstanceId, IpAddress);
                Log.LogMessage(MessageImportance.Normal, "Associated IP Address {0} with InstanceId {1}", IpAddress, InstanceId);
            }
        }
    }
}
