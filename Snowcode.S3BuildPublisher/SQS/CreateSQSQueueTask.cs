using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Logging;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild task to create a Simple Queue Service queue.
    /// </summary>
    public class CreateSQSQueueTask : SqsTaskBase
    {
        #region Constructors

        public CreateSQSQueueTask()
            : base()
        { }

        public CreateSQSQueueTask(IAwsClientFactory awsClientFactory, ITaskLogger logger)
            : base(awsClientFactory, logger)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the queue to create.
        /// </summary>
        [Required]
        public string QueueName { get; set; }

        /// <summary>
        /// Gets and sets the Queue Url returned from the CreateQueue service call.
        /// </summary>
        [Output]
        public string QueueUrl { get; set; }

        #endregion

        protected override bool Execute(AmazonSQS client)
        {
            Logger.LogMessage(MessageImportance.Normal, "Creating SQS Queue {0}", QueueName);

            var request = new CreateQueueRequest { QueueName = QueueName };
            CreateQueueResponse response = client.CreateQueue(request);

            if (response.IsSetCreateQueueResult())
            {
                QueueUrl = response.CreateQueueResult.QueueUrl;
                Logger.LogMessage(MessageImportance.Normal, "Creates SQS Queue {0} at {1}", QueueName, QueueUrl);
                return true;
            }

            Logger.LogMessage(MessageImportance.Normal, "Failed to create SQS Queue {0}", QueueName);
            return false;
        }
    }
}
