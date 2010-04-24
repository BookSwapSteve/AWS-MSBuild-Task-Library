using System;
using System.Security.Cryptography;
using System.Text;

namespace Snowcode.S3BuildPublisher
{
    public class EncryptionHelper
    {
        /// <summary>
        /// Encrypts the value passed in.
        /// </summary>
        /// <param name="containerName">the container</param>
        /// <param name="toEncrypt">The clear value to encrypt</param>
        /// <returns>returns a base 64 encoded string</returns>
        public static string Encrypt(string containerName, string toEncrypt)
        {
            byte[] dataToEncrypt = Encoding.ASCII.GetBytes(toEncrypt);

            var parameters = new CspParameters {KeyContainerName = containerName};

            // Create a new instance of the RSACryptoServiceProvider class 
            var cryptoServiceProvider = new RSACryptoServiceProvider(parameters);

            // Encrypt the byte array and specify no OAEP padding.  
            // OAEP padding is only available on Microsoft Windows XP or later.  
            byte[] encryptedData = cryptoServiceProvider.Encrypt(dataToEncrypt, false);

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Decrypts the value passed in.
        /// </summary>
        /// <param name="containerName">The container name storing the </param>
        /// <param name="toDecrypt">The encrypted value to decrypt.  Base64 encoded</param>
        /// <returns></returns>
        public static string Decrypt(string containerName, string toDecrypt)
        {
            var parameters = new CspParameters {KeyContainerName = containerName};
            var cryptoServiceProvider = new RSACryptoServiceProvider(parameters);

            // Pass the data to ENCRYPT and boolean flag specifying no OAEP padding.
            byte[] decryptedData = cryptoServiceProvider.Decrypt(Convert.FromBase64String(toDecrypt), false);

            return Encoding.ASCII.GetString(decryptedData);
        }
    }
}
