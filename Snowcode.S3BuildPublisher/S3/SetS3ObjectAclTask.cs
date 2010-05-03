using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.S3
{
    public class SetS3ObjectAclTask : AwsTaskBase
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
        /// Gets and sets the ACL value
        /// </summary>
        /// <value>One of AuthenticatedRead, BucketOwnerFullControl, BucketOwnerRead, NoACL, Private, PublicRead, PublicReadWrite</value>
        [Required]
        public string CannedAcl { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Setting ACL {0} on AWS S3 object {0} in bucket {1}", CannedAcl, Key, BucketName);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                SetAcl(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void SetAcl(AwsClientDetails clientDetails)
        {
            using (var helper = new S3Helper(clientDetails))
            {
                helper.SetAcl(BucketName, CannedAcl, Key);
                Log.LogMessage(MessageImportance.High, "Setting Acl {0} on object {0} in bucket {1}", CannedAcl, Key, BucketName);
            }
        }
    }
}
