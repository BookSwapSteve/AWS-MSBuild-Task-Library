using System.Linq;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Logging;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild task to receive a message from a queue.  
    /// </summary>
    public class ReceiveSQSMessageTask : SqsTaskBase
    {
        #region Constructors

        public ReceiveSQSMessageTask()
            : base()
        { }

        public ReceiveSQSMessageTask(IAwsClientFactory awsClientFactory, ITaskLogger logger)
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

        protected override bool Execute(AmazonSQS client)
        {
            Logger.LogMessage(MessageImportance.Normal, "Receiving message from Queue {0}", QueueUrl);

            var request = new ReceiveMessageRequest { MaxNumberOfMessages = 1, QueueUrl = QueueUrl };
            ReceiveMessageResponse response = client.ReceiveMessage(request);

            if (response.IsSetReceiveMessageResult())
            {
                Message message = response.ReceiveMessageResult.Message.FirstOrDefault();
                ProcessMessage(message);
            }

            // return true even if no message received as the task executed ok.
            return true;
        }

        private void ProcessMessage(Message message)
        {
            if (message != null)
            {
                MessageId = message.MessageId;
                MessageBody = message.Body;
                ReceiptHandle = message.ReceiptHandle;
                HasMessage = true;
                Logger.LogMessage(MessageImportance.Normal, "Recieved message {0} from queue {1}", MessageId, QueueUrl);
            }
            else
            {
                MessageId = string.Empty;
                MessageBody = string.Empty;
                ReceiptHandle = string.Empty;
                HasMessage = false;
                Logger.LogMessage(MessageImportance.High, "No message received from queue {0}", QueueUrl);
            }
        }
    }
}
