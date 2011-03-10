using Snowcode.S3BuildPublisher.SQS;

namespace Snowcode.S3BuildPublisher.Test
{
    static class TestHelper
    {
        /// <summary>
        /// Create a queue on AWS to use for testing.
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="encryptionContainerName"></param>
        /// <returns></returns>
        public static string CreateQueue(string queueName, string encryptionContainerName)
        {
            var createSqsQueueTask = new CreateSQSQueueTask
                                         {
                                             QueueName = queueName,
                                             EncryptionContainerName = encryptionContainerName
                                         };
            createSqsQueueTask.Execute();
            return createSqsQueueTask.QueueUrl;
        }
    }
}
