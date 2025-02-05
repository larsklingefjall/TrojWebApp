using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
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
    public class InvoiceSummarysController : IdenityController
    {
        private readonly TariffTypesConnection _tariffTypesConnection;
        private readonly InvoicesConnection _invoicesConnection;
        private readonly UserConnection _userConnection;

        public InvoiceSummarysController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _tariffTypesConnection = new TariffTypesConnection(context);
            _invoicesConnection = new InvoicesConnection(context, configuration["CryKey"]);
            _userConnection = new UserConnection(context);
        }

        // GET: InvoiceSummarysController
        public async Task<ActionResult> Index(int? id)
        {
            if (id == null)
                return NoContent();

            InvoicesModel currentInvoice = await _invoicesConnection.GetInvoice(id.Value);
            if (currentInvoice == null)
                return NoContent();

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            ViewBag.InvoiceId = currentInvoice.InvoiceId.ToString();
            ViewBag.InvoiceLinkText = currentInvoice.InvoiceNumber;

            var tariffTypeList = await _tariffTypesConnection.GetActiveTariffTypeAndLevel();

            List<SelectListItem> tariffTypes = new List<SelectListItem>();
            foreach (var item in tariffTypeList)
                if (item.TariffLevel > 0)
                    tariffTypes.Add(new SelectListItem { Value = item.TariffTypeId.ToString(), Text = item.TariffType + ", " + item.TariffLevel });
            ViewBag.TariffTypesWithLevel = tariffTypes;

            tariffTypes = new List<SelectListItem>();
            foreach (var item in tariffTypeList)
                if (item.TariffLevel == 0)
                    tariffTypes.Add(new SelectListItem { Value = item.TariffTypeId.ToString(), Text = item.TariffType });
            ViewBag.TariffTypesNoneLevel = tariffTypes;

            TotalSumAndHoursModel totalSumAndHours = await _invoicesConnection.GetSumAndHoursOfInvoiceSummeries(id.Value);
            ViewBag.TotalSum = totalSumAndHours.TotalSum;
            ViewBag.TotalHours = totalSumAndHours.TotalHours;

            IEnumerable <InvoiceSummarysViewModel> summeries = await _invoicesConnection.GetInvoiceSummeries(id.Value);
            return View(summeries);
        }

        // POST: InvoiceSummarysController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            if (!collection.TryGetValue("InvoiceId", out StringValues invoiceId))
                return NoContent();

            if (!collection.TryGetValue("TariffTypeId", out StringValues tariffTypeId))
                return NoContent();

            if (!collection.TryGetValue("UnitCounts", out StringValues unitCounts))
                return NoContent();

            await _invoicesConnection.CreateInvoiceSummary(int.Parse(invoiceId), int.Parse(tariffTypeId), double.Parse(unitCounts));

            return RedirectToAction("Index", new { id = int.Parse(invoiceId) });
        }

        // POST: InvoiceSummarysController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNoneLevel(IFormCollection collection)
        {
            if (!collection.TryGetValue("InvoiceId", out StringValues invoiceId))
                return NoContent();

            if (!collection.TryGetValue("TariffTypeId", out StringValues tariffTypeId))
                return NoContent();

            if (!collection.TryGetValue("UnitCounts", out StringValues unitCounts))
                return NoContent();

            double unitCost = 0;
            if (collection.TryGetValue("UnitCost", out StringValues inputUnitCost))
                unitCost = double.Parse(inputUnitCost);

            await _invoicesConnection.CreateInvoiceSummary(int.Parse(invoiceId), int.Parse(tariffTypeId), double.Parse(unitCounts), unitCost, UserName);

            return RedirectToAction("Index", new { id = int.Parse(invoiceId) });
        }

        // POST: InvoiceSummarysController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            if (!collection.TryGetValue("InvoiceSummaryId", out StringValues invoiceSummaryId))
                return NoContent();

            if (!collection.TryGetValue("InvoiceId", out StringValues invoiceId))
                return NoContent();

            if (!collection.TryGetValue("UnitCounts", out StringValues unitCounts))
                return NoContent();

            int response = await _invoicesConnection.UpdateInvoiceSummary(int.Parse(invoiceSummaryId), double.Parse(unitCounts), UserName);
            if (response == 0)
                return NoContent();

            return RedirectToAction("Index", new { id = invoiceId });
        }

        // GET: InvoiceSummarysController/Delete/5
        public async Task<ActionResult> Delete(int invoiceSummaryId, int invoiceId)
        {
            int response = await _invoicesConnection.DeleteInvoiceSummary(invoiceSummaryId);
            if (response == 0)
                return NoContent();
            return RedirectToAction("Index", new { id = invoiceId });
        }
    }
}
