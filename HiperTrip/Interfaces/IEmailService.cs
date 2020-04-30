using Entities.Helpers;
using HiperTrip.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiperTrip.Interfaces
{
    public interface IEmailService
    {
        void Send(EmailMessage emailMessage, EmailSettings emailSettings);
        Task<List<EmailMessage>> ReceiveEmail(EmailSettings emailSettings, int maxCount = 10);
        void SendEmailActivateAccount(EmailAddress emailTo, EmailSettings emailSettings, string username, string subject = "", string content = "");
        void SendEmail(EmailMessage emailMessage, EmailSettings emailSettings);
        bool ValidEmail(string email, string regExp);
    }
}