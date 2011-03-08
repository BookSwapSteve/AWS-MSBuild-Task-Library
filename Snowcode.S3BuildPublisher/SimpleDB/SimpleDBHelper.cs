using System;
using System.Collections.Generic;
using System.Linq;
using Amazon;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Snowcode.S3BuildPublisher.Client;
using Attribute = Amazon.SimpleDB.Model.Attribute;

namespace Snowcode.S3BuildPublisher.SimpleDB
{
    /// <summary>
    /// AWS SimpleDB helper class.
    /// </summary>
    public class SimpleDBHelper : IDisposable
    {
        private bool _disposed;

        #region Constructors

        public SimpleDBHelper(string awsAccessKeyId, string awsSecretAccessKey)
        {
            Client = AWSClientFactory.CreateAmazonSimpleDBClient(awsAccessKeyId, awsSecretAccessKey);
        }

        public SimpleDBHelper(AwsClientDetails clientDetails)
        {
            Client = AWSClientFactory.CreateAmazonSimpleDBClient(clientDetails.AwsAccessKeyId, clientDetails.AwsSecretAccessKey);
        }

        public SimpleDBHelper(AmazonSimpleDB amazonSimpleDBClient)
        {
            Client = amazonSimpleDBClient;
        }

        ~SimpleDBHelper()
        {
            Dispose(false);
        }

        #endregion

        protected AmazonSimpleDB Client
        {
            get;
            set;
        }

        /// <summary>
        /// Creates the SimpleDB Domain
        /// </summary>
        /// <param name="domainName"></param>
        public void CreateDomain(string domainName)
        {
            var request = new CreateDomainRequest { DomainName = domainName };

            Client.CreateDomain(request);
        }

        /// <summary>
        /// Deletes the simpleDB Domain
        /// </summary>
        /// <param name="domainName"></param>
        public void DeleteDomain(string domainName)
        {
            var request = new DeleteDomainRequest { DomainName = domainName };

            Client.DeleteDomain(request);
        }

        /// <summary>
        /// Puts attribiutes in SimpleDB
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="itemName"></param>
        /// <param name="name"></param>
        /// <param name="replace"></param>
        /// <param name="value"></param>
        public void PutAttribute(string domainName, string itemName, string name, bool replace, string value)
        {
            var replaceableAttribute = new ReplaceableAttribute
                                           {
                                               Name = name,
                                               Replace = replace,
                                               Value = value
                                           };

            var request = new PutAttributesRequest
                              {
                                  DomainName = domainName,
                                  ItemName = itemName,
                                  Attribute = new List<ReplaceableAttribute>() { replaceableAttribute }
                              };

            Client.PutAttributes(request);
        }

        /// <summary>
        /// Get a single attribute back from the item.
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="itemName"></param>
        /// <param name="name"></param>
        /// <returns>Returns the value of the attribute if it exists, otherwise an empty string.</returns>
        /// <remarks>Can't do multiple as no guarantee as to order.</remarks>
        public string GetAttribute(string domainName, string itemName, string name)
        {
            var request = new GetAttributesRequest
                              {
                                  DomainName = domainName,
                                  ItemName = itemName,
                                  AttributeName = new List<string> { name }
                              };

            GetAttributesResponse response = Client.GetAttributes(request);

            if (response.IsSetGetAttributesResult())
            {
                if (response.GetAttributesResult.Attribute.Count > 0)
                {
                    return response.GetAttributesResult.Attribute[0].Value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Deletes the attributes.
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="itemName"></param>
        /// <param name="names"></param>
        public void DeleteAttributes(string domainName, string itemName, IEnumerable<string> names)
        {
            List<Attribute> attributes = names.Select(name => new Attribute {Name = name}).ToList();
            var request = new DeleteAttributesRequest
                              {
                                  DomainName = domainName,
                                  ItemName = itemName,
                                  Attribute = attributes
                              };

            Client.DeleteAttributes(request);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        virtual protected void Dispose(bool disposing)
        {
            if (_disposed)
            {
                if (!disposing)
                {
                    try
                    {
                        if (Client != null)
                        {
                            Client.Dispose();
                        }
                    }
                    finally
                    {
                        _disposed = true;
                    }
                }
            }
        }

        #endregion
    }
}
