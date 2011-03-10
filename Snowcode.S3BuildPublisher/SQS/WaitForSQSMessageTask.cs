using System;
using System.Linq;
using System.Threading;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Logging;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild task to wait for a message on the SQS Queue
    /// </summary>
    public class WaitForSQSMessageTask : SqsTaskBase
    {
        #region Constructors

        public WaitForSQSMessageTask()
            : base()
        { }

        public WaitForSQSMessageTask(IAwsClientFactory awsClientFactory, ITaskLogger logger)
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

        protected override bool Execute(AmazonSQS client)
        {
            Logger.LogMessage(MessageImportance.High, "Waiting to receive a message from Queue {0}, Poll interval {1} seconds, Timeout in {2} seconds", QueueUrl, PollIntervalSeconds, TimeOutSeconds);

            Message message = WaitForMessage(client);

            MessageId = message.MessageId;
            MessageBody = message.Body;
            ReceiptHandle = message.ReceiptHandle;
            Logger.LogMessage(MessageImportance.Normal, "Recieved message {0} from queue {1}", MessageId, QueueUrl);

            return true;
        }

        /// <summary>
        /// Wait for a message on the Queue
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException">thrown if timeOutSeconds is exceeded.</exception>
        private Message WaitForMessage(AmazonSQS client)
        {
            DateTime waitUntil = DateTime.Now.AddSeconds(TimeOutSeconds);

            do
            {
                Message message = ReceiveMessage(client);

                if (message != null)
                {
                    return message;
                }

                Thread.Sleep(new TimeSpan(0, 0, PollIntervalSeconds));

            } while (DateTime.Now <= waitUntil);

            throw new TimeoutException(string.Format("Timeout waiting for a message on the Queue {0}", QueueUrl));
        }

        /// <summary>
        /// Receives a message from the SQS Queue
        /// </summary>
        /// <param name="client"></param>
        /// <param name="queueUrl"></param>
        /// <returns></returns>
        private Message ReceiveMessage(AmazonSQS client)
        {
            var request = new ReceiveMessageRequest { MaxNumberOfMessages = 1, QueueUrl = QueueUrl };

            ReceiveMessageResponse response = client.ReceiveMessage(request);

            if (response.IsSetReceiveMessageResult())
            {
                return response.ReceiveMessageResult.Message.FirstOrDefault();
            }
            return null;
        }
    }
}
