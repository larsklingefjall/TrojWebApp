using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class WorkingTimesController : IdenityController
    {
        private readonly PersonsConnection _personConnection;
        private readonly EmployeesConnection _employeeConnection;
        private readonly CasesConnection _caseConnection;
        private readonly TariffTypesConnection _tariffTypesConnection;
        private readonly CaseTypesConnection _caseTypeConnection;
        private readonly WorkingTimesConnection _workingTimesConnection;
        private readonly ConfigurationsConnection _configurationConnection;
        private readonly UserConnection _userConnection;

        public WorkingTimesController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _personConnection = new PersonsConnection(context, configuration["CryKey"]);
            _employeeConnection = new EmployeesConnection(context);
            _caseConnection = new CasesConnection(context, configuration["CryKey"]);
            _tariffTypesConnection = new TariffTypesConnection(context);
            _caseTypeConnection = new CaseTypesConnection(context);
            _workingTimesConnection = new WorkingTimesConnection(context, configuration["CryKey"]);
            _configurationConnection = new ConfigurationsConnection(context);
            _userConnection = new UserConnection(context);
        }

        // GET: WorkingTimesController
        public async Task<ActionResult> Index(int? page, int? size, int? reset, IFormCollection collection)
        {
            var stopwatch = Stopwatch.StartNew();

            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            ViewBag.CreatePermission = _userConnection.AccessToSubPage(HttpContext.Request, "Create", UserName);
            ViewBag.EditPermission = _userConnection.AccessToSubPage(HttpContext.Request, "Edit", UserName);
            ViewBag.DeletePermission = _userConnection.AccessToSubPage(HttpContext.Request, "Delete", UserName);

            ViewBag.CaseMenu = await _userConnection.GetMenuItems("Cases", UserName);
            ViewBag.PersonMenu = await _userConnection.GetMenuItems("Persons", UserName);
            ViewBag.UnderlayMenu = await _userConnection.GetMenuItems("InvoiceUnderlays", UserName);
            ViewBag.InvoiceMenu = await _userConnection.GetMenuItems("Invoices", UserName);

            if (HttpContext.Session.GetInt32("TrojWorkingTimeSize").HasValue == false)
            {
                HttpContext.Session.SetInt32("TrojWorkingTimeSize", 20);
            }

            if (size != null)
            {
                HttpContext.Session.SetInt32("TrojWorkingTimeSize", size.Value);
            }

            int iSize = HttpContext.Session.GetInt32("TrojWorkingTimeSize").Value;
            ViewBag.Size = iSize.ToString();

            int iPage = 0;
            int iOffset = 0;
            if (page != null)
            {
                iPage = page.Value;
                iOffset = (iPage - 1) * iSize;
            }
            ViewBag.Page = iPage;

            HttpContext.Session.SetInt32("TrojWorkingTimeStart", 1);
            HttpContext.Session.SetInt32("TrojWorkingTimeEnd", 10);
            if (iPage > HttpContext.Session.GetInt32("TrojWorkingTimeEnd"))
            {
                HttpContext.Session.SetInt32("TrojWorkingTimeStart", iPage - 1);
                HttpContext.Session.SetInt32("TrojWorkingTimeEnd", iPage + 9);
            }

            if (HttpContext.Session.GetInt32("TrojWorkingTimeStart") == 1)
            {
                ViewBag.Start = 0;
            }
            else
            {
                ViewBag.Start = HttpContext.Session.GetInt32("TrojWorkingTimeStart") - 1;
            }

            ViewBag.TrojWorkingTimeStart = HttpContext.Session.GetInt32("TrojWorkingTimeStart").Value;

            int trojWorkingTimeEnd = HttpContext.Session.GetInt32("TrojWorkingTimeEnd").Value;
            ViewBag.TrojWorkingTimeEnd = trojWorkingTimeEnd;

            if (collection != null && collection.Count > 0)
            {
                HttpContext.Session.SetString("TrojWorkingTimeFilter", "Set");
                collection.TryGetValue("CaseId", out StringValues caseId);
                collection.TryGetValue("WhenDate", out StringValues whenDate);
                collection.TryGetValue("CaseTypeId", out StringValues caseTypeId);
                collection.TryGetValue("EmployeeId", out StringValues employeeId);
                collection.TryGetValue("Title", out StringValues title);
                collection.TryGetValue("FirstName", out StringValues firstName);
                collection.TryGetValue("LastName", out StringValues lastName);
                HttpContext.Session.SetString("TrojWorkingTimeCaseId", caseId.ToString());
                HttpContext.Session.SetString("TrojWorkingTimeWhenDate", whenDate.ToString());
                HttpContext.Session.SetString("TrojWorkingTimeCaseTypeId", caseTypeId.ToString());
                HttpContext.Session.SetString("TrojWorkingTimeEmployeeId", employeeId.ToString());
                HttpContext.Session.SetString("TrojWorkingTimeCaseTitle", title.ToString());
                HttpContext.Session.SetString("TrojWorkingTimeFirstName", firstName.ToString());
                HttpContext.Session.SetString("TrojWorkingTimeLastName", lastName.ToString());
            }

            if (reset != null)
            {
                HttpContext.Session.SetString("TrojWorkingTimeFilter", "Reset");
                HttpContext.Session.Remove("TrojWorkingTimeCaseId");
                HttpContext.Session.Remove("TrojWorkingTimeWhenDate");
                HttpContext.Session.Remove("TrojWorkingTimeCaseTypeId");
                HttpContext.Session.Remove("TrojWorkingTimeEmployeeId");
                HttpContext.Session.Remove("TrojWorkingTimeCaseTitle");
                HttpContext.Session.Remove("TrojWorkingTimeFirstName");
                HttpContext.Session.Remove("TrojWorkingTimeLastName");
            }


            string currentDate = DateTime.Now.ToShortDateString();
            string nextDate = DateTime.Now.AddDays(1).ToShortDateString();
            ViewBag.CurrentDate = currentDate;
            IEnumerable<WorkingTimesViewModel> workingTimes;
            IEnumerable<CasesClientViewModel> caseList;
            TotalSumAndHoursModel totalSumModel;
            int numberOfWorkingTimes;
            if (HttpContext.Session.GetString("TrojWorkingTimeFilter") == "Set")
            {
                string caseId = HttpContext.Session.GetString("TrojWorkingTimeCaseId");
                string whenDate = HttpContext.Session.GetString("TrojWorkingTimeWhenDate");
                string caseTypeId = HttpContext.Session.GetString("TrojWorkingTimeCaseTypeId");
                string employeeId = HttpContext.Session.GetString("TrojWorkingTimeEmployeeId");
                string title = HttpContext.Session.GetString("TrojWorkingTimeCaseTitle");
                string firstName = HttpContext.Session.GetString("TrojWorkingTimeFirstName");
                string lastName = HttpContext.Session.GetString("TrojWorkingTimeLastName");
                workingTimes = await _workingTimesConnection.GetFilteredWorkingTimes(caseId, whenDate, caseTypeId, title, employeeId, firstName, lastName, iOffset, iSize);
                numberOfWorkingTimes = await _workingTimesConnection.GetNumberOfFilteredWorkingTimes(caseId, whenDate, caseTypeId, title, employeeId, firstName, lastName);
                caseList = await _caseConnection.GetFilteredCasesAndClient(caseId, caseTypeId, title, employeeId, firstName, lastName);
                totalSumModel = await _workingTimesConnection.GetFilteredTotalSum(caseId, whenDate, caseTypeId, title, employeeId, firstName, lastName);
                ViewBag.CaseId = caseId;
                ViewBag.WhenDate = whenDate;
                ViewBag.CaseTypeId = caseTypeId;
                ViewBag.CaseTitle = title;
                ViewBag.EmployeeId = employeeId;
                ViewBag.FirstName = firstName;
                ViewBag.LastName = lastName;
                if (ViewBag.WhenDate != "")
                    ViewBag.PageTitle = "Åtgärder för, " + ViewBag.WhenDate;
                else
                    ViewBag.PageTitle = "Eftersökta åtgärder";
            }
            else
            {
                workingTimes = await _workingTimesConnection.GetWorkingTimesForCurrentDay(currentDate, nextDate, iOffset, iSize);
                numberOfWorkingTimes = await _workingTimesConnection.GetNumberOfWorkingTimesForCurrentDay(currentDate, nextDate);
                caseList = await _caseConnection.GetCasesAndClient();
                totalSumModel = await _workingTimesConnection.GetTotalSum(currentDate, nextDate);
                ViewBag.CaseId = "";
                ViewBag.WhenDate = "";
                ViewBag.CaseTypeId = "";
                ViewBag.EmployeeId = "";
                ViewBag.CaseTitle = "";
                ViewBag.FirstName = "";
                ViewBag.LastName = "";
                ViewBag.PageTitle = "Dagens åtgärder";
            }

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

            List<SelectListItem> caseTypes = new List<SelectListItem>();
            var caseTypeList = await _caseTypeConnection.GetCaseTypes();
            caseTypes.Add(new SelectListItem { Value = "0", Text = "" });
            foreach (var caseType in caseTypeList)
                caseTypes.Add(new SelectListItem { Value = caseType.CaseTypeId.ToString(), Text = caseType.CaseType });
            ViewBag.CaseTypes = caseTypes;

            List<SelectListItem> cases = new List<SelectListItem>();
            foreach (var item in caseList)
                cases.Add(new SelectListItem { Value = item.CaseId.ToString(), Text = item.LastName + ", " + item.FirstName + "/" + item.PersonId + " | " + item.CaseType + "/" + item.CaseId });
            ViewBag.Cases = cases;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            foreach (var item in employeesList)
                employees.Add(new SelectListItem { Value = item.EmployeeId.ToString(), Text = item.Initials });
            ViewBag.Employees = employees;

            List<SelectListItem> tariffTypes = new List<SelectListItem>();
            var tariffTypeList = await _tariffTypesConnection.GetActiveTariffTypeAndLevel();
            foreach (var item in tariffTypeList)
                if (item.TariffLevel > 0)
                    tariffTypes.Add(new SelectListItem { Value = item.TariffTypeId.ToString(), Text = item.TariffType + ", " + item.TariffLevel });
                else
                    tariffTypes.Add(new SelectListItem { Value = item.TariffTypeId.ToString(), Text = item.TariffType });
            ViewBag.TariffTypes = tariffTypes;

            WorkingTimesViewModel zeroWorkingTime = new WorkingTimesViewModel
            {
                ChangedBy = ""
            };
            WorkingTimesViewModel firstWorkingTime = zeroWorkingTime;
            if (workingTimes != null && workingTimes.Count() > 0)
            {
                firstWorkingTime = workingTimes.FirstOrDefault<WorkingTimesViewModel>();
            }
            ViewBag.FirstWorkingTime = firstWorkingTime;

            ViewBag.CurrentEmployee = "0";
            if (string.IsNullOrEmpty(UserName) == false)
            {
                EmployeesModel currentEmployee = await _employeeConnection.GetEmployee(UserName);
                if (currentEmployee != null)
                    ViewBag.CurrentEmployee = currentEmployee.EmployeeId.ToString();
                else
                    ViewBag.CurrentEmployee = "0";
            }

            string maxTitleLenght = "20";
            ConfigurationsModel configuration = await _configurationConnection.GetConfigurationWithkey("ShowMaxTitleLenght");
            if (configuration != null)
                maxTitleLenght = configuration.ConfigValue;
            if (Int32.TryParse(maxTitleLenght, out int parsedMaxLenght))
            {
                ViewBag.MaxTitleLenght = parsedMaxLenght;
            }
            else
            {
                ViewBag.MaxTitleLenght = 20;
            }

            stopwatch.Stop();
            ViewBag.LoadTime = stopwatch.ElapsedMilliseconds;

            return View(workingTimes);
        }

        // GET: WorkingTimesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Cases", new { id });

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            IEnumerable<WorkingTimesViewModel> workingTimes = await _workingTimesConnection.GetWorkingTimesForCase(id);

            TotalSumAndHoursModel totalSumModel = await _workingTimesConnection.GetTotalSumForCase(id);
            ViewBag.TotalSum = totalSumModel.TotalSum;
            ViewBag.TotalHours = totalSumModel.TotalHours;

            WorkingTimesViewModel zeroWorkingTime = new WorkingTimesViewModel
            {
                ChangedBy = ""
            };
            WorkingTimesViewModel firstWorkingTime = zeroWorkingTime;
            if (workingTimes.Count() > 0)
            {
                firstWorkingTime = workingTimes.FirstOrDefault<WorkingTimesViewModel>();
            }
            ViewBag.FirstWorkingTime = firstWorkingTime;

            CasesViewModel currentCase = await _caseConnection.GetCase(id);
            if (currentCase == null)
                return NoContent();
            ViewBag.CaseId = currentCase.CaseId.ToString();
            ViewBag.CaseLinkText = currentCase.CaseType + "/" + currentCase.CaseId.ToString();

            return View(workingTimes);
        }

        // GET: WorkingTimesController/Create
        public async Task<ActionResult> Create()
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            List<SelectListItem> persons = new List<SelectListItem>();
            var personList = await _personConnection.GetActivePersons();
            foreach (var item in personList)
                persons.Add(new SelectListItem { Value = item.PersonId.ToString(), Text = item.LastName + " " + item.FirstName });
            ViewBag.Persons = persons;

            List<SelectListItem> cases = new List<SelectListItem>();
            var caseList = await _caseConnection.GetCasesAndClient();
            foreach (var item in caseList)
                cases.Add(new SelectListItem { Value = item.CaseId.ToString(), Text = item.LastName + ", " + item.FirstName + "/" + item.PersonId + " | " + item.CaseType + "/" + item.CaseId });
            ViewBag.Cases = cases;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            foreach (var item in employeesList)
                employees.Add(new SelectListItem { Value = item.EmployeeId.ToString(), Text = item.Initials });
            ViewBag.Employees = employees;

            List<SelectListItem> tariffTypes = new List<SelectListItem>();
            var tariffTypeList = await _tariffTypesConnection.GetActiveTariffTypeAndLevel();
            foreach (var item in tariffTypeList)
                if (item.TariffLevel > 0)
                    tariffTypes.Add(new SelectListItem { Value = item.TariffTypeId.ToString(), Text = item.TariffType + ", " + item.TariffLevel });
                else
                    tariffTypes.Add(new SelectListItem { Value = item.TariffTypeId.ToString(), Text = item.TariffType });
            ViewBag.TariffTypes = tariffTypes;

            WorkingTimesModel workingTime = new WorkingTimesModel
            {
                WorkingTimeId = 0,
                PersonId = 0,
                CaseId = 0,
                TariffTypeId = 1,
                EmployeeId = 0,
                WhenDate = DateTime.Now,
                TariffLevel = 0,
                NumberOfHours = 1,
                Cost = 0,
                Sum = 0,
                Billed = false
            };

            return View(workingTime);
        }

        // POST: WorkingTimesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (!collection.TryGetValue("CaseId", out StringValues caseId))
                return NoContent();

            if (!collection.TryGetValue("WhenDate", out StringValues whenDate))
                return NoContent();

            if (!collection.TryGetValue("EmployeeId", out StringValues employeeId))
                return NoContent();

            if (!collection.TryGetValue("TariffTypeId", out StringValues tariffTypeId))
                return NoContent();

            if (!collection.TryGetValue("Comment", out StringValues comment))
                return NoContent();

            if (!collection.TryGetValue("NumberOfHours", out StringValues numberOfHours))
                return NoContent();

            if (!collection.TryGetValue("Cost", out StringValues cost))
                return NoContent();

            DateTime? inputWhenDate = null;
            if (whenDate != "")
                inputWhenDate = DateTime.Parse(whenDate);

            double? inputCost = null;
            if (cost != "")
                inputCost = Double.Parse(cost.ToString());

            WorkingTimesModel workingTimesModel = await _workingTimesConnection.CreateWorkingTime(Int32.Parse(caseId.ToString()), Int32.Parse(tariffTypeId.ToString()), Int32.Parse(employeeId.ToString()), Double.Parse(numberOfHours.ToString()), inputCost, comment.ToString(), inputWhenDate, UserName);
            if (workingTimesModel == null)
                return NoContent();

            return RedirectToAction("Index");
        }

        // POST: WorkingTimesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (!collection.TryGetValue("WorkingTimeId", out StringValues workingTimeId))
                return NoContent();

            if (!collection.TryGetValue("Comment", out StringValues comment))
                return NoContent();

            if (!collection.TryGetValue("NumberOfHours", out StringValues numberOfHours))
                return NoContent();

            var response = await _workingTimesConnection.UpdateWorkingTime(int.Parse(workingTimeId.ToString()), Double.Parse(numberOfHours.ToString()), comment.ToString(), UserName);
            if (response == 0)
                return NoContent();

            return RedirectToAction("Index");
        }

        // POST: WorkingTimesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (!collection.TryGetValue("WorkingTimeId", out StringValues workingTimeId))
                return NoContent();

            var response = await _workingTimesConnection.DeleteWorkingTime(int.Parse(workingTimeId.ToString()));
            if (response == 0)
                return NoContent();

            return RedirectToAction("Index");
        }

        // Get: WorkingTimesController/Reset
        public ActionResult Reset()
        {
            return RedirectToAction("Index", new { reset = 1 });
        }
    }
}
