using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Logging;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild task to delete a Simple Queue Service Queue.
    /// </summary>
    public class DeleteSQSQueueTask : SqsTaskBase
    {
        #region Constructors

        public DeleteSQSQueueTask()
            : base()
        { }

        public DeleteSQSQueueTask(IAwsClientFactory awsClientFactory, ITaskLogger logger)
            : base(awsClientFactory, logger)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the Url of the Queue to delete.
        /// </summary>
        [Required]
        public string QueueUrl { get; set; }

        #endregion

        protected override bool Execute(AmazonSQS client)
        {
            Logger.LogMessage(MessageImportance.Normal, "Deleting SQS Queue at {0}", QueueUrl);

            var request = new DeleteQueueRequest { QueueUrl = QueueUrl };
            client.DeleteQueue(request);

            Logger.LogMessage(MessageImportance.Normal, "Deleted SQS Queue at {0}", QueueUrl);

            return true;
        }
    }
}
