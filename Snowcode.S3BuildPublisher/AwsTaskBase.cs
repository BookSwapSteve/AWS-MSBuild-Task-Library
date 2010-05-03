using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher
{
    public abstract class AwsTaskBase : Task
    {
        /// <summary>
        /// Gets or sets the container to be used when decrypting the stored credentials.
        /// </summary>
        [Required]
        public string EncryptionContainerName { get; set; }

        virtual protected AwsClientDetails GetClientDetails()
        {
            var clientDetailsStore = new ClientDetailsStore();
            AwsClientDetails clientDetails = clientDetailsStore.Load(EncryptionContainerName);
            Log.LogMessage(MessageImportance.Normal, "Connecting to AWS using AwsAccessKeyId: {0}", clientDetails.AwsAccessKeyId);
            return clientDetails;
        }

        virtual protected string Join(string[] values)
        {
            return string.Join(";", values);
        }
    }
}
