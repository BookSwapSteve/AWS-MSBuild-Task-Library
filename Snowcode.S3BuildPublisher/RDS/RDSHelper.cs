using System;
using System.Collections.Generic;
using System.Diagnostics;
using Amazon;
using Amazon.RDS;
using Amazon.RDS.Model;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.RDS
{
    public class RDSHelper : IDisposable
    {
        private bool _disposed;

        #region Constructors

        public RDSHelper(string awsAccessKeyId, string awsSecretAccessKey)
        {
            Client = AWSClientFactory.CreateAmazonRDSClient(awsAccessKeyId, awsSecretAccessKey);
        }

        public RDSHelper(AwsClientDetails clientDetails)
        {
            Client = AWSClientFactory.CreateAmazonRDSClient(clientDetails.AwsAccessKeyId, clientDetails.AwsSecretAccessKey);
        }

        public RDSHelper(AmazonRDS amazonRDSClient)
        {
            Client = amazonRDSClient;
        }

        ~RDSHelper()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        protected AmazonRDS Client
        {
            get;
            set;
        }

        #endregion

        #region Public methods

        #region Database Instance

        public DBInstance CreateDatabase(CreateDBInstanceRequest request)           
        {
            CreateDBInstanceResponse response = Client.CreateDBInstance(request);

            if (response.IsSetCreateDBInstanceResult())
            {
                return response.CreateDBInstanceResult.DBInstance;
            }
            throw new Exception("Failed to get CreateDBInstanceResult response");
        }

        public void DescriveInstances()
        {
            
        }

        public void DescribeDatabase()
        {
            
        }

        public void DescribeEvents()
        {
            
        }

        public void ModifyDatabase()
        {
            
        }

        public void DeleteDatabase()
        {
            
        }

        public void RebootDatabase(string identifier)
        {
            var request = new RebootDBInstanceRequest {DBInstanceIdentifier = identifier};
            RebootDBInstanceResponse response = Client.RebootDBInstance(request);

            Trace.WriteLine(response.RebootDBInstanceResult.DBInstance.DBName);
        }

        public void Describe(string identifier)
        {
            var request = new DescribeDBInstancesRequest {DBInstanceIdentifier = identifier};
            DescribeDBInstancesResponse response = Client.DescribeDBInstances(request);
        }

        #endregion

        #region Database Parameters

        public void CreateParameterGroup()
        {
            
        }

        public void DeleteParameterGroup()
        {
            
        }

        public void DescribeParameterGroup()
        {
            
        }

        public void ModifyParameterGroup()
        {
            
        }
        #endregion

        #region Security Groups

        public void CreateSecurityGroup()
        {
            
        }

        public void DeleteSecurityGroup()
        {
            
        }

        public void AuthorizeSecurityGroup()
        {
            
        }

        public void RevokeSecurityGroup()
        {
            
        }

        public void DescriveSecurityGroup()
        {
            
        }

        #endregion

        #region SnapShots

        public void CreateSnapShot()
        {
            
        }

        public void DeleteSnapShot()
        {
            
        }

        public void RestoreFromSnapShot()
        {
            
        }

        public void DescribeSnapShot()
        {
            
        }

        #endregion

        public void RestoreToPointInTime()
        {
            
        }

        #endregion

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
