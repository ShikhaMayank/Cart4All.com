using System.Net.Mail;
namespace CoreProject.Models
{
     public class Credentials {
        public  string Subject { get; set; }
        public string Body { get; set; }
        public string ToEmail { get; set; }
        public string SMTPUser { get; set; }
        public string SMTPPassword { get; set; }
        public string DisplayName { get; set; }
        public string Host { get; set; }
        public int Port;
        public bool EnableSsl; 
        public bool IsBodyHtml;
    }
    public class SendMail
    {
        // Toenable gmail sending mails, you have to do following things:
        // Change your settings to allow less secure apps into your account.We don't recommend this option because it can make it easier for someone to break into your account. If you want to allow access anyway, follow these steps:
        // Go to your Google Account.
        // On the left navigation panel, click Security.
        // On the bottom of the page, in the Less secure app access panel, click Turn on access.
        // If you don't see this setting, your administrator might have turned off less secure app account access.
        
        private bool Gmail(Credentials objMail)
        {
            bool flag = false;            
            try
            {                               
                MailMessage mail = new MailMessage();                
                mail.From = new MailAddress(objMail.SMTPUser, objMail.DisplayName);                
                mail.To.Add(objMail.ToEmail);                
                mail.Subject = objMail.Subject;                
                mail.Body = objMail.Body;                
                mail.IsBodyHtml = objMail.IsBodyHtml;                
                mail.Priority = MailPriority.Normal;                
                SmtpClient smtp = new SmtpClient();                
                smtp.Host = objMail.Host;                
                smtp.Port = objMail.Port;                
                smtp.Credentials = new System.Net.NetworkCredential(objMail.SMTPUser, objMail.SMTPPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = objMail.EnableSsl;
                smtp.Send(mail);
                flag = true;
            }
            catch (SmtpException ex)
            {
                flag = false;
            }            
            return flag;
        }
    }
}
