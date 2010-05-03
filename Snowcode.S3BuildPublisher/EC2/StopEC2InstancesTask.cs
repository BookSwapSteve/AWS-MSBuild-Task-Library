using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to stop EC2 Instances.  These should be EBS based instances to support stopping
    /// </summary>
    public class StopEC2InstancesTask : AwsTaskBase
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
            Log.LogMessage(MessageImportance.Normal, "Stopping AWS EC2 instances {0}", Join(InstanceIds));

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                StopInstances(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void StopInstances(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                helper.StopInstances(InstanceIds);
                Log.LogMessage(MessageImportance.Normal, "Stopped Instances {0}", Join(InstanceIds));
            }
        }
    }
}
