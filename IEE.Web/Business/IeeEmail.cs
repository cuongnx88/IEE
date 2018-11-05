using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;

namespace IEE.Web.Business
{
    public class MailModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        
    }
    public class IeeEmailService
    {
        public MailAddress to { get; set; }
        public MailAddress from { get; set; }
        public string title { get; set; }
        public string body { get; set; }

        public void Send()
        {
            
            using (MailMessage mailMessage = new MailMessage())
            {

                mailMessage.From        = from;

                mailMessage.Subject     = title;
                mailMessage.Body        = body;
                mailMessage.IsBodyHtml  = true;

                mailMessage.To.Add(to);

                SmtpClient smtp = new SmtpClient();

                smtp.Host       = ConfigurationManager.AppSettings["Host"];
                smtp.EnableSsl  = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);

                NetworkCredential NetworkCred = new System.Net.NetworkCredential();

                NetworkCred.UserName = ConfigurationManager.AppSettings["UserName"];

                NetworkCred.Password = ConfigurationManager.AppSettings["Password"];

                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);

                smtp.Send(mailMessage);

            }
        }

        public async Task SendAsync(MailModel model)
        {
            SmtpClient smtp = new SmtpClient();

            smtp.Host = ConfigurationManager.AppSettings["Host"];
            smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);

            NetworkCredential NetworkCred = new System.Net.NetworkCredential();

            NetworkCred.UserName = ConfigurationManager.AppSettings["UserName"];

            NetworkCred.Password = ConfigurationManager.AppSettings["Password"];

            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;

            smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);

            await smtp.SendMailAsync(model.From,model.To,model.Body);
        }
    }
}