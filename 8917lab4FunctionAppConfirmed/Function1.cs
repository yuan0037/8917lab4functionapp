using System;
using Azure.Communication.Email;
using Azure;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace _8917lab4FunctionAppConfirmed
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public void Run([ServiceBusTrigger("booking-status", "confirmed", 
            Connection = "MyConnection"
            )] ServiceBusReceivedMessage message)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                if (message.ApplicationProperties.ContainsKey("label"))
                {

                    if (message.ApplicationProperties.GetValueOrDefault("label").ToString() == "confirmed")
                    {
                        SendEmail(["confirmed"]);
                    }
                    else
                    {
                        if (message.ApplicationProperties.GetValueOrDefault("label").ToString() == "confirmed")
                        {
                            SendEmail(["rejected"]);
                        }
                        else
                        {
                            SendEmail(["unknow status"]);
                        }
                    }
                } else
                {
                    SendEmail(["missing status label"]);
                }
            }
            catch (ArgumentNullException ex)
            {
                // handle the argumentnullexception here
                Console.WriteLine("argumentnullexception occurred: " + ex.Message);
            }
        }

        static void SendEmail(string[] args)
        {
            // This code demonstrates how to send email using Azure Communication Services.
            var connectionString = "endpoint=https://yuan0037communicationservicelab4.canada.communication.azure.com/;accesskey=cg9kfyCSvKqYxoNsl731owHTi79JkhwT0mdwfUpob0Xz/MTKb9qu9/jRjEazdQ6QmRqOYd+Gnw72LNBlJCWjAQ==";
            var emailClient = new EmailClient(connectionString);

            var sender = "DoNotReply@1f805881-bf05-4e2b-8372-2f600b7990a3.azurecomm.net";
            var recipient = "bobyuan@gmail.com";
            var subject = "Order Status " + args[0];
            var htmlContent = "<html><body><h1>Order Status " + args[0] + " </h1><br/><h4>Your order is "+ args[0] + "</h4><p>Happy Shopping!!</p></body></html>";

            try
            {
                var emailSendOperation = emailClient.Send(
                    wait: WaitUntil.Completed,
                    senderAddress: sender, // The email address of the domain registered with the Communication Services resource
                    recipientAddress: recipient,
                    subject: subject,
                    htmlContent: htmlContent);
                Console.WriteLine($"Email Sent. Status = {emailSendOperation.Value.Status}");

                /// Get the OperationId so that it can be used for tracking the message for troubleshooting
                string operationId = emailSendOperation.Id;
                Console.WriteLine($"Email operation id = {operationId}");
            }
            catch (RequestFailedException ex)
            {
                /// OperationID is contained in the exception message and can be used for troubleshooting purposes
                Console.WriteLine($"Email send operation failed with error code: {ex.ErrorCode}, message: {ex.Message}");
            }
        }
    }
}
