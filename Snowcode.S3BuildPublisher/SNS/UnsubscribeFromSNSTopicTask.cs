using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SNS
{
    /// <summary>
    /// MSBuild task to unsubscribe from an AWS Simple Notification Service Subscription.
    /// </summary>
    public class UnsubscribeFromSNSTopicTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the SubscriptionArn returned by the subscribe action.
        /// </summary>
        [Required]
        public string SubscriptionArn { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Unsubscribing from subscription {0}", SubscriptionArn);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                Unsubscribe(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void Unsubscribe(AwsClientDetails clientDetails)
        {
            using (var helper = new SNSHelper(clientDetails))
            {
                helper.Unsubscribe(SubscriptionArn);
                Log.LogMessage(MessageImportance.Normal, "Unsubscribed from SubscriptionArn {0}", SubscriptionArn);
            }
        }
    }
}
