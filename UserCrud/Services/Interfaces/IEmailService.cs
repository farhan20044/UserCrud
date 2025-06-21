using System.Threading.Tasks;

namespace UserCrud.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string toEmail, string confirmationLink);
    }
} 