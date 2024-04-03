using Azure.Communication.Email;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8917lab4FunctionAppConfirmed
{
    public static class Utility
    {
        public static void SendEmail(string[] args)
        {
            // This code demonstrates how to send email using Azure Communication Services.
            var connectionString = "endpoint=https://yuan0037communicationservicelab4.canada.communication.azure.com/;accesskey=cg9kfyCSvKqYxoNsl731owHTi79JkhwT0mdwfUpob0Xz/MTKb9qu9/jRjEazdQ6QmRqOYd+Gnw72LNBlJCWjAQ==";
            var emailClient = new EmailClient(connectionString);

            var sender = "DoNotReply@1f805881-bf05-4e2b-8372-2f600b7990a3.azurecomm.net";
            var recipient = "bobyuan@gmail.com";
            var subject = "Order Status " + args[0] + " sent by " + args[1];
            var htmlContent = "<html><body><h1>Order Status " + args[0] + " </h1><br/><h4>Your order is " + args[0] + "</h4><p>Happy Shopping!!</p></body></html>";

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
