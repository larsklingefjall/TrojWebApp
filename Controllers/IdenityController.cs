using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TrojWebApp.Controllers
{
    public class IdenityController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IdenityController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string UserName
        {
            get
            {
                var userName = _userManager.GetUserName(User);
                if (string.IsNullOrEmpty(userName))
                    return "";
                else
                    return userName;
            }
        }
    }
}
