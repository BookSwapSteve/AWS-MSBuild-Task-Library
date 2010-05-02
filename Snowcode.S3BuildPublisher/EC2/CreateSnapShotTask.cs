using Microsoft.Build.Framework;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to create a SnapShot of a volume.
    /// </summary>
    public class CreateSnapShotTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the VolumeId of the volume to attach
        /// </summary>
        [Required]
        public string VolumeId { get; set; }

        /// <summary>
        /// Gets or sets the description to associate with the snapshot
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the created SnapShotId
        /// </summary>
        [Output]
        public string SnapShotId { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Snapshotting volume {0}", VolumeId);

            AwsClientDetails clientDetails = GetClientDetails();

            CreateSnapShot(clientDetails);

            return true;
        }

        private void CreateSnapShot(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                SnapShotId = helper.CreateSnapShot(VolumeId, Description);
                Log.LogMessage(MessageImportance.Normal, "Snapshot {0} created of volume {1} ", SnapShotId, VolumeId);
            }
        }
    }
}
