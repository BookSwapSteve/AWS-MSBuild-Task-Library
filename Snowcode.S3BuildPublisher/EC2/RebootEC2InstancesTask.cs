using Microsoft.Build.Framework;

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
            Log.LogMessage(MessageImportance.Normal, "Rebooting AWS EC2 instances {0}", string.Join(";", InstanceIds));

            AwsClientDetails clientDetails = GetClientDetails();

            RebootInstances(clientDetails);

            return true;
        }

        private void RebootInstances(AwsClientDetails clientDetails)
        {
            var helper = new EC2Helper(clientDetails);
            helper.RebootInstance(InstanceIds);
            Log.LogMessage(MessageImportance.Normal, "Stopped Instances {0}", string.Join(";", InstanceIds));
        }
    }
}
