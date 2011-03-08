using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// Allow the source send message to SQS (e.g. allow SNS to send 
    /// </summary>
    public class GrantSendMessageRightsTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the queue to create.
        /// </summary>
        [Required]
        public string QueueUrl { get; set; }

        /// <summary>
        /// Arn of the source (i.e. sns arn)
        /// </summary>
        [Required]
        public string SourceArn { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Granting SendMessage rights to SQS Queue at {0}", QueueUrl);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                GrantRights(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void GrantRights(AwsClientDetails clientDetails)
        {
            using (var helper = new SQSHelper(clientDetails))
            {
                helper.GrantSendMessageRights(QueueUrl, SourceArn);
                Log.LogMessage(MessageImportance.Normal, "Granted rights for source {0} to SendMessage to SQS at {1}", SourceArn, QueueUrl);
            }
        }
    }
}
