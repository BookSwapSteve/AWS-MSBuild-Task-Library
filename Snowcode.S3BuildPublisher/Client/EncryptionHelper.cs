using System;
using System.Security.Cryptography;
using System.Text;

namespace Snowcode.S3BuildPublisher.Client
{
    public class EncryptionHelper
    {
        /// <summary>
        /// Encrypts the value passed in.
        /// </summary>
        /// <param name="containerName">the container</param>
        /// <param name="toEncrypt">The clear value to encrypt</param>
        /// <returns>returns a base 64 encoded string</returns>
        public static string Encrypt(string password, string toEncrypt)
        {
            byte[] dataToEncrypt = Encoding.ASCII.GetBytes(toEncrypt);

            var parameters = new CspParameters { KeyContainerName = SaltedPassword(password) };

            // Create a new instance of the RSACryptoServiceProvider class 
            var cryptoServiceProvider = new RSACryptoServiceProvider(parameters);

            // Encrypt the byte array and specify no OAEP padding.  
            // OAEP padding is only available on Microsoft Windows XP or later.  
            byte[] encryptedData = cryptoServiceProvider.Encrypt(dataToEncrypt, false);

            return Convert.ToBase64String(encryptedData);
        }

        private static string SaltedPassword(string password)
        {
            // ADH: 2010.05.01 
            // since this is our own private server, and any "attacker" won't have access to the encrypt method
            // this is sufficient we don't need to HASH the password, and don't need a new salt for each value we will 
            // be encrypting.
            return string.Format("{0}{1}", "91A827BB-8D82-490C-80BB-198F080B8C31", password);
        }


        /// <summary>
        /// Decrypts the value passed in.
        /// </summary>
        /// <param name="containerName">The container name storing the </param>
        /// <param name="toDecrypt">The encrypted value to decrypt.  Base64 encoded</param>
        /// <returns></returns>
        public static string Decrypt(string password, string toDecrypt)
        {
            var parameters = new CspParameters { KeyContainerName = SaltedPassword(password) };
            var cryptoServiceProvider = new RSACryptoServiceProvider(parameters);

            // Pass the data to ENCRYPT and boolean flag specifying no OAEP padding.
            byte[] decryptedData = cryptoServiceProvider.Decrypt(Convert.FromBase64String(toDecrypt), false);

            return Encoding.ASCII.GetString(decryptedData);
        }
    }
}
