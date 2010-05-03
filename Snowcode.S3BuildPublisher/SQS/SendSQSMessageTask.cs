using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild task to send a message to a SQS Queue.
    /// </summary>
    public class SendSQSMessageTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets and sets the Url of the Queue to delete.
        /// </summary>
        [Required]
        public string QueueUrl { get; set; }

        /// <summary>
        /// Gets and sets the message to send to the queue
        /// </summary>
        [Required]
        public string MessageBody { get; set; }

        /// <summary>
        /// Gets and sets the MessageId of the sent message.
        /// </summary>
        [Output]
        public string MessageId { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Sending message to Queue {0}", QueueUrl);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                SendMessage(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void SendMessage(AwsClientDetails clientDetails)
        {
            using (var helper = new SQSHelper(clientDetails))
            {
                MessageId = helper.SendMessage(MessageBody, QueueUrl);
                Log.LogMessage(MessageImportance.Normal, "Sent message to Queue {0}", QueueUrl);
            }
        }
    }
}
