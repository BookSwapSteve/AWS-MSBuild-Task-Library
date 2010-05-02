using Microsoft.Build.Framework;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to terminate (kill) EC2 Instances.
    /// </summary>
    public class TerminateEC2InstancesTask : AwsTaskBase
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
            Log.LogMessage(MessageImportance.Normal, "Terminating AWS EC2 instances {0}", string.Join(";", InstanceIds));

            AwsClientDetails clientDetails = GetClientDetails();

            TerminateInstances(clientDetails);

            return true;
        }

        private void TerminateInstances(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                helper.TerminateInstance(InstanceIds);
                Log.LogMessage(MessageImportance.Normal, "Terminiated Instances {0}", string.Join(";", InstanceIds));
            }
        }
    }
}
