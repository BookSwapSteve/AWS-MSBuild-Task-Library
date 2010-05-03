using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to wait for the EC2 instances to become in the desired state
    /// </summary>
    public class WaitForEC2InstancesTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Instance Id's
        /// </summary>
        [Required]
        public string[] InstanceIds { get; set; }

        /// <summary>
        /// The desired state of the instances 
        /// </summary>
        /// <value>one of stopped, running, shutting-down, stopping, pending, terminated</value>
        public string DesiredState { get; set; }

        /// <summary>
        /// The maximum time to wait for the state change in seconds.
        /// </summary>
        [Required]
        public int TimeOutSeconds { get; set; }

        /// <summary>
        /// How often to poll
        /// </summary>
        [Required]
        public int PollIntervalSeconds { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Waiting for instances {0} to be in the state {1}", Join(InstanceIds), DesiredState);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                WaitForInstances(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        #region Private methods

        private void WaitForInstances(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                helper.WaitForInstances(InstanceIds, DesiredState.ToLower(), TimeOutSeconds, PollIntervalSeconds);
                Log.LogMessage(MessageImportance.Normal, "All Instances {0} in the state {1}", Join(InstanceIds), DesiredState);
            }
        }

        #endregion
    }
}
