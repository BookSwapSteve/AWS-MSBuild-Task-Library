using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SNS
{
    /// <summary>
    /// MSBuild task to set the permissions on a Simple Notificiation Service Topic
    /// </summary>
    public class AddSNSPermissionsTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Topic Arn to set permissions on
        /// </summary>
        [Required]
        public string TopicArn { get; set; }

        /// <summary>
        /// Gets or sets the action names allowed
        /// </summary>
        [Required]
        public string[] ActionNames { get; set; }

        /// <summary>
        /// Gets or sets the AwsAccountIds with permission to this notification.  This is the 12 digiti AWS account Id, without the hyphens.
        /// </summary>
        [Required]
        public string[] AwsAccountIds { get; set; }

        /// <summary>
        /// A unique identifier for the new policy statement.
        /// </summary>
        [Required]
        public string Label { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Adding SNS permissions to Topic {0}", TopicArn);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                AddPermissions(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void AddPermissions(AwsClientDetails clientDetails)
        {
            using (var helper = new SNSHelper(clientDetails))
            {
                helper.AddPermission(ActionNames, AwsAccountIds, Label, TopicArn);
                Log.LogMessage(MessageImportance.Normal, "Set permissiosn for AWS Accounts {0} to Topic Arn {1}", Join(AwsAccountIds), TopicArn);
            }
        }
    }
}
