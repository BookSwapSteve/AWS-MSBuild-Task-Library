using Amazon.IdentityManagement;
using Amazon.SQS;

namespace Snowcode.S3BuildPublisher
{
    public interface IAwsClientFactory
    {
        AmazonSQS CreateAmazonSQSClient(string awsAccessKey, string awsSecretAccessKey);
        AmazonIdentityManagementService CreateAmazonIdentityManagementService(string awsAccessKey, string awsSecretAccessKey);
    }
}
