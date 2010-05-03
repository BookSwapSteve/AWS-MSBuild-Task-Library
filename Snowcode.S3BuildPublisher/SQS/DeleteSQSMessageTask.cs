using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild task to delete a message from a queue.
    /// </summary>
    public class DeleteSQSMessageTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets and sets the Url of the Queue to delete the message from.
        /// </summary>
        [Required]
        public string QueueUrl { get; set; }

        /// <summary>
        /// Gets and sets the message receipt handle to delete.
        /// </summary>
        [Required]
        public string ReceiptHandle { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Deleting message {0} from Queue {1}", ReceiptHandle, QueueUrl);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                DeleteMessage(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void DeleteMessage(AwsClientDetails clientDetails)
        {
            using (var helper = new SQSHelper(clientDetails))
            {
                helper.DeleteMessage(QueueUrl, ReceiptHandle);

                Log.LogMessage(MessageImportance.Normal, "Deleted message {0} from queue {1}", ReceiptHandle, QueueUrl);
            }
        }
    }
}
