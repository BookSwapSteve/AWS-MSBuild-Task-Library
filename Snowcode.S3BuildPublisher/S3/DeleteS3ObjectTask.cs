using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.S3
{
    /// <summary>
    /// MSBuild task to delete an object from S3
    /// </summary>
    public class DeleteS3ObjectTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets and sets the name of the bucket.
        /// </summary>
        [Required]
        public string BucketName { get; set; }

        /// <summary>
        /// Gets and sets the key to the S3 object.
        /// </summary>
        [Required]
        public string Key { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Deleting AWS S3 Object {0} from bucket {1} ", Key, BucketName);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                CreateBucket(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void CreateBucket(AwsClientDetails clientDetails)
        {
            using (var helper = new S3Helper(clientDetails))
            {
                helper.DeleteObject(BucketName, Key);
                Log.LogMessage(MessageImportance.High, "Deleted AWS S3 Object {0} from bucket {1} ", Key, BucketName);
            }
        }
    }
}
