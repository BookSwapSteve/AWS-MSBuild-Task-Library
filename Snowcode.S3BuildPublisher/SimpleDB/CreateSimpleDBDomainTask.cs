using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SimpleDB
{
    /// <summary>
    /// MSBuild task to create an AWS SimpleDB Domain
    /// </summary>
    public class CreateSimpleDBDomainTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the DomainName of the SimpleDB Database.
        /// </summary>
        [Required]
        public string DomainName { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Creating SimpleDB Domain {0}", DomainName);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                CreateDomain(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void CreateDomain(AwsClientDetails clientDetails)
        {
            using (var helper = new SimpleDBHelper(clientDetails))
            {
                helper.CreateDomain(DomainName);
                Log.LogMessage(MessageImportance.Normal, "Created SimpleDB Domain {0}", DomainName);
            }
        }
    }
}
