using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuikd task to create a new AWS EC2 Volume
    /// </summary>
    public class CreateVolumeTask : AwsTaskBase
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
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets the VolumeId of the created volume.
        /// </summary>
        [Output]
        public string VolumeId { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Creating new volume in {0} of size {1}MiB", AvailabilityZone, Size);

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
                VolumeId = helper.CreateNewVolume(AvailabilityZone, Size);
                Log.LogMessage(MessageImportance.Normal, "Created volume of size {0}MiB with VolumeId {1}", Size, VolumeId);
            }
        }
    }
}
