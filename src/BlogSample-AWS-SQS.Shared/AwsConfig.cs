using Amazon.SQS;
using Newtonsoft.Json;

namespace BlogSample_AWS_SQS.Shared
{
    public class AwsConfig
    {
        private AwsConfig()
        {

        }

        public string AccessKeyId { get; set; }
        public string SecretKey { get; set; }
        public string QueueUrl { get; set; }

        public static AwsConfig Read()
        {
            var json = System.IO.File.ReadAllText("awsconfig.json");
            return JsonConvert.DeserializeObject<AwsConfig>(json);
        }

        public AmazonSQSClient CreateSQSClient()
        {
            return new AmazonSQSClient(
                awsAccessKeyId: AccessKeyId,
                awsSecretAccessKey: SecretKey,
                region: Amazon.RegionEndpoint.EUWest1);
        }

    }
}
