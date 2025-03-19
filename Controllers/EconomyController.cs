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
    public class EconomyController : IdenityController
    {
        private readonly PersonsConnection _personConnection;
        private readonly EmployeesConnection _employeeConnection;
        private readonly CasesConnection _caseConnection;
        private readonly TariffTypesConnection _tariffTypesConnection;
        private readonly CaseTypesConnection _caseTypeConnection;
        private readonly WorkingTimesConnection _workingTimesConnection;
        private readonly EconomyConnection _economyConnection;
        private readonly InvoicesConnection _invoicesConnection;
        private readonly UserConnection _userConnection;

        public EconomyController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _personConnection = new PersonsConnection(context, configuration["CryKey"]);
            _employeeConnection = new EmployeesConnection(context);
            _caseConnection = new CasesConnection(context, configuration["CryKey"]);
            _tariffTypesConnection = new TariffTypesConnection(context);
            _caseTypeConnection = new CaseTypesConnection(context);
            _workingTimesConnection = new WorkingTimesConnection(context, configuration["CryKey"]);
            _economyConnection = new EconomyConnection(context, configuration["CryKey"]);
            _invoicesConnection = new InvoicesConnection(context, configuration["CryKey"]);
            _userConnection = new UserConnection(context);
        }

        // GET: EconomyController
        public async Task<ActionResult> Index(int? page, int? size, int? reset, IFormCollection collection)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            ViewBag.CaseMenu = await _userConnection.GetMenuItems("Cases", UserName);
            ViewBag.PersonMenu = await _userConnection.GetMenuItems("Persons", UserName);
            ViewBag.UnderlayMenu = await _userConnection.GetMenuItems("InvoiceUnderlays", UserName);
            ViewBag.InvoiceMenu = await _userConnection.GetMenuItems("Invoices", UserName);

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            if (HttpContext.Session.GetInt32("TrojEconomyWorkingTimeSize").HasValue == false)
            {
                HttpContext.Session.SetInt32("TrojEconomyWorkingTimeSize", 20);
            }

            if (size != null)
            {
                HttpContext.Session.SetInt32("TrojEconomyWorkingTimeSize", size.Value);
            }

            int iSize = HttpContext.Session.GetInt32("TrojEconomyWorkingTimeSize").Value;
            ViewBag.Size = iSize.ToString();

            int iPage = 0;
            int iOffset = 0;
            if (page != null)
            {
                iPage = page.Value;
                iOffset = (iPage - 1) * iSize;
            }
            ViewBag.Page = iPage;

            HttpContext.Session.SetInt32("TrojEconomyWorkingTimeStart", 1);
            HttpContext.Session.SetInt32("TrojEconomyWorkingTimeEnd", 10);
            if (iPage > HttpContext.Session.GetInt32("TrojEconomyWorkingTimeEnd"))
            {
                HttpContext.Session.SetInt32("TrojEconomyWorkingTimeStart", iPage - 1);
                HttpContext.Session.SetInt32("TrojEconomyWorkingTimeEnd", iPage + 9);
            }

            if (HttpContext.Session.GetInt32("TrojEconomyWorkingTimeStart") == 1)
            {
                ViewBag.Start = 0;
            }
            else
            {
                ViewBag.Start = HttpContext.Session.GetInt32("TrojEconomyWorkingTimeStart") - 1;
            }

            ViewBag.TrojEconomyWorkingTimeStart = HttpContext.Session.GetInt32("TrojEconomyWorkingTimeStart").Value;

            int trojWorkingTimeEnd = HttpContext.Session.GetInt32("TrojEconomyWorkingTimeEnd").Value;
            ViewBag.TrojEconomyWorkingTimeEnd = trojWorkingTimeEnd;


            if (collection.Count > 0)
            {
                HttpContext.Session.SetString("TrojEconomyWorkingTimeFilter", "Set");
                collection.TryGetValue("StartWhenDate", out StringValues inputStartDate);
                collection.TryGetValue("EndWhenDate", out StringValues inputEndDate);
                collection.TryGetValue("EmployeeId", out StringValues inputEmployeeId);
                HttpContext.Session.SetString("TrojEconomyWorkingTimeStartWhenDate", inputStartDate.ToString());
                HttpContext.Session.SetString("TrojEconomyWorkingTimeEndWhenDate", inputEndDate.ToString());
                HttpContext.Session.SetString("TrojEconomyWorkingTimeEmployeeId", inputEmployeeId.ToString());
            }

            if (reset != null)
            {
                HttpContext.Session.SetString("TrojEconomyWorkingTimeFilter", "Reset");
                HttpContext.Session.Remove("TrojEconomyWorkingTimeStartWhenDate");
                HttpContext.Session.Remove("TrojEconomyWorkingTimeEndWhenDate");
                HttpContext.Session.Remove("TrojEconomyWorkingTimeEmployeeId");
            }

            IEnumerable<WorkingTimesEconomyModel> workingTimes;
            IEnumerable<SumOfWorkingTimesModel> sumOfWorkingTimesBefore;
            IEnumerable<SumOfWorkingTimesModel> sumOfUnderlay;
            IEnumerable<InvoiceAndCaseModel> invoices;
            IEnumerable<WorkingTimesPeriodEconomyModel> periods;
            TotalSumAndHoursModel totalSumModel;
            string startWhenDate;
            string endWhenDate;
            int numberOfWorkingTimes;
            string employeeId;
            if (HttpContext.Session.GetString("TrojEconomyWorkingTimeFilter") == "Set")
            {
                startWhenDate = HttpContext.Session.GetString("TrojEconomyWorkingTimeStartWhenDate");
                endWhenDate = HttpContext.Session.GetString("TrojEconomyWorkingTimeEndWhenDate");
                employeeId = HttpContext.Session.GetString("TrojEconomyWorkingTimeEmployeeId");
            }
            else
            {
                startWhenDate = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy-MM-dd");
                endWhenDate = DateTime.Now.ToString("yyyy-MM-dd");
                employeeId = "0";
            }
            workingTimes = await _economyConnection.GetEconomyWorkingTimes(startWhenDate, endWhenDate, employeeId, iOffset, iSize);

            periods = await _economyConnection.GetEconomyTimePeriod(startWhenDate, endWhenDate, employeeId, iOffset, iSize);
            ViewBag.Periods = periods;

            sumOfWorkingTimesBefore = await _economyConnection.GetSumOfWorkingTimesBefore(startWhenDate, endWhenDate, employeeId);
            ViewBag.SumOfWorkingTimesBefore = sumOfWorkingTimesBefore;

            sumOfUnderlay = await _economyConnection.GetSumOfUnderlay(startWhenDate, endWhenDate, employeeId);
            ViewBag.SumOfUnderlays = sumOfUnderlay;

            invoices = await _economyConnection.GetInvoices(startWhenDate, endWhenDate, employeeId);
            ViewBag.Invoices = invoices;

            numberOfWorkingTimes = await _workingTimesConnection.GetNumberOfWorkingTimesForCurrentDay(startWhenDate, endWhenDate);
            totalSumModel = await _workingTimesConnection.GetTotalSum(startWhenDate, startWhenDate);

            ViewBag.StartWhenDate = startWhenDate;
            ViewBag.EndWhenDate = endWhenDate;
            ViewBag.EmployeeId = employeeId;

            double dMaxPage = Math.Ceiling((double)numberOfWorkingTimes / iSize);
            int maxPage = Convert.ToInt32(dMaxPage);
            ViewBag.MaxPage = maxPage;

            ViewBag.ShowMaxPage = false;
            if (trojWorkingTimeEnd < maxPage - 1)
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

            ViewBag.TotalSum = totalSumModel.TotalSum;
            ViewBag.TotalHours = totalSumModel.TotalHours;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            foreach (var item in employeesList)
                employees.Add(new SelectListItem { Value = item.EmployeeId.ToString(), Text = item.Initials });
            ViewBag.Employees = employees;

            return View(workingTimes);
        }

        // Get: EconomyController/Reset
        public ActionResult Reset()
        {
            return RedirectToAction("Index", new { reset = 1 });
        }

        // Get: EconomyController/Reset
        public ActionResult ResetDetails()
        {
            return RedirectToAction("Details", new { reset = 1 });
        }

        // GET: InvoicesController/Details/5
        public async Task<IActionResult> Details(int? id, int? reset, IFormCollection collection)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            ViewBag.CaseMenu = await _userConnection.GetMenuItems("Cases", UserName);
            ViewBag.PersonMenu = await _userConnection.GetMenuItems("Persons", UserName);
            ViewBag.UnderlayMenu = await _userConnection.GetMenuItems("InvoiceUnderlays", UserName);
            ViewBag.InvoiceMenu = await _userConnection.GetMenuItems("Invoices", UserName);

            List<SelectListItem> clients = new List<SelectListItem>();
            var personList = await _personConnection.GetActivePersons();
            string firstClientId = "0";
            foreach (var item in personList)
            {
                if (firstClientId.Equals("0"))
                    firstClientId = item.PersonId.ToString();
                clients.Add(new SelectListItem { Value = item.PersonId.ToString(), Text = item.FirstName + " " + item.LastName });
            }
            ViewBag.Clients = clients;

            if (collection.Count > 0)
            {
                HttpContext.Session.SetString("TrojEconomyWorkingTimeFilter", "Set");
                collection.TryGetValue("StartWhenDate", out StringValues inputStartDate);
                collection.TryGetValue("EndWhenDate", out StringValues inputEndDate);
                collection.TryGetValue("ClientId", out StringValues inputClientId);
                HttpContext.Session.SetString("TrojEconomyWorkingTimeStartWhenDate", inputStartDate.ToString());
                HttpContext.Session.SetString("TrojEconomyWorkingTimeEndWhenDate", inputEndDate.ToString());
                HttpContext.Session.SetString("TrojEconomyWorkingInputClientId", inputClientId.ToString());
            }

            if (reset != null)
            {
                HttpContext.Session.SetString("TrojEconomyWorkingTimeFilter", "Reset");
                HttpContext.Session.Remove("TrojEconomyWorkingTimeStartWhenDate");
                HttpContext.Session.Remove("TrojEconomyWorkingTimeEndWhenDate");
                HttpContext.Session.Remove("TrojEconomyWorkingInputClientId");
            }

            string startWhenDate;
            string endWhenDate;
            string clientId;
            if (HttpContext.Session.GetString("TrojEconomyWorkingTimeFilter") == "Set")
            {
                startWhenDate = HttpContext.Session.GetString("TrojEconomyWorkingTimeStartWhenDate");
                endWhenDate = HttpContext.Session.GetString("TrojEconomyWorkingTimeEndWhenDate");
                clientId = HttpContext.Session.GetString("TrojEconomyWorkingInputClientId");
            }
            else
            {
                startWhenDate = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy-MM-dd");
                endWhenDate = DateTime.Now.ToString("yyyy-MM-dd");
                clientId = firstClientId;
            }

            if (id != null)
            {
                if (collection.Count > 0)
                {
                    ViewBag.ClientId = clientId;
                    if (Int32.TryParse(clientId, out int parsedId))
                    {
                        id = parsedId;
                    }
                    else
                    {
                        id = 0;
                    }
                }
                else
                {
                    ViewBag.ClientId = id.ToString();
                }
            }
            else
            {
                ViewBag.ClientId = clientId;
                if (Int32.TryParse(clientId, out int parsedId))
                {
                    id = parsedId;
                }
                else
                {
                    id = 0;
                }
            }

            ViewBag.Invoices = await _invoicesConnection.GetInvoices(id.Value);
            ViewBag.NumberOfInvoices = await _invoicesConnection.GetNumberOfInvoices(id.Value);
            ViewBag.StartWhenDate = startWhenDate;
            ViewBag.EndWhenDate = endWhenDate;

            IEnumerable<WorkingTimesViewModel> workingTimes = await _workingTimesConnection.GetWorkingTimeForClient(id.Value, startWhenDate, endWhenDate);
            ViewBag.WorkingTimes = workingTimes;

            MaxMinDateModel maxMin = await _workingTimesConnection.GetMaxMinDateWorkingTimeForClient(id.Value, startWhenDate, endWhenDate);
            ViewBag.MaxDate = maxMin.MaxDate;
            ViewBag.MinDate = maxMin.MinDate;

            int numberOf = await _workingTimesConnection.GetNumberOfWorkingTimeForClient(id.Value, startWhenDate, endWhenDate);
            ViewBag.NumberOf = numberOf;

            return View();
        }


        // GET: EconomyController/Bargain
        public async Task<ActionResult> Bargain(int? size, int? reset, IFormCollection collection)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            if (collection.Count > 0)
            {
                HttpContext.Session.SetString("TrojEconomyWorkingTimeFilter", "Set");
                collection.TryGetValue("StartWhenDate", out StringValues inputStartDate);
                collection.TryGetValue("EndWhenDate", out StringValues inputEndDate);
                collection.TryGetValue("Initial", out StringValues inputInitial);
                HttpContext.Session.SetString("TrojEconomyWorkingTimeStartWhenDate", inputStartDate.ToString());
                HttpContext.Session.SetString("TrojEconomyWorkingTimeEndWhenDate", inputEndDate.ToString());
                HttpContext.Session.SetString("TrojEconomyWorkingTimeInitial", inputInitial.ToString());
            }

            if (reset != null)
            {
                HttpContext.Session.SetString("TrojEconomyWorkingTimeFilter", "Reset");
                HttpContext.Session.Remove("TrojEconomyWorkingTimeStartWhenDate");
                HttpContext.Session.Remove("TrojEconomyWorkingTimeEndWhenDate");
                HttpContext.Session.Remove("TrojEconomyWorkingTimeInitial");
            }

            string startWhenDate;
            string endWhenDate;
            string initial;
            if (HttpContext.Session.GetString("TrojEconomyWorkingTimeFilter") == "Set")
            {
                startWhenDate = HttpContext.Session.GetString("TrojEconomyWorkingTimeStartWhenDate");
                endWhenDate = HttpContext.Session.GetString("TrojEconomyWorkingTimeEndWhenDate");
                initial = HttpContext.Session.GetString("TrojEconomyWorkingTimeInitial");
            }
            else
            {
                startWhenDate = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy-MM-dd");
                endWhenDate = DateTime.Now.ToString("yyyy-MM-dd");
                initial = "";
            }
            IEnumerable<InvoiceUnderlaysCaseViewModel> list = await _economyConnection.GetUnderlayForYearAndEmployee(startWhenDate, endWhenDate, initial);
            ViewBag.StartWhenDate = startWhenDate;
            ViewBag.EndWhenDate = endWhenDate;
            ViewBag.Initial = initial;
            ViewBag.NumberofUnderlays = list.Count();

            IEnumerable<InvoiceAndInvoiceUnderlayModel> invoices = await _economyConnection.GetInvoicesForYear(startWhenDate, endWhenDate, initial);
            ViewBag.Invoices = invoices;

            IEnumerable<TotalSumIdModel> underlays = await _economyConnection.GetUnderlaySum(startWhenDate, endWhenDate, initial);
            ViewBag.Underlays = underlays;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            foreach (var item in employeesList)
                employees.Add(new SelectListItem { Value = item.Initials.ToString(), Text = item.FirstName + " " + item.LastName });
            ViewBag.Employees = employees;

            return View(list);
        }

        public ActionResult ResetBargain()
        {
            return RedirectToAction("Bargain", new { reset = 1 });
        }
    }
}
