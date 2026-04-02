using BucketSurvey.Api.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace BucketSurvey.Api.Services;

public class EmailService(
    IOptions<MailSettings> mailsettings,
    ILogger<EmailService> logger) : IEmailSender
{
    private readonly MailSettings _Mailsettings = mailsettings.Value;
    private readonly ILogger<EmailService> _Logger = logger;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        //header assigned
        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_Mailsettings.User),
            Subject = subject
        };
        // the reciever email 
        message.To.Add(MailboxAddress.Parse(email));

        //Email Body
        var messageBody = new BodyBuilder()
        {
            HtmlBody = htmlMessage
        };

        message.Body = messageBody.ToMessageBody();

        _Logger.LogInformation("Email Send Service ");

        using (var smtpclient = new SmtpClient())
        {
            smtpclient.Connect(_Mailsettings.Host, _Mailsettings.port, SecureSocketOptions.StartTls);

            smtpclient.Authenticate(_Mailsettings.User, _Mailsettings.Pass);

            await smtpclient.SendAsync(message);

            smtpclient.Disconnect(true);
        }



    }
}
