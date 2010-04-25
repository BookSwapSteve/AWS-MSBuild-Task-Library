using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Snowcode.S3BuildPublisher
{
    /// <summary>
    /// MSBuild task to publish a set of files to a S3 bucket.
    /// </summary>
    /// <remarks>If made public the files will be available at https://s3.amazonaws.com/bucket_name/file_name</remarks>
    public class S3BuildPublisher : Task
    {
        #region Properties

        /// <summary>
        /// Gets or sets the source files to be stored.
        /// </summary>
        /// <remarks>Subfolders are not supported.</remarks>
        [Required]
        public string[] SourceFiles { get; set; }

        /// <summary>
        /// Gets or sets the AWS S3 bucket to store the files in
        /// </summary>
        [Required]
        public string DestinationBucket { get; set; }

        /// <summary>
        /// Gets or sets the container to be used when decrypting the stored credentials.
        /// </summary>
        [Required]
        public string EncryptionContainerName { get; set; }

        /// <summary>
        /// Gets or sets if the files should be publically readable
        /// </summary>
        public bool PublicRead { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Publishing Sourcefiles={0} to {1}", string.Join(";", SourceFiles), DestinationBucket);

            // TODO: Validate that the bucket doesn't contain .
            
            ShowAclWarnings();

            AwsClientDetails clientDetails = GetClientDetails();

            PublishFiles(clientDetails);

            return true;
        }

        #region Private methods

        private void ShowAclWarnings()
        {
            if (PublicRead)
            {
                Log.LogMessage(MessageImportance.High, "File(s) will be public accessible");
            }
        }

        private AwsClientDetails GetClientDetails()
        {
            var clientDetailsStore = new ClientDetailsStore();
            AwsClientDetails clientDetails = clientDetailsStore.Load(EncryptionContainerName);
            Log.LogMessage(MessageImportance.Normal, "Connecting to AWS using AwsAccessKeyId: {0}", clientDetails.AwsAccessKeyId);
            return clientDetails;
        }

        private void PublishFiles(AwsClientDetails clientDetails)
        {
            var helper = new S3Helper(clientDetails);
            helper.Publish(SourceFiles, DestinationBucket, PublicRead);
            Log.LogMessage(MessageImportance.Normal, "Published {0} files to S3", SourceFiles.Length);
        }

        #endregion
    }
}
