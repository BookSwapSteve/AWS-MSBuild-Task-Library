using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to create a volume from a SnapShot
    /// </summary>
    public class CreateVolumeFromSnapshotTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the AWS AvailabilityZone
        /// </summary>
        [Required]
        public string AvailabilityZone { get; set; }

        /// <summary>
        /// Gets or sets the volume size in MiB
        /// </summary>
        [Required]
        public string SnapShotId { get; set; }

        /// <summary>
        /// Gets or sets the VolumeId of the created volume.
        /// </summary>
        [Output]
        public string VolumeId { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Creating volume from SnapShot {0} in {1}", SnapShotId, AvailabilityZone);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                CreateVolume(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void CreateVolume(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                VolumeId = helper.CreateVolumeFromSnapshot(AvailabilityZone, SnapShotId);
                Log.LogMessage(MessageImportance.Normal, "Created volume {0} from SnapShot {1}", VolumeId, SnapShotId);
            }
        }
    }
}
