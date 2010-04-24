using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Snowcode.S3BuildPublisher
{

    public class S3BuildPublisher : Task
    {
        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.Normal, "Hello, Sourcefile={0}", SourceFile);
            var helper = new S3Helper();
            helper.DoStuff(DestinationBucket, awsAccessKeyId, awsSecretAccesskey);
            return true;
        }

        [Required]
        public string SourceFile { get; set; }
        [Required]
        public string DestinationBucket { get; set; }
        [Required]
        public string awsAccessKeyId { get; set; }
        [Required]
        public string awsSecretAccesskey { get; set; }
    }

}
