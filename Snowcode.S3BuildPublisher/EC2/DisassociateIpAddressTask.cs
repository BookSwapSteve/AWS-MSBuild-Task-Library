using System;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// MSBuild task to disaccociate a AWS public IP Address from it's current instance.
    /// </summary>
    public class DisassociateIpAddressTask : AwsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the IpAddress
        /// </summary>
        [Required]
        public string IpAddress { get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Disassociating IP Address {0}", IpAddress);

            try
            {
                AwsClientDetails clientDetails = GetClientDetails();

                DisassociateIpAddress(clientDetails);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }

        private void DisassociateIpAddress(AwsClientDetails clientDetails)
        {
            using (var helper = new EC2Helper(clientDetails))
            {
                helper.DisassociateIpAddress(IpAddress);
                Log.LogMessage(MessageImportance.Normal, "Disassiociated IPAddress {0}", IpAddress);
            }
        }
    }
}
