using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config) => _config = config;
    
    public async Task SendEmailAsync(string fromName, string subject, string message, string toEmail)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(fromName, _config.GetValue<string>("EmailSettings:SenderEmail")));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = message };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"], 
            int.Parse(_config["EmailSettings:SmtpPort"]), 
            MailKit.Security.SecureSocketOptions.StartTls);
        
        await smtp.AuthenticateAsync(_config["EmailSettings:SenderEmail"], 
            _config["EmailSettings:AppPassword"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}