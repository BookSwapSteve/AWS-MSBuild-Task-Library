using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.S3
{
    /// <summary>
    /// MSBuild task to create an AWS S3 Bucket.
    /// </summary>
    public class CreateS3BucketTask : AwsTaskBase
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
            Log.LogMessage(MessageImportance.Normal, "Creating AWS S3 Bucket {0} ", BucketName);

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
                helper.CreateBucket(BucketName);
                Log.LogMessage(MessageImportance.Normal, "Created S3 Bucket {0}", BucketName);
            }
        }
    }
}
