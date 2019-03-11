// using SendGrid's C# Library
// https://github.com/sendgrid/sendgrid-csharp
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace CoreProject.Models
{
    public class SendMail
    {
        public static async Task Execute()
        {
            var apiKey = Environment.GetEnvironmentVariable("ex1SPp_2TkeGoypREkEE3Q");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("admin@articoletech.com", "Cart4All.com");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("mayank.gpt1@gmail.com.com", "Mr. Mayank Gupta");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
    
}
