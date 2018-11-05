using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;

namespace IEE.Web.IeeEmailService
{
    public class MailServices
    {
        public async Task SendAsync(MailModel model)
        {
            SmtpClient smtp = new SmtpClient();

            smtp.Host = ConfigurationManager.AppSettings["Host"];
            smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);

            NetworkCredential NetworkCred = new System.Net.NetworkCredential();

            NetworkCred.UserName = ConfigurationManager.AppSettings["UserName"];

            NetworkCred.Password = ConfigurationManager.AppSettings["Password"];
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;

            smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);

            await Task.Run(() => smtp.Send(model.From, model.To, model.Subject, model.Body));

        }


    }
    public class MailModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }

        public MailModel()
        {
            From = ConfigurationManager.AppSettings["UserName"];
        }

    }
}