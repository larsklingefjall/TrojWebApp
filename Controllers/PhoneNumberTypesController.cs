using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class PhoneNumberTypesController : IdenityController
    {
        private readonly PhoneNumberTypesConnection _phoneNumberTypesConnection;
        private readonly UserConnection _userConnection;

        public PhoneNumberTypesController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _phoneNumberTypesConnection = new PhoneNumberTypesConnection(context);
            _userConnection = new UserConnection(context);
        }

        // GET: PhoneNumberTypesController
        public async Task<IActionResult> Index()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            ViewBag.CreatePermission = _userConnection.AccessToSubPage(HttpContext.Request, "Create", UserName);

            var types = await _phoneNumberTypesConnection.GetPhoneNumberTypes();
            return View(types);
        }

        // GET: PhoneNumberTypesController/Create
        public async Task<IActionResult> Create(string PhoneNumberType)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (PhoneNumberType == null)
                return NoContent();

            PhoneNumberTypesModel phoneNumberType = await _phoneNumberTypesConnection.CreatePhoneNumberType(PhoneNumberType, UserName);
            if (phoneNumberType == null)
                return NoContent();

            if (phoneNumberType.PhoneNumberType != PhoneNumberType)
                return NoContent();

            var types = await _phoneNumberTypesConnection.GetPhoneNumberTypes();
            return View("Index", types);
        }

    }
}
