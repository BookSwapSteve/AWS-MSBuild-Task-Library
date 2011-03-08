using System;
using System.Collections.Generic;
using System.Diagnostics;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SNS
{
    /// <summary>
    /// Helper class for Amazon Simple Notification Service.
    /// </summary>
    public class SNSHelper : IDisposable
    {
        private bool _disposed;

        #region Constructors

        public SNSHelper(string awsAccessKeyId, string awsSecretAccessKey)
        {
            Client = AWSClientFactory.CreateAmazonSNSClient(awsAccessKeyId, awsSecretAccessKey);
        }

        public SNSHelper(AwsClientDetails clientDetails)
        {
            Client = AWSClientFactory.CreateAmazonSNSClient(clientDetails.AwsAccessKeyId, clientDetails.AwsSecretAccessKey);
        }

        public SNSHelper(AmazonSimpleNotificationService amazonSNSClient)
        {
            Client = amazonSNSClient;
        }

        ~SNSHelper()
        {
            Dispose(false);
        }

        #endregion

        protected AmazonSimpleNotificationService Client
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a topic.  Should only be needed to be used once.
        /// </summary>
        /// <param name="topicName"></param>
        /// <returns></returns>
        public string CreateTopic(string topicName)
        {
            var request = new CreateTopicRequest { Name = topicName };

            CreateTopicResponse response = Client.CreateTopic(request);

            return response.CreateTopicResult.TopicArn;
        }

        /// <summary>
        /// Delete a SNS Topic
        /// </summary>
        /// <param name="topicArn"></param>
        public void DeleteTopic(string topicArn)
        {
            var request = new DeleteTopicRequest {TopicArn = topicArn};

            Client.DeleteTopic(request);
        }

        /// <summary>
        /// Adds permission to the Topic for Aws Accounts to performe the actions named.
        /// </summary>
        /// <param name="actionNames"></param>
        /// <param name="awsAccountIds"></param>
        /// <param name="label"></param>
        /// <param name="topicArn"></param>
        public void AddPermission(IEnumerable<string> actionNames, IEnumerable<string> awsAccountIds, string label, string topicArn)
        {
            var request = new AddPermissionRequest
                              {
                                  ActionNames = new List<string>(actionNames),
                                  AWSAccountIds = new List<string>(awsAccountIds),
                                  Label = label,
                                  TopicArn = topicArn
                              };

            // NB: As of version 1.0.8.1 of AWS SDK there appears to be a problem with action names and aws account Id's 
            // validations and these fail whilst apparently having valid values.
            Client.AddPermission(request);
        }

        /// <summary>
        /// Publish a notification
        /// </summary>
        /// <param name="topicArn"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        public string Publish(string topicArn, string subject, string message)
        {
            var request = new PublishRequest { TopicArn = topicArn, Subject = subject, Message = message };

            PublishResponse response = Client.Publish(request);

            return response.PublishResult.MessageId;
        }

        /// <summary>
        /// List the SNS Topics.
        /// </summary>
        /// <param name="nextToken"></param>
        /// <returns></returns>
        public string[] ListTopics(string nextToken)
        {
            ListTopicsRequest request = new ListTopicsRequest();
            request.NextToken = nextToken;
            ListTopicsResponse response = Client.ListTopics(request);

            var topics = new List<string>();
            foreach (Topic topic in response.ListTopicsResult.Topics)
            {
                Debug.WriteLine(topic.TopicArn, "TopicArn");
                topics.Add(topic.TopicArn);
            }

            Debug.WriteLine(response.ListTopicsResult.NextToken, "NextToken");

            return topics.ToArray();
        }

        /// <summary>
        /// Subscribe to a SNS Topic
        /// </summary>
        /// <param name="topicArn"></param>
        /// <param name="protocol"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public string Subscribe(string topicArn, string protocol, string endpoint)
        {
            var request = new SubscribeRequest { TopicArn = topicArn, Protocol = protocol, Endpoint = endpoint };

            SubscribeResponse response = Client.Subscribe(request);

            Trace.WriteLine(response.SubscribeResult.SubscriptionArn, "SubscriptionArn");

            return response.SubscribeResult.SubscriptionArn;
        }

        /// <summary>
        /// Unsubscribe from a SNS Topic
        /// </summary>
        /// <param name="subscriptionArn"></param>
        public void Unsubscribe(string subscriptionArn)
        {
            var request = new UnsubscribeRequest {SubscriptionArn = subscriptionArn};

            Client.Unsubscribe(request);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        virtual protected void Dispose(bool disposing)
        {
            if (_disposed)
            {
                if (!disposing)
                {
                    try
                    {
                        if (Client != null)
                        {
                            Client.Dispose();
                        }
                    }
                    finally
                    {
                        _disposed = true;
                    }
                }
            }
        }

        #endregion
    }
}
