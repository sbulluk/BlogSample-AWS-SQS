using BlogSample_AWS_SQS.Shared;

namespace BlogSample_AWS_SQS.Receiver
{
    public class ReceivedMessage
    {
        public EmailMessage Email { get; set; }
        public string ReceiptHandle { get; set; }
    }
}
