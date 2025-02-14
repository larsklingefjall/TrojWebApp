using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class CaseNumbersController : IdenityController
    {
        private readonly CourtsConnection _courtConnection;
        private readonly CasesConnection _caseConnection;
        private readonly UserConnection _userConnection;
        private static int _currentCaseId;

        public CaseNumbersController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _courtConnection = new CourtsConnection(context);
            _caseConnection = new CasesConnection(context, configuration["CryKey"]);
            _userConnection = new UserConnection(context);
        }

        // GET: CaseNumbersController/Create
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return NoContent();
            _currentCaseId = id.Value;

            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "Cases", new { id = _currentCaseId });

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            CasesViewModel currentCase = await _caseConnection.GetCase(id.Value);
            if (currentCase == null)
                return NoContent();

            ViewBag.CaseId = id.Value.ToString();
            ViewBag.CaseLinkText = currentCase.CaseType + "/" + currentCase.CaseId.ToString();
            ViewBag.CaseActive = currentCase.Active;

            var caseNumbers = await _caseConnection.GetCaseNumbers(id.Value);
            ViewBag.NumberOfCaseNumbers = caseNumbers.Count();

            List<SelectListItem> courts = new List<SelectListItem>();
            var courtList = await _courtConnection.GetCourts();
            foreach (var court in courtList)
                courts.Add(new SelectListItem { Value = court.CourtId.ToString(), Text = court.CourtName });
            ViewBag.Courts = courts;

            return View();
        }

        // POST: CaseNumbersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "Cases", new { id = _currentCaseId });

            if (!collection.TryGetValue("CaseId", out StringValues caseId))
                return NoContent();

            if (!collection.TryGetValue("CourtId", out StringValues courtId))
                return NoContent();

            if (!collection.TryGetValue("CaseNumber", out StringValues caseNumber))
                return NoContent();

            CaseNumbersModel caseNumberModel = await _caseConnection.CreateCaseNumber(Int32.Parse(caseId.ToString()), Int32.Parse(courtId.ToString()), caseNumber.ToString(), UserName);
            if (caseNumberModel == null)
                return NoContent();

            return RedirectToAction("Details", "Cases", new { id = caseNumberModel.CaseId });
        }

        // GET: CaseNumbersController/Delete/5
        public async Task<ActionResult> Delete(int id, int caseId)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "Cases", new { id = caseId });

            var response = await _caseConnection.DeleteCaseNumber(id);
            if (response == 0)
                return NoContent();
            return RedirectToAction("Details", "Cases", new { id = caseId });
        }
    }
}
