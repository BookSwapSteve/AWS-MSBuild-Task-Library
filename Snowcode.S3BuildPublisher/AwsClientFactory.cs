using Amazon;
using Amazon.IdentityManagement;
using Amazon.SQS;

namespace Snowcode.S3BuildPublisher
{
    /// <summary>
    /// Factory wrapper around static AWSClientFactory to allow for instantiation of client
    /// when the credentials are know and help with mocking.
    /// </summary>
    public class AwsClientFactory : IAwsClientFactory
    {
        public AmazonSQS CreateAmazonSQSClient(string awsAccessKey, string awsSecretAccessKey)
        {
            return AWSClientFactory.CreateAmazonSQSClient(awsAccessKey, awsSecretAccessKey);
        }

        public AmazonIdentityManagementService CreateAmazonIdentityManagementService(string awsAccessKey, string awsSecretAccessKey)
        {
            return AWSClientFactory.CreateAmazonIdentityManagementClient(awsAccessKey, awsSecretAccessKey);
        }
    }
}
