using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class HomeController : IdenityController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WorkingTimesConnection _workingTimesConnection;
        private readonly EmployeesConnection _employeeConnection;
        private readonly CasesConnection _casesConnection;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UserConnection _userConnection;

        public HomeController(ILogger<HomeController> logger, TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _logger = logger;
            _workingTimesConnection = new WorkingTimesConnection(context, configuration["CryKey"]);
            _employeeConnection = new EmployeesConnection(context);
            _casesConnection = new CasesConnection(context, configuration["CryKey"]);
            _userConnection = new UserConnection(context);
            _userManager = userManager;
        }

        public async Task<ActionResult> Index(IFormCollection collection)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            ViewBag.CaseMenu = await _userConnection.GetMenuItems("Cases", UserName);
            ViewBag.PersonMenu = await _userConnection.GetMenuItems("Persons", UserName);
            ViewBag.UnderlayMenu = await _userConnection.GetMenuItems("InvoiceUnderlays", UserName);
            ViewBag.InvoiceMenu = await _userConnection.GetMenuItems("Invoices", UserName);

            ViewBag.UserName = UserName;

            var request = HttpContext.Request;
            var currentUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            ViewBag.Link = currentUrl;


            int currentEmployeeId = 0;
            DateTime currentDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));

            List<SelectListItem> employees = new List<SelectListItem>();
            IEnumerable<EmployeesModel> employeesList = await _employeeConnection.GetOnlyActiveEmployees();
            int firstEmployeeId = employeesList.First().EmployeeId;

            foreach (var item in employeesList)
                employees.Add(new SelectListItem { Value = item.EmployeeId.ToString(), Text = item.Initials });
            ViewBag.Employees = employees;

            if (collection.Count > 0)
            {
                collection.TryGetValue("WhenDate", out StringValues whenDate);
                collection.TryGetValue("EmployeeId", out StringValues employeeId);
                HttpContext.Session.SetString("TrojWorkingTimeEmployeeId", employeeId.ToString());
                HttpContext.Session.SetString("TrojWorkingTimeWhenDate", whenDate.ToString());

                ViewBag.WhenDate = DateOnly.FromDateTime(DateTime.ParseExact(whenDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));
                ViewBag.EmployeeId = employeeId;
                currentEmployeeId = Int32.Parse(employeeId);
                currentDate = DateTime.Parse(whenDate);
            }
            else
            {
                string whenDate = HttpContext.Session.GetString("TrojWorkingTimeWhenDate");
                string employeeId = HttpContext.Session.GetString("TrojWorkingTimeEmployeeId");
                if (string.IsNullOrEmpty(whenDate) == false && string.IsNullOrEmpty(employeeId) == false)
                {
                    ViewBag.WhenDate = DateOnly.FromDateTime(DateTime.ParseExact(whenDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));
                    ViewBag.EmployeeId = employeeId;
                    currentEmployeeId = int.Parse(employeeId);
                }
                else
                {
                    ViewBag.WhenDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                    EmployeesModel employee = await _employeeConnection.GetEmployee(UserName);
                    if (employee != null)
                    {
                        ViewBag.EmployeeId = employee.EmployeeId.ToString();
                        currentEmployeeId = employee.EmployeeId;
                    }
                    else
                    {
                        ViewBag.EmployeeId = firstEmployeeId.ToString();
                        currentEmployeeId = firstEmployeeId;
                    }
                }
            }

            EmployeesModel currentEmployee = await _employeeConnection.GetEmployee(currentEmployeeId);

            string currentUserName = currentEmployee.UserName;

            IEnumerable<WorkingTimesSummarysModel> workingTimesSummeriesOfYear = await _workingTimesConnection.GetUnderlaySummariesOfYear(currentDate, currentUserName);
            ViewBag.WorkingTimesSummeriesOfYear = workingTimesSummeriesOfYear;
            ViewBag.WorkingTimesSummeriesOfYearCount = workingTimesSummeriesOfYear.Count();

            double workingTimesSummeriesOfYearSum = await _workingTimesConnection.GetUnderlaySummariesOfYearSum(currentDate, currentUserName);
            ViewBag.TotalSumOfYear = workingTimesSummeriesOfYearSum;


            IEnumerable<WorkingTimesSummarysModel> workingTimesSummeriesOfWeek = await _workingTimesConnection.GetUnderlaySummariesOfWeek(currentDate, currentUserName);
            if (workingTimesSummeriesOfWeek == null)
            {
                ViewBag.Sql = _workingTimesConnection.GetSqlCommand();
                ViewBag.WorkingTimesSummeriesOfWeekCount = 0;
            }
            else
            {
                ViewBag.WorkingTimesSummeriesOfWeek = workingTimesSummeriesOfWeek;
                ViewBag.WorkingTimesSummeriesOfWeekCount = workingTimesSummeriesOfWeek.Count();
            }

            double workingTimesSummeriesOfWeekSum = await _workingTimesConnection.GetUnderlaySummariesOfWeekSum(currentDate, currentUserName);
            ViewBag.TotalSumOfWeek = workingTimesSummeriesOfWeekSum;


            IEnumerable<WorkingTimesSummarysModel> workingTimesSummeriesOfDay = await _workingTimesConnection.GetUnderlaySummariesOfDay(currentDate, currentUserName);
            if (workingTimesSummeriesOfDay == null)
            {
                ViewBag.Sql = _workingTimesConnection.GetSqlCommand();
                ViewBag.WorkingTimesSummeriesOfDayCount = 0;
            }
            else
            {
                ViewBag.WorkingTimesSummeriesOfDay = workingTimesSummeriesOfDay;
                ViewBag.WorkingTimesSummeriesOfDayCount = workingTimesSummeriesOfDay.Count();
            }

            double workingTimesSummeriesOfDaySum = await _workingTimesConnection.GetUnderlaySummariesOfDaySum(currentDate, currentUserName);
            ViewBag.TotalSumOfDay = workingTimesSummeriesOfDaySum;


            IEnumerable<ActiveCasesWorkingTimesViewModel> cases = await _casesConnection.GetActiveCasesWithWorkingtimes(currentUserName);
            ViewBag.Cases = cases;
            ViewBag.CasesCount = cases.Count();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
