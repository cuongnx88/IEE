using System.Linq;
using System.Security.Principal;

namespace IEE.Model
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role)
        {
            if (Roles.Any(r => role.Contains(r)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public CustomPrincipal(string email)
        {
            this.Identity = new GenericIdentity(email);
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string[] Roles { get; set; }
    }

    public class CustomPrincipalSerializeModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string[] Roles { get; set; }
    }
}
