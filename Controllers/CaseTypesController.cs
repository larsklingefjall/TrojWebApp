using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrojWebApp.Models;
using TrojWebApp.Services;


namespace TrojWebApp.Controllers
{
    [Authorize]
    public class CaseTypesController : IdenityController
    {
        private readonly CaseTypesConnection _caseTypeConnection;
        private readonly UserConnection _userConnection;

        public CaseTypesController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _caseTypeConnection = new CaseTypesConnection(context);
            _userConnection = new UserConnection(context);
        }

        public async Task<IActionResult> Index()
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            ViewBag.CreatePermission = _userConnection.AccessToSubPage(HttpContext.Request, "Create", UserName);

            var types = await _caseTypeConnection.GetCaseTypes();
            return View(types);
        }

        public async Task<IActionResult> Create(string CaseType)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (CaseType == null)
                return NoContent();

            CaseTypesModel caseType = await _caseTypeConnection.CreateCaseTypes(CaseType, UserName);
            if (caseType == null)
                return NoContent();

            if (caseType.CaseType != CaseType)
                return NoContent();

            return RedirectToAction("Index");
        }
    }
}
