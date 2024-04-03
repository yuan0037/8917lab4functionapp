using System;
using Azure.Communication.Email;
using Azure;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace _8917lab4FunctionAppConfirmed
{
    public class ConfirmedFunc
    {
        private readonly ILogger<ConfirmedFunc> _logger;

        public ConfirmedFunc(ILogger<ConfirmedFunc> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ConfirmedFunc))]
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
                        Utility.SendEmail(["confirmed", "ConfirmedFunc"]);
                    }
                    else
                    {
                        if (message.ApplicationProperties.GetValueOrDefault("label").ToString() == "rejected")
                        {
                            //we added filter in the subscription level. so the following code should not be executed.
                            Utility.SendEmail(["rejected", "ConfirmedFunc"]);
                        }
                        else
                        {
                            Utility.SendEmail(["unknow", "ConfirmedFunc"]);
                        }
                    }
                } else
                {
                    Utility.SendEmail(["missing status", "ConfirmedFunc"]);
                }
            }
            catch (ArgumentNullException ex)
            {
                // handle the argumentnullexception here
                Console.WriteLine("argumentnullexception occurred: " + ex.Message);
            }
        }

        
    }
}
