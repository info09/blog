using BlogCMS.WebApp.Models;

namespace BlogCMS.WebApp.Services
{
    public interface IEmailSender
    {
        //Task SendEmail(EmailData emailData, CancellationToken cancellationToken = new CancellationToken());
        Task SendEmailAsync(EmailData request, CancellationToken cancellationToken = new CancellationToken());
    }
}
