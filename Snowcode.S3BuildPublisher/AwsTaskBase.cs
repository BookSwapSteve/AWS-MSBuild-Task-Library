using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Snowcode.S3BuildPublisher.Client;
using Snowcode.S3BuildPublisher.Logging;

namespace Snowcode.S3BuildPublisher
{
    public abstract class AwsTaskBase : Task
    {
        private ITaskLogger _logger;

        #region Constructors

        protected AwsTaskBase()
            : this(new AwsClientFactory())
        { }

        protected AwsTaskBase(IAwsClientFactory awsClientFactory, ITaskLogger logger)
            : this(awsClientFactory)
        {
            if (logger == null) throw new ArgumentNullException("logger");

            Logger = logger;
        }

        internal AwsTaskBase(IAwsClientFactory awsClientFactory)
        {
            if (awsClientFactory == null) throw new ArgumentNullException("awsClientFactory");

            AwsClientFactory = awsClientFactory;
        }

        #endregion

        #region Common MSBuild Properties

        /// <summary>
        /// Gets or sets the container to be used when decrypting the stored credentials.
        /// </summary>
        [Required]
        public string EncryptionContainerName { get; set; }

        #endregion

        protected IAwsClientFactory AwsClientFactory { get; set; }

        public override bool Execute()
        {
            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                return Execute(clientDetails);
            }
            catch (Exception ex)
            {
                Logger.LogErrorFromException(ex);
                return false;
            }
        }

        protected virtual bool Execute(AwsClientDetails clientDetails)
        {
            // TODO: Update all the tasks to use this execute method
            // and make this abstract
            return false;
        }

        /// <summary>
        /// Dependency Injection logger to allow for testing when the 
        /// Task.Log is not set by MSBuild
        /// </summary>
        public ITaskLogger Logger
        {
            get { return _logger ?? (_logger = new MsBuildTaskLogger(Log)); }
            set { _logger = value; }
        }

        #region Helper Methods

        virtual protected AwsClientDetails GetClientDetails()
        {
            if (string.IsNullOrEmpty(EncryptionContainerName))
            {
                throw new Exception("EncryptionContainerName not set");
            }

            var clientDetailsStore = new ClientDetailsStore();
            AwsClientDetails clientDetails = clientDetailsStore.Load(EncryptionContainerName);
            Logger.LogMessage(MessageImportance.Normal, "Connecting to AWS using AwsAccessKeyId: {0}", clientDetails.AwsAccessKeyId);
            return clientDetails;
        }

        virtual protected string Join(string[] values)
        {
            return string.Join(";", values);
        }

        #endregion
    }
}
