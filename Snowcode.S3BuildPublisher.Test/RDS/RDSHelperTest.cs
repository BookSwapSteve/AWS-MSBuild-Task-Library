using NUnit.Framework;
using Snowcode.S3BuildPublisher.RDS;

namespace Snowcode.S3BuildPublisher.Test.RDS
{
    [TestFixture]
    public class RDSHelperTest
    {
        private const string AwsAccessKey = "***";
        private const string AwsSecretAccessKey = "***";
        private const string TestDatabaseName = "***";

        [Test]
        [Ignore("Not using correct AWS Keys")]
        public void TestReboot()
        {
            var helper = new RDSHelper(AwsAccessKey, AwsSecretAccessKey);
            helper.RebootDatabase(TestDatabaseName);
        }

        [Test]
        [Ignore("Not using correct AWS Keys")]
        public void TestDescribe()
        {
            var helper = new RDSHelper(AwsAccessKey, AwsSecretAccessKey);
            helper.Describe(TestDatabaseName);
        }
    }
}
