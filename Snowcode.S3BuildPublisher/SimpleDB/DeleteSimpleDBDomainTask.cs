using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SimpleDB
{
    /// <summary>
    /// MSBuild task to delete an AWS SimpleDB Domain
    /// </summary>
    public class DeleteSimpleDBDomainTask : AwsTaskBase
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
            Log.LogMessage(MessageImportance.Normal, "Deleting SimpleDB Domain {0}", DomainName);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                DeleteDomain(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void DeleteDomain(AwsClientDetails clientDetails)
        {
            using (var helper = new SimpleDBHelper(clientDetails))
            {
                helper.DeleteDomain(DomainName);
                Log.LogMessage(MessageImportance.High, "Deleted SimpleDB Domain {0}", DomainName);
            }
        }
    }
}
