using System;
using System.Diagnostics;
using Snowcode.S3BuildPublisher.Logging;
using Snowcode.S3BuildPublisher.SQS;

namespace Snowcode.S3BuildPublisher.Test
{
    static class TestHelper
    {
        //public const string EncryptionContainerName = "MySecretContainer";
        public const string EncryptionContainerName = "BuildMachineNet";

        /// <summary>
        /// Create a queue on AWS to use for testing.
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="encryptionContainerName"></param>
        /// <returns></returns>
        public static string CreateQueue(string queueName, string encryptionContainerName)
        {
            var createSqsQueueTask = new CreateSQSQueueTask(new AwsClientFactory(), new NullLogger())
                                         {
                                             QueueName = queueName,
                                             EncryptionContainerName = encryptionContainerName
                                         };
            createSqsQueueTask.Execute();

            Debug.WriteLine("TestHelper created SQS Queue: " + createSqsQueueTask.QueueUrl);

            return createSqsQueueTask.QueueUrl;
        }

        /// <summary>
        /// Delete the queue used for testing
        /// </summary>
        /// <param name="queueUrl"></param>
        /// <param name="encryptionContainerName"></param>
        public static void DeleteQueue(string queueUrl, string encryptionContainerName)
        {
            var createSqsQueueTask = new DeleteSQSQueueTask(new AwsClientFactory(), new NullLogger())
            {
                QueueUrl = queueUrl,
                EncryptionContainerName = encryptionContainerName
            };

            createSqsQueueTask.Execute();

            Debug.WriteLine("Delted SQS Queue: " + createSqsQueueTask.QueueUrl);
        }
    }
}
