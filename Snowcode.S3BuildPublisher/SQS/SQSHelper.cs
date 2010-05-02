using System.Collections.Generic;
using System.Diagnostics;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Snowcode.S3BuildPublisher.SQS
{
    public class SQSHelper
    {
        #region Constructors

        public SQSHelper(string awsAccessKeyId, string awsSecretAccessKey)
        {
            Client = new AmazonSQSClient(awsAccessKeyId, awsSecretAccessKey);
        }

        public SQSHelper(AwsClientDetails clientDetails)
        {
            Client = new AmazonSQSClient(clientDetails.AwsAccessKeyId, clientDetails.AwsSecretAccessKey);
        }

        #endregion

        protected AmazonSQSClient Client
        {
            get;
            set;
        }

        public string CreateQueue(string queueName)
        {
            var request = new CreateQueueRequest { QueueName = queueName };

            CreateQueueResponse response = Client.CreateQueue(request);

            return response.CreateQueueResult.QueueUrl;
        }

        public void SetQueuePermissions(string queueUrl, string label, IEnumerable<string> awsAccountIds)
        {
            var request = new AddPermissionRequest
                              {
                                  ActionName = new List<string> { "*" },
                                  QueueUrl = queueUrl,
                                  AWSAccountId = new List<string>(awsAccountIds),
                                  Label = label
                              };

            Client.AddPermission(request);
        }

        public void DeleteQueue(string queueUrl)
        {
            var request = new DeleteQueueRequest { QueueUrl = queueUrl };

            Client.DeleteQueue(request);
        }

        public string[] ListQueues()
        {
            var request = new ListQueuesRequest();

            ListQueuesResponse response = Client.ListQueues(request);

            return response.ListQueuesResult.QueueUrl.ToArray();
        }

        public string SendMessage(string messageBody, string queueUrl)
        {
            var request = new SendMessageRequest { MessageBody = messageBody, QueueUrl = queueUrl };

            SendMessageResponse response = Client.SendMessage(request);

            return response.SendMessageResult.MessageId;
        }

        public string[] ReceiveMessage(decimal maxNumberOfMessages, string queueUrl, bool deleteOnRead)
        {
            var request = new ReceiveMessageRequest { MaxNumberOfMessages = maxNumberOfMessages, QueueUrl = queueUrl };

            ReceiveMessageResponse response = Client.ReceiveMessage(request);

            var messageBodies = new List<string>();
            if (response.IsSetReceiveMessageResult())
            {
                foreach (Message message in response.ReceiveMessageResult.Message)
                {
                    messageBodies.Add(message.Body);

                    // Delete the message onces it's been read.
                    if (deleteOnRead)
                    {
                        DeleteMessage(queueUrl, message.ReceiptHandle);
                    }
                }
            }
            return messageBodies.ToArray();
        }

        private void DeleteMessage(string queueUrl, string receiptHandle)
        {
            var request = new DeleteMessageRequest { QueueUrl = queueUrl, ReceiptHandle = receiptHandle };

            Client.DeleteMessage(request);
        }
    }
}
