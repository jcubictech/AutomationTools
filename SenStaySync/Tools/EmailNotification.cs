using System.Text;
using System.Net.Mail;

namespace SenStaySync
{
    public static class EmailNotification
    {
        public static void Send(string to, string subject, string body)
        {
            var message = new MailMessage
            {
                Subject = subject,
                Body = body,
                BodyEncoding = Encoding.GetEncoding("UTF-8"),
                From = new MailAddress(Config.I.ErrorsEmail, Config.I.ErrorsEmailDisplayName)
            };

            var addresses = to.Split(',');
            foreach (var address in addresses)
            {
                message.To.Add(new MailAddress(address));
            }

            var smtp = new SmtpClient
            {
                Host = Config.I.ErrorsEmailSMTP,
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(
                    Config.I.ErrorsEmail,
                    Config.I.ErrorsEmailPassword)
            };

            smtp.Send(message);
        }
    }
}
