using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Service
{
    public static class MailService
    {
        public static void SendMail(string to, string message, string subject = "DMS - Notification", bool isHtml = false, string[] bccList = null, bool disableTo = false)
        {
            MailMessage msg = new MailMessage(); //create the message
            msg.From = new MailAddress(ConfigurationManager.AppSettings.Get("MailAddress"), "DMS", Encoding.UTF8);
            if (!disableTo)
                msg.To.Add(to);

            if (bccList != null)
            {
                for (int i = 0; i < bccList.Length; i++)
                {
                    msg.Bcc.Add(bccList[i]);
                }
            }

            msg.Subject = subject;
            msg.SubjectEncoding = Encoding.UTF8;
            msg.Body = message;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = isHtml;
            msg.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient(); //Network Credentials for Gmail
            client.Credentials = new NetworkCredential(
                ConfigurationManager.AppSettings.Get("MailAddress"),
                ConfigurationManager.AppSettings.Get("MailPassword"));
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static void SendMail(string from, string pwd, string host, int port, string to, string message, string subject, bool isHtml)
        {
            MailMessage msg = new MailMessage(); //create the message
            msg.To.Add(to);
            msg.From = new MailAddress(from, from, Encoding.UTF8);
            msg.Subject = subject;
            msg.SubjectEncoding = Encoding.UTF8;
            msg.Body = message;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = isHtml;
            msg.Priority = MailPriority.High;
            SmtpClient client = new SmtpClient(); //Network Credentials for Gmail
            client.Credentials = new NetworkCredential(from, pwd);
            client.Port = port;
            client.Host = host;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
