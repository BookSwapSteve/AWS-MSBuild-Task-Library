using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SNS
{
    /// <summary>
    /// MSBuild task to subscribe to a Simple Notification Service Topic.
    /// </summary>
    public class SubscribeToSNSTopicTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the TopicArn to subscribe to
        /// </summary>
        [Required]
        public string TopicArn { get; set; }

        /// <summary>
        /// Gets or sets the protocol. Options are http, https, email, email-json, sqs
        /// </summary>
        [Required]
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the endpoint.  i.e. email address, http(s) url, SQS Arn.
        /// </summary>
        /// <remarks>
        /// For http the endpoint is an URL beginning with "http://"
        /// For https the endpoint is a URL beginning with "https://"
        /// For email the endpoint is an e-mail address
        /// For email-json the endpoint is an e-mail address
        /// For sqs the endpoint is the ARN of an Amazon SQS queue
        /// </remarks>
        [Required]
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the SubscriptionArn returned by the subscribe action.
        /// </summary>
        [Output]
        public string SubscriptionArn { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Subscribing to SNS Topic {0}", TopicArn);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                Subscribe(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void Subscribe(AwsClientDetails clientDetails)
        {
            using (var helper = new SNSHelper(clientDetails))
            {
                SubscriptionArn = helper.Subscribe(TopicArn, Protocol, Endpoint);
                Log.LogMessage(MessageImportance.Normal, "Subscribed to Topic {0}, SubscriptionArn {1}", TopicArn, SubscriptionArn);
            }
        }
    }
}
