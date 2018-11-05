using System;
using System.Collections.Generic;
using System.Linq;
using IEE.ViewModel;
using System.Web.Mvc;
using IEE.Infrastructure.DbContext;

using AutoMapper;
using PagedList;
using System.Configuration;
using System.IO;
using System.Web;
using IEE.Web.Business;
using IEE.Web.Models;
using IEE.Service.Abstract;
using IEE.Infrastructure;
using IEE.Service;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly IRepository<User> _userRepository;
        private readonly IEncryptionService _encryptionService;
        public ProfileController()
        {
            var unitOfWork = new UnitOfWork();
           
            _userRepository = unitOfWork.GetRepository<User>();
            _encryptionService = new EncryptionService();
        }
        //Staff
        //------------------------------------------------------------------------//
        #region Staff
        [Authorize(Roles = "Admin,Editor,Business,Accounting")]
        public ActionResult Staff()
        {
            var user = _userRepository.Get(t => t.Id == User.UserId);
            TeacherViewModel model = new TeacherViewModel();
            MappUserToModel(model, user);
            return View(model);

        }
        [Authorize(Roles = "Admin,Editor,Business,Accounting")]
        [HttpPost]
        public ActionResult Staff(TeacherViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _userRepository.Get(t => t.Id == User.UserId);

            MappModelToUser(model, user);
            //process upload an image
            string file_name = string.Empty, file_src = string.Empty;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    file_name = Guid.NewGuid() + Path.GetExtension(fileName);
                    file_src = Path.Combine(Server.MapPath("~/photo/staff/"), file_name);
                    file.SaveAs(file_src);
                    break;
                }
            }
            if (!string.IsNullOrEmpty(file_name))
            {
                user.Photo = file_name;
            }
            user.ModifiedBy = User.UserId;
            user.ModifiedDate = DateTime.Now;

            _userRepository.UpdateAndSubmit(user);
            return View(model);
        }

        [Authorize(Roles = "Admin,Editor,Business,Accounting")]
        public FileResult StaffDownload(string fileName)
        {
            var filepath = Path.Combine(Server.MapPath("/Photo/staff/"), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }
        #endregion

        //Common
        //------------------------------------------------------------------------//
        #region Common
        [Authorize(Roles = "Teacher,Student,Admin,Editor,Business,Accounting")]
        public ActionResult ChangePassword()
        {
            return View();
        }
        [Authorize(Roles = "Teacher,Student,Admin,Editor,Business,Accounting")]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = _userRepository.Get(t => t.Id == User.UserId);
            user.Password = _encryptionService.EncryptPassword(model.NewPassword, user.Salt);
            _userRepository.UpdateAndSubmit(user);
            return RedirectToAction("staff", "profile");

        }
        
        

        public ActionResult SettingPassword(int id)
        {
            SettingPasswordViewModel model = new SettingPasswordViewModel();
            model.Id = id;
            ViewBag.From= User.UserId;
            return View(model);
        }
        [HttpPost]
        public ActionResult SettingPassword(SettingPasswordViewModel model, int from)
        {
            using (var unitWork= new UnitOfWork())
            {
                var repo = unitWork.GetRepository<User>();
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var user = repo.Get(t => t.Id == model.Id);
                user.Password = _encryptionService.EncryptPassword(model.NewPassword, user.Salt);

                repo.UpdateAndSubmit(user);
            }
           
            return RedirectToAction("edit", "staff", new { id = model.Id });
        }
        #endregion

        //Private
        //------------------------------------------------------------------------//
        private void MappModelToUser(TeacherViewModel mode, User teacher)
        {
            teacher.Email = mode.Email;
            teacher.Address = mode.Address;
            teacher.Birthday = mode.Birthday;
            teacher.Name = mode.Name;
            teacher.Photo = mode.Photo;
            teacher.Phone = mode.Phone;
        }
        private void MappUserToModel(TeacherViewModel mode, User teacher)
        {
            mode.Id = teacher.Id;
            mode.Email = teacher.Email;
            mode.Address = teacher.Address;
            if(teacher.Birthday.HasValue)
                mode.Birthday = teacher.Birthday.Value;
            mode.Name = teacher.Name;
            mode.Photo = teacher.Photo;
            mode.Phone = teacher.Phone;
        }
    }
}