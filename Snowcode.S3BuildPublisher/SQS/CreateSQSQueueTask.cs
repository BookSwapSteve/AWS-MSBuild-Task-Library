using System;
using Microsoft.Build.Framework;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild task to create a Simple Queue Service queue.
    /// </summary>
    public class CreateSQSQueueTask : AwsTaskBase
    {
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

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Creating SQS Queue {0}", QueueName);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                CreateQueue(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void CreateQueue(AwsClientDetails clientDetails)
        {
            using (var helper = new SQSHelper(clientDetails))
            {
                QueueUrl = helper.CreateQueue(QueueName);
                Log.LogMessage(MessageImportance.Normal, "Creates SQS Queue {0} at {1}", QueueName, QueueUrl);
            }
        }
    }
}
