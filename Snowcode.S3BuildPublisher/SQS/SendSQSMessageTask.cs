using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Logging;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild task to send a message to a SQS Queue.
    /// </summary>
    public class SendSQSMessageTask : SqsTaskBase
    {
        #region Constructors

        public SendSQSMessageTask()
            : base()
        { }

        public SendSQSMessageTask(IAwsClientFactory awsClientFactory, ITaskLogger logger)
            : base(awsClientFactory, logger)
        { }

        #endregion

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

        protected override bool Execute(AmazonSQS client)
        {
            var request = new SendMessageRequest { MessageBody = MessageBody, QueueUrl = QueueUrl };

            SendMessageResponse response = client.SendMessage(request);

            if (response.IsSetSendMessageResult())
            {
                MessageId = response.SendMessageResult.MessageId;

                Logger.LogMessage(MessageImportance.Normal, "Sent message {0} to Queue {1}", MessageId, QueueUrl);
                return true;
            }

            Logger.LogMessage(MessageImportance.High, "Message failed to send to to Queue {0}", QueueUrl);
            return false;
        }
    }
}
