using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain.Entities.Helper;
using MiHairCareApp.Domain.Exceptions;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace MiHairCareApp.Application.ServicesImplementation
{
    public class EmailServices : IEmailServices
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailServices> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public EmailServices(IOptions<EmailSettings> emailSettings, ILogger<EmailServices> logger, IUnitOfWork unitOfWork)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> SendEmailAsync(string link, string email, string id)
        {
            var bodyBuilder = new BodyBuilder { HtmlBody = $"<a href='{link}'>Click here to confirm your email</a>" };

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.Email));
            emailMessage.To.Add(new MailboxAddress(email, email));
            emailMessage.Subject = "Confirm your email";
            emailMessage.Body = bodyBuilder.ToMessageBody();

            try
            {
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port);
                await client.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
                return "success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email.");
                // Do not perform destructive side effects (like deleting user) here.
                throw new ServiceException("Error occurred while sending email", ex);
            }
        }

        public async Task<bool> SendMailAsync(MailRequest mailRequest)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.Email));
            emailMessage.To.Add(new MailboxAddress(mailRequest.ToEmail, mailRequest.ToEmail));
            emailMessage.Subject = mailRequest.Subject;
            var bodyBuilder = new BodyBuilder { HtmlBody = $"{mailRequest.Body}<br/>" };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.SslOnConnect).ConfigureAwait(false);
                await client.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password).ConfigureAwait(false);
                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email.");
                throw new ServiceException("Error occurred while sending email", ex);
            }
        }
    }
}
