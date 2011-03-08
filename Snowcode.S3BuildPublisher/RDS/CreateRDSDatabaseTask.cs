using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.RDS
{
    /// <summary>
    /// Create a database instance
    /// </summary>
    public class CreateRDSDatabaseTask : AwsTaskBase
    {
        #region Properties

        [Required]
        public decimal AllocatedStorage { get; set; }

        [Required]
        public string AvailabilityZone { get; set; }

        [Required]
        public string DatabaseInstanceClass { get; set; }

        [Required]
        public string DatabaseInstanceIdentifier { get; set; }

        [Required]
        public string DatabaseName { get; set; }

        [Required]
        public string DatabaseParameterGroupName { get; set; }

        [Required]
        public string[] DatabaseSecurityGroups { get; set; }

        [Required]
        public string Engine { get; set; }

        [Required]
        public string MasterUsername { get; set; }

        [Required]
        public string MasterUserPassword { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        public string PreferredBackupWindow { get; set; }

        [Required]
        public string PreferredMaintenanceWindow { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Creating RDS Datasase {0} ", DatabaseName);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                CreateRdsDatabase(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void CreateRdsDatabase(AwsClientDetails clientDetails)
        {
            using (var helper = new RDSHelper(clientDetails))
            {
                throw new NotImplementedException("");
            }

        }
    }
}
