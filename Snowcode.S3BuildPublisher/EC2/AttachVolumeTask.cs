using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to attach a volume to an EC2 instance.
    /// </summary>
    public class AttachVolumeTask : AwsTaskBase
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

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Attaching volume {0} to instance {1} Device:{2}", VolumeId, InstanceId, Device);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                AttachVolume(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void AttachVolume(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                helper.AttachVolume(Device, InstanceId, VolumeId);
                Log.LogMessage(MessageImportance.Normal, "Attached volume");
            }
        }
    }
}
