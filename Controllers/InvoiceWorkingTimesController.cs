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
    public class InvoiceWorkingTimesController : IdenityController
    {
        private readonly CasesConnection _caseConnection;
        private readonly WorkingTimesConnection _workingTimesConnection;
        private readonly InvoiceUnderlaysConnection _invoiceUnderlaysConnection;
        private readonly UserConnection _userConnection;
        private static int _invoiceUnderlayId;

        public InvoiceWorkingTimesController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _caseConnection = new CasesConnection(context, configuration["CryKey"]);
            _workingTimesConnection = new WorkingTimesConnection(context, configuration["CryKey"]);
            _invoiceUnderlaysConnection = new InvoiceUnderlaysConnection(context, configuration["CryKey"]);
            _userConnection = new UserConnection(context);
        }

        // GET: InvoiceWorkingTimesController
        public async Task<ActionResult> Index(int? id)
        {
            if (id == null)
                return NoContent();
            _invoiceUnderlayId = id.Value;

            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "InvoiceUnderlays", new {id = _invoiceUnderlayId });

            ViewBag.MovePermission = _userConnection.AccessToSubPage(HttpContext.Request, "MoveWorkingTime", UserName);
            ViewBag.MoveAllPermission = _userConnection.AccessToSubPage(HttpContext.Request, "MoveAllWorkingTimes", UserName);
            ViewBag.RemovePermission = _userConnection.AccessToSubPage(HttpContext.Request, "RemoveWorkingTime", UserName);
            ViewBag.RemoveAllPermission = _userConnection.AccessToSubPage(HttpContext.Request, "RemoveAllWorkingTimes", UserName);


            InvoiceUnderlaysViewModel underlay = await _invoiceUnderlaysConnection.GetUnderlay(_invoiceUnderlayId);
            if (underlay == null)
                return NoContent();

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            ViewBag.InvoiceUnderlayId = underlay.InvoiceUnderlayId;
            ViewBag.UnderlayLinkText = underlay.UnderlayNumber;
            ViewBag.CaseId = underlay.CaseId.ToString();
            ViewBag.CaseLinkText = underlay.CaseType + "/" + underlay.CaseId.ToString();
            ViewBag.UnderlayLocked = underlay.Locked;

            ViewBag.WorkinTimes = await _workingTimesConnection.GetWorkingTimesForCaseNotBilled(underlay.CaseId);

            IEnumerable<InvoiceWorkingTimesViewModel> workingTimes = await _invoiceUnderlaysConnection.GetUnderlayWorkingTimes(_invoiceUnderlayId);
            return View(workingTimes);
        }

        // POST: CasesController/MoveWorkingTime/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MoveWorkingTime(IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", new { id = _invoiceUnderlayId });

            if (collection.Count == 0)
            {
                return NoContent();
            }

            if (!collection.TryGetValue("WorkingTimeId", out StringValues workingTimeId))
                return NoContent();

            if (!collection.TryGetValue("InvoiceUnderlayId", out StringValues invoiceUnderlayId))
                return NoContent();

            await _invoiceUnderlaysConnection.MoveWorkingTime(Int32.Parse(workingTimeId.ToString()), Int32.Parse(invoiceUnderlayId.ToString()), UserName);

            return RedirectToAction("Index", new { id = Int32.Parse(invoiceUnderlayId.ToString()) }); 
        }

        // POST: CasesController/MoveWorkingTime/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MoveAllWorkingTimes(IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", new { id = _invoiceUnderlayId });

            if (collection.Count == 0)
            {
                return NoContent();
            }

            if (!collection.TryGetValue("CaseId", out StringValues caseId))
                return NoContent();

            if (!collection.TryGetValue("InvoiceUnderlayId", out StringValues invoiceUnderlayId))
                return NoContent();

            await _invoiceUnderlaysConnection.MoveAllWorkingTimes(Int32.Parse(caseId.ToString()), Int32.Parse(invoiceUnderlayId.ToString()));

            return RedirectToAction("Index", new { id = Int32.Parse(invoiceUnderlayId.ToString()) });
        }

        // POST: CasesController/RemoveWorkingTime/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveWorkingTime(IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", new { id = _invoiceUnderlayId });

            if (collection.Count == 0)
            {
                return NoContent();
            }

            if (!collection.TryGetValue("InvoiceWorkingTimeId", out StringValues invoiceWorkingTimeId))
                return NoContent();

            if (!collection.TryGetValue("InvoiceUnderlayId", out StringValues invoiceUnderlayId))
                return NoContent();

            await _invoiceUnderlaysConnection.RemoveWorkingTime(Int32.Parse(invoiceWorkingTimeId.ToString()));

            return RedirectToAction("Index", new { id = Int32.Parse(invoiceUnderlayId.ToString()) });
        }

        // POST: CasesController/RemoveWorkingTime/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveAllWorkingTimes(IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", new { id = _invoiceUnderlayId });

            if (collection.Count == 0)
            {
                return NoContent();
            }

            if (!collection.TryGetValue("InvoiceUnderlayId", out StringValues invoiceUnderlayId))
                return NoContent();

            await _invoiceUnderlaysConnection.RemoveAllWorkingTimes(Int32.Parse(invoiceUnderlayId.ToString()));

            return RedirectToAction("Index", new { id = Int32.Parse(invoiceUnderlayId.ToString()) });
        }
    }
}
