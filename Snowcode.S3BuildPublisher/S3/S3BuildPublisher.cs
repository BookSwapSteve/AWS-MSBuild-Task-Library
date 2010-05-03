using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.S3
{
    /// <summary>
    /// MSBuild task to publish a set of files to a S3 bucket.
    /// </summary>
    /// <remarks>If made public the files will be available at https://s3.amazonaws.com/bucket_name/folder/file_name</remarks>
    public class S3BuildPublisher : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the source files to be stored.
        /// </summary>
        /// <remarks>Subfolders are not supported.</remarks>
        [Required]
        public string[] SourceFiles { get; set; }

        /// <summary>
        /// Gets or sets the destination folder.
        /// </summary>
        public string DestinationFolder { get; set; }

        /// <summary>
        /// Gets or sets the AWS S3 bucket to store the files in
        /// </summary>
        [Required]
        public string DestinationBucket { get; set; }

        /// <summary>
        /// Gets or sets if the files should be publically readable
        /// </summary>
        public bool PublicRead { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Publishing Sourcefiles={0} to {1}", Join(SourceFiles), DestinationBucket);

            ShowAclWarnings();

            try
            {
                ValidateBucketName();

                ValidateFolder();

                AwsClientDetails clientDetails = GetClientDetails();

                PublishFiles(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        #region Private methods

        private void ShowAclWarnings()
        {
            if (PublicRead)
            {
                Log.LogMessage(MessageImportance.High, "File(s) will be public accessible");
            }
        }

        private void ValidateBucketName()
        {
            if (DestinationBucket.Contains("."))
            {
                throw new Exception("Bucket must not contain a .");
            }
        }

        private void ValidateFolder()
        {
            // ignore if null.
            if (DestinationFolder == null)
            {
                return;
            }

            if (DestinationFolder.StartsWith(@"\") || DestinationFolder.StartsWith("/"))
            {
                throw new Exception(@"Folder should not start with a \ or /");
            }

            if (DestinationFolder.EndsWith(@"\"))
            {
                throw new Exception(@"Folder should not end with a \");
            }
        }

        private void PublishFiles(AwsClientDetails clientDetails)
        {
            using (var helper = new S3Helper(clientDetails))
            {
                helper.Publish(SourceFiles, DestinationBucket, DestinationFolder, PublicRead);
                Log.LogMessage(MessageImportance.Normal, "Published {0} files to S3", SourceFiles.Length);
            }
        }

        #endregion
    }
}
