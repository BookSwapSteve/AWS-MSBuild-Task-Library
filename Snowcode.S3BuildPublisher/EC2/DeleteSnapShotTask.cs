using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to delete a SnapShot.
    /// </summary>
    public class DeleteSnapShotTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the SnapShotId of the SnapShot to delete.
        /// </summary>
        [Required]
        public string SnapShotId { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Deleting SnapShot {0}", SnapShotId);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                DeleteSnapShot(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void DeleteSnapShot(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                helper.DeleteSnapShot(SnapShotId);
                Log.LogMessage(MessageImportance.Normal, "Deleted SnapShot {0}", SnapShotId);
            }
        }
    }
}
