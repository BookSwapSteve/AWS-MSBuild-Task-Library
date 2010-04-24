using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.S3.Util;
using Amazon.S3.Model;
using Amazon.S3;

namespace Snowcode.S3BuildPublisher
{
    public class S3Helper
    {
       

        public void DoStuff(string bucketName, string awsAccessKeyId ,string awsSecretAccesskey)
        {
            var s3 = new AmazonS3Client(awsAccessKeyId, awsSecretAccesskey);

            if (!AmazonS3Util.DoesS3BucketExist(bucketName, s3))
            {
                var request = new PutBucketRequest
                {
                    BucketName = bucketName
                };
                s3.PutBucket(request);
            }

            var or = new PutObjectRequest();
            or.WithContentBody("Hello from Snowcode cloud hackday in Cambridge, woot,woot!")
                .WithBucketName(bucketName)
                .WithKey("mykey3");

            // normally wrapped with a responseWithMetaData
            s3.PutObject(or);


        }


    } // class
} // namespace
