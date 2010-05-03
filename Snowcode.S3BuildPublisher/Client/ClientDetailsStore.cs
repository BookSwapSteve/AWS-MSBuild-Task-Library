using System;
using Microsoft.Win32;

namespace Snowcode.S3BuildPublisher.Client
{
    /// <summary>
    /// Store AWS client details in the HKCU registry hive.
    /// </summary>
    public class ClientDetailsStore
    {
        #region Private fields

        private const string DefaultRegistryKey = "Software\\SnowCode\\S3BuildPublisher";
        private const string AccessKeyIdKeyName = "AwsAccessKeyId";
        private const string SecretAccessKeyKeyName = "AwsSecretAccessKey";

        #endregion

        #region Constructors

        public ClientDetailsStore()
            : this(DefaultRegistryKey)
        { }

        public ClientDetailsStore(string registrySubKey)
        {
            RegistrySubKey = registrySubKey;
        }

        #endregion

        protected string RegistrySubKey
        {
            get;
            set;
        }

        /// <summary>
        /// Save the Aws Client Details to the registry, encrypts the secret key in the process.
        /// </summary>
        /// <param name="containerName">Encryption container to use</param>
        /// <param name="clientDetails">Client details to store</param>
        public void Save(string containerName, AwsClientDetails clientDetails)
        {
            string encryptedKey = EncryptionHelper.Encrypt(containerName, clientDetails.AwsSecretAccessKey);

            CreateSubKeyIfNotExists();

            using (var registryKey = Registry.CurrentUser.OpenSubKey(RegistrySubKey, true))
            {
                if (registryKey == null)
                {
                    throw new ApplicationException("Failed to open registry key.");
                }

                registryKey.SetValue(AccessKeyIdKeyName, clientDetails.AwsAccessKeyId);
                registryKey.SetValue(SecretAccessKeyKeyName, encryptedKey);
            }
        }

        /// <summary>
        /// Load AWS client details from the registry.
        /// </summary>
        /// <param name="containerName">Encryption container name</param>
        /// <returns>Aws Client Details</returns>
        public AwsClientDetails Load(string containerName)
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistrySubKey))
            {
                if (registryKey == null)
                {
                    throw new ApplicationException("Registry key does not exist.");
                }

                var clientDetails = new AwsClientDetails();

                clientDetails.AwsAccessKeyId = (string)registryKey.GetValue(AccessKeyIdKeyName);
                var encrypredPassword = (string)registryKey.GetValue(SecretAccessKeyKeyName);
                clientDetails.AwsSecretAccessKey = EncryptionHelper.Decrypt(containerName, encrypredPassword);

                return clientDetails;
            }
        }

        /// <summary>
        /// Opens a writable subkey, creating it if needed.
        /// </summary>
        /// <returns></returns>
        private void CreateSubKeyIfNotExists()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistrySubKey);

            if (key == null)
            {
                Registry.CurrentUser.CreateSubKey(RegistrySubKey);
            }
        }
    }
}
