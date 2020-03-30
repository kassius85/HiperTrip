using Entities.Helpers;
using HiperTrip.Interfaces;
using HiperTrip.Settings;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HiperTrip.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailConfiguration;

        public EmailService(IOptions<EmailSettings> emailConfiguration)
        {
            if (emailConfiguration != null)
            {
                _emailConfiguration = emailConfiguration.Value;
            }
        }

        public void Send(EmailMessage emailMessage)
        {
            if (emailMessage != null)
            {
                MimeMessage message = new MimeMessage();
                message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
                message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

                message.Subject = emailMessage.Subject;

                //We will say we are sending HTML. But there are options for plaintext etc. 
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = emailMessage.Content
                };

                //Be careful that the SmtpClient class is the one from Mailkit not the framework!
                using (SmtpClient emailClient = new SmtpClient())
                {
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, SecureSocketOptions.SslOnConnect);

                    //Remove any OAuth functionality as we won't be using it. 
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                    emailClient.Send(message);

                    emailClient.Disconnect(true);
                }
            }
        }

        public async Task<List<EmailMessage>> ReceiveEmail(int maxCount = 10)
        {
            using (Pop3Client emailClient = new Pop3Client())
            {
                await emailClient.ConnectAsync(_emailConfiguration.PopServer, _emailConfiguration.PopPort, true).ConfigureAwait(true);

                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                await emailClient.AuthenticateAsync(_emailConfiguration.PopUsername, _emailConfiguration.PopPassword).ConfigureAwait(true);

                List<EmailMessage> emails = new List<EmailMessage>();

                for (int i = 0; i < emailClient.Count && i < maxCount; i++)
                {
                    MimeMessage message = await emailClient.GetMessageAsync(i).ConfigureAwait(true);

                    EmailMessage emailMessage = new EmailMessage
                    {
                        Content = !string.IsNullOrEmpty(message.HtmlBody) ? message.HtmlBody : message.TextBody,
                        Subject = message.Subject
                    };

                    emailMessage.ToAddresses.AddRange(message.To.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                    emailMessage.FromAddresses.AddRange(message.From.Select(x => (MailboxAddress)x).Select(x => new EmailAddress { Address = x.Address, Name = x.Name }));
                }

                return emails;
            }
        }

        public void SendEmailActivateAccount(EmailAddress emailTo, string name, string subject = "", string content = "")
        {
            subject = "Account Activation - HiperTrip";

            content = string.Format(new CultureInfo("es-Cr"), @"<!DOCTYPE html><html><head> <title>Account Activation - HiperTrip - {0}</title></head><body> <table style=""height: 100 %; width: 500px; font - family: sans - serif; ""> <tbody> <tr> <td> <div style=""background - color: #009bd4; color: #ffffff; text-align: center; padding-top: 1px; padding-bottom: 1px;""> <h1>Verification Notice!</h1> <h2>ACTION REQUIRED</h2> </div> <div style=""padding: 20px 0px 10px 0px; line-height: 180%; font-size: 14px; color: #2e6c80; text-align: justify; border-bottom: 1px #bbb solid;""> <span style=""font-weight: bold;"">Notice:</span> To ensure you receive our future emails such as maintenance notices and renewal notices, please add us to your contact list.</div> <div style=""padding: 25px 0px 0px 0px;""> <p style=""color: #2e6c80;"">Hi {1}:</p> <p style=""color: #ff6600; font-weight: bold;"">You're one step away from becoming a HiperTrip member.</p> </div> <div style=""padding: 10px 0px 0px 0px;""> <p style=""color: #2e6c80;"">Below is your account login information:</p> <p style=""color: #2e6c80;"">Username: <span style=""color: #ff6600; font-weight: bold;"">{1}</span></p> </div> <div style=""padding: 10px 0px 0px 0px;""> <p style=""color: #2e6c80; font-weight: bold;"">Please Click Below To Activate Your Account:</p> <p><a href=""https://www.w3schools.com"">http://www.hipertrip.com/emailverify/{1}/e55e959b997ad4cc65e657915c1f6/</a> </p> </div> <div style=""color: #2e6c80;""> <p>(Please copy and paste the above URL to your browser if the link doesn't work.)</p> </div> <div style=""color: #2e6c80; padding: 10px 0px 0px 0px;""> <p>If you have questions or concerns, please contact us at:</p> <p><a href=""https://www.w3schools.com"">http://www.hipertrip.com/contact/</a></p> </div> <div style=""color: #2e6c80; padding: 10px 0px 25px 0px;""> <p>-HiperTrip Team</p> </div> <div style=""padding: 0px 0 5px 0; line-height: 180%; font-size: 14px; color: #2e6c80; text-align: center; border-bottom: 1px #bbb solid; border-top: 1px #bbb solid; font-weight: bold;""> DO NOT REPLY TO THIS EMAIL </div> </td> </tr> </tbody> </table></body></html>", emailTo, name);

            EmailAddress emailFrom = new EmailAddress()
            {
                Name = _emailConfiguration.NameToReply,
                Address = _emailConfiguration.EmailToReply
            };

            List<EmailAddress> fromAddresses = new List<EmailAddress>
            {
                emailFrom
            };

            List<EmailAddress> toAddresses = new List<EmailAddress>
            {
                emailTo
            };

            EmailMessage emailMessage = new EmailMessage()
            {
                FromAddresses = fromAddresses,
                ToAddresses = toAddresses,
                Subject = subject,
                Content = content
            };

            this.Send(emailMessage);
        }

        public void SendEmailActivateAccount(EmailMessage emailMessage)
        {
            if (emailMessage != null)
            {
                MimeMessage message = new MimeMessage();
                message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
                message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

                message.Subject = emailMessage.Subject;

                //We will say we are sending HTML. But there are options for plaintext etc. 
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = emailMessage.Content
                };

                //Be careful that the SmtpClient class is the one from Mailkit not the framework!
                using (SmtpClient emailClient = new SmtpClient())
                {
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, SecureSocketOptions.Auto);

                    //Remove any OAuth functionality as we won't be using it.
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

                    emailClient.Send(message);

                    emailClient.Disconnect(true);
                }
            }
        }

        public bool ValidEmail(string email)
        {
            return Regex.IsMatch(email, @_emailConfiguration.RegExp);
        }
    }
}