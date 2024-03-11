using BlogCMS.Core.ConfigOptions;
using BlogCMS.WebApp.Models.Systems;
using HandlebarsDotNet;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BlogCMS.WebApp.Services
{
    public class EmailService : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly SmtpClient _smtpClient;
        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
            _smtpClient = new SmtpClient();
        }

        public async Task SendEmailAsync(EmailData request, CancellationToken cancellationToken = default)
        {
            var emailMessage = new MimeMessage
            {
                Sender = new MailboxAddress(_emailSettings.DisplayName, request.From ?? _emailSettings.From),
                Subject = request.Subject,
                Body = new BodyBuilder
                {
                    HtmlBody = request.Body
                }.ToMessageBody()
            };

            if (request.ToAddresses.Any())
            {
                foreach (var item in request.ToAddresses)
                {
                    emailMessage.To.Add(MailboxAddress.Parse(item));
                }
            }
            else
            {
                var toAddress = request.ToAddress;
                emailMessage.To.Add(MailboxAddress.Parse(toAddress));
            }

            try
            {
                await _smtpClient.ConnectAsync(_emailSettings.SMTPServer, _emailSettings.Port, _emailSettings.UseSsl, cancellationToken);
                await _smtpClient.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password, cancellationToken);
                await _smtpClient.SendAsync(emailMessage, cancellationToken);
                await _smtpClient.DisconnectAsync(true, cancellationToken);
            }
            catch (Exception ex)
            {
                //_logger.Error(ex.Message, ex);
            }
            finally
            {
                await _smtpClient.DisconnectAsync(true, cancellationToken);
                _smtpClient.Dispose();
            }
        }

        //public async Task SendEmail(EmailData request, CancellationToken cancellationToken = new CancellationToken())
        //{
        //    var emailMessage = new MimeMessage
        //    {
        //        Sender = new MailboxAddress(_emailSettings.DisplayName, request.From ?? _emailSettings.From),
        //        Subject = request.Subject,
        //        Body = new BodyBuilder
        //        {
        //            HtmlBody = request.Body
        //        }.ToMessageBody()
        //    };

        //    var toAddress = request.ToEmail;
        //    emailMessage.To.Add(MailboxAddress.Parse(toAddress));

        //    var bodyBuilder = new BodyBuilder();
        //    if (!string.IsNullOrEmpty(request.Template))
        //    {
        //        // Read the email template
        //        var templatePath = request.Template;
        //        var template = File.ReadAllText(templatePath);

        //        // Compile the template using Handlebars
        //        var compiledTemplate = Handlebars.Compile(template);

        //        // Create the HTML body by rendering the template with dynamic data
        //        var htmlBody = compiledTemplate(request.TemplateData);

        //        bodyBuilder.HtmlBody = htmlBody;
        //    }
        //    var textBody = request.Content;

        //    bodyBuilder.TextBody = textBody;
        //    emailMessage.Body = bodyBuilder.ToMessageBody();

        //    try
        //    {
        //        await _smtpClient.ConnectAsync(_emailSettings.SMTPServer, _emailSettings.Port, _emailSettings.UseSsl, cancellationToken);
        //        await _smtpClient.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password, cancellationToken);
        //        await _smtpClient.SendAsync(emailMessage, cancellationToken);
        //        await _smtpClient.DisconnectAsync(true, cancellationToken);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    finally
        //    {
        //        await _smtpClient.DisconnectAsync(true, cancellationToken);
        //        _smtpClient.Dispose();
        //    }
        //}
    }
}
