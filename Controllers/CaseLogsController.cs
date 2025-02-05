using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class CaseLogsController : IdenityController
    {
        private readonly CasesConnection _caseConnection;
        private readonly UserConnection _userConnection;

        public CaseLogsController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _caseConnection = new CasesConnection(context, configuration["CryKey"]);
            _userConnection = new UserConnection(context);
        }

        // GET: CaseLogsController
        public async Task<ActionResult> Index(int? id)
        {
            if (id == null)
                return NoContent();

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            CasesViewModel currentCase = await _caseConnection.GetCase(id.Value);
            if (currentCase == null)
                return NoContent();

            ViewBag.CaseId = currentCase.CaseId.ToString();
            ViewBag.CaseLinkText = currentCase.CaseType + "/" + currentCase.CaseId.ToString();
            ViewBag.Client = currentCase.FirstName + " " + currentCase.LastName;
            ViewBag.PersonId = currentCase.PersonId.ToString();

            string currentDate = DateTime.Now.ToShortDateString();
            ViewBag.CurrentDate = currentDate;

            var logs = await _caseConnection.GetCaseLogs(id.Value);
            return View(logs);
        }

        // POST: CaseLogsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            if (!collection.TryGetValue("CaseId", out StringValues caseId))
                return NoContent();

            if (!collection.TryGetValue("Comment", out StringValues comment))
                return NoContent();

            if (!collection.TryGetValue("WhenDate", out StringValues whenDate))
                return NoContent();

            DateTime inputWhenDate = DateTime.Now;
            if (whenDate != "")
                inputWhenDate = DateTime.Parse(whenDate);

            CaseLogsModel caseLog = await _caseConnection.CreateCaseLog(Int32.Parse(caseId.ToString()), inputWhenDate, comment.ToString(), UserName);
            if (caseLog == null)
                return NoContent();

            return RedirectToAction("Index", new { id = caseLog.CaseId });
        }

        // GET: CaseLogsController/Delete/5
        public async Task<ActionResult> Delete(int id, int caseId)
        {
            var response = await _caseConnection.DeleteCaseLog(id);
            if (response == 0)
                return NoContent();
            return RedirectToAction("Index", new { id = caseId });
        }

    }
}
