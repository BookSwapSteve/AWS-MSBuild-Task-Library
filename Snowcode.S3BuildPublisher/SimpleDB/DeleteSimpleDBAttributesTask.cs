using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.SimpleDB
{
    /// <summary>
    /// MSBuild task to deletes attributes from an item in SimpleDB
    /// </summary>
    /// <remarks>Note that this is plural and takes a list of attribute names to delete.</remarks>
    public class DeleteSimpleDBAttributesTask : AwsTaskBase
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
        /// Gets and sets the Attribute names to delete
        /// </summary>
        [Required]
        public string[] AttributeNames { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Deleting Attributes {0} for Item {1} in SimpleDB Domain {2}",
                                                        Join(AttributeNames),
                                                        ItemName,
                                                        DomainName);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                DeleteAttributes(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void DeleteAttributes(AwsClientDetails clientDetails)
        {
            using (var helper = new SimpleDBHelper(clientDetails))
            {
                helper.DeleteAttributes(DomainName, ItemName, AttributeNames);
                Log.LogMessage(MessageImportance.Normal, "Deleted Attributes {0} for Item {1}", Join(AttributeNames), ItemName);
            }
        }
    }
}
