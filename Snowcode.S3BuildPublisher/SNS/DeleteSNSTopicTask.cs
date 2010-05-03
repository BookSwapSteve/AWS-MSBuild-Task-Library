using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SNS
{
    /// <summary>
    /// MSBuild task to delete a Simple Notification Service Topic.
    /// </summary>
    public class DeleteSNSTopicTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the TopicArn
        /// </summary>
        [Required]
        public string TopicArn { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Deleting SNS Topic {0}", TopicArn);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                DeleteTopic(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void DeleteTopic(AwsClientDetails clientDetails)
        {
            using (var helper = new SNSHelper(clientDetails))
            {
                helper.DeleteTopic(TopicArn);
                Log.LogMessage(MessageImportance.Normal, "Deleted SNS Topic {0}", TopicArn);
            }
        }
    }
}
