using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.S3
{
    /// <summary>
    /// MSBuild task to write some text into a AWS S3 object.
    /// </summary>
    public class PutS3TextObjectTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets and sets the name of the bucket.
        /// </summary>
        [Required]
        public string BucketName { get; set; }

        /// <summary>
        /// Gets and sets the key (name) of the object
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Gets and sets the text of the object.
        /// </summary>
        [Required]
        public string Text { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Creating AWS S3 object {0} in bucket {1}",Key, BucketName);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                PutTextObject(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void PutTextObject(AwsClientDetails clientDetails)
        {
            using (var helper = new S3Helper(clientDetails))
            {
                helper.PutTextObject(BucketName, Key, Text);
                Log.LogMessage(MessageImportance.Normal, "Put object {0} into bucket {1}",Key, BucketName);
            }
        }
    }
}
