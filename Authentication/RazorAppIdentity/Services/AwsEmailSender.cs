using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace RazorAppIdentity.Services
{
    public class AwsEmailSender : IEmailSender
    {
        private readonly ILogger<AwsEmailSender> _logger;
        private readonly AwsSesOptions _awsSesOptions;

        public AwsEmailSender(ILogger<AwsEmailSender> logger,
                              IOptions<AwsSesOptions> awsSesOptions)
        {
            _logger = logger;
            _awsSesOptions = awsSesOptions.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            using var client = new AmazonSimpleEmailServiceClient(
                _awsSesOptions.AccessKey, 
                _awsSesOptions.SecretKey, 
                RegionEndpoint.APSoutheast1);

            var sendRequest = new SendEmailRequest
            {
                Source = _awsSesOptions.SenderAddress,
                Destination = new Destination
                {
                    ToAddresses = [email]
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = htmlMessage
                        }
                    }
                },
            };

            try
            {
                var response = await client.SendEmailAsync(sendRequest);
                _logger.LogInformation("The email was sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The email was not sent.");
            }
        }
    }
}
