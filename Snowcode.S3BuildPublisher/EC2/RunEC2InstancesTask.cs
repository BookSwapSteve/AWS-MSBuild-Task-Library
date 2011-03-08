using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to run a EC2 instance from the ami image id.
    /// </summary>
    public class RunEC2InstancesTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the AMI image id launch
        /// </summary>
        [Required]
        public string ImageId { get; set; }

        /// <summary>
        /// Gets or sets the number of instances to launch
        /// </summary>
        [Required]
        public int NumberOfInstances { get; set; }

        /// <summary>
        /// Name of the key pair to launch instances with
        /// </summary>
        [Required]
        public string KeyName { get; set; }

        /// <summary>
        /// User data available to the launched instances.  Base64 encoded.
        /// </summary>
        public string UserData { get; set; }

        /// <summary>
        /// Security Groups.
        /// </summary>
        [Required]
        public string[] SecurityGroups { get; set; }

        public string AvailabilityZone
        {
            get; set;
        }

        /// <summary>
        /// MSBuild output parameter with the result of the InstanceId's that have been run.
        /// </summary>
        [Output]
        public string[] InstanceIds { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Running {0} AWS EC2 instances from ImageId {1}", NumberOfInstances, ImageId);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                RunInstances(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        #region Private methods

        private void RunInstances(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                // Run up the instances and return the InstanceId's
                InstanceIds = helper.RunInstance(ImageId, NumberOfInstances, KeyName, UserData, SecurityGroups, AvailabilityZone).ToArray();
                Log.LogMessage(MessageImportance.Normal, "Run {0} Instances", NumberOfInstances);
            }
        }

        #endregion
    }
}
