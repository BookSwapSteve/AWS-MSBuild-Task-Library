using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild task to delete a Simple Queue Service Queue.
    /// </summary>
    public class DeleteSQSQueueTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets and sets the Url of the Queue to delete.
        /// </summary>
        [Required]
        public string QueueUrl { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Deleting SQS Queue at {0}", QueueUrl);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                DeleteQueue(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void DeleteQueue(AwsClientDetails clientDetails)
        {
            using (var helper = new SQSHelper(clientDetails))
            {
                helper.DeleteQueue(QueueUrl);
                Log.LogMessage(MessageImportance.Normal, "Deleted SQS Queue at {0}", QueueUrl);
            }
        }
    }
}
