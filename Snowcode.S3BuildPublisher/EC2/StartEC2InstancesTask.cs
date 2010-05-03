using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to start EC2 Instances.  These should be EBS based instances to support stopping
    /// </summary>
    public class StartEC2InstancesTask : AwsTaskBase
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
            Log.LogMessage(MessageImportance.Normal, "Starting AWS EC2 instances {0}", Join(InstanceIds));

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
                helper.StartInstances(InstanceIds);
                Log.LogMessage(MessageImportance.Normal, "Started Instances {0}", Join(InstanceIds));
            }
        }

        #endregion
    }
}
