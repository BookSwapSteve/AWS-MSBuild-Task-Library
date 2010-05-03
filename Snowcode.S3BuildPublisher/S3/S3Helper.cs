using System;
using System.IO;
using Amazon.S3.Util;
using Amazon.S3.Model;
using Amazon.S3;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.S3
{
    /// <summary>
    /// Helper class to connect to Amazon aws S3 and store files.
    /// </summary>
    public class S3Helper : IDisposable
    {
        private bool _disposed;

        #region Constructors

        public S3Helper(string awsAccessKeyId, string awsSecretAccessKey)
        {
            Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey);
        }

        public S3Helper(AwsClientDetails clientDetails)
        {
            Client = new AmazonS3Client(clientDetails.AwsAccessKeyId, clientDetails.AwsSecretAccessKey);
        }

        ~S3Helper()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        protected AmazonS3Client Client
        {
            get;
            set;
        }

        #endregion

        #region Public methods

        public void Publish(string[] files, string bucketName, string folder, bool publicRead)
        {
            CreateBucketIfNeeded(bucketName);

            string destinationFolder = GetDestinationFolder(folder);

            StoreFiles(files, bucketName, destinationFolder, publicRead);
        }

        /// <summary>
        /// Creates a S3 Bucket.
        /// </summary>
        /// <param name="bucketName"></param>
        public void CreateBucket(string bucketName)
        {
            var request = new PutBucketRequest { BucketName = bucketName };
            Client.PutBucket(request);
        }

        public void DeleteBucket(string bucketName)
        {
            var request = new DeleteBucketRequest { BucketName = bucketName };
            Client.DeleteBucket(request);
        }

        public void DeleteObject(string bucketName, string key)
        {
            var request = new DeleteObjectRequest { BucketName = bucketName, Key = key };
            Client.DeleteObject(request);
        }

        /// <summary>
        /// Sets the ACL
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="cannedACL">ACL to use, AuthenticaticatedRead, BucketOwnerFullControl, BucketOwnerRead, NoACL, Private, PublicRead, PublicReadWrite</param>
        public void SetAcl(string bucketName, string cannedACL, string key)
        {
            var request = new SetACLRequest
                              {
                                  BucketName = bucketName,
                                  CannedACL = (S3CannedACL)Enum.Parse(typeof(S3CannedACL), cannedACL),
                                  Key = key
                              };

            Client.SetACL(request);
        }

        #endregion

        #region Private Methods

        private void CreateBucketIfNeeded(string bucketName)
        {
            if (!AmazonS3Util.DoesS3BucketExist(bucketName, Client))
            {
                CreateBucket(bucketName);
            }
        }

        private void StoreFiles(string[] files, string bucketName, string destinationFolder, bool publicRead)
        {
            foreach (string file in files)
            {
                // Use the filename as the key (aws filename).
                string key = Path.GetFileName(file);
                StoreFile(file, destinationFolder + key, bucketName, publicRead);
            }
        }

        private string GetDestinationFolder(string folder)
        {
            string destinationFolder = folder ?? string.Empty;

            // Append a folder seperator if a folder has been specified without one.
            if (!string.IsNullOrEmpty(destinationFolder) && !destinationFolder.EndsWith("/"))
            {
                destinationFolder += "/";
            }

            return destinationFolder;
        }

        private void StoreFile(string file, string key, string bucketName, bool publicRead)
        {
            S3CannedACL acl = publicRead ? S3CannedACL.PublicRead : S3CannedACL.Private;

            var request = new PutObjectRequest();
            request
                .WithCannedACL(acl)
                .WithFilePath(file)
                .WithBucketName(bucketName)
                .WithKey(key);

            Client.PutObject(request);
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        virtual protected void Dispose(bool disposing)
        {
            if (_disposed)
            {
                if (!disposing)
                {
                    try
                    {
                        if (Client != null)
                        {
                            Client.Dispose();
                        }
                    }
                    finally
                    {
                        _disposed = true;
                    }
                }
            }
        }

        #endregion
    }
}
