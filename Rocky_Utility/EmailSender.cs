using Microsoft.AspNetCore.Identity.UI.Services;
using ElasticEmail.Api;
using ElasticEmail.Client;
using ElasticEmail.Model;
using Microsoft.Extensions.Configuration;

namespace Rocky.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private ElasticEmailSettings _elasticEmailSettings;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {

            _elasticEmailSettings = _configuration.GetSection("ElasticEmail").Get<ElasticEmailSettings>();
            Configuration config = new Configuration();
            // Configure API key authorization: apikey
            config.ApiKey.Add(_elasticEmailSettings.ApiKey, _elasticEmailSettings.SecretKey);

            var apiInstance = new EmailsApi(config);

            List<EmailRecipient> emailRecipients = new List<EmailRecipient>();
            var recipients = new EmailRecipient(email);
            emailRecipients.Add(recipients);
            EmailMessageData emailData = new EmailMessageData(recipients: emailRecipients);
            emailData.Content = new EmailContent();
            emailData.Content.Body = new List<BodyPart>();
            emailData.Content.EnvelopeFrom = "My Company";
            BodyPart htmlBodyPart = new BodyPart();
            htmlBodyPart.ContentType = BodyContentType.HTML;
            htmlBodyPart.Charset = "utf-8";
            htmlBodyPart.Content = body;
            emailData.Content.Body.Add(htmlBodyPart);
            emailData.Content.From = "dmitiyminets@yandex.by";
            emailData.Content.ReplyTo = "dmitiyminets@yandex.by";
            emailData.Content.Subject = subject;


            try
            {
                await apiInstance.EmailsPostAsync(emailData);
            }
            catch (ApiException e)
            {
                Console.WriteLine("Exception when calling EmailsApi.EmailsPost: " + e.Message);
                Console.WriteLine("Status Code: " + e.ErrorCode);
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }
    }
}
