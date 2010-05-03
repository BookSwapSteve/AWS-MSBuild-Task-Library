using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SNS
{
    /// <summary>
    /// MSBuild task to publish a Simple Notification Services notification.
    /// </summary>
    public class PublishSNSNotificationTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the TopicArn
        /// </summary>
        [Required]
        public string TopicArn { get; set; }

        /// <summary>
        /// Gets or set the notification subject
        /// </summary>
        [Required]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the notification message
        /// </summary>
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the returned MessageId
        /// </summary>
        [Output]
        public string MessageId { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Publishing SNS Notification to Topic {0}", TopicArn);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                PublishNotifiation(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void PublishNotifiation(AwsClientDetails clientDetails)
        {
            using (var helper = new SNSHelper(clientDetails))
            {
                MessageId = helper.Publish(TopicArn, Subject, Message);
                Log.LogMessage(MessageImportance.Normal, "Published SNS Notification {0}", Subject);
            }
        }
    }
}
