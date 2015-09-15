using System;
using System.Windows.Forms;
using Amazon.SQS;
using Amazon.SQS.Model;
using BlogSample_AWS_SQS.Shared;
using Newtonsoft.Json;

namespace BlogSample_AWS_SQS.Sender
{
    public partial class Form1 : Form
    {
        private readonly AwsConfig _awsConfig;

        public Form1()
        {
            InitializeComponent();

            _awsConfig = AwsConfig.Read();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var message = SerializeEmail();
            var request = new SendMessageRequest(
                queueUrl: _awsConfig.QueueUrl,
                messageBody: message);

            var client = _awsConfig.CreateSQSClient();
            var response = client.SendMessage(request);

            MessageBox.Show("Sent message. Id: " + response.MessageId);
        }

        private string SerializeEmail()
        {
            var email = new EmailMessage
            {
                From = txtFrom.Text,
                To = txtTo.Text,
                Subject = txtSubject.Text,
                Body = txtBody.Text
            };
            return JsonConvert.SerializeObject(email);
        }
    }
}
