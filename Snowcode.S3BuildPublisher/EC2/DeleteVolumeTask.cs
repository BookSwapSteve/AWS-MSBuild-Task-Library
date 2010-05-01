using Microsoft.Build.Framework;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to delete a EC2 volume.
    /// </summary>
    public class DeleteVolumeTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the VolumeId of the Volume to delete.
        /// </summary>
        [Required]
        public string VolumeId { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Deleting Volume {0}", VolumeId);

            AwsClientDetails clientDetails = GetClientDetails();

            DeleteVolume(clientDetails);

            return true;
        }

        private void DeleteVolume(AwsClientDetails clientDetails)
        {
            var helper = new EC2Helper(clientDetails);
            helper.DeleteVolume(VolumeId);
            Log.LogMessage(MessageImportance.Normal, "Deleted Volume {0}", VolumeId);
        }
    }
}
