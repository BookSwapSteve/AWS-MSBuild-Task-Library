using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.S3
{
    public class PutS3FileObjectTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets and sets the name of the bucket.
        /// </summary>
        [Required]
        public string BucketName { get; set; }

        /// <summary>
        /// Gets and sets the key (name) of the object.
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Gets and sets the file to put.
        /// </summary>
        [Required]
        public string FileName { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Putting file {0} into AWS S3 object {0} in bucket {1}", FileName, Key, BucketName);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                PutFileObject(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void PutFileObject(AwsClientDetails clientDetails)
        {
            using (var helper = new S3Helper(clientDetails))
            {
                helper.PutFileObject(BucketName, Key, FileName);
                Log.LogMessage(MessageImportance.Normal, "Put file {0} into object {0} in bucket {1}", FileName, Key, BucketName);
            }
        }
    }
}
