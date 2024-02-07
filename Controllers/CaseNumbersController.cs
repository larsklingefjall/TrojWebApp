using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
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

        public CaseNumbersController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _courtConnection = new CourtsConnection(context);
            _caseConnection = new CasesConnection(context, configuration["CryKey"]);
        }

        // GET: CaseNumbersController/Create
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return NoContent();

            CasesViewModel currentCase = await _caseConnection.GetCase(id.Value);
            if (currentCase == null)
                return NoContent();

            ViewBag.CaseId = id.Value.ToString();
            ViewBag.CaseLinkText = currentCase.CaseType + "/" + currentCase.CaseId.ToString();

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
            var response = await _caseConnection.DeleteCaseNumber(id);
            if (response == 0)
                return NoContent();
            return RedirectToAction("Details", "Cases", new { id = caseId });
        }
    }
}
