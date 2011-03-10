using Amazon.SQS;
using Snowcode.S3BuildPublisher.Client;


namespace Snowcode.S3BuildPublisher.SQS
{
    public abstract class SqsTaskBase : AwsTaskBase
    {
        #region Constructors

        protected SqsTaskBase()
            : base()
        { }

        protected SqsTaskBase(IAwsClientFactory awsClientFactory, Logging.ITaskLogger logger)
            : base(awsClientFactory, logger)
        { }

        #endregion

        protected AmazonSQS GetClient(AwsClientDetails clientDetails)
        {
            return AwsClientFactory.CreateAmazonSQSClient(clientDetails.AwsAccessKeyId, clientDetails.AwsSecretAccessKey);
        }

        protected override bool Execute(AwsClientDetails clientDetails)
        {
            using (AmazonSQS client = GetClient(clientDetails))
            {
                return Execute(client);
            }
        }

        protected abstract bool Execute(AmazonSQS client);
    }
}
