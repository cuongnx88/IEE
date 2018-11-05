
using System.Collections.Generic;
using IEE.Infrastructure.DbContext;

namespace IEE.Service.Abstract
{
    public interface IMembershipService
    {
        Membership ValidateUser(string email, string password);
        User ValidateMemeber(string email, string password);
        User CreateUser(User user);
        User GetUser(int userId);
        List<Role> GetUserRoles(string username);
    }
}
