using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Build.Framework;
using Attribute = Amazon.SQS.Model.Attribute;

namespace Snowcode.S3BuildPublisher.SQS
{
    /// <summary>
    /// MSBuild Task to allow the source send message to SQS (e.g. allow SNS to send to SQS)
    /// </summary>
    /// <seealso cref="http://www.elastician.com/2010/04/subscribing-sqs-queue-to-sns-topic.html"/>
    public class GrantSendMessageRightsTask : SqsTaskBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the queue to create.
        /// </summary>
        [Required]
        public string QueueUrl { get; set; }

        /// <summary>
        /// Arn of the source (i.e. sns arn)
        /// </summary>
        [Required]
        public string SourceArn { get; set; }

        #endregion

        protected override bool Execute(AmazonSQS client)
        {
            Log.LogMessage(MessageImportance.Normal, "Granting SendMessage rights to SQS Queue at {0}", QueueUrl);

            string queueArn = GetQueueArn(client, QueueUrl);
            Log.LogMessage(MessageImportance.Low, "Queue {0} Arn: {1}", QueueUrl, queueArn);

            var request = new SetQueueAttributesRequest { QueueUrl = QueueUrl };
            var attribute = new Attribute { Name = "Policy", Value = ConstructPolicy(queueArn, SourceArn) };
            request.Attribute = new List<Attribute> { attribute };

            client.SetQueueAttributes(request);

            Logger.LogMessage(MessageImportance.Normal, "Granted rights for source {0} to SendMessage to SQS at {1}", SourceArn, QueueUrl);

            return true;
        }

        private string ConstructPolicy(string queueArn, string sourceArn)
        {
            var policy = new StringBuilder();
            policy.Append("{");
            policy.Append("\"Version\":\"2008-10-17\",");
            policy.Append("\"Id\":\"MyQueuePolicy\",");
            policy.Append("\"Statement\" : [");
            policy.Append("{");
            policy.Append("\"Sid\":\"Allow-SNS-SendMessage\",");
            policy.Append("\"Effect\":\"Allow\",");
            policy.Append("\"Principal\" : {\"AWS\": \"*\"},");
            policy.Append("\"Action\":[\"sqs:SendMessage\"],");
            policy.AppendFormat("\"Resource\": \"{0}\",", queueArn);
            policy.Append("\"Condition\" : {");
            policy.Append("\"ArnEquals\" : {");
            policy.AppendFormat("\"aws:SourceArn\":\"{0}\"", sourceArn);
            policy.Append("}");
            policy.Append("}");
            policy.Append("}");
            policy.Append("]");
            policy.Append("}");

            return policy.ToString();
        }

        private string GetQueueArn(AmazonSQS client, string queueUrl)
        {
            var request = new GetQueueAttributesRequest { QueueUrl = queueUrl, AttributeName = new List<string>(new[] { "QueueArn" }) };
            GetQueueAttributesResponse response = client.GetQueueAttributes(request);

            if (response.IsSetGetQueueAttributesResult())
            {
                return response.GetQueueAttributesResult.Attribute.Where(x => x.Name == "QueueArn").First().Value;
            }

            throw new Exception("No SetQueueAttribute result");
        }
    }
}
