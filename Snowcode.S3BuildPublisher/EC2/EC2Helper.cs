using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Snowcode.S3BuildPublisher.Client;

namespace Snowcode.S3BuildPublisher.EC2
{
    /// <summary>
    /// Helper class to control AWS EC2 instances.
    /// </summary>
    public class EC2Helper : IDisposable
    {
        private bool _disposed;

        #region Constructors

        public EC2Helper(string awsAccessKeyId, string awsSecretAccessKey)
        {
            Client = AWSClientFactory.CreateAmazonEC2Client(awsAccessKeyId, awsSecretAccessKey);
        }

        public EC2Helper(AwsClientDetails clientDetails)
        {
            Client = AWSClientFactory.CreateAmazonEC2Client(clientDetails.AwsAccessKeyId, clientDetails.AwsSecretAccessKey);
        }

        ~EC2Helper()
        {
            Dispose(false);
        }

        #endregion

        protected AmazonEC2 Client
        {
            get;
            set;
        }

        #region EBS based EC2 instance handling

        /// <summary>
        /// Start's instances - these should be EBS block storage instances.
        /// </summary>
        /// <param name="instanceIds">The instance Id of an EC2 instance</param>
        /// <remarks>This uses EBS storage EC2 instances which can be stopped and started.  The instance should be stopped.</remarks>
        public void StartInstances(IEnumerable<string> instanceIds)
        {
            var request = new StartInstancesRequest { InstanceId = new List<string>(instanceIds) };
            StartInstancesResponse resonse = Client.StartInstances(request);

            if (resonse.IsSetStartInstancesResult())
            {
                foreach (InstanceStateChange instanceStateChange in resonse.StartInstancesResult.StartingInstances)
                {
                    Trace.WriteLine(string.Format("Starting instance {0}", instanceStateChange.InstanceId));
                }
            }
        }

        /// <summary>
        /// Stop Amazon EC2 instances.
        /// </summary>
        /// <param name="instances"></param>
        public void StopInstances(string[] instances)
        {
            var request = new StopInstancesRequest { InstanceId = new List<string>(instances) };
            StopInstancesResponse response = Client.StopInstances(request);

            if (response.IsSetStopInstancesResult())
            {
                foreach (InstanceStateChange instanceStateChange in response.StopInstancesResult.StoppingInstances)
                {
                    Trace.WriteLine(string.Format("Stopping instance {0}", instanceStateChange.InstanceId));
                }
            }
        }

        #endregion

        #region AMI based instance handling

        /// <summary>
        /// Creates (Runs) a new EC2 instance from the stored AMI image.
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="numberOfInstances"></param>
        /// <param name="keyName"></param>
        /// <param name="userData"></param>
        /// <param name="securityGroups"></param>
        /// <returns></returns>
        public List<string> RunInstance(string imageId, int numberOfInstances, string keyName, string userData, string[] securityGroups)
        {
            var request = new RunInstancesRequest
                              {
                                  ImageId = imageId,
                                  MinCount = numberOfInstances,
                                  MaxCount = numberOfInstances,
                                  KeyName = keyName,
                                  UserData = userData,
                                  SecurityGroup = new List<string>(securityGroups)
                              };

            RunInstancesResponse response = Client.RunInstances(request);

            return response.RunInstancesResult.Reservation.RunningInstance.Select(runningInstance => runningInstance.InstanceId).ToList();
        }

        /// <summary>
        /// Terminates an EC2 instance.
        /// </summary>
        /// <param name="instanceIds"></param>
        public void TerminateInstance(IEnumerable<string> instanceIds)
        {
            var request = new TerminateInstancesRequest { InstanceId = new List<string>(instanceIds) };

            Client.TerminateInstances(request);
        }

        /// <summary>
        /// Reboots an EC2 instance
        /// </summary>
        /// <param name="instanceIds"></param>
        public void RebootInstance(IEnumerable<string> instanceIds)
        {
            var request = new RebootInstancesRequest { InstanceId = new List<string>(instanceIds) };

            Client.RebootInstances(request);
        }

        #endregion

        #region IP Address handling

        /// <summary>
        /// Associate a public IP Address with an EC2 instance
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="publicIpAddress"></param>
        public void AssociateIpAddress(string instanceId, string publicIpAddress)
        {
            var request = new AssociateAddressRequest { InstanceId = instanceId, PublicIp = publicIpAddress };
            Client.AssociateAddress(request);
        }

        /// <summary>
        /// Disassociate a public IP Address from it's current EC2 instance
        /// </summary>
        /// <param name="publicIpAddress"></param>
        public void DisassociateIpAddress(string publicIpAddress)
        {
            var request = new DisassociateAddressRequest { PublicIp = publicIpAddress };
            Client.DisassociateAddress(request);
        }

        #endregion

        #region Volume handling

        /// <summary>
        /// Creates a new volume
        /// </summary>
        /// <param name="avilabilityZone">The Availability zone to create the volume in</param>
        /// <param name="size"></param>
        /// <returns>Returns the VolumeId of the newly created volume</returns>
        public string CreateNewVolume(string availabilityZone, string size)
        {
            var request = new CreateVolumeRequest { AvailabilityZone = availabilityZone, Size = size };

            CreateVolumeResponse response = Client.CreateVolume(request);

            return response.CreateVolumeResult.Volume.VolumeId;
        }

        /// <summary>
        /// Create a snapshot of a volume.
        /// </summary>
        /// <param name="volumeId"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public string CreateSnapShot(string volumeId, string description)
        {
            var request = new CreateSnapshotRequest { VolumeId = volumeId, Description = description };

            CreateSnapshotResponse response = Client.CreateSnapshot(request);

            return response.CreateSnapshotResult.Snapshot.SnapshotId;
        }

        /// <summary>
        /// Create a volume from a snapshot
        /// </summary>
        /// <param name="avilabilityZone">The Availability zone to create the volume in</param>
        /// <param name="snapshotId">The SnapShot to create the volume from</param>
        /// <returns>Returns the VolumeId of the newly created volume</returns>
        public string CreateVolumeFromSnapshot(string avilabilityZone, string snapshotId)
        {
            CreateVolumeRequest request = new CreateVolumeRequest();
            request.AvailabilityZone = avilabilityZone;
            request.SnapshotId = snapshotId;

            CreateVolumeResponse response = Client.CreateVolume(request);

            return response.CreateVolumeResult.Volume.VolumeId;
        }

        /// <summary>
        /// Deletes a volume
        /// </summary>
        /// <param name="volumeId"></param>
        public void DeleteVolume(string volumeId)
        {
            var request = new DeleteVolumeRequest { VolumeId = volumeId };

            Client.DeleteVolume(request);
        }

        /// <summary>
        /// Deletes a snapshot
        /// </summary>
        /// <param name="snapShotId"></param>
        public void DeleteSnapShot(string snapShotId)
        {
            var request = new DeleteSnapshotRequest { SnapshotId = snapShotId };

            Client.DeleteSnapshot(request);
        }

        /// <summary>
        /// Attaches a volume to a EC2 instance.
        /// </summary>
        /// <param name="device">xvdf through xvdp</param>
        /// <param name="instanceId"></param>
        /// <param name="volumeId"></param>
        public void AttachVolume(string device, string instanceId, string volumeId)
        {
            var request = new AttachVolumeRequest {Device = device, InstanceId = instanceId, VolumeId = volumeId};

            Client.AttachVolume(request);
        }

        /// <summary>
        /// Detatches a volume from an EC2 instance.
        /// </summary>
        /// <param name="device">xvdf through xvdp</param>
        /// <param name="volumeId"></param>
        /// <param name="instanceId"></param>
        /// <param name="force"></param>
        public void DetachVolume(string device, string instanceId, string volumeId, bool force)
        {
            var request = new DetachVolumeRequest
                                {
                                    Device = device,
                                    InstanceId = instanceId,
                                    VolumeId = volumeId,
                                    Force = force
                                };

            Client.DetachVolume(request);
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
