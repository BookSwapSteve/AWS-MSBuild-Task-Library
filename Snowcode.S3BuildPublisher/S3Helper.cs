using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.S3.Util;
using Amazon.S3.Model;
using Amazon.S3;

namespace Snowcode.S3BuildPublisher
{
    /// <summary>
    /// Helper class to connect to Amazon aws S3 and store files.
    /// </summary>
    public class S3Helper
    {
        #region Constructors

        public S3Helper(string awsAccessKeyId, string awsSecretAccessKey)
        {
            Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey);
        }

        public S3Helper(AwsClientDetails clientDetails)
        {
            Client = new AmazonS3Client(clientDetails.AwsAccessKeyId, clientDetails.AwsSecretAccessKey);
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

        public void Publish(string[] files, string bucketName, bool publicRead)
        {
            CreateBucketIfNeeded(bucketName);

            StoreFiles(files, bucketName, publicRead);
        }

        #endregion

        #region Private Methods

        private void CreateBucketIfNeeded(string bucketName)
        {
            if (!AmazonS3Util.DoesS3BucketExist(bucketName, Client))
            {
                var request = new PutBucketRequest { BucketName = bucketName };
                Client.PutBucket(request);
            }
        }

        private void StoreFiles(string[] files, string bucketName, bool publicRead)
        {
            foreach (string file in files)
            {
                // Use just the filename as the key (aws filename).
                string key = System.IO.Path.GetFileName(file);
                StoreFile(file, key, bucketName, publicRead);
            }
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

            // normally wrapped with a responseWithMetaData
            Client.PutObject(request);
        }

        #endregion
    }
}
