using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Helpers;

namespace HiperTrip.Interfaces
{
    public interface IEmailService
    {
        void Send(EmailMessage emailMessage);
        Task<List<EmailMessage>> ReceiveEmail(int maxCount = 10);
        bool ValidEmail(string email);
        void SendEmailActivateAccount(EmailAddress emailTo, string username, string subject = "", string content = "");
        void SendEmail(EmailMessage emailMessage);
    }
}