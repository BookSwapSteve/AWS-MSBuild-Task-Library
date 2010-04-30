using Microsoft.Build.Framework;

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
        public string IpAddress{ get; set; }

        #endregion

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Disassociating IP Address {0}", IpAddress);

            AwsClientDetails clientDetails = GetClientDetails();

            DisassociateIpAddress(clientDetails);

            return true;
        }

        private void DisassociateIpAddress(AwsClientDetails clientDetails)
        {
            var helper = new EC2Helper(clientDetails);
            helper.DisassociateIpAddress(IpAddress);
            Log.LogMessage(MessageImportance.Normal, "Disassiociated IPAddress {0}", IpAddress);
        }
    }
}
