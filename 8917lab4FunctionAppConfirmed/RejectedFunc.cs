using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace _8917lab4FunctionAppConfirmed
{
    public class RejectedFunc
    {
        private readonly ILogger<RejectedFunc> _logger;

        public RejectedFunc(ILogger<RejectedFunc> logger)
        {
            _logger = logger;
        }

        [Function(nameof(RejectedFunc))]
        public async Task Run(
            [ServiceBusTrigger("booking-status", "rejected", Connection = "MyConnection")]
            ServiceBusReceivedMessage message
            )
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                if (message.ApplicationProperties.ContainsKey("label"))
                {

                    if (message.ApplicationProperties.GetValueOrDefault("label").ToString() == "rejected")
                    {
                        Utility.SendEmail(["rejected", "RejectedFunc"]);
                    }
                    else
                    {
                        if (message.ApplicationProperties.GetValueOrDefault("label").ToString() == "confirmed")
                        {
                            //we added filter in the subscription level. so the following code should not be executed.
                            Utility.SendEmail(["confirmed", "RejectedFunc"]);
                        }
                        else
                        {
                            Utility.SendEmail(["unknow", "RejectedFunc"]);
                        }
                    }
                }
                else
                {
                    Utility.SendEmail(["missing status", "RejectedFunc"]);
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
