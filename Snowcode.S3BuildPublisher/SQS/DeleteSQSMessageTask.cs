using System;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild task to delete a message from a queue.
    /// </summary>
    public class DeleteSQSMessageTask :  SqsTaskBase
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

        protected override bool Execute(AmazonSQS client)
        {
            Logger.LogMessage(MessageImportance.Normal, "Deleting message {0} from Queue {1}", ReceiptHandle, QueueUrl);

            var request = new DeleteMessageRequest { QueueUrl = QueueUrl, ReceiptHandle = ReceiptHandle };
            client.DeleteMessage(request);

            Logger.LogMessage(MessageImportance.Normal, "Deleted message {0} from queue {1}", ReceiptHandle, QueueUrl);

            return true;
        }
    }
}
