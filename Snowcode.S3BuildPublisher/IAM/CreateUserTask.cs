using System;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Microsoft.Build.Framework;
using Snowcode.S3BuildPublisher.Client;
using Snowcode.S3BuildPublisher.Logging;

namespace Snowcode.S3BuildPublisher.IAM
{
    /// <summary>
    /// Create a user task
    /// </summary>
    public class CreateUserTask : IamTaskBase
    {
        #region Constructors

        public CreateUserTask()
            : base()
        { }

        public CreateUserTask(IAwsClientFactory awsClientFactory, ITaskLogger logger)
            : base(awsClientFactory, logger)
        { }

        #endregion

        #region MSBuild Properties

        /// <summary>
        /// UserName for the user to create.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Path for the user.
        /// </summary>
        /// <value>
        /// Path for the user. If not specified then defaults to /
        /// </value>
        /// <seealso cref="http://docs.amazonwebservices.com/IAM/latest/UserGuide/index.html?Using_Identifiers.html"/>
        public string Path { get; set; }

        [Output]
        public string Arn { get; set; }

        [Output]
        public string UserId { get; set; }

        #endregion

        #region Execute Methods

        protected override bool Execute(AwsClientDetails clientDetails)
        {
            Logger.LogMessage(MessageImportance.Normal, "Creating IAM User {0}", UserName);

            using (AmazonIdentityManagementService service = GetService(clientDetails))
            {
                var request = new CreateUserRequest { UserName = UserName, Path = Path };
                CreateUserResponse response = service.CreateUser(request);

                if (response.CreateUserResult.User != null)
                {
                    Arn = response.CreateUserResult.User.Arn;
                    UserId = response.CreateUserResult.User.UserId;
                    Logger.LogMessage(MessageImportance.Normal, "Created User with Arn: {0}", Arn);
                    return true;
                }

                Logger.LogMessage(MessageImportance.Normal, "Failed to create User {0}", UserName);
                return false;
            }
        }

        #endregion
    }
}
