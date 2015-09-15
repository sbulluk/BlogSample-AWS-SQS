using System;
using System.Threading;
using Amazon.SQS;
using Amazon.SQS.Model;
using BlogSample_AWS_SQS.Shared;
using Newtonsoft.Json;

namespace BlogSample_AWS_SQS.Receiver
{
    class Program
    {
        private static AwsConfig _awsConfig;
        private static AmazonSQSClient _client;

        static void Main()
        {
            _awsConfig = AwsConfig.Read();
            _client = _awsConfig.CreateSQSClient();

            Console.WriteLine("Monitoring....");
            Console.WriteLine("Press Ctrl+C to stop");

            while (true)
            {
                Console.WriteLine("Checking");
                var receivedMessage = Receive();
                if (receivedMessage != null)
                {
                    Console.WriteLine("Sending email");
                    Console.WriteLine("    From:    " + receivedMessage.Email.From);
                    Console.WriteLine("    To:      " + receivedMessage.Email.To);
                    Console.WriteLine("    Subject: " + receivedMessage.Email.Subject);

                    //Send email

                    Console.WriteLine("Email sent");

                    DeleteMessageFromQueue(receivedMessage.ReceiptHandle);
                    Console.WriteLine("Removed from queue");
                }
                Thread.Sleep(1000);
            }
        }

        private static ReceivedMessage Receive()
        {
            var request = new ReceiveMessageRequest
                {
                    QueueUrl = _awsConfig.QueueUrl,
                    MaxNumberOfMessages = 1
                };
            var response = _client.ReceiveMessage(request);

            if (response.Messages.Count == 0) return null;
            var message = response.Messages[0];
            var json = message.Body;
            return new ReceivedMessage
                {
                    Email = JsonConvert.DeserializeObject<EmailMessage>(json),
                    ReceiptHandle = message.ReceiptHandle
                };
        }
        private static void DeleteMessageFromQueue(string receiptHandle)
        {
            var request = new DeleteMessageRequest(
                queueUrl: _awsConfig.QueueUrl,
                receiptHandle: receiptHandle);
            var response = _client.DeleteMessage(request);
        }
    }
}
