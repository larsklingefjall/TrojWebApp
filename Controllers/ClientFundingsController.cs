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
    public class ClientFundingsController : IdenityController
    {
        private readonly CasesConnection _caseConnection;
        private readonly UserConnection _userConnection;
        private static int _currentCaseId;

        public ClientFundingsController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _caseConnection = new CasesConnection(context, configuration["CryKey"]);
            _userConnection = new UserConnection(context);
        }

        // GET: ClientFundingsController
        public async Task<ActionResult> Index(int? id)
        {
            if (id == null)
                return NoContent();
            _currentCaseId = id.Value;

            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "Cases", new { id = _currentCaseId });

            ViewBag.CreatePermission = _userConnection.AccessToSubPage(HttpContext.Request, "Create", UserName);
            ViewBag.DeletePermission = _userConnection.AccessToSubPage(HttpContext.Request, "Delete", UserName);

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            CasesViewModel currentCase = await _caseConnection.GetCase(id.Value);
            if (currentCase == null)
                return NoContent();

            ViewBag.CaseId = currentCase.CaseId.ToString();
            ViewBag.CaseLinkText = currentCase.CaseType + "/" + currentCase.CaseId.ToString();
            ViewBag.Client = currentCase.FirstName + " " + currentCase.LastName;
            ViewBag.PersonId = currentCase.PersonId.ToString();
            ViewBag.CaseActive = currentCase.Active;

            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.CurrentDate = currentDate;

            TotalSumModel totalSumModel = await _caseConnection.GetTotalSum(id.Value);
            ViewBag.TotalSum = totalSumModel.TotalSum;

            var clientFundings = await _caseConnection.GetClientFunding(id.Value);
            return View(clientFundings);
        }

        // POST: ClientFundingsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", new { id = _currentCaseId });

            if (!collection.TryGetValue("CaseId", out StringValues caseId))
                return NoContent();

            if (!collection.TryGetValue("ClientSum", out StringValues clientSum))
                return NoContent();

            if (!collection.TryGetValue("Comment", out StringValues comment))
                return NoContent();

            if (!collection.TryGetValue("ClientFundDate", out StringValues clientFundDate))
                return NoContent();

            DateTime inputClientFundDate = DateTime.Now;
            if (clientFundDate != "")
                inputClientFundDate = DateTime.Parse(clientFundDate);

            ClientFundingsModel clientFunding = await _caseConnection.CreateClientFund(Int32.Parse(caseId.ToString()), double.Parse(clientSum.ToString()), inputClientFundDate, comment.ToString(), UserName);
            if (clientFunding == null)
                return NoContent();

            return RedirectToAction("Index", new { id = clientFunding.CaseId });
        }

        // GET: ClientFundingsController/Delete/5
        public async Task<ActionResult> Delete(int id, int caseId)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", new { id = _currentCaseId });

            var response = await _caseConnection.DeleteClientFunding(id);
            if (response == 0)
                return NoContent();
            return RedirectToAction("Index", new { id = caseId });
        }

    }
}
