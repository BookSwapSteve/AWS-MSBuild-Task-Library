using System;
using System.Linq;
using Amazon.SQS.Model;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SQS
{
    public class GetSQSAttribuitesTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets and sets the Queue Url returned from the CreateQueue service call.
        /// </summary>
        [Required]
        public string QueueUrl { get; set; }

        /// <summary>
        /// Gets the approximate number of visible messages in a queue. For more information, see Resources Required to Process Messages in the Amazon SQS Developer Guide.
        /// </summary>
        [Output]
        public string ApproximateNumberOfMessages { get; set; }

        /// <summary>
        /// returns the approximate number of messages that are not timed-out and not deleted. For more information, see Resources Required to Process Messages in the Amazon SQS Developer Guide.
        /// </summary>
        [Output]
        public string ApproximateNumberOfMessagesNotVisible { get; set; }

        /// <summary>
        /// returns the visibility timeout for the queue. For more information about visibility timeout, see Visibility Timeout in the Amazon SQS Developer Guide.
        /// </summary>
        [Output]
        public string VisibilityTimeout { get; set; }

        /// <summary>
        /// returns the time when the queue was created (epoch time in seconds).
        /// </summary>
        [Output]
        public string CreatedTimestamp { get; set; }

        /// <summary>
        /// returns the time when the queue was last changed (epoch time in seconds).
        /// </summary>
        [Output]
        public string LastModifiedTimestamp { get; set; }

        /////// <summary>
        /////// returns the queue's policy.
        /////// </summary>
        ////[Output]
        ////public string Policy { get; set; }

        /// <summary>
        /// returns the limit of how many bytes a message can contain before Amazon SQS rejects it.
        /// </summary>
        [Output]
        public string MaximumMessageSize { get; set; }

        /// <summary>
        /// returns the number of seconds Amazon SQS retains a message
        /// </summary>
        [Output]
        public string MessageRetentionPeriod { get; set; }

        /// <summary>
        /// returns the queue's Amazon resource name (ARN).
        /// </summary>
        [Output]
        public string QueueArn { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Getting SQS Queue attributes for queue at {0}", QueueUrl);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                GetQueueAttributes(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void GetQueueAttributes(AwsClientDetails clientDetails)
        {
            using (var helper = new SQSHelper(clientDetails))
            {
                GetQueueAttributesResult attributes = helper.GetQueueAttributes(QueueUrl);
                Log.LogMessage(MessageImportance.Normal, "Got SQS attributes for Queue {0}", QueueUrl);

                ApproximateNumberOfMessages = GetAttributeValue(attributes, "ApproximateNumberOfMessages");
                ApproximateNumberOfMessagesNotVisible = GetAttributeValue(attributes, "ApproximateNumberOfMessagesNotVisible");
                VisibilityTimeout = GetAttributeValue(attributes, "VisibilityTimeout");
                CreatedTimestamp = GetAttributeValue(attributes, "CreatedTimestamp");
                LastModifiedTimestamp = GetAttributeValue(attributes, "LastModifiedTimestamp");
                //Policy is not a string.
                //Policy = GetAttributeValue(attributes, "Policy");
                MaximumMessageSize = GetAttributeValue(attributes, "MaximumMessageSize");
                MessageRetentionPeriod = GetAttributeValue(attributes, "MessageRetentionPeriod");
                QueueArn = GetAttributeValue(attributes, "QueueArn");
            }
        }

        private string GetAttributeValue(GetQueueAttributesResult attributeResults, string attributeName)
        {
            return attributeResults.Attribute.Where(x => x.Name == attributeName).FirstOrDefault().Value;
        }
    }
}
