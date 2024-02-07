using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly PhoneNumberTypesConnection _connection;

        public PhoneNumberTypesController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _connection = new PhoneNumberTypesConnection(context);
        }

        // GET: PhoneNumberTypesController
        public async Task<IActionResult> Index()
        {
            var types = await _connection.GetPhoneNumberTypes();
            return View(types);
        }

        // GET: PhoneNumberTypesController/Create
        public async Task<IActionResult> Create(string PhoneNumberType)
        {
            if (PhoneNumberType == null)
                return NoContent();

            PhoneNumberTypesModel phoneNumberType = await _connection.CreatePhoneNumberType(PhoneNumberType, UserName);
            if (phoneNumberType == null)
                return NoContent();

            if (phoneNumberType.PhoneNumberType != PhoneNumberType)
                return NoContent();

            var types = await _connection.GetPhoneNumberTypes();
            return View("Index", types);
        }

    }
}
