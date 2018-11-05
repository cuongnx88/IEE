using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using IEE.Infrastructure.DbContext;
using IEE.ViewModel;
using AutoMapper;
using IEE.Service.Abstract;
using IEE.Service;
using IEE.Web.Controllers;
using System.Collections.Generic;
using System;
using IEE.Web.Models;


namespace IEE.Web.Areas.ttn_content.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SATAccountController : Controller
    {
        private SATEntities db = new SATEntities();
        private readonly IEncryptionService _encryptionService = new EncryptionService();
        private readonly IMembershipService _membershipService = new MembershipService();
        // GET: ttn_content/Account
        public ActionResult Index()
        {
            return View(db.Users.Where(u => u.IsSystem == null && u.IsDeleted != true).ToList());
        }

        // GET: ttn_content/Account/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SATAccount sATAccount = db.SATAccounts.Find(id);
            if (sATAccount == null)
            {
                return HttpNotFound();
            }
            return View(sATAccount);
        }

        // GET: ttn_content/Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ttn_content/Account/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,Phone,Password,IsLocked")] SATAccount userModel)
        {
            if (ModelState.IsValid)
            {
                var model = userModel.Map<Infrastructure.DbContext.User>();
                model.Name = userModel.Username;
                db.Users.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userModel);
        }

        // GET: ttn_content/Account/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userEnt = db.Users.Find(id);
            if (userEnt == null)
            {
                return HttpNotFound();
            }
            return View(userEnt);
        }

        // POST: ttn_content/Account/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Phone")] User userModel)
        {
            try
            {

                var entity = db.Users.Find(userModel.Id);
                entity.Name = userModel.Name;
                entity.Email = userModel.Email;
                entity.Phone = userModel.Phone;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return View(userModel);
            }
            
        }

        // GET: ttn_content/Account/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: ttn_content/Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userEnt = db.Users.Find(id);
            userEnt.IsDeleted = true;
            //db.Users.Remove(userEnt);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [AllowAnonymous]
        public ActionResult Register()
        {
            var model = new SATAccountRegisterModel();
            model.IsLocked = false;
            return View("~/Areas/ttn_content/Views/SATAccount/Register.cshtml", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public bool Register(SATAccountRegisterModel userModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // check email exist
                    // check account exist
                    // check confirm password
                    if (!userModel.ConfirmPassword.Equals(userModel.Password))
                    {
                        ModelState.AddModelError("ConfirmPasswordErr", "Xác nhận mật khẩu chưa đúng");
                        // return View("~/Areas/ttn_content/Views/SATAccount/Register.cshtml", userModel);
                        return false;
                    }
                    userModel.IsLocked = false;


                    var userEntity = new User();
                    userEntity = Mapper.Map<SATAccountRegisterModel, User>(userModel);
                    //var passwordSalt= _encryptionService.CreateSalt();

                    userEntity.CreatedBy = null;
                    userEntity.ModifiedBy = null;
                    userEntity.IsDeleted = false;
                    userEntity.IsSystem = false;
                    userEntity.CreatedDate = userEntity.ModifiedDate = DateTime.Now;

                    userEntity.rowguid = System.Guid.NewGuid();
                    userEntity.IsLocked = false;
                    userEntity.IsDeleted = false;
                    userEntity.IsSystem = null;
                    _membershipService.CreateUser(userEntity);

                    db.UserRoles.Add(new UserRole { UserId = db.Users.SingleOrDefault(x => x.rowguid == userEntity.rowguid).Id, RoleId = 2 });
                    db.SaveChanges();
                    Session.Abandon();
                    //  return Content(Utils.Instance.RenderViewToString("~/Areas/ttn_content/Views/SATAccount/_DisplayMessage.cshtml", null));
                    //return RedirectToRoute("SATTestIndex");
                    return true;
                }
                // return View("~/Areas/ttn_content/Views/SATAccount/Register.cshtml", userModel);
                return false;
            }
            catch (System.Exception ex)
            {
                return false;
                //  return View("~/Areas/ttn_content/Views/SATAccount/Register.cshtml", userModel);
            }


        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            var model = new Web.Models.LoginViewModel();
            model.RememberMe = false;
            return RedirectToAction("Login", "Account");
        }
        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult Login(LoginViewModel model)
        //{
        //    var checkPassword = _membershipService.ValidateMemeber(model.Email,model.Password);

        //    if (checkPassword!=null)
        //    {
        //        Session["AccountEmail"] = model.Email;
        //        Session["TestLogin"] = true;
        //        return new AccountController().Login(model, "SATOnline/Index");

        //        //return RedirectToRoute("SATTestIndex");
        //    }
        //    ModelState.AddModelError("LoginErr", "Tài khoản hoặc mật khẩu chưa đúng");
        //    return View("~/Areas/ttn_content/Views/SATAccount/Login.cshtml",model);
        //}

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
