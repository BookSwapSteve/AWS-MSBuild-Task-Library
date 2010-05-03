using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

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

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                DeleteVolume(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void DeleteVolume(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                helper.DeleteVolume(VolumeId);
                Log.LogMessage(MessageImportance.Normal, "Deleted Volume {0}", VolumeId);
            }
        }
    }
}
