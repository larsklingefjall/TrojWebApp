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
    public class InvoicesController : IdenityController
    {
        private readonly InvoiceUnderlaysConnection _invoiceUnderlaysConnection;
        private readonly EmployeesConnection _employeesConnection;
        private readonly ConfigurationsConnection _configurationsConnection;
        private readonly EmployeesConnection _employeeConnection;
        private readonly InvoicesConnection _invoicesConnection;
        private static InvoicesModel _currentInvoice;
        private readonly CasesConnection _caseConnection;


        public InvoicesController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _invoiceUnderlaysConnection = new InvoiceUnderlaysConnection(context, configuration["CryKey"]);
            _employeesConnection = new EmployeesConnection(context);
            _configurationsConnection = new ConfigurationsConnection(context);
            _employeeConnection = new EmployeesConnection(context);
            _invoicesConnection = new InvoicesConnection(context, configuration["CryKey"]);
            _caseConnection = new CasesConnection(context, configuration["CryKey"]);
        }

        // GET: InvoicesController
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

        // GET: InvoicesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            InvoicesViewModel invoice = await _invoicesConnection.GetInvoiceView(id);

            StringBuilder receiver = new StringBuilder();
            receiver.AppendFormat("{0}<br/>", invoice.ReceiverName);
            if (string.IsNullOrEmpty(invoice.CareOf) == false)
                receiver.AppendFormat("C/O: {0}<br/>", invoice.CareOf);
            if (string.IsNullOrEmpty(invoice.StreetName) == false)
                receiver.AppendFormat("{0} {1}<br/>", invoice.StreetName, invoice.StreetNumber);
            receiver.AppendFormat("{0} {1}<br/>", invoice.PostalCode, invoice.PostalAddress);
            if (string.IsNullOrEmpty(invoice.Country) == false)
                receiver.AppendFormat("{0}<br/>", invoice.Country);
            ViewBag.Receiver = receiver.ToString();

            ViewBag.Summaries = await _invoicesConnection.GetInvoiceSummeries(id);
            TotalSumAndHoursModel totalSumAndHours = await _invoicesConnection.GetSumAndHoursOfInvoiceSummeries(id);

            double vat = (double)invoice.Vat;
            double invoiceSum = 0.0;
            double totalSum = 0.0;
            double totalHours = 0.0;
            if (totalSumAndHours != null)
            {
                totalHours = totalSumAndHours.TotalHours;
                totalSum = totalSumAndHours.TotalSum;
                double addVat = 1.0 + (double)vat / 100;
                invoiceSum = (double)totalSum * addVat;
            }
            ViewBag.Vat = invoice.Vat;
            ViewBag.TotalHours = totalHours;
            ViewBag.TotalSum = totalSum;
            ViewBag.TotalSumWithVat = invoiceSum;
            ViewBag.TotalVatSum = invoiceSum - (double)totalSum;

            TotalSumModel totalSumModel = await _invoicesConnection.GetClientFundTotalSum(id);
            ViewBag.ClientFundingTotalSum = totalSumModel.TotalSum;

            return View(invoice);
        }

        // GET: InvoicesController/Invoice/5
        public async Task<IActionResult> Invoice(int id)
        {
            InvoicesViewModel invoice = await _invoicesConnection.GetInvoiceView(id);

            ConfigurationsModel configuration = await _configurationsConnection.GetConfigurationWithkey("AdressFot");
            if (configuration != null)
            {
                ViewBag.AddressFoot = configuration.ConfigValue;
            }

            configuration = await _configurationsConnection.GetConfigurationWithkey("AntalKolumnerAdressFot");
            if (configuration != null)
            {
                ViewBag.NumberOfColumns = configuration.ConfigValue;
            }

            IEnumerable<CaseNumbersViewModel> caseNumbers = await _caseConnection.GetCaseNumbers(invoice.CaseId);
            if (caseNumbers.Count<CaseNumbersViewModel>() == 0)
            {
                ViewBag.CaseNumbers = null;
            }
            else
            {
                ViewBag.CaseNumbers = caseNumbers;
            }

            ViewBag.Summaries = await _invoicesConnection.GetInvoiceSummeries(id);
            TotalSumAndHoursModel totalSumAndHours = await _invoicesConnection.GetSumAndHoursOfInvoiceSummeries(id);

            double vat = (double)invoice.Vat;
            double invoiceSum = 0.0;
            double totalSum = 0.0;
            double totalHours = 0.0;
            if (totalSumAndHours != null)
            {
                totalHours = totalSumAndHours.TotalHours;
                totalSum = totalSumAndHours.TotalSum;
                double addVat = 1.0 + (double)vat / 100;
                invoiceSum = (double)totalSum * addVat;
            }
            ViewBag.Vat = invoice.Vat;
            ViewBag.TotalHours = totalHours;
            ViewBag.TotalSum = totalSum;
            ViewBag.TotalSumWithVat = invoiceSum;
            ViewBag.TotalVatSum = invoiceSum - (double)totalSum;

            var employee = await _employeeConnection.GetEmployee(invoice.EmployeeId);
            ViewBag.EmployeeFirstName = employee.FirstName;
            ViewBag.EmployeeLastName = employee.LastName;
            ViewBag.EmployeeTitle = employee.EmployeeTitle;
            ViewBag.SignatureLink = "https://localhost:44365/images/" + employee.SignatureLink;

            TotalSumModel totalSumModel = await _invoicesConnection.GetClientFundTotalSum(id);
            ViewBag.ClientFundingTotalSum = totalSumModel.TotalSum;
            ViewBag.ToBePaid = 0;
            if (totalSumModel.TotalSum != 0)
            {
                ViewBag.ToBePaid = invoiceSum + totalSumModel.TotalSum;
            }

            return View(invoice);
        }

        // GET: InvoicesController/Create
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return NoContent();

            InvoiceUnderlaysViewModel currentUnderlay = await _invoiceUnderlaysConnection.GetUnderlay(id.Value);
            if (currentUnderlay == null)
                return NoContent();

            ViewBag.CaseId = currentUnderlay.CaseId.ToString();
            ViewBag.CaseLinkText = currentUnderlay.CaseType + "/" + currentUnderlay.CaseId.ToString();
            ViewBag.InvoiceUnderlayId = currentUnderlay.InvoiceUnderlayId;
            ViewBag.InvoiceUnderlayLinkText = currentUnderlay.UnderlayNumber;
            ViewBag.PersonId = currentUnderlay.PersonId;
            ViewBag.ClientLinkText = currentUnderlay.FirstName + " " + currentUnderlay.LastName + "/" + currentUnderlay.PersonId.ToString();

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeesConnection.GetActiveEmployees();
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.EmployeeId.ToString(), Text = employee.Initials });
            ViewBag.Employees = employees;
            ViewBag.EmployeeId = currentUnderlay.EmployeeId.ToString();

            ViewBag.Vat = currentUnderlay.Vat;
            ViewBag.Place = currentUnderlay.UnderlayPlace;

            string currentDate = DateTime.Now.ToShortDateString();
            ViewBag.CurrentDate = currentDate;

            int respite = 30;
            ConfigurationsModel configuration = await _configurationsConnection.GetConfigurationWithkey("Fakturafrist");
            if (configuration != null)
                if (int.TryParse(configuration.ConfigValue, out respite) == false)
                    respite = 30;
            string expirationDate = DateTime.Now.AddDays(respite).ToShortDateString();
            ViewBag.ExpirationDate = expirationDate;

            TotalSumModel totalSumModel = await _caseConnection.GetTotalSum(currentUnderlay.CaseId);
            double roundedClientFundingTotalSum = 0;
            if (totalSumModel != null && totalSumModel.TotalSum != null)
                roundedClientFundingTotalSum = Math.Round((double)totalSumModel.TotalSum, 2);
            ViewBag.ClientFundingTotalSum = roundedClientFundingTotalSum;

            return View();
        }

        // POST: InvoicesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            if (!collection.TryGetValue("InvoiceUnderlayId", out StringValues invoiceUnderlayId))
                return NoContent();

            if (!collection.TryGetValue("EmployeeId", out StringValues employeeId))
                return NoContent();

            if (!collection.TryGetValue("Place", out StringValues place))
                return NoContent();

            if (!collection.TryGetValue("Vat", out StringValues vat))
                return NoContent();

            if (!collection.TryGetValue("Division", out StringValues division))
                return NoContent();

            if (!collection.TryGetValue("InvoiceDate", out StringValues invoiceDate))
                return NoContent();

            if (!collection.TryGetValue("ExpirationDate", out StringValues expirationDate))
                return NoContent();

            if (!collection.TryGetValue("ClientFundingTotalSum", out StringValues clientFundingTotalSum))
                return NoContent();


            InvoicesModel newInvoice = await _invoicesConnection.CreateInvoice(int.Parse(invoiceUnderlayId), int.Parse(employeeId), place, DateTime.Parse(invoiceDate), DateTime.Parse(expirationDate), int.Parse(vat), int.Parse(division), double.Parse(clientFundingTotalSum), UserName);
            if (newInvoice == null)
                return NoContent();

            return RedirectToAction("Details", new { id = newInvoice.InvoiceId });
        }

        // GET: InvoicesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            _currentInvoice = await _invoicesConnection.GetInvoice(id);
            if (_currentInvoice == null)
                return NoContent();

            ViewBag.InvoiceId = _currentInvoice.InvoiceId.ToString();
            ViewBag.InvoiceLinkText = _currentInvoice.InvoiceNumber;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.EmployeeId.ToString(), Text = employee.Initials });
            ViewBag.Employees = employees;

            return View(_currentInvoice);
        }

        // POST: InvoicesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            if (!collection.TryGetValue("EmployeeId", out StringValues employeeId))
                return NoContent();

            if (!collection.TryGetValue("InvoiceDate", out StringValues invoiceDate))
                return NoContent();

            if (!collection.TryGetValue("ExpirationDate", out StringValues expirationDate))
                return NoContent();

            if (!collection.TryGetValue("InvoicePlace", out StringValues invoicePlace))
                return NoContent();

            if (!collection.TryGetValue("Headline1", out StringValues headline1))
                return NoContent();

            if (!collection.TryGetValue("Headline2", out StringValues headline2))
                return NoContent();

            if (!collection.TryGetValue("Text1", out StringValues text1))
                return NoContent();

            bool locked = false;
            string[] str = collection["Locked"].ToArray();
            if (str.Length > 1)
            {
                locked = true;
            }

            bool hideClientFunding = false;
            str = collection["HideClientFunding"].ToArray();
            if (str.Length > 1)
            {
                hideClientFunding = true;
            }

            InvoicesModel updatedInvoice = await _invoicesConnection.UpdateInvoice(id, int.Parse(employeeId), DateTime.Parse(invoiceDate), DateTime.Parse(expirationDate), invoicePlace, headline1, headline2, text1, locked, hideClientFunding, _currentInvoice, UserName);
            if (updatedInvoice == null)
                return NoContent();

            return RedirectToAction("Details", new { id = updatedInvoice.InvoiceId });
        }

        public ActionResult Reset()
        {
            return RedirectToAction("Index", new { reset = 1 });
        }
    }
}
