using System;
using System.Collections.Generic;
using System.Linq;
using IEE.ViewModel;
using System.Web.Mvc;
using IEE.Infrastructure.DbContext;

using AutoMapper;
using System.Configuration;
using System.Web;
using IEE.Web.Business;
using System.Net.Mail;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using IEE.Infrastructure;

namespace IEE.Web.Controllers
{
    public class CmsBaseController : Controller
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Consultant> _consultRepo;
        private readonly IRepository<Program> _proRepo;
        private readonly IRepository<Post> _postRepo;
        private readonly IRepository<Setting> _settingRepo;
        private readonly IRepository<StudentHighlight> _studentRepo;
        private readonly IRepository<TeacherHighlight> _teacherRepo;
        private readonly IRepository<UniversityHighlight> _universityRepo;
        private readonly IRepository<Banner> _bannerRepo;
        public CmsBaseController()
        {
            UnitOfWork dbContext = new UnitOfWork();


            _categoryRepo = dbContext.GetRepository<Category>();
            _settingRepo = dbContext.GetRepository<Setting>();
            _postRepo = dbContext.GetRepository<Post>();
            _bannerRepo = dbContext.GetRepository<Banner>();


            _consultRepo = dbContext.GetRepository<Consultant>();
            _proRepo = dbContext.GetRepository<Program>();
            _universityRepo = dbContext.GetRepository<UniversityHighlight>();


            _studentRepo = dbContext.GetRepository<StudentHighlight>();
            _teacherRepo = dbContext.GetRepository<TeacherHighlight>();


        }

        public ActionResult Header()
        {

            var hotline = _settingRepo.Get(t => t.Id == 1);
            ViewBag.Hotline = hotline;

            ViewBag.Facebook = _settingRepo.Get(t => t.Id == 2).Value;
            ViewBag.Youtube = _settingRepo.Get(t => t.Id == 3).Value;
            ViewBag.Linked = _settingRepo.Get(t => t.Id == 6).Value;
            ViewBag.Gplus = _settingRepo.Get(t => t.Id == 5).Value;
            ViewBag.Instagram = _settingRepo.Get(t => t.Id == 4).Value;

            var categories = _categoryRepo.GetMany(t => t.IsDeleted == false && t.IsMenu == true && t.ParentId == 1).OrderBy(t => t.OrderNumber);
            return PartialView("_Header", categories);
        }
        public ActionResult Footer()
        {
            var hotline = _settingRepo.Get(t => t.Id == 1);
            ViewBag.Hotline = hotline;

            var categories = _categoryRepo.GetMany(t => t.IsDeleted == false && t.IsMenu == true);
            ViewBag.Categories = categories;

            ViewBag.Facebook = _settingRepo.Get(t => t.Id == 2).Value;
            ViewBag.Youtube = _settingRepo.Get(t => t.Id == 3).Value;
            ViewBag.Linked = _settingRepo.Get(t => t.Id == 6).Value;
            ViewBag.Gplus = _settingRepo.Get(t => t.Id == 5).Value;
            ViewBag.Instagram = _settingRepo.Get(t => t.Id == 4).Value;

            ViewBag.Hotline = _settingRepo.Get(t => t.Id == 1).Value;
            ViewBag.Phone = _settingRepo.Get(t => t.Id == 8).Value;
            ViewBag.Address = _settingRepo.Get(t => t.Id == 7).Value;

            return PartialView("_Footer");
        }
        public ActionResult ConsultRegister()
        {
            var programs = _proRepo.GetMany(t => t.IsDeleted == false);
            ViewBag.Programs = programs;
            return PartialView("_ConsultRegister");
        }

        [HttpPost]
        public ActionResult ConsultRegister(ConsultantViewModel model)
        {
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Email))
            {
                return Json(false);
            }
            else if (!this.IsValidEmail(model.Email))
            {
                return Json(false);
            }
            else
            {
                var consult = Mapper.Map<ConsultantViewModel, Consultant>(model);
                consult.CreatedDate = DateTime.Now;
                consult.IsDeleted = false;
                _consultRepo.InsertAndSubmit(consult);

                using (MailMessage mailMessage = new MailMessage())
                {
                    StringBuilder content = new StringBuilder();
                    content.Append("IEE nhận được yêu cầu tư vấn với các thông tin.");
                    content.AppendLine();
                    content.Append("Họ và tên: " + model.Name);
                    content.AppendLine();
                    content.Append("Địa chỉ: " + model.School);
                    content.AppendLine();
                    content.Append("Lớp: " + model.Class);
                    content.AppendLine();
                    content.Append("Điện thoại: " + model.Phone);
                    content.AppendLine();
                    content.Append("Email: " + model.Email);
                    content.AppendLine();
                    if (model.Program.HasValue)
                    {
                        string programName = _proRepo.Get(t => t.Id == model.Program.Value).Name;
                        content.Append("Chương trình mong muốn: " + programName);
                        content.AppendLine();
                    }
                    content.Append("Nội dung:");
                    content.AppendLine();
                    content.Append(model.Message);

                    mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["UserName"]);
                    mailMessage.Subject = "IEE nhận được yêu cầu tư vấn với các thông tin";
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

                return Json(true);
            }
        }

        public ActionResult University()
        {
            var universities = _universityRepo.GetMany(t => t.IsDeleted == false);
            return PartialView("_University", universities);
        }
        public ActionResult Student()
        {
            var students = _studentRepo.GetMany(t => t.IsDeleted == false);
            return PartialView("_Student", students);
        }
        public ActionResult Teacher()
        {
            var teachers = _teacherRepo.GetMany(t => t.IsDeleted == false).OrderBy(o => o.DisplayIndex).ToList();
            return PartialView("_Teacher", teachers);
        }
        public ActionResult LeftPostRandom()
        {
            Random rnd = new Random();
            var posts = _postRepo.GetMany(t => t.IsDeleted == false && t.PostCategories.Select(c => c.Category).Any(c => c.Controller.Equals("news")) == true).OrderByDescending(t => rnd.Next()).Take(5);
            var student = _studentRepo.GetMany(t => t.IsDeleted == false).OrderByDescending(t => rnd.Next()).Take(1).FirstOrDefault();
            ViewBag.Student = student;

            var sat = _settingRepo.Get(t => t.Id == 11);
            ViewBag.Sat = sat;

            var toefl = _settingRepo.Get(t => t.Id == 13);
            ViewBag.Toefl = toefl;

            var ielfs = _settingRepo.Get(t => t.Id == 15);
            ViewBag.Ielfs = ielfs;

            var sat_new = _settingRepo.Get(t => t.Id == 11);
            ViewBag.SatNew = sat_new;

            var toefl_new = _settingRepo.Get(t => t.Id == 13);
            ViewBag.ToeflNew = toefl_new;

            var ielfs_new = _settingRepo.Get(t => t.Id == 15);
            ViewBag.IelfsNew = ielfs_new;

            return PartialView("_LeftPostRandom", posts);
        }
        public ActionResult LeftPostProgramRandom(int category)
        {
            Random rnd = new Random();
            var posts = _postRepo.GetMany(t => t.IsDeleted == false && t.PostCategories.Select(c => c.Category).Any(c => c.Controller.Equals("news")) == true).OrderByDescending(t => rnd.Next()).Take(2);
            var student = _studentRepo.GetMany(t => t.IsDeleted == false).OrderByDescending(t => rnd.Next()).Take(1).FirstOrDefault();
            ViewBag.Student = student;

            var sat = _settingRepo.Get(t => t.Id == 11);
            ViewBag.Sat = sat;

            var toefl = _settingRepo.Get(t => t.Id == 13);
            ViewBag.Toefl = toefl;

            var ielfs = _settingRepo.Get(t => t.Id == 15);
            ViewBag.Ielfs = ielfs;

            var sat_new = _settingRepo.Get(t => t.Id == 11);
            ViewBag.SatNew = sat_new;

            var toefl_new = _settingRepo.Get(t => t.Id == 13);
            ViewBag.ToeflNew = toefl_new;

            var ielfs_new = _settingRepo.Get(t => t.Id == 15);
            ViewBag.IelfsNew = ielfs_new;

            var categoryObj = _categoryRepo.Get(t => t.Id == category);
            ViewBag.Categories = _categoryRepo.GetMany(c => c.ParentId == categoryObj.Id).OrderBy(t => t.OrderNumber).OrderBy(t => t.Name);

            return PartialView("_LeftPostProgramRandom", posts);
        }
        public ActionResult LeftAbout()
        {
            Random rnd = new Random();
            var student = _studentRepo.GetMany(t => t.IsDeleted == false).OrderByDescending(t => rnd.Next()).Take(1).FirstOrDefault();
            ViewBag.Student = student;

            var sat = _settingRepo.Get(t => t.Id == 11);
            ViewBag.Sat = sat;

            var toefl = _settingRepo.Get(t => t.Id == 13);
            ViewBag.Toefl = toefl;

            var ielfs = _settingRepo.Get(t => t.Id == 15);
            ViewBag.Ielfs = ielfs;

            var sat_new = _settingRepo.Get(t => t.Id == 11);
            ViewBag.SatNew = sat_new;

            var toefl_new = _settingRepo.Get(t => t.Id == 13);
            ViewBag.ToeflNew = toefl_new;

            var ielfs_new = _settingRepo.Get(t => t.Id == 15);
            ViewBag.IelfsNew = ielfs_new;

            var categories = _categoryRepo.GetMany(t => t.IsDeleted == false && t.IsMenu == true && t.Controller.Equals("about") && (t.ParentId.HasValue && t.ParentId.Value == 2)).OrderBy(t => t.OrderNumber).ThenBy(t => t.Name);

            return PartialView("_LeftAbout", categories);
        }
        public ActionResult TeacherPopup(int id)
        {
            var teacher = _teacherRepo.Get(t => t.Id == id);
            return PartialView("_TeacherPopup", teacher);
        }
        public ActionResult YoutubePopup(int id)
        {
            var banner = _bannerRepo.Get(t => t.Id == id);
            return PartialView("_YoutubePopup", banner);
        }
        public ActionResult FooterBanner()
        {

            var banner = _bannerRepo.GetMany(t => t.IsDeleted == false && t.IsLock == true && t.IsHeader == true).OrderByDescending(t => t.Id).FirstOrDefault();
            return PartialView("_FooterBanner", banner);
        }
        private string GetTitle(string url)
        {
            return GetArgs(new WebClient().DownloadString($"http://youtube.com/get_video_info?video_id={GetArgs(url, "v", '?')}"), "title", '&');
        }
        private string GetArgs(string args, string key, char query)
        {
            var iqs = args.IndexOf(query);
            return iqs == -1
                ? string.Empty
                : HttpUtility.ParseQueryString(iqs < args.Length - 1 ? args.Substring(iqs + 1) : string.Empty)[key];
        }

        public bool IsValidEmail(string InputEmail)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(InputEmail);
            if (match.Success)
                return true;
            else
                return false;
        }
        [HttpPost]
        public JsonResult IsEmailAvaiable(string Email)
        {

            var isAvail = false;
            using (var db = new SATEntities())
            {
                isAvail = db.Users.Any(a => Email.Trim() == a.Email);
            }
            if (isAvail)
            {
                return Json("Giá trị đã tồn tại", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult IsAccountAvaiable(string Name)
        {

            var isAvail = false;
            using (var db = new SATEntities())
            {
                isAvail = db.Users.Any(a => Name.Trim() == a.Name);
            }
            if (isAvail)
            {
                return Json("Giá trị đã tồn tại", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);


        }
    }
}