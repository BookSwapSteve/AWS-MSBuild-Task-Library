using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to detach a volume from an EC2 Instance
    /// </summary>
    public class DetachVolumeTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets Instance Id to attach the volume to
        /// </summary>
        [Required]
        public string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the device name to use
        /// </summary>
        [Required]
        public string Device { get; set; }

        /// <summary>
        /// Gets or sets the VolumeId of the volume to attach
        /// </summary>
        [Required]
        public string VolumeId { get; set; }

        /// <summary>
        /// Gets or sets if the action of detaching should be forced.
        /// </summary>
        public bool Force { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Detaching volume {0} from instance {1}", VolumeId, InstanceId);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                DetachVolume(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void DetachVolume(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                helper.DetachVolume(Device, InstanceId, VolumeId, Force);
                Log.LogMessage(MessageImportance.Normal, "Detached volume");
            }
        }
    }
}
