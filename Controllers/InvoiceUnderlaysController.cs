﻿using Microsoft.AspNetCore.Authorization;
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
    public class InvoiceUnderlaysController : IdenityController
    {
        private readonly CasesConnection _caseConnection;
        private readonly EmployeesConnection _employeesConnection;
        private readonly CourtsConnection _courtsConnection;
        private readonly ConfigurationsConnection _configurationsConnection;
        private readonly CaseTypesConnection _caseTypesConnection;
        private readonly InvoicesConnection _invoicesConnection;
        private readonly InvoiceUnderlaysConnection _invoiceUnderlaysConnection;
        private static InvoiceUnderlaysModel _currentUnderlay;
        private readonly UserConnection _userConnection;
        private static int _currentCaseId;
        private static int _currentUnderlayId;

        public InvoiceUnderlaysController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _caseConnection = new CasesConnection(context, configuration["CryKey"]);
            _employeesConnection = new EmployeesConnection(context);
            _courtsConnection = new CourtsConnection(context);
            _configurationsConnection = new ConfigurationsConnection(context);
            _caseTypesConnection = new CaseTypesConnection(context);
            _invoicesConnection = new InvoicesConnection(context, configuration["CryKey"]);
            _invoiceUnderlaysConnection = new InvoiceUnderlaysConnection(context, configuration["CryKey"]);
            _userConnection = new UserConnection(context);
        }

        // GET: InvoiceUnderlaysController
        public async Task<ActionResult> Index(int? page, int? size, int? reset, IFormCollection collection)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            ViewBag.CaseMenu = await _userConnection.GetMenuItems("Cases", UserName);
            ViewBag.PersonMenu = await _userConnection.GetMenuItems("Persons", UserName);
            ViewBag.InvoiceMenu = await _userConnection.GetMenuItems("Invoices", UserName);
            ViewBag.UnderlayMenu = await _userConnection.GetMenuItems(HttpContext.Request, UserName);

            if (HttpContext.Session.GetInt32("TrojInvoiceUnderlaySize").HasValue == false)
            {
                HttpContext.Session.SetInt32("TrojInvoiceUnderlaySize", 20);
            }

            if (size != null)
            {
                HttpContext.Session.SetInt32("TrojInvoiceUnderlaySize", size.Value);
            }

            int iSize = HttpContext.Session.GetInt32("TrojInvoiceUnderlaySize").Value;
            ViewBag.Size = iSize.ToString();

            int iPage = 0;
            int iOffset = 0;
            if (page != null)
            {
                iPage = page.Value;
                iOffset = (iPage - 1) * iSize;
            }
            ViewBag.Page = iPage;

            HttpContext.Session.SetInt32("TrojUnderlayStart", 1);
            HttpContext.Session.SetInt32("TrojUnderlayEnd", 10);
            if (iPage > HttpContext.Session.GetInt32("TrojUnderlayEnd"))
            {
                HttpContext.Session.SetInt32("TrojUnderlayStart", iPage - 1);
                HttpContext.Session.SetInt32("TrojUnderlayEnd", iPage + 9);
            }

            if (HttpContext.Session.GetInt32("TrojUnderlayStart") == 1)
            {
                ViewBag.Start = 0;
            }
            else
            {
                ViewBag.Start = HttpContext.Session.GetInt32("TrojUnderlayStart") - 1;
            }

            ViewBag.TrojUnderlayStart = HttpContext.Session.GetInt32("TrojUnderlayStart").Value;
            int trojUnderlayEnd = HttpContext.Session.GetInt32("TrojUnderlayEnd").Value;
            ViewBag.TrojUnderlayEnd = trojUnderlayEnd;

            if (collection.Count > 0)
            {
                HttpContext.Session.SetString("TrojUnderlayFilter", "Set");
                collection.TryGetValue("CaseId", out StringValues caseId);
                collection.TryGetValue("UnderlayNumber", out StringValues underlayNumber);
                collection.TryGetValue("UnderlayDate", out StringValues underlayDate);
                collection.TryGetValue("CaseTypeId", out StringValues caseTypeId);
                collection.TryGetValue("EmployeeId", out StringValues employeeId);
                collection.TryGetValue("FirstName", out StringValues firstName);
                collection.TryGetValue("LastName", out StringValues lastName);
                HttpContext.Session.SetString("TrojUnderlayCaseId", caseId.ToString());
                HttpContext.Session.SetString("TrojUnderlayUnderlayNumber", underlayNumber.ToString());
                HttpContext.Session.SetString("TrojUnderlayUnderlayDate", underlayDate.ToString());
                HttpContext.Session.SetString("TrojUnderlayCaseTypeId", caseTypeId.ToString());
                HttpContext.Session.SetString("TrojUnderlayEmployeeId", employeeId.ToString());
                HttpContext.Session.SetString("TrojUnderlayFirstName", firstName.ToString());
                HttpContext.Session.SetString("TrojUnderlayLastName", lastName.ToString());
            }

            if (reset != null)
            {
                HttpContext.Session.SetString("TrojUnderlayFilter", "Reset");
                HttpContext.Session.Remove("TrojUnderlayCaseId");
                HttpContext.Session.Remove("TrojUnderlayUnderlayNumber");
                HttpContext.Session.Remove("TrojUnderlayUnderlayDate");
                HttpContext.Session.Remove("TrojUnderlayCaseTypeId");
                HttpContext.Session.Remove("TrojUnderlayEmployeeId");
                HttpContext.Session.Remove("TrojUnderlayFirstName");
                HttpContext.Session.Remove("TrojUnderlayLastName");
            }

            IEnumerable<InvoiceUnderlaysViewModel> underlays;
            int numberOfUnderlays;
            if (HttpContext.Session.GetString("TrojUnderlayFilter") == "Set")
            {
                string caseId = HttpContext.Session.GetString("TrojUnderlayCaseId");
                string underlayNumber = HttpContext.Session.GetString("TrojUnderlayUnderlayNumber");
                string underlayDate = HttpContext.Session.GetString("TrojUnderlayUnderlayDate");
                string caseTypeId = HttpContext.Session.GetString("TrojUnderlayCaseTypeId");
                string employeeId = HttpContext.Session.GetString("TrojUnderlayEmployeeId");
                string firstName = HttpContext.Session.GetString("TrojUnderlayFirstName");
                string lastName = HttpContext.Session.GetString("TrojUnderlayLastName");
                underlays = await _invoiceUnderlaysConnection.GetFilteredUnderlays(caseId, underlayNumber, underlayDate, caseTypeId, employeeId, firstName, lastName, iOffset, iSize);
                numberOfUnderlays = await _invoiceUnderlaysConnection.GetNumberOfFilteredUnderlays(caseId, underlayNumber, underlayDate, caseTypeId, employeeId, firstName, lastName);
                ViewBag.CaseId = caseId;
                ViewBag.UnderlayNumber = underlayNumber;
                ViewBag.UnderlayDate = underlayDate;
                ViewBag.CaseTypeId = caseTypeId;
                ViewBag.EmployeeId = employeeId;
                ViewBag.FirstName = firstName;
                ViewBag.LastName = lastName;
            }
            else
            {
                underlays = await _invoiceUnderlaysConnection.GetUnderlays(iOffset, iSize);
                numberOfUnderlays = await _invoiceUnderlaysConnection.GetNumberOfUnderlays();
                ViewBag.CaseId = "";
                ViewBag.UnderlayNumber = "";
                ViewBag.UnderlayDate = "";
                ViewBag.CaseTypeId = "";
                ViewBag.EmployeeId = "";
                ViewBag.FirstName = "";
                ViewBag.LastName = "";
            }

            double dMaxPage = Math.Ceiling((double)numberOfUnderlays / iSize);
            int maxPage = Convert.ToInt32(dMaxPage);
            ViewBag.MaxPage = maxPage;

            ViewBag.ShowMaxPage = false;
            if (trojUnderlayEnd < maxPage - 1)
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

            List<SelectListItem> caseTypes = new List<SelectListItem>();
            var caseTypeList = await _caseTypesConnection.GetCaseTypes();
            caseTypes.Add(new SelectListItem { Value = "0", Text = "" });
            foreach (var caseType in caseTypeList)
                caseTypes.Add(new SelectListItem { Value = caseType.CaseTypeId.ToString(), Text = caseType.CaseType });
            ViewBag.CaseTypes = caseTypes;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeesConnection.GetActiveEmployees();
            employees.Add(new SelectListItem { Value = "0", Text = "" });
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.EmployeeId.ToString(), Text = employee.Initials });
            ViewBag.Employees = employees;

            return View(underlays);
        }

        // GET: InvoiceUnderlaysController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            ViewBag.CaseMenu = await _userConnection.GetMenuItems("Cases", UserName);
            ViewBag.PersonMenu = await _userConnection.GetMenuItems("Persons", UserName);
            ViewBag.InvoiceMenu = await _userConnection.GetMenuItems("Invoices", UserName);
            ViewBag.UnderlayMenu = await _userConnection.GetMenuItems(HttpContext.Request, UserName);

            InvoiceUnderlaysViewModel underlay = await _invoiceUnderlaysConnection.GetUnderlay(id);
            if (underlay == null)
                return NoContent();

            ViewBag.UnderlayLocked = underlay.Locked;

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            string underlayNumber = underlay.UnderlayNumber;
            if (string.IsNullOrEmpty(underlay.UnderlayNumber))
                underlayNumber = "Inget underlagsnummer";
            if (string.IsNullOrEmpty(underlay.Title))
                ViewBag.UnderlayPageTitle = "Underlag: " + underlayNumber;
            else
                ViewBag.UnderlayPageTitle = "Underlag: " + underlayNumber + ", " + underlay.Title;

            StringBuilder receiver = new StringBuilder();
            receiver.AppendFormat("{0}<br/>", underlay.ReceiverName);
            if (string.IsNullOrEmpty(underlay.CareOf) == false)
                receiver.AppendFormat("C/O: {0}<br/>", underlay.CareOf);
            if (string.IsNullOrEmpty(underlay.StreetName) == false)
                receiver.AppendFormat("{0} {1}<br/>", underlay.StreetName, underlay.StreetNumber);
            receiver.AppendFormat("{0} {1}<br/>", underlay.PostalCode, underlay.PostalAddress);
            if (string.IsNullOrEmpty(underlay.Country) == false)
                receiver.AppendFormat("{0}<br/>", underlay.Country);
            ViewBag.Receiver = receiver.ToString();

            IEnumerable<InvoicesPartialViewModel> invoices = await _invoicesConnection.GetInvoicesForUnderly(id);
            ViewBag.Invoices = invoices;

            IEnumerable<InvoiceUnderlaySummarysModel> underlaySummeries = await _invoiceUnderlaysConnection.GetUnderlaySummaries(id);
            ViewBag.UnderlaySummeries = underlaySummeries;

            TotalSumModel underlayTotalSum = await _invoiceUnderlaysConnection.GetUnderlaySummariesTotalSum(id);
            double vat = (double)underlay.Vat;
            double totalSum = 0.0;
            double totalSumWithVat = 0.0;
            if (underlayTotalSum != null)
            {
                totalSum = (double)underlayTotalSum.TotalSum;
                double addVat = 1.0 + (double)vat / 100;
                totalSumWithVat = (double)totalSum * addVat;
            }
            ViewBag.TotalSum = totalSum;
            ViewBag.TotalSumWithVat = totalSumWithVat;
            ViewBag.TotalVatSum = totalSumWithVat - (double)totalSum;

            IEnumerable<InvoiceWorkingTimesViewModel> workingTimes = await _invoiceUnderlaysConnection.GetUnderlayWorkingTimes(id);
            ViewBag.WorkingTimes = workingTimes;

            return View(underlay);
        }

        public async Task<IActionResult> Print(int id)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "InvoiceUnderlays", new { id });

            ViewBag.CaseMenu = await _userConnection.GetMenuItems("Cases", UserName);
            ViewBag.PersonMenu = await _userConnection.GetMenuItems("Persons", UserName);
            ViewBag.InvoiceMenu = await _userConnection.GetMenuItems("Invoices", UserName);
            ViewBag.UnderlayMenu = await _userConnection.GetMenuItems(HttpContext.Request, UserName);

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            _currentUnderlayId = id;
            ViewBag.InvoiceUnderlayId = id;
            ViewBag.UnderlayPageTitle = "Skriv ut kostnadsräkning och arbetsredogörelse";
            IEnumerable<InvoicePrintingFieldsModel> fields = await _invoiceUnderlaysConnection.GetInvoicePrintingFields(id);

            ViewBag.Case = false;
            ViewBag.Employee = false;
            ViewBag.Date = false;
            ViewBag.Client = true;
            ViewBag.Address = false;
            ViewBag.CaseNumber = false;
            if (fields.FirstOrDefault(u => u.FieldName == "Case") != null)
                ViewBag.Case = true;
            if (fields.FirstOrDefault(u => u.FieldName == "Employee") != null)
                ViewBag.Employee = true;
            if (fields.FirstOrDefault(u => u.FieldName == "Date") != null)
                ViewBag.Date = true;
            if (fields.FirstOrDefault(u => u.FieldName == "Client") != null)
                ViewBag.Client = false;
            if (fields.FirstOrDefault(u => u.FieldName == "Address") != null)
                ViewBag.Address = true;
            if (fields.FirstOrDefault(u => u.FieldName == "CaseNumber") != null)
                ViewBag.CaseNumber = true;

            return View(fields);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Case(bool isChecked)
        {
            await _invoiceUnderlaysConnection.SetField(_currentUnderlayId, "Case", UserName);
            return RedirectToAction("Print", new { id = _currentUnderlayId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Emplyee(bool isChecked)
        {
            await _invoiceUnderlaysConnection.SetField(_currentUnderlayId, "Employee", UserName);
            return RedirectToAction("Print", new { id = _currentUnderlayId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Date(bool isChecked)
        {
            await _invoiceUnderlaysConnection.SetField(_currentUnderlayId, "Date", UserName);
            return RedirectToAction("Print", new { id = _currentUnderlayId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Client(bool isChecked)
        {
            await _invoiceUnderlaysConnection.SetField(_currentUnderlayId, "Client", UserName);
            return RedirectToAction("Print", new { id = _currentUnderlayId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CaseNumber(bool isChecked)
        {
            await _invoiceUnderlaysConnection.SetField(_currentUnderlayId, "CaseNumber", UserName);
            return RedirectToAction("Print", new { id = _currentUnderlayId });
        }

        // GET: InvoiceUnderlaysController/Costreport/5
        public async Task<IActionResult> Costreport(int id)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "InvoiceUnderlays", new { id });

            IEnumerable<InvoicePrintingFieldsModel> fields = await _invoiceUnderlaysConnection.GetInvoicePrintingFields(id);
            ViewBag.Case = false;
            ViewBag.Employee = false;
            ViewBag.Date = false;
            ViewBag.Client = true;
            ViewBag.Address = false;
            ViewBag.CaseNumber = false;
            if (fields.FirstOrDefault(u => u.FieldName == "Case") != null)
                ViewBag.Case = true;
            if (fields.FirstOrDefault(u => u.FieldName == "Employee") != null)
                ViewBag.Employee = true;
            if (fields.FirstOrDefault(u => u.FieldName == "Date") != null)
                ViewBag.Date = true;
            if (fields.FirstOrDefault(u => u.FieldName == "Client") != null)
                ViewBag.Client = false;
            if (fields.FirstOrDefault(u => u.FieldName == "CaseNumber") != null)
                ViewBag.CaseNumber = true;

            InvoiceUnderlaysViewModel underlay = await _invoiceUnderlaysConnection.GetUnderlay(id);
            if (underlay == null)
                return NoContent();

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

            IEnumerable<CaseNumbersViewModel> caseNumbersView = await _caseConnection.GetCaseNumbers(underlay.CaseId);
            if (caseNumbersView.Count<CaseNumbersViewModel>() == 0)
            {
                ViewBag.CaseNumbers = "";
            }
            else
            {
                string caseNumbers = "";
                string nl = "";
                foreach (CaseNumbersViewModel caseNumber in caseNumbersView)
                {
                    if (string.IsNullOrEmpty(caseNumber.CaseNumber))
                        caseNumbers = caseNumbers + nl + caseNumber.CourtName;
                    else
                        caseNumbers = caseNumbers + nl + caseNumber.CaseNumber;
                    nl = ", ";
                }
                ViewBag.CaseNumbers = caseNumbers;
            }

            IEnumerable<InvoiceUnderlaySummarysModel> underlaySummeries = await _invoiceUnderlaysConnection.GetUnderlaySummaries(id);
            ViewBag.UnderlaySummeries = underlaySummeries;

            TotalSumModel underlayTotalSum = await _invoiceUnderlaysConnection.GetUnderlaySummariesTotalSum(id);
            double vat = (double)underlay.Vat;
            double totalSum = 0.0;
            double totalSumWithVat = 0.0;
            if (underlayTotalSum != null)
            {
                totalSum = (double)underlayTotalSum.TotalSum;
                double addVat = 1.0 + (double)vat / 100;
                totalSumWithVat = (double)totalSum * addVat;
            }
            ViewBag.Vat = underlay.Vat;
            ViewBag.TotalSum = totalSum;
            ViewBag.TotalSumWithVat = totalSumWithVat;
            ViewBag.TotalVatSum = totalSumWithVat - (double)totalSum;

            var employee = await _employeesConnection.GetEmployee(underlay.EmployeeId);
            ViewBag.EmployeeFirstName = employee.FirstName;
            ViewBag.EmployeeLastName = employee.LastName;
            ViewBag.EmployeeTitle = employee.EmployeeTitle;

            var request = HttpContext.Request;
            var currentUrl = $"{request.Scheme}://{request.Host}";
            ViewBag.SignatureLink = currentUrl + "/images/" + employee.SignatureLink;
            ViewBag.BlankImage = currentUrl + "/images/blank.gif";
            ViewBag.LogoImage = currentUrl + "/images/logo.png";

            return View(underlay);
        }

        // GET: InvoiceUnderlaysController/Workingreport/5
        public async Task<IActionResult> Workingreport(int id)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "InvoiceUnderlays", new { id });

            IEnumerable<InvoicePrintingFieldsModel> fields = await _invoiceUnderlaysConnection.GetInvoicePrintingFields(id);
            ViewBag.Case = false;
            ViewBag.Employee = false;
            ViewBag.Date = false;
            ViewBag.Client = true;
            ViewBag.Address = false;
            ViewBag.CaseNumber = false;
            if (fields.FirstOrDefault(u => u.FieldName == "Case") != null)
                ViewBag.Case = true;
            if (fields.FirstOrDefault(u => u.FieldName == "Employee") != null)
                ViewBag.Employee = true;
            if (fields.FirstOrDefault(u => u.FieldName == "Date") != null)
                ViewBag.Date = true;
            if (fields.FirstOrDefault(u => u.FieldName == "Client") != null)
                ViewBag.Client = false;
            if (fields.FirstOrDefault(u => u.FieldName == "CaseNumber") != null)
                ViewBag.CaseNumber = true;

            InvoiceUnderlaysViewModel underlay = await _invoiceUnderlaysConnection.GetUnderlay(id);
            if (underlay == null)
                return NoContent();

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

            IEnumerable<CaseNumbersViewModel> caseNumbersView = await _caseConnection.GetCaseNumbers(underlay.CaseId);
            if (caseNumbersView.Count<CaseNumbersViewModel>() == 0)
            {
                ViewBag.CaseNumbers = "";
            }
            else
            {
                string caseNumbers = "";
                string nl = "";
                foreach (CaseNumbersViewModel caseNumber in caseNumbersView)
                {
                    if (string.IsNullOrEmpty(caseNumber.CaseNumber))
                        caseNumbers = caseNumbers + nl + caseNumber.CourtName;
                    else
                        caseNumbers = caseNumbers + nl + caseNumber.CourtName + ": " + caseNumber.CaseNumber;
                    nl = "<br />";
                }
                ViewBag.CaseNumbers = caseNumbers;
            }

            IEnumerable<InvoiceWorkingTimesViewModel> workingTimes = await _invoiceUnderlaysConnection.GetUnderlayWorkingTimes(id);
            ViewBag.WorkingTimes = workingTimes;

            IEnumerable<InvoiceUnderlaySummarysModel> underlaySummeries = await _invoiceUnderlaysConnection.GetUnderlaySummaries(id);
            ViewBag.UnderlaySummeries = underlaySummeries;

            TotalSumModel underlayTotalSum = await _invoiceUnderlaysConnection.GetUnderlaySummariesTotalSum(id);
            double totalSum = 0.0;
            if (underlayTotalSum != null)
            {
                totalSum = (double)underlayTotalSum.TotalSum;
            }
            ViewBag.TotalSum = totalSum;

            var employee = await _employeesConnection.GetEmployee(underlay.EmployeeId);
            ViewBag.EmployeeFirstName = employee.FirstName;
            ViewBag.EmployeeLastName = employee.LastName;
            ViewBag.EmployeeTitle = employee.EmployeeTitle;

            var request = HttpContext.Request;
            var currentUrl = $"{request.Scheme}://{request.Host}";
            ViewBag.BlankImage = currentUrl + "/images/blank.gif";
            ViewBag.LogoImage = currentUrl + "/images/logo.png";

            return View(underlay);
        }

        // GET: InvoiceUnderlaysController/Create
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return NoContent();

            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "Cases", new { id = id.Value });

            _currentCaseId = id.Value;

            CasesViewModel currentCase = await _caseConnection.GetCase(id.Value);
            if (currentCase == null)
                return NoContent();

            ViewBag.NumberOfUnderlays = await _invoiceUnderlaysConnection.GetNumberOfUnderlays4Case(id.Value);

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            ViewBag.CaseId = currentCase.CaseId.ToString();
            ViewBag.CaseLinkText = currentCase.CaseType + "/" + currentCase.CaseId.ToString();
            ViewBag.CaseActive = currentCase.Active;

            List<SelectListItem> persons = new List<SelectListItem>();
            var personList = await _caseConnection.GetAllPersonsAtCase(id.Value);
            foreach (var person in personList)
                persons.Add(new SelectListItem { Value = person.PersonId.ToString(), Text = person.FirstName + " " + person.LastName + ", " + person.PersonType });
            ViewBag.Persons = persons;

            List<SelectListItem> courts = new List<SelectListItem>();
            var courtList = await _caseConnection.GetAllCourtsAtCase(id.Value);
            foreach (var court in courtList)
                courts.Add(new SelectListItem { Value = court.CourtId.ToString(), Text = court.CourtName });
            ViewBag.Courts = courts;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeesConnection.GetActiveEmployees();
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.EmployeeId.ToString(), Text = employee.Initials });
            ViewBag.Employees = employees;

            string vat = "";
            ConfigurationsModel configuration = await _configurationsConnection.GetConfigurationWithkey("Moms");
            if (configuration != null)
                vat = configuration.ConfigValue;
            ViewBag.Vat = vat;

            string underlayPlace = "";
            configuration = await _configurationsConnection.GetConfigurationWithkey("Ort");
            if (configuration != null)
                underlayPlace = configuration.ConfigValue;
            ViewBag.Place = underlayPlace;

            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.CurrentDate = currentDate;

            return View();
        }

        // POST: InvoiceUnderlaysController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "Cases", new { id = _currentCaseId });

            if (!collection.TryGetValue("CaseId", out StringValues caseId))
                return NoContent();

            if (!collection.TryGetValue("CourtId", out StringValues courtId))
                return NoContent();

            if (!collection.TryGetValue("PersonId", out StringValues personId))
                return NoContent();

            if (!collection.TryGetValue("EmployeeId", out StringValues employeeId))
                return NoContent();

            if (!collection.TryGetValue("Place", out StringValues place))
                return NoContent();

            if (!collection.TryGetValue("UnderlayDate", out StringValues underlayDate))
                return NoContent();

            if (!collection.TryGetValue("Vat", out StringValues vat))
                return NoContent();

            DateTime date = DateTime.Now;
            if (underlayDate != "")
                date = DateTime.Parse(underlayDate);

            InvoiceUnderlaysModel newUnderlay = await _invoiceUnderlaysConnection.CreateUnderlay(Int32.Parse(caseId.ToString()), Int32.Parse(employeeId.ToString()), Int32.Parse(courtId.ToString()), Int32.Parse(personId.ToString()), place, date, Int32.Parse(vat.ToString()), UserName);
            if (newUnderlay == null)
                return NoContent();
            else
            {
                string menuTitle = newUnderlay.UnderlayNumber + ": " + newUnderlay.UnderlayDate.ToString("yyyy-MM-dd") + " / " + newUnderlay.InvoiceUnderlayId;
                await _userConnection.CreateMenuItem(HttpContext.Request, menuTitle, newUnderlay.InvoiceUnderlayId, UserName);
            }

            return RedirectToAction("Details", new { id = newUnderlay.InvoiceUnderlayId });
        }

        // GET: InvoiceUnderlaysController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "InvoiceUnderlays", new { id });

            _currentUnderlay = await _invoiceUnderlaysConnection.GetUnderlay2(id);
            if (_currentUnderlay == null)
                return NoContent();

            ViewBag.UnderlayLocked = _currentUnderlay.Locked;

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            ViewBag.InvoiceUnderlayId = _currentUnderlay.InvoiceUnderlayId;
            ViewBag.UnderlayLinkText = _currentUnderlay.UnderlayNumber;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeesConnection.GetActiveEmployees();
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.EmployeeId.ToString(), Text = employee.Initials });
            ViewBag.Employees = employees;

            return View(_currentUnderlay);
        }

        // POST: InvoiceUnderlaysController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "InvoiceUnderlays", new { id });

            if (!collection.TryGetValue("Title", out StringValues title))
                return NoContent();

            if (!collection.TryGetValue("EmployeeId", out StringValues employeeId))
                return NoContent();

            if (!collection.TryGetValue("UnderlayDate", out StringValues underlayDate))
                return NoContent();

            if (!collection.TryGetValue("UnderlayPlace", out StringValues underlayPlace))
                return NoContent();

            if (!collection.TryGetValue("Headline1", out StringValues headline1))
                return NoContent();

            if (!collection.TryGetValue("Headline2", out StringValues headline2))
                return NoContent();

            if (!collection.TryGetValue("WorkingReport", out StringValues workingReport))
                return NoContent();

            bool locked = false;
            string[] str = collection["Locked"].ToArray();
            if (str.Length > 1)
            {
                locked = true;
            }

            InvoiceUnderlaysModel updatedUnderlay = await _invoiceUnderlaysConnection.UpdateUnderlay(id, int.Parse(employeeId), title, DateTime.Parse(underlayDate), underlayPlace, headline1, headline2, workingReport, locked, _currentUnderlay, UserName);
            if (updatedUnderlay == null)
                return NoContent();
            else
            {
                string menuTitle = updatedUnderlay.UnderlayNumber + ": " + updatedUnderlay.UnderlayDate.ToString("yyyy-MM-dd") + " / " + updatedUnderlay.InvoiceUnderlayId;
                await _userConnection.CreateMenuItem(HttpContext.Request, menuTitle, updatedUnderlay.InvoiceUnderlayId, UserName);
            }

            return RedirectToAction("Details", new { id = updatedUnderlay.InvoiceUnderlayId });
        }

        // POST: InvoiceUnderlaysController/Unlock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Unlock(IFormCollection collection)
        {
            if (!collection.TryGetValue("InvoiceUnderlayId", out StringValues invoiceUnderlayId))
                return NoContent();

            InvoiceUnderlaysModel updatedUnderlay = await _invoiceUnderlaysConnection.Unlock(Int32.Parse(invoiceUnderlayId.ToString()), UserName);
            if (updatedUnderlay == null)
                return NoContent();
            else
            {
                string menuTitle = updatedUnderlay.UnderlayNumber + ": " + updatedUnderlay.UnderlayDate.ToString("yyyy-MM-dd") + " / " + updatedUnderlay.InvoiceUnderlayId;
                await _userConnection.CreateMenuItem(HttpContext.Request, menuTitle, updatedUnderlay.InvoiceUnderlayId, UserName);
            }
            return RedirectToAction("Edit", new { id = updatedUnderlay.InvoiceUnderlayId });
        }

        public ActionResult Reset()
        {
            return RedirectToAction("Index", new { reset = 1 });
        }
    }
}
