using System;
using NUnit.Framework;
using Snowcode.S3BuildPublisher.Client;
using Snowcode.S3BuildPublisher.SimpleDB;

namespace Snowcode.S3BuildPublisher.Test.SimpleDB
{
    [TestFixture]
    [Category("IntegrationTest")]
    public class SimpleDBHelperTest
    {
        private const string Container = "MySecretContainer";

        [Test]
        public void CreateDomain_Should_Succeed()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SimpleDBHelper helper = new SimpleDBHelper(clientDetails);

            const string domainName = "TestDomain";
            helper.CreateDomain(domainName);
        }

        [Test]
        public void CreateDomainMultipleTimes_Should_Succeed()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SimpleDBHelper helper = new SimpleDBHelper(clientDetails);

            const string domainName = "TestDomain";
            helper.CreateDomain(domainName);
            helper.CreateDomain(domainName);
        }

        [Test]
        public void DeleteDomain_Should_Succeed()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SimpleDBHelper helper = new SimpleDBHelper(clientDetails);

            const string domainName = "TestDomain";
            helper.CreateDomain(domainName);

            helper.DeleteDomain(domainName);
        }

        [Test]
        public void PutAttrubute_Should_Succeed()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SimpleDBHelper helper = new SimpleDBHelper(clientDetails);

            const string domainName = "TestDomain";
            const string itemName = "TesItem";
            const string name = "attributeName";
            const bool replace = true;
            string value = DateTime.Now.ToLongTimeString();

            helper.PutAttribute(domainName, itemName, name, replace, value);
        }

        [Test]
        public void GetAttribute_Should_Succeed()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SimpleDBHelper helper = new SimpleDBHelper(clientDetails);

            const string domainName = "TestDomain";
            const string itemName = "TesItem";
            const string attributeName = "attributeName";
            string expectedValue = DateTime.Now.ToLongTimeString();

            // Ensure the domain exists and store the test item.
            helper.CreateDomain(domainName);
            helper.PutAttribute(domainName, itemName, attributeName, true, expectedValue);

            string actualValue = helper.GetAttribute(domainName, itemName, attributeName);

            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void GetUnknownAttribute_Should_ReturnEmptyString()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SimpleDBHelper helper = new SimpleDBHelper(clientDetails);

            const string domainName = "TestDomain";
            const string itemName = "TesItem";
            const string attributeName = "UnknownAttributeThatDoesntExist";
            string expectedValue = string.Empty;

            // Ensure the domain exists and store the test item.
            helper.CreateDomain(domainName);

            string actualValue = helper.GetAttribute(domainName, itemName, attributeName);

            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void DeleteAttributes_Should_Succeed()
        {
            var store = new ClientDetailsStore();
            AwsClientDetails clientDetails = store.Load(Container);

            SimpleDBHelper helper = new SimpleDBHelper(clientDetails);

            const string domainName = "TestDomain";
            const string itemName = "TesItem";
            const string attributeName = "attributeName";
            string expectedValue = DateTime.Now.ToLongTimeString();

            // Ensure the domain exists and store the test item.
            helper.CreateDomain(domainName);
            helper.PutAttribute(domainName, itemName, attributeName, true, expectedValue);

            helper.DeleteAttributes(domainName, itemName, new [] { attributeName });
        }
    }
}
