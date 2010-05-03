using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Snowcode.S3BuildPublisher.Client
{
    /// <summary>
    /// MSBuid task responsible for storing Amazon AWS client connection details.
    /// </summary>
    public class StoreClientDetailsTask : Task
    {
        [Required]
        public string AwsAccessKeyId { get; set; }

        [Required]
        public string AwsSecretAccessKey { get; set; }

        [Required]
        public string EncryptionContainerName { get; set; }

        #region Overrides of Task

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Storing AWS Client details");
            try
            {
                var clientDetails = new AwsClientDetails
                                        {
                                            AwsAccessKeyId = AwsAccessKeyId,
                                            AwsSecretAccessKey = AwsSecretAccessKey
                                        };

                // TODO: Allow for dependency injection to facilitate testing.
                var store = new ClientDetailsStore();
                store.Save(EncryptionContainerName, clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        #endregion
    }
}
