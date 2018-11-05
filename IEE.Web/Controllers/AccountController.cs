using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using IEE.ViewModel;
using System.Web.Security;
using IEE.Service.Abstract;
using IEE.Infrastructure.DbContext;
using Newtonsoft.Json;
using IEE.Infrastructure;
using System.Collections.Generic;
using IEE.Service;
using IEE.Web;
using IEE.Web.IeeEmailService;
using Google.Apis.Util;


namespace IEE.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly IRepository<User> _userRepository;
        private readonly IEncryptionService _encryptionService;
        public AccountController()
        {
            var unitOfWork = new UnitOfWork();

            _membershipService = new MembershipService();
            _userRepository = unitOfWork.GetRepository<User>();
            _encryptionService = new EncryptionService();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            var model = new LoginViewModel();
            model.RememberMe = false;
            return View(model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _membershipService.ValidateMemeber(model.Email, model.Password);
            if (user != null)
            {
                var userRole = user.UserRoles.Select(r => r.RoleId).ToList();
                var roles = new string[] { };
                using (var db = new UnitOfWork())
                {
                    var roleRepo = db.GetRepository<Role>();
                    roles = roleRepo.GetMany(r => userRole.Contains(r.Id)).Select(r => r.Name).ToArray();
                }

                Model.CustomPrincipalSerializeModel serializeModel = new Model.CustomPrincipalSerializeModel();
                serializeModel.UserId = user.Id;
                serializeModel.Name = user.Name;
                serializeModel.Roles = roles;

                string userData = JsonConvert.SerializeObject(serializeModel);
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                        1,
                        user.Email,
                        DateTime.Now,
                        DateTime.Now.AddDays(30),
                        model.RememberMe,
                        userData);

                string encTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                faCookie.Expires = authTicket.Expiration;
                Response.Cookies.Add(faCookie);
                if (user.UserRoles.Any(u => u.RoleId == 2 && u.UserId == user.Id))
                {
                    Session["AccountEmail"] = model.Email;

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    var url = ViewBag.ReturnUrl != null ? ViewBag.ReturnUrl : "~";
                    return Redirect(url);

                }
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = HttpUtility.HtmlEncode(returnUrl);

                    return Redirect(returnUrl);
                }

                return RedirectToAction("index", "post", new { Area = "ttn_content" });
            }
            else
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác");

                return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            // Clear authentication cookie.
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new SATEntities())
                {
                    var user = db.Users.FirstOrDefault(a => a.Email == model.Email && (a.IsDeleted == null || a.IsDeleted == false));
                    if (user != null)
                    {
                        user.ResetPasswordToken = Models.Utils.Instance.RandomString(10);

                        db.Entry<User>(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        var resetUrl = string.Format("{0}/Account/ResetPassword", Request.Url.GetLeftPart(UriPartial.Authority));
                        MailServices mailServices = new MailServices();
                        var body = string.Format("Chào bạn, \r\n Một yêu cầu thiết lập lại mật khẩu đã được gửi đến địa chỉ {0}.\r\n Đây là mã xác thực: {2}\r\n Dưới đây là link thiết lập lại mật khẩu \r\n " +
                            "{1}" + "\r\n Nếu không phải bạn gửi yêu cầu, làm ơn bỏ qua thư này.", model.Email, resetUrl, user.ResetPasswordToken);
                        var mailModel = new MailModel { Subject = "Thiết lập lại mật khẩu", Body = body, To = model.Email };
                        await mailServices.SendAsync(mailModel);

                        ViewBag.ResponseMessage = "Yêu cầu thiết lập lại mật khẩu của bạn đã được gửi đi,\r\n vui lòng kiểm tra thư điện tử của bạn.";
                    }

                }

            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {


            ResetPasswordViewModel model = new ResetPasswordViewModel();
            return View(model);

        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.Get(u => u.Email == model.Email);
                if (user != null && model.Code == user.ResetPasswordToken)
                {
                    var encPassword = _encryptionService.EncryptPassword(model.Password, user.Salt);
                    user.Password = encPassword;

                    _userRepository.Update(user);
                    _userRepository.SaveChanges();

                    return RedirectToAction("Login");
                }
                ViewBag.ResponseMessage = "Không tìm thấy dữ liệu phù hợp với thông tin bạn nhập vào.";
                return View(model);
            }
            return View(model);
        }
    }
}