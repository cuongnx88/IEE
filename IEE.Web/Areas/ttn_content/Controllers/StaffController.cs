using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IEE.Service.Abstract;
using IEE.ViewModel;
using IEE.Infrastructure.DbContext;
using AutoMapper;
using System.Configuration;
using PagedList;
using System.IO;
using System.Data;
using IEE.Web.Models;
using IEE.Infrastructure;
using IEE.Service;

namespace IEE.Web.Areas.ttn_content.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StaffController : BaseController
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IMembershipService _memeberService;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        public StaffController()
        {
            var unitOfWork = new UnitOfWork();
            
            _encryptionService = new EncryptionService();
            _memeberService = new MembershipService();
            _userRepository = unitOfWork.GetRepository<User>();
            _roleRepository = unitOfWork.GetRepository<Role>();
        }

        public ActionResult Index(int? page, string CurrentFilter, string keyword)
        {
            var model = _userRepository.GetMany(t => t.IsDeleted == false);

            if (!string.IsNullOrEmpty(keyword))
            {
                model = model.Where(t => t.Name.ToLower().Contains(keyword.ToLower()));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                page = 1;
            }
            else
            {
                keyword = CurrentFilter;
            }

            ViewBag.CurrentFilter = keyword;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(t => t.Name).ThenByDescending(t => t.Id).ToPagedList(pageNumber, int.Parse(ConfigurationManager.AppSettings["PageSize"])));

        }

        public ActionResult Create()
        {
            var roles = _roleRepository.GetMany(t => t.Id != 2 && t.Id != 3);
            ViewBag.Roles = roles;
            return View();
        }
        [HttpPost]

        public ActionResult Create(UserViewModel model, string[] selectedRoles)
        {
            if (!ModelState.IsValid)
            {
                var roles = _roleRepository.GetMany(t => t.Id != 2 && t.Id != 3);
                ViewBag.Roles = roles;
                return View(model);
            }
            try
            {
                var user = Mapper.Map<UserViewModel, User>(model);

                var existingUser = _userRepository.Get(t => t.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email của học viên đã được sử dụng.");
                    return View(model);
                }
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
                user.Photo = file_name;



                user.CreatedBy = User.UserId;
                user.ModifiedBy = User.UserId;
                user.IsDeleted = false;
                user.IsSystem = false;
                user.CreatedDate = user.ModifiedDate = DateTime.Now;
                var createUser = _memeberService.CreateUser(user);
                if (createUser != null)
                {
                    using (var db = new UnitOfWork())
                    {
                        //process role
                        if (selectedRoles != null && selectedRoles.Count() > 0)
                        {
                            int[] uniInts = selectedRoles.Select(int.Parse).ToArray();
                            var roles = _roleRepository.GetMany(t => uniInts.Contains(t.Id));
                            foreach (var u in roles)
                            {
                                var _userRole = new UserRole { RoleId = u.Id, UserId = createUser.Id };
                                var userRoleRepo = db.GetRepository<UserRole>();
                                userRoleRepo.Insert(_userRole);
                                db.Commit();
                                user.UserRoles.Add(_userRole);
                            }
                        }

                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                throw new Exception();
            }
        }
        public ActionResult Edit(int id)
        {
            var model = _userRepository.Get(t => t.Id == id);
            //var model   = Mapper.Map<User, UserViewModel> (user);

            var roles = _roleRepository.GetMany(t => t.Id != 2 && t.Id != 3);
            ViewBag.Roles = roles;
            ViewBag.UserRoles = model.UserRoles.Select(r=>r.Role).ToList() ;


            return View(model);
        }
        [HttpPost]

        public ActionResult Edit(User user, string[] selectedRoles, int id)
        {
            if (!ModelState.IsValid)
            {
                var roles = _roleRepository.GetMany(t => t.Id != 2 && t.Id != 3);
                ViewBag.Roles = roles;

                user = _userRepository.Get(t => t.Id == user.Id);
                ViewBag.UserRoles = user.UserRoles;
                return View(user);
            }
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
                }

            }
            if (!string.IsNullOrEmpty(file_name))
                user.Photo = file_name;
            //process role
            UpdateUserRoles(selectedRoles, user);
            user.ModifiedBy = User.UserId;
            user.ModifiedDate = DateTime.Now;
            user.IsDeleted = false;
            user.IsSystem = false;
            using (var unitWork = new UnitOfWork())
            {
                var repo = unitWork.GetRepository<User>();
                var _user = repo.GetById(id);
                user.Password = _user.Password;
            }
            //string password = _userRepository.GetById(id).Password;
            //user.Password = password;
            var editModel = Mapper.Map<User, User>(user);
            _userRepository.UpdateAndSubmit(editModel);

            return RedirectToAction("Index");
        }
        public ActionResult ChangePassword(int id)
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            model.Id = id;
            return PartialView("_ChangePassword", model);
        }
        [HttpPost]

        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_ChangePassword", model);
            try
            {
                var user = _userRepository.Get(t => t.Id == model.Id);

                user.Password = _encryptionService.EncryptPassword(model.NewPassword, user.Salt);
                _userRepository.UpdateAndSubmit(user);
                return RedirectToAction("edit", "staff", new { Areas = "ttn_content", id = model.Id });
            }
            catch
            {
                throw new Exception();
            }
        }
        public ActionResult Delete(int id)
        {
            try
            {
                var model = _userRepository.Get(t => t.Id == id);

                if (model.Email.Equals(User.Name) || (model.IsSystem.HasValue && model.IsSystem.Value))
                {
                    ModelState.AddModelError("", "Không cho phép xóa. bạn hãy thử lại hoặc liên hệ với quản trị hệ thống.");
                    return RedirectToAction("index");
                }

                model.IsDeleted = true;
                model.ModifiedBy = User.UserId;
                model.ModifiedDate = DateTime.Now;
                _userRepository.UpdateAndSubmit(model);
                return RedirectToAction("index");
            }
            catch (Exception)
            {
                return RedirectToAction("index");
            }
        }

        public FileResult Download(string fileName)
        {
            var filepath = Path.Combine(Server.MapPath("/Photo/staff/"), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }
        public ActionResult Import()
        {
            DataSet ds = this.Render(Request);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                try
                {
                    User user = new User();
                    user.Name = ds.Tables[0].Rows[i][0].ToString();
                    user.Email = ds.Tables[0].Rows[i][1].ToString();
                    user.Phone = ds.Tables[0].Rows[i][2].ToString();
                    user.Birthday = DateTime.Now;

                    var existUser = _userRepository.GetMany(t => t.Email.Equals(user.Email)).FirstOrDefault();
                    if (existUser != null)
                        continue;

                    user.CreatedBy = user.ModifiedBy = User.UserId;
                    user.CreatedDate = user.ModifiedDate = DateTime.Now;
                    user.IsDeleted = false;
                    user.IsLocked = false;
                    user.IsSystem = false;
                    user.Password = "iee@123";
                    var role = _roleRepository.Get(t => t.Id == 1);
                    // user.Roles.Add(role);
                    var createUser = _memeberService.CreateUser(user);

                    using (var db = new UnitOfWork())
                    {
                        //process role

                        var _userRole = new UserRole { RoleId = role.Id, UserId = createUser.Id };
                        var userRoleRepo = db.GetRepository<UserRole>();
                        userRoleRepo.Insert(_userRole);
                        db.Commit();
                        user.UserRoles.Add(_userRole);

                    }
                }
                catch (Exception)
                {
                    continue;
                }

            }

            return RedirectToAction("Index");
        }
        private void UpdateUserRoles(string[] selectedRoles, User userToUpdate)
        {
            if (selectedRoles.Count() == 0)
            {
                userToUpdate.UserRoles = new List<UserRole>();
                return;
            }
            var selectedRolesHs = new HashSet<string>(selectedRoles);
            var roleIds = new HashSet<int>(userToUpdate.UserRoles.Select(t => t.RoleId));
            var roles = _roleRepository.GetMany(t => t.IsDeleted == false);

            foreach (var role in roles)
            {
                if (selectedRoles.Contains(role.Id.ToString()))
                {
                    if (!roleIds.Contains(role.Id))
                    {
                        var _userRole = new UserRole {RoleId= role.Id,UserId= userToUpdate.Id};
                        userToUpdate.UserRoles.Add(_userRole);
                    }
                }
                else
                {
                    if (roleIds.Contains(role.Id))
                    {
                        var _userRole = new UserRole { RoleId = role.Id, UserId = userToUpdate.Id };
                        userToUpdate.UserRoles.Remove(_userRole);
                    }
                }
            }

        }
    }
}