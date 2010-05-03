using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild task to wait for a message on the SQS Queue
    /// </summary>
    public class WaitForSQSMessageTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets and sets the Url of the Queue to delete.
        /// </summary>
        [Required]
        public string QueueUrl { get; set; }

        /// <summary>
        /// The time to wait for a message before giving up, in seconds.
        /// </summary>
        [Required]
        public int TimeOutSeconds { get; set; }

        /// <summary>
        /// How often to poll the queue
        /// </summary>
        [Required]
        public int PollIntervalSeconds { get; set; }

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

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Waiting to receive a message from Queue {0}, Poll interval {1} seconds, Timeout in {2} seconds", QueueUrl, PollIntervalSeconds, TimeOutSeconds);

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
                Amazon.SQS.Model.Message message = helper.WaitForMessage(QueueUrl, TimeOutSeconds, PollIntervalSeconds);

                MessageId = message.MessageId;
                MessageBody = message.Body;
                ReceiptHandle = message.ReceiptHandle;
                Log.LogMessage(MessageImportance.Normal, "Recieved message {0} from queue {1}", MessageId, QueueUrl);
            }
        }
    }
}
