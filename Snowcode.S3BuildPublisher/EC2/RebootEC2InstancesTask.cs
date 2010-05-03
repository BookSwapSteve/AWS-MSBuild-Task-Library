using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to reboot ec2 instances.
    /// </summary>
    public class RebootEC2InstancesTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Instance Id's
        /// </summary>
        [Required]
        public string[] InstanceIds { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Rebooting AWS EC2 instances {0}", Join(InstanceIds));

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                RebootInstances(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void RebootInstances(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                helper.RebootInstance(InstanceIds);
                Log.LogMessage(MessageImportance.Normal, "Stopped Instances {0}", Join(InstanceIds));
            }
        }
    }
}
