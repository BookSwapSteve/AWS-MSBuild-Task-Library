﻿using Microsoft.Build.Framework;

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

            AwsClientDetails clientDetails = GetClientDetails();

            DeleteSnapShot(clientDetails);

            return true;
        }

        private void DeleteSnapShot(AwsClientDetails clientDetails)
        {
            var helper = new EC2Helper(clientDetails);
            helper.DeleteSnapShot(SnapShotId);
            Log.LogMessage(MessageImportance.Normal, "Deleted SnapShot {0}", SnapShotId);
        }
    }
}
