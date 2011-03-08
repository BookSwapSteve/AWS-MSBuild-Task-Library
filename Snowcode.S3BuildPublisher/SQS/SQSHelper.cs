using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Snowcode.S3BuildPublisher.Client;
using Attribute = Amazon.SQS.Model.Attribute;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// Helper class for Amazon Simple Queue Service.
    /// </summary>
    public class SQSHelper : IDisposable
    {
        private bool _disposed;

        #region Constructors

        public SQSHelper(string awsAccessKeyId, string awsSecretAccessKey)
        {
            Client = AWSClientFactory.CreateAmazonSQSClient(awsAccessKeyId, awsSecretAccessKey);
        }

        public SQSHelper(AwsClientDetails clientDetails)
        {
            Client = AWSClientFactory.CreateAmazonSQSClient(clientDetails.AwsAccessKeyId, clientDetails.AwsSecretAccessKey);
        }

        public SQSHelper(AmazonSQS amazonSQSClient)
        {
            Client = amazonSQSClient;
        }

        ~SQSHelper()
        {
            Dispose(false);
        }

        #endregion

        protected AmazonSQS Client
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a SQS queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public string CreateQueue(string queueName)
        {
            var request = new CreateQueueRequest { QueueName = queueName };

            CreateQueueResponse response = Client.CreateQueue(request);

            return response.CreateQueueResult.QueueUrl;
        }

        /// <summary>
        /// Sets the permissions on the queue
        /// </summary>
        /// <param name="queueUrl"></param>
        /// <param name="label"></param>
        /// <param name="actionNames"></param>
        /// <param name="awsAccountIds"></param>
        public void SetQueuePermissions(string queueUrl, string label, IEnumerable<string> actionNames, IEnumerable<string> awsAccountIds)
        {
            var request = new AddPermissionRequest
                              {
                                  ActionName = new List<string>(actionNames),
                                  QueueUrl = queueUrl,
                                  AWSAccountId = new List<string>(awsAccountIds),
                                  Label = label
                              };

            Client.AddPermission(request);
        }

        /// <summary>
        /// Deletes the SQS Queue
        /// </summary>
        /// <param name="queueUrl"></param>
        public void DeleteQueue(string queueUrl)
        {
            var request = new DeleteQueueRequest { QueueUrl = queueUrl };

            Client.DeleteQueue(request);
        }

        /// <summary>
        /// Lists the Queues
        /// </summary>
        /// <returns></returns>
        public string[] ListQueues()
        {
            var request = new ListQueuesRequest();

            ListQueuesResponse response = Client.ListQueues(request);

            return response.ListQueuesResult.QueueUrl.ToArray();
        }

        /// <summary>
        /// Sends a message to the SQS Queue
        /// </summary>
        /// <param name="messageBody"></param>
        /// <param name="queueUrl"></param>
        /// <returns></returns>
        public string SendMessage(string messageBody, string queueUrl)
        {
            var request = new SendMessageRequest { MessageBody = messageBody, QueueUrl = queueUrl };

            SendMessageResponse response = Client.SendMessage(request);

            return response.SendMessageResult.MessageId;
        }

        /// <summary>
        /// Receives a message from the SQS Queue
        /// </summary>
        /// <param name="queueUrl"></param>
        /// <returns></returns>
        public Message ReceiveMessage(string queueUrl)
        {
            var request = new ReceiveMessageRequest { MaxNumberOfMessages = 1, QueueUrl = queueUrl };

            ReceiveMessageResponse response = Client.ReceiveMessage(request);

            if (response.IsSetReceiveMessageResult())
            {
                return response.ReceiveMessageResult.Message.FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Wait for a message on the Queue
        /// </summary>
        /// <param name="queueUrl"></param>
        /// <param name="timeOutSeconds"></param>
        /// <param name="pollIntervalSeconds"></param>
        /// <returns></returns>
        /// <exception cref="TimeoutException">thrown if timeOutSeconds is exceeded.</exception>
        public Message WaitForMessage(string queueUrl, int timeOutSeconds, int pollIntervalSeconds)
        {
            DateTime waitUntil = DateTime.Now.AddSeconds(timeOutSeconds);

            do
            {
                Message message = ReceiveMessage(queueUrl);
                if (message != null)
                {
                    return message;
                }
                Thread.Sleep(new TimeSpan(0, 0, pollIntervalSeconds));
            } while (DateTime.Now <= waitUntil);

            throw new TimeoutException(string.Format("Timeout waiting for a message on the Queue {0}", queueUrl));
        }

        /// <summary>
        /// Deletes a message from the queue
        /// </summary>
        /// <param name="queueUrl"></param>
        /// <param name="receiptHandle"></param>
        public void DeleteMessage(string queueUrl, string receiptHandle)
        {
            var request = new DeleteMessageRequest { QueueUrl = queueUrl, ReceiptHandle = receiptHandle };

            Client.DeleteMessage(request);
        }

        /// <summary>
        /// Gets the Queue's attributes
        /// </summary>
        /// <param name="queueUrl"></param>
        /// <returns></returns>
        /// <remarks>
        /// All - returns all values.
        /// ApproximateNumberOfMessages — returns the approximate number of visible messages in a queue. For more information, see Resources Required to Process Messages in the Amazon SQS Developer Guide.
        /// ApproximateNumberOfMessagesNotVisible — returns the approximate number of messages that are not timed-out and not deleted. For more information, see Resources Required to Process Messages in the Amazon SQS Developer Guide.
        /// VisibilityTimeout — returns the visibility timeout for the queue. For more information about visibility timeout, see Visibility Timeout in the Amazon SQS Developer Guide.
        /// CreatedTimestamp — returns the time when the queue was created (epoch time in seconds).
        /// LastModifiedTimestamp — returns the time when the queue was last changed (epoch time in seconds).
        /// Policy — returns the queue's policy.
        /// MaximumMessageSize — returns the limit of how many bytes a message can contain before Amazon SQS rejects it.
        /// MessageRetentionPeriod — returns the number of seconds Amazon SQS retains a message.
        /// QueueArn — returns the queue's Amazon resource name (ARN).
        /// </remarks>
        public GetQueueAttributesResult GetQueueAttributes(string queueUrl)
        {
            var request = new GetQueueAttributesRequest { QueueUrl = queueUrl, AttributeName = new List<string>(new[] { "All" }) };

            GetQueueAttributesResponse response = Client.GetQueueAttributes(request);

            return response.GetQueueAttributesResult;
        }

        /// <summary>
        /// Allow a Sns notification to publish to the queue
        /// </summary>
        /// <seealso cref="http://www.elastician.com/2010/04/subscribing-sqs-queue-to-sns-topic.html"/>
        public void GrantSendMessageRights(string queueUrl, string sourceArn)
        {
            string queueArn = GetQueueArn(queueUrl);

            var request = new SetQueueAttributesRequest { QueueUrl = queueUrl };
            var attribute = new Attribute { Name = "Policy", Value = ConstructPolicy(queueArn, sourceArn) };
            request.Attribute = new List<Attribute> { attribute };

            Client.SetQueueAttributes(request);
        }

        private string ConstructPolicy(string queueArn, string sourceArn)
        {
            var policy = new StringBuilder();
            policy.Append("{");
            policy.Append("\"Version\":\"2008-10-17\",");
            policy.Append("\"Id\":\"MyQueuePolicy\",");
            policy.Append("\"Statement\" : [");
            policy.Append("{");
            policy.Append("\"Sid\":\"Allow-SNS-SendMessage\",");
            policy.Append("\"Effect\":\"Allow\",");
            policy.Append("\"Principal\" : {\"AWS\": \"*\"},");
            policy.Append("\"Action\":[\"sqs:SendMessage\"],");
            policy.AppendFormat("\"Resource\": \"{0}\",", queueArn);
            policy.Append("\"Condition\" : {");
            policy.Append("\"ArnEquals\" : {");
            policy.AppendFormat("\"aws:SourceArn\":\"{0}\"", sourceArn);
            policy.Append("}");
            policy.Append("}");
            policy.Append("}");
            policy.Append("]");
            policy.Append("}");

            return policy.ToString();
        }

        private string GetQueueArn(string queueUrl)
        {
            var request = new GetQueueAttributesRequest { QueueUrl = queueUrl, AttributeName = new List<string>(new[] { "QueueArn" }) };

            GetQueueAttributesResponse response = Client.GetQueueAttributes(request);

            if (response.IsSetGetQueueAttributesResult())
            {
                return response.GetQueueAttributesResult.Attribute.Where(x => x.Name == "QueueArn").First().Value;
            }
            throw new Exception("No SetQueueAttribute result");
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
