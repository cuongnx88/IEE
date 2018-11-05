using IEE.Service.Abstract;

using System;
using System.Collections.Generic;
using System.Linq;


using System.Security.Principal;
using IEE.Infrastructure.DbContext;
using IEE.Infrastructure.Base;
using IEE.Infrastructure;

namespace IEE.Service
{
    public class MembershipService : IMembershipService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly  IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        

        public MembershipService()
        {
            var unitOfWork = new UnitOfWork();
            _userRepository = unitOfWork.GetRepository<User>();
            _encryptionService = new EncryptionService();
            _roleRepository = unitOfWork.GetRepository<Role>();
        }
        public User CreateUser(User user)
        {
            var existingUser = _userRepository.Get(t => t.Email == user.Email);
            
            if (existingUser != null)
            {
                throw new Exception("Username is already in use");
            }

            var passwordSalt = _encryptionService.CreateSalt();
            
            user.Salt = passwordSalt;
            user.Password = _encryptionService.EncryptPassword(user.Password, passwordSalt);

            _userRepository.InsertAndSubmit(user);

            return user;
        }

        public User GetUser(int userId)
        {
            return _userRepository.GetById(userId);
        }

        public List<Role> GetUserRoles(string email)
        {
            var user = _userRepository.Get(t => t.Email == email);
            var listRoleID = user.UserRoles.Where(u => u.UserId == user.Id).Select(r => r.RoleId).ToList();
            var listRole = _roleRepository.GetMany(r => listRoleID.Contains(r.Id)).ToList();
            return listRole;
        }

        public Membership ValidateUser(string email, string password)
        {
            var membershipCtx = new Membership();
            var user = _userRepository.Get(t => t.Email == email && t.IsDeleted== false);
            if (user != null && isUserValid(user, password))
            {
                var userRoles = GetUserRoles(user.Email);
                membershipCtx.User = user;

                var identity = new GenericIdentity(user.Email);
                membershipCtx.Principal = new GenericPrincipal(
                    identity,
                    userRoles.Select(x => x.Name).ToArray());
            }

            return membershipCtx;
        }

        public User ValidateMemeber(string email, string password)
        {
            var user = _userRepository.Get(t => t.Email == email && t.IsDeleted == false && t.IsLocked == false);
            if (user != null && isUserValid(user, password))
            {
                return user;
            }
            return null;
        }

        private bool isPasswordValid(User user, string password)
        {
            return string.Equals(_encryptionService.EncryptPassword(password, user.Salt), user.Password);
        }

        private bool isUserValid(User user, string password)
        {
            if (isPasswordValid(user, password))
            {
                return !user.IsLocked.Value;
            }

            return false;
        }

        List<Role> IMembershipService.GetUserRoles(string username)
        {
            var user = _userRepository.Get(t => t.Email == username);
           var listRoleID =  user.UserRoles.Where(u => u.UserId == user.Id).Select(r => r.RoleId).ToList();
            var listRole = _roleRepository.GetMany(r => listRoleID.Contains(r.Id)).ToList();
            return listRole;
        }
    }
}
