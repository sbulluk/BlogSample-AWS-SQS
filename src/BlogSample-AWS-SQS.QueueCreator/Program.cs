using System;
using System.Collections.Generic;
using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;
using BlogSample_AWS_SQS.Shared;

namespace BlogSample_AWS_SQS.QueueCreator
{
    class Program
    {
        private static AwsConfig _awsConfig;
        private static AmazonSQSClient _client;

        static void Main(string[] args)
        {
            _awsConfig = AwsConfig.Read();
            _client = _awsConfig.CreateSQSClient();

            Console.Write("What would you like to call your queue? ");
            var queueName = Console.ReadLine().Trim();

            Console.Write("What is the ARN of your dead letter queue (Hit return for no dead letter queue)?");
            var deadLetterQueueName = Console.ReadLine().Trim();

            try
            {
                var deadLetterQueueArn = string.IsNullOrEmpty(deadLetterQueueName) ? null : LookupDeadLetterQueueArn(deadLetterQueueName);
                var newQueueUrl = CreateQueue(queueName, deadLetterQueueArn);
                Console.WriteLine("New queue url is " + newQueueUrl);
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static string LookupDeadLetterQueueArn(string queueName)
        {
            var queueUrl = GetQueueUrl(queueName);
            return GetQueueArn(queueUrl);
        }
        private static string GetQueueUrl(string queueName)
        {
            var request = new GetQueueUrlRequest(queueName);
            var response = _client.GetQueueUrl(request);
            if (response.HttpStatusCode != HttpStatusCode.OK) throw new Exception("Http error " + (int)response.HttpStatusCode);
            return response.QueueUrl;
        }
        private static string GetQueueArn(string queueUrl)
        {
            var request = new GetQueueAttributesRequest(queueUrl, new List<string> { "QueueArn" });
            var response = _client.GetQueueAttributes(request);
            return response.QueueARN;
        }
        private static string CreateQueue(string queueName, string deadLetterQueueArn)
        {
            var request = new CreateQueueRequest
                {
                    QueueName = queueName
                };
            request.Attributes.Add(QueueAttributeName.MaximumMessageSize, "128");
            if (!string.IsNullOrEmpty(deadLetterQueueArn))
            {
                request.Attributes.Add(QueueAttributeName.RedrivePolicy, "{\"maxReceiveCount\":\"3\", \"deadLetterTargetArn\":\"" + deadLetterQueueArn + "\"}");
            }
            var response = _client.CreateQueue(request);
            if (response.HttpStatusCode != HttpStatusCode.OK) throw new Exception("Http error " + (int)response.HttpStatusCode);
            return response.QueueUrl;
        }
    }
}
