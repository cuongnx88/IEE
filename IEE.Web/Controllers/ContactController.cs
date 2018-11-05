using IEE.Infrastructure.DbContext;
using IEE.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace IEE.Web.Controllers
{
    public class ContactController : Controller
    {
        private SATEntities db = new SATEntities();
        // GET: Contact
        public ActionResult Index()
        {
            SetInforHomepage(db);
            return View();
        }

        [HttpPost]
        public ActionResult Index(ContactViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (MailMessage mailMessage = new MailMessage())
            {
                StringBuilder content = new StringBuilder();
                content.Append("IEE nhận được email với các thông tin.");
                content.AppendLine();
                content.Append("Người gửi: " + model.Name);
                content.AppendLine();
                content.Append("Địa chỉ: " + model.Address);
                content.AppendLine();
                content.Append("Điện thoại: " + model.Phone);
                content.AppendLine();
                content.Append("Tiêu đề: " + model.Title);
                content.AppendLine();
                content.Append("Email: " + model.Email);
                content.AppendLine();
                content.Append("Nội dung:");
                content.AppendLine();
                content.Append(model.Content);

                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["UserName"]);
                mailMessage.Subject = model.Title;
                mailMessage.Body = content.ToString();
                mailMessage.IsBodyHtml = true;

                mailMessage.To.Add(ConfigurationManager.AppSettings["Email"]);

                SmtpClient smtp = new SmtpClient();

                smtp.Host = ConfigurationManager.AppSettings["Host"];
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);

                NetworkCredential NetworkCred = new System.Net.NetworkCredential();

                NetworkCred.UserName = ConfigurationManager.AppSettings["UserName"];
                NetworkCred.Password = ConfigurationManager.AppSettings["Password"];
                smtp.Credentials = NetworkCred;


                smtp.Send(mailMessage); 

            }
            TempData["message"] = "Cảm ơn bạn đã liên hệ tới IEE, chúng tôi sẽ feedback tới bạn trong thời gian sớm nhất.";
            return View(new ContactViewModel());
        }
        public void SetInforHomepage(SATEntities db)
        {
            var categories = db.Categories.Where(t => t.ParentId == 1).OrderBy(t => t.OrderNumber).ToList();
            ViewBag.Categories = categories;
            ViewBag.Facebook = db.Settings.FirstOrDefault(t => t.Id == 2).Value;
            ViewBag.Youtube = db.Settings.FirstOrDefault(t => t.Id == 3).Value;
            ViewBag.Linked = db.Settings.FirstOrDefault(t => t.Id == 6).Value;
            ViewBag.Gplus = db.Settings.FirstOrDefault(t => t.Id == 5).Value;
            ViewBag.Instagram = db.Settings.FirstOrDefault(t => t.Id == 4).Value;

            ViewBag.Hotline = db.Settings.FirstOrDefault(t => t.Id == 1).Value;
            ViewBag.Address = db.Settings.FirstOrDefault(t => t.Key == "address").Value;
            ViewBag.Phone = db.Settings.FirstOrDefault(t => t.Key.Equals("phone")).Value;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}