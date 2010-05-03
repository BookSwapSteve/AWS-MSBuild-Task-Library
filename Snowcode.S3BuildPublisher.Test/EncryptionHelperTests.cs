using NUnit.Framework;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.Test
{
    [TestFixture]
    public class EncryptionHelperTests
    {
        [Test]
        public void EncryptAndDecrpytPassword_ReturnsSamePassword()
        {
            const string containerName = "S3BuildPublisher.TestContainer.EncryptionHelper";
            const string password = "someRandomText1234567890-Passwor!d$$";

            string encrypted = EncryptionHelper.Encrypt(containerName, password);
            string actual = EncryptionHelper.Decrypt(containerName, encrypted);

            Assert.AreEqual(password, actual);
        }
    }
}
