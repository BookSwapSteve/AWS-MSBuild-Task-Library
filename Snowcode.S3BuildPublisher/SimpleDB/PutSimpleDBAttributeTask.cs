using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SimpleDB
{
    /// <summary>
    /// MSBuild task to store an attribute value in SimpleDB
    /// </summary>
    public class PutSimpleDBAttributeTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets and sets the DomainName of the SimpleDB Database.
        /// </summary>
        [Required]
        public string DomainName { get; set; }

        /// <summary>
        /// Gets and sets the name of the item (record key)
        /// </summary>
        [Required]
        public string ItemName { get; set; }

        /// <summary>
        /// Gets and sets the Attribute name to store
        /// </summary>
        [Required]
        public string AttributeName { get; set; }

        /// <summary>
        /// Gets and sets the Attribute value to store.
        /// </summary>
        [Required]
        public string AttributeValue { get; set; }

        /// <summary>
        /// Gets and sets if the value should be replaced.
        /// </summary>
        public bool Replace { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Storing Attribute {0} for Item {1} in SimpleDB Domain {2}", AttributeName, ItemName, DomainName);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                PutAttribute(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void PutAttribute(AwsClientDetails clientDetails)
        {
            using (var helper = new SimpleDBHelper(clientDetails))
            {
                helper.PutAttribute(DomainName, ItemName, AttributeName, Replace, AttributeValue);
                Log.LogMessage(MessageImportance.Normal, "Stored Attribute {0} for Item {1}", AttributeName, ItemName);
            }
        }
    }
}
