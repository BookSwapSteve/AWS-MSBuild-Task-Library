using Amazon.IdentityManagement;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.IAM
{
    public class IamTaskBase : AwsTaskBase
    {
        #region Constructors

        protected IamTaskBase() : base()
        { }

        protected IamTaskBase(IAwsClientFactory awsClientFactory, Logging.ITaskLogger logger) : base(awsClientFactory, logger)
        { }

        #endregion

        protected AmazonIdentityManagementService GetService(AwsClientDetails clientDetails)
        {
            return AwsClientFactory.CreateAmazonIdentityManagementService(clientDetails.AwsAccessKeyId,
                                                                          clientDetails.AwsSecretAccessKey);
        }
    }
}
