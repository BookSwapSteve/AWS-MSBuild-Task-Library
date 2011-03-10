using Amazon.IdentityManagement;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.IAM
{
    public abstract class IamTaskBase : AwsTaskBase
    {
        #region Constructors

        protected IamTaskBase() : base()
        { }

        protected IamTaskBase(IAwsClientFactory awsClientFactory, Logging.ITaskLogger logger) : base(awsClientFactory, logger)
        { }

        #endregion

        protected override bool Execute(AwsClientDetails clientDetails)
        {
            using (AmazonIdentityManagementService client = GetService(clientDetails))
            {
                return Execute(client);
            }
        }

        protected abstract bool Execute(AmazonIdentityManagementService service);

        protected AmazonIdentityManagementService GetService(AwsClientDetails clientDetails)
        {
            return AwsClientFactory.CreateAmazonIdentityManagementService(clientDetails.AwsAccessKeyId,
                                                                          clientDetails.AwsSecretAccessKey);
        }
    }
}
