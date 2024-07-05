using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Here you can implement the actual email sending logic.
        // For now, we'll just simulate it.
        return Task.CompletedTask;
    }
}
