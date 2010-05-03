using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.S3
{
    /// <summary>
    /// MSBuild task to delete a S3 Bucket.
    /// </summary>
    public class DeleteS3BucketTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets and sets the name of the bucket.
        /// </summary>
        [Required]
        public string BucketName { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Deleting AWS S3 Bucket {0} ", BucketName);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                DeleteBucket(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void DeleteBucket(AwsClientDetails clientDetails)
        {
            using (var helper = new S3Helper(clientDetails))
            {
                helper.DeleteBucket(BucketName);
                Log.LogMessage(MessageImportance.High, "Deleted S3 Bucket {0}", BucketName);
            }
        }
    }
}