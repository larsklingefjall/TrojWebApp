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
using System.Text;
using System.Threading.Tasks;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class FortknoxController : IdenityController
    {
        private readonly InvoiceUnderlaysConnection _invoiceUnderlaysConnection;
        private readonly EmployeesConnection _employeesConnection;
        private readonly ConfigurationsConnection _configurationsConnection;
        private readonly EmployeesConnection _employeeConnection;
        private readonly InvoicesConnection _invoicesConnection;
        private static InvoicesModel _currentInvoice;
        private readonly CasesConnection _caseConnection;


        public FortknoxController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _invoiceUnderlaysConnection = new InvoiceUnderlaysConnection(context, configuration["CryKey"]);
            _employeesConnection = new EmployeesConnection(context);
            _configurationsConnection = new ConfigurationsConnection(context);
            _employeeConnection = new EmployeesConnection(context);
            _invoicesConnection = new InvoicesConnection(context, configuration["CryKey"]);
            _caseConnection = new CasesConnection(context, configuration["CryKey"]);
        }

        // GET: FortknoxController
        public async Task<IActionResult> Index(int? page, int? size, int? reset, IFormCollection collection)
        {
            if (HttpContext.Session.GetInt32("TrojInvoiceSize").HasValue == false)
            {
                HttpContext.Session.SetInt32("TrojInvoiceSize", 20);
            }

            if (size != null)
            {
                HttpContext.Session.SetInt32("TrojInvoiceSize", size.Value);
            }

            int iSize = HttpContext.Session.GetInt32("TrojInvoiceSize").Value;
            ViewBag.Size = iSize.ToString();

            int iPage = 0;
            int iOffset = 0;
            if (page != null)
            {
                iPage = page.Value;
                iOffset = (iPage - 1) * iSize;
            }
            ViewBag.Page = iPage;

            HttpContext.Session.SetInt32("TrojInvoiceStart", 1);
            HttpContext.Session.SetInt32("TrojInvoiceEnd", 10);
            if (iPage > HttpContext.Session.GetInt32("TrojInvoiceEnd"))
            {
                HttpContext.Session.SetInt32("TrojInvoiceStart", iPage - 1);
                HttpContext.Session.SetInt32("TrojInvoiceEnd", iPage + 9);
            }

            if (HttpContext.Session.GetInt32("TrojInvoiceStart") == 1)
            {
                ViewBag.Start = 0;
            }
            else
            {
                ViewBag.Start = HttpContext.Session.GetInt32("TrojInvoiceStart") - 1;
            }

            ViewBag.TrojInvoiceStart = HttpContext.Session.GetInt32("TrojInvoiceStart").Value;

            int trojInvoiceEnd = HttpContext.Session.GetInt32("TrojInvoiceEnd").Value;
            ViewBag.TrojInvoiceEnd = trojInvoiceEnd;

            if (collection.Count > 0)
            {
                HttpContext.Session.SetString("TrojInvoiceTimeFilter", "Set");
                collection.TryGetValue("InvoiceNumber", out StringValues invoiceNumber);
                collection.TryGetValue("UnderlayNumber", out StringValues underlayNumber);
                collection.TryGetValue("InvoiceDate", out StringValues invoiceDate);
                collection.TryGetValue("ReceiverName", out StringValues receiverName);
                collection.TryGetValue("EmployeeId", out StringValues employeeId);
                collection.TryGetValue("FirstName", out StringValues firstName);
                collection.TryGetValue("LastName", out StringValues lastName);
                collection.TryGetValue("Locked", out StringValues locked);
                HttpContext.Session.SetString("TrojInvoiceInvoiceNumber", invoiceNumber.ToString());
                HttpContext.Session.SetString("TrojInvoiceUnderlayNumber", underlayNumber.ToString());
                HttpContext.Session.SetString("TrojInvoiceInvoiceDate", invoiceDate.ToString());
                HttpContext.Session.SetString("TrojInvoiceReceiverName", receiverName.ToString());
                HttpContext.Session.SetString("TrojInvoiceEmployeeId", employeeId.ToString());
                HttpContext.Session.SetString("TrojInvoiceFirstName", firstName.ToString());
                HttpContext.Session.SetString("TrojInvoiceLastName", lastName.ToString());
                HttpContext.Session.SetString("TrojInvoiceLocked", locked.ToString());
            }

            if (reset != null)
            {
                HttpContext.Session.SetString("TrojInvoiceTimeFilter", "Reset");
                HttpContext.Session.Remove("TrojInvoiceInvoiceNumber");
                HttpContext.Session.Remove("TrojInvoiceUnderlayNumber");
                HttpContext.Session.Remove("TrojInvoiceInvoiceDate");
                HttpContext.Session.Remove("TrojInvoiceReceiverName");
                HttpContext.Session.Remove("TrojInvoiceEmployeeId");
                HttpContext.Session.Remove("TrojInvoiceFirstName");
                HttpContext.Session.Remove("TrojInvoiceLastName");
                HttpContext.Session.Remove("TrojInvoiceLocked");
            }

            IEnumerable<InvoicesViewModel> invoices;
            int numberOfInvoices;
            if (HttpContext.Session.GetString("TrojInvoiceTimeFilter") == "Set")
            {
                string invoiceNumber = HttpContext.Session.GetString("TrojInvoiceInvoiceNumber");
                string underlayNumber = HttpContext.Session.GetString("TrojInvoiceUnderlayNumber");
                string invoiceDate = HttpContext.Session.GetString("TrojInvoiceInvoiceDate");
                string receiverName = HttpContext.Session.GetString("TrojInvoiceReceiverName");
                string employeeId = HttpContext.Session.GetString("TrojInvoiceEmployeeId");
                string firstName = HttpContext.Session.GetString("TrojInvoiceFirstName");
                string lastName = HttpContext.Session.GetString("TrojInvoiceLastName");
                string locked = HttpContext.Session.GetString("TrojInvoiceLocked");
                invoices = await _invoicesConnection.GetFilyeredInvoices(invoiceNumber, underlayNumber, invoiceDate, receiverName, employeeId, firstName, lastName, locked, iOffset, iSize);
                numberOfInvoices = await _invoicesConnection.GetNumberOfFilteredInvoices(invoiceNumber, underlayNumber, invoiceDate, receiverName, employeeId, firstName, lastName, locked);
                ViewBag.InvoiceNumber = invoiceNumber;
                ViewBag.UnderlayNumber = underlayNumber;
                ViewBag.InvoiceDate = invoiceDate;
                ViewBag.ReceiverName = receiverName;
                ViewBag.EmployeeId = employeeId;
                ViewBag.FirstName = firstName;
                ViewBag.LastName = lastName;
                ViewBag.Locked = locked;
            }
            else
            {
                invoices = await _invoicesConnection.GetInvoices(iOffset, iSize);
                numberOfInvoices = await _invoicesConnection.GetNumberOfInvoices();
                ViewBag.CaseId = "";
                ViewBag.WhenDate = "";
                ViewBag.CaseTypeId = "";
                ViewBag.EmployeeId = "";
                ViewBag.CaseTitle = "";
                ViewBag.FirstName = "";
                ViewBag.LastName = "";
                ViewBag.Locked = "";
            }

            double dMaxPage = Math.Ceiling((double)numberOfInvoices / iSize);
            int maxPage = Convert.ToInt32(dMaxPage);
            ViewBag.MaxPage = maxPage;

            ViewBag.ShowMaxPage = false;
            if (trojInvoiceEnd < maxPage - 1)
            {
                ViewBag.ShowMaxPage = true;
            }

            List<SelectListItem> listSizes = new List<SelectListItem>();
            listSizes.Add(new SelectListItem { Value = "5", Text = "5" });
            listSizes.Add(new SelectListItem { Value = "10", Text = "10" });
            listSizes.Add(new SelectListItem { Value = "20", Text = "20" });
            listSizes.Add(new SelectListItem { Value = "40", Text = "40" });
            listSizes.Add(new SelectListItem { Value = "80", Text = "80" });
            ViewBag.ListSizes = listSizes;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeesConnection.GetActiveEmployees();
            employees.Add(new SelectListItem { Value = "0", Text = "" });
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.EmployeeId.ToString(), Text = employee.Initials });
            ViewBag.Employees = employees;

            return View(invoices);
        }


        // GET: FortknoxController/Activate
        public ActionResult Activate()
        {
            return View();
        }

        public ActionResult Reset()
        {
            return RedirectToAction("Index", new { reset = 1 });
        }

    }
}
