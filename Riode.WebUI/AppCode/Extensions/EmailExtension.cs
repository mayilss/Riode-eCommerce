using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Riode.WebUI.AppCode.Extensions
{
	public static partial class Extension
	{
		public static bool SendEmail(
            this IConfiguration configuration, 
            string toEmail, 
            string subject, 
            string messageText, 
            CancellationToken cancellationToken)
		{
			try
			{
                var displayName = configuration["emailAccount:displayName"];
                var smtpServer = configuration["emailAccount:smtpServer"];
                var smtpPort = Convert.ToInt32(configuration["emailAccount:smtpPort"]);
                var userName = configuration["emailAccount:userName"];
                var password = configuration["emailAccount:password"];
                var cc = configuration["emailAccount:cc"];

                SmtpClient client = new SmtpClient(smtpServer, smtpPort);
                client.Credentials = new NetworkCredential(userName, password);
                client.EnableSsl = true;

                var from = new MailAddress(userName, displayName);
                MailMessage message = new MailMessage(from, new MailAddress(toEmail));
                message.Subject = subject;
                message.Body = messageText;
                message.IsBodyHtml = true;

                string[] ccs = cc.Split(";", StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in ccs)
                {
                    message.Bcc.Add(item);
                }
                client.SendAsync(message, cancellationToken);

                return true;
            }
			catch (Exception ex)
			{

				return false;
			}
        }

        public static bool SendEmail(
            this IConfiguration configuration,
            string toEmail,
            string subject,
            string messageText)
        {
            try
            {
                var displayName = configuration["emailAccount:displayName"];
                var smtpServer = configuration["emailAccount:smtpServer"];
                var smtpPort = Convert.ToInt32(configuration["emailAccount:smtpPort"]);
                var userName = configuration["emailAccount:userName"];
                var password = configuration["emailAccount:password"];
                var cc = configuration["emailAccount:cc"];

                SmtpClient client = new SmtpClient(smtpServer, smtpPort);
                client.Credentials = new NetworkCredential(userName, password);
                client.EnableSsl = true;

                var from = new MailAddress(userName, displayName);
                MailMessage message = new MailMessage(from, new MailAddress(toEmail));
                message.Subject = subject;
                message.Body = messageText;
                message.IsBodyHtml = true;

                string[] ccs = cc.Split(";", StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in ccs)
                {
                    message.Bcc.Add(item);
                }
                client.Send(message);

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}
