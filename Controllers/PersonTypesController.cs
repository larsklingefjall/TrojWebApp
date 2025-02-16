using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class PersonTypesController : IdenityController
    {
        private readonly PersonTypesConnection _connection;
        private readonly UserConnection _userConnection;

        public PersonTypesController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _connection = new PersonTypesConnection(context);
            _userConnection = new UserConnection(context);
        }

        // GET: PersonTypesController
        public async Task<IActionResult> Index()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            ViewBag.CreatePermission = _userConnection.AccessToSubPage(HttpContext.Request, "Create", UserName);

            var types = await _connection.GetPersonTypes();
            return View(types);
        }

        // GET: PersonTypesController/Create
        public async Task<IActionResult> Create(string PersonType)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (PersonType == null)
                return NoContent();

            PersonTypesModel personType = await _connection.CreatePersonTypes(PersonType, UserName);
            if (personType == null)
                return NoContent();

            if (personType.PersonType != PersonType)
                return NoContent();

            var types = await _connection.GetPersonTypes();
            return View("Index", types);
        }
    }
}
