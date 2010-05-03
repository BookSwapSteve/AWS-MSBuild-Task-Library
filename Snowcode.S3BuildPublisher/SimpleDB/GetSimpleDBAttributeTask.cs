using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SimpleDB
{
    /// <summary>
    /// MSBuild task to read an attribute from AWS SimpleDB
    /// </summary>
    public class GetSimpleDBAttributeTask : AwsTaskBase
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
        [Output]
        public string AttributeValue { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Reading Attribute {0} for Item {1} in SimpleDB Domain {2}", AttributeName, ItemName, DomainName);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                GetAttribute(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void GetAttribute(AwsClientDetails clientDetails)
        {
            using (var helper = new SimpleDBHelper(clientDetails))
            {
                AttributeValue = helper.GetAttribute(DomainName, ItemName, AttributeName);
                Log.LogMessage(MessageImportance.Normal, "Read Attribute {0} for Item {1}", AttributeName, ItemName);
            }
        }
    }
}
