using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild task to receive a message from a queue.  
    /// </summary>
    public class ReceiveSQSMessageTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets and sets the Url of the Queue to delete.
        /// </summary>
        [Required]
        public string QueueUrl { get; set; }

        /// <summary>
        /// Gets and sets the MessageId of the message received.
        /// </summary>
        [Output]
        public string MessageId { get; set; }

        /// <summary>
        /// Gets and sets the message body received.
        /// </summary>
        [Output]
        public string MessageBody { get; set; }

        /// <summary>
        /// Gets and sets the message receipt handle, this is need to delete the message from the queue.
        /// </summary>
        [Output]
        public string ReceiptHandle { get; set; }

        /// <summary>
        /// Gets and sets if a message has been received from the queue
        /// </summary>
        [Output]
        public bool HasMessage { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Receiving message from Queue {0}", QueueUrl);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                ReceiveMessage(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void ReceiveMessage(AwsClientDetails clientDetails)
        {
            using (var helper = new SQSHelper(clientDetails))
            {
                Amazon.SQS.Model.Message message = helper.ReceiveMessage(QueueUrl);

                if (message != null)
                {
                    MessageId = message.MessageId;
                    MessageBody = message.Body;
                    ReceiptHandle = message.ReceiptHandle;
                    HasMessage = true;
                    Log.LogMessage(MessageImportance.Normal, "Recieved message {0} from queue {1}", MessageId, QueueUrl);
                }
                else
                {
                    MessageId = string.Empty;
                    MessageBody = string.Empty;
                    ReceiptHandle = string.Empty;
                    HasMessage = false;
                    Log.LogMessage(MessageImportance.Normal, "No message received from queue {0}", QueueUrl);
                }
            }
        }
    }
}
