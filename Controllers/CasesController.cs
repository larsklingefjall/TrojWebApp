using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class CasesController : IdenityController
    {
        private readonly PersonsConnection _personConnection;
        private readonly EmployeesConnection _employeeConnection;
        private readonly CaseTypesConnection _caseTypeConnection;
        private readonly InvoiceUnderlaysConnection _invoiceUnderlaysConnection;
        private readonly CasesConnection _caseConnection;
        private static CasesViewModel _currentCase;
        private readonly UserConnection _userConnection;

        public CasesController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _personConnection = new PersonsConnection(context, configuration["CryKey"]);
            _employeeConnection = new EmployeesConnection(context);
            _caseTypeConnection = new CaseTypesConnection(context);
            _invoiceUnderlaysConnection = new InvoiceUnderlaysConnection(context, configuration["CryKey"]);
            _caseConnection = new CasesConnection(context, configuration["CryKey"]);
            _userConnection = new UserConnection(context);
        }

        // GET: CasesController
        public async Task<ActionResult> Index(int? page, int? size, int? reset, IFormCollection collection)
        {
            ViewBag.CaseMenu = await _userConnection.GetMenuItems(HttpContext.Request, UserName);
            ViewBag.PersonMenu = await _userConnection.GetMenuItems("Persons", UserName);
            ViewBag.UnderlayMenu = await _userConnection.GetMenuItems("InvoiceUnderlays", UserName);
            ViewBag.InvoiceMenu = await _userConnection.GetMenuItems("Invoices", UserName);

            if (HttpContext.Session.GetInt32("TrojCaseSize").HasValue == false)
            {
                HttpContext.Session.SetInt32("TrojCaseSize", 20);
            }

            if (size != null)
            {
                HttpContext.Session.SetInt32("TrojCaseSize", size.Value);
            }

            int iSize = HttpContext.Session.GetInt32("TrojCaseSize").Value;
            ViewBag.Size = iSize.ToString();

            int iPage = 0;
            int iOffset = 0;
            if (page != null)
            {
                iPage = page.Value;
                iOffset = (iPage - 1) * iSize;
            }
            ViewBag.Page = iPage;

            HttpContext.Session.SetInt32("TrojCaseStart", 1);
            HttpContext.Session.SetInt32("TrojCaseEnd", 10);


            if (iPage > HttpContext.Session.GetInt32("TrojCaseEnd"))
            {
                HttpContext.Session.SetInt32("TrojCaseStart", iPage - 1);
                HttpContext.Session.SetInt32("TrojCaseEnd", iPage + 9);
            }

            if (HttpContext.Session.GetInt32("TrojCaseStart") == 1)
            {
                ViewBag.Start = 0;
            }
            else
            {
                ViewBag.Start = HttpContext.Session.GetInt32("TrojCaseStart") - 1;
            }

            int trojCaseStart = HttpContext.Session.GetInt32("TrojCaseStart").Value;
            ViewBag.TrojCaseStart = trojCaseStart;

            int trojCaseEnd = HttpContext.Session.GetInt32("TrojCaseEnd").Value;
            ViewBag.TrojCaseEnd = trojCaseEnd;

            if (collection.Count > 0)
            {
                HttpContext.Session.SetString("TrojCaseFilter", "Set");
                collection.TryGetValue("CaseId", out StringValues caseId);
                collection.TryGetValue("WhenDate", out StringValues whenDate);
                collection.TryGetValue("CaseTypeId", out StringValues caseTypeId);
                collection.TryGetValue("Initials", out StringValues initials);
                collection.TryGetValue("Title", out StringValues title);
                collection.TryGetValue("FirstName", out StringValues firstName);
                collection.TryGetValue("LastName", out StringValues lastName);
                HttpContext.Session.SetString("TrojCaseCaseId", caseId.ToString());
                HttpContext.Session.SetString("TrojCaseWhenDate", whenDate.ToString());
                HttpContext.Session.SetString("TrojCaseCaseTypeId", caseTypeId.ToString());
                HttpContext.Session.SetString("TrojCaseInitials", initials.ToString());
                HttpContext.Session.SetString("TrojCaseCaseTitle", title.ToString());
                HttpContext.Session.SetString("TrojCaseFirstName", firstName.ToString());
                HttpContext.Session.SetString("TrojCaseLastName", lastName.ToString());
            }

            if (reset != null)
            {
                HttpContext.Session.SetString("TrojCaseFilter", "Reset");
                HttpContext.Session.Remove("TrojCaseCaseId");
                HttpContext.Session.Remove("TrojCaseWhenDate");
                HttpContext.Session.Remove("TrojCaseCaseTypeId");
                HttpContext.Session.Remove("TrojCaseInitials");
                HttpContext.Session.Remove("TrojCaseCaseTitle");
                HttpContext.Session.Remove("TrojCaseFirstName");
                HttpContext.Session.Remove("TrojCaseLastName");
            }

            IEnumerable<CasesViewModel> cases;
            int numberOfCases;
            if (HttpContext.Session.GetString("TrojCaseFilter") == "Set")
            {
                string caseId = HttpContext.Session.GetString("TrojCaseCaseId");
                string whenDate = HttpContext.Session.GetString("TrojCaseWhenDate");
                string caseTypeId = HttpContext.Session.GetString("TrojCaseCaseTypeId");
                string initials = HttpContext.Session.GetString("TrojCaseInitials");
                string title = HttpContext.Session.GetString("TrojCaseCaseTitle");
                string firstName = HttpContext.Session.GetString("TrojCaseFirstName");
                string lastName = HttpContext.Session.GetString("TrojCaseLastName");
                cases = await _caseConnection.GetFilteredCases(caseId, whenDate, caseTypeId, title, initials, firstName, lastName, iOffset, iSize);
                numberOfCases = await _caseConnection.GetNumberOfFilteredCases(caseId, whenDate, caseTypeId, title, initials, firstName, lastName);
                ViewBag.CaseId = caseId;
                ViewBag.WhenDate = whenDate;
                ViewBag.CaseTypeId = caseTypeId;
                ViewBag.CaseTitle = title;
                ViewBag.Initials = initials;
                ViewBag.FirstName = firstName;
                ViewBag.LastName = lastName;
            }
            else
            {
                cases = await _caseConnection.GetCases(iOffset, iSize);
                numberOfCases = await _caseConnection.GetNumberOfCases();
                ViewBag.CaseId = "";
                ViewBag.WhenDate = "";
                ViewBag.CaseTypeId = "";
                ViewBag.CaseTitle = "";
                ViewBag.Initials = "";
                ViewBag.FirstName = "";
                ViewBag.LastName = "";
            }

            ViewBag.NumberOfCases = numberOfCases;
            double dMaxPage = Math.Ceiling((double)numberOfCases / iSize);
            int maxPage = Convert.ToInt32(dMaxPage);
            ViewBag.MaxPage = maxPage;

            ViewBag.ShowMaxPage = false;
            if (trojCaseEnd < maxPage - 1)
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
            var caseTypeList = await _caseTypeConnection.GetCaseTypes();
            caseTypes.Add(new SelectListItem { Value = "0", Text = "" });
            foreach (var caseType in caseTypeList)
                caseTypes.Add(new SelectListItem { Value = caseType.CaseTypeId.ToString(), Text = caseType.CaseType });
            ViewBag.CaseTypes = caseTypes;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            employees.Add(new SelectListItem { Value = "", Text = "" });
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.Initials, Text = employee.Initials });
            ViewBag.Employees = employees;

            return View(cases);
        }

        // GET: PersonsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            ViewBag.DeleteCaseNumberPermission = _userConnection.AccessToSubPage("CaseNumbers", "Delete", UserName);
            ViewBag.DeletePersonPermission = _userConnection.AccessToSubPage("PersonCases", "Delete", UserName);

            ViewBag.CaseMenu = await _userConnection.GetMenuItems(HttpContext.Request, UserName);
            ViewBag.PersonMenu = await _userConnection.GetMenuItems("Persons", UserName);
            ViewBag.UnderlayMenu = await _userConnection.GetMenuItems("InvoiceUnderlays", UserName);
            ViewBag.InvoiceMenu = await _userConnection.GetMenuItems("Invoices", UserName);

            CasesViewModel currentCase = await _caseConnection.GetCase(id);
            if (currentCase == null)
                return NoContent();

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            if (string.IsNullOrEmpty(currentCase.Title))
                ViewBag.CaseTypeAndTitle = "Uppdrag: " + currentCase.CaseType + "/" + id;
            else
                ViewBag.CaseTypeAndTitle = "Uppdrag: " + currentCase.CaseType + "/" + id + ", " + currentCase.Title;

            ViewBag.Client = currentCase.FirstName + " " + currentCase.LastName;
            if (currentCase.Comment != null)
                ViewBag.Comment = currentCase.Comment.Replace(Environment.NewLine, "<br />");
            else
                ViewBag.Comment = "";

            var caseNumbers = await _caseConnection.GetCaseNumbers(id);
            if (caseNumbers.Count() == 0)
            {
                ViewBag.CaseNumbers = null;
            }
            else
            {
                ViewBag.CaseNumbers = caseNumbers;
            }

            var personsAtCase = await _caseConnection.GetOtherPersonsAtCase(id);
            if (personsAtCase.Count() == 0)
            {
                ViewBag.PersonsAtCase = null;
            }
            else
            {
                ViewBag.PersonsAtCase = personsAtCase;
            }

            ViewBag.Underlays = await _invoiceUnderlaysConnection.GetUnderlayForCase(id);

            return View(currentCase);
        }

        // GET: CasesController/Create
        public async Task<ActionResult> Create(int? id)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (id == null)
                return NoContent();

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            ViewBag.PersonId = id.Value.ToString();
            PersonsModel currentPerson = await _personConnection.GetPerson(id.Value);
            if (currentPerson == null)
                return NoContent();
            ViewBag.PersonName = currentPerson.FirstName + " " + currentPerson.LastName;

            List<SelectListItem> caseTypes = new List<SelectListItem>();
            var caseTypeList = await _caseTypeConnection.GetCaseTypes();
            foreach (var caseType in caseTypeList)
                caseTypes.Add(new SelectListItem { Value = caseType.CaseTypeId.ToString(), Text = caseType.CaseType });
            ViewBag.CaseTypes = caseTypes;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.Initials, Text = employee.Initials });
            ViewBag.Employees = employees;

            return View();
        }

        // POST: CasesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            if (!collection.TryGetValue("CaseTypeId", out StringValues caseTypeId))
                return NoContent();

            if (!collection.TryGetValue("Title", out StringValues title))
                return NoContent();

            if (!collection.TryGetValue("Responsible", out StringValues responsible))
                return NoContent();

            if (!collection.TryGetValue("PersonId", out StringValues personId))
                return NoContent();

            CasesViewModel casesModel = await _caseConnection.CreateCase(Int32.Parse(caseTypeId.ToString()), title.ToString(), responsible.ToString(), Int32.Parse(personId.ToString()), UserName);
            if (casesModel == null)
                return NoContent();
            else
            {
                string menuTitle = casesModel.CaseType + "/" + casesModel.CaseId;
                await _userConnection.CreateMenuItem(HttpContext.Request, menuTitle, casesModel.CaseId, UserName);
            }

            return RedirectToAction("Details", "Persons", new { id = casesModel.PersonId });
        }

        // GET: CasesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            _currentCase = await _caseConnection.GetCase(id);
            if (_currentCase == null)
                return NoContent();

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            List<SelectListItem> caseTypes = new List<SelectListItem>();
            var caseTypeList = await _caseTypeConnection.GetCaseTypes();
            foreach (var caseType in caseTypeList)
                caseTypes.Add(new SelectListItem { Value = caseType.CaseTypeId.ToString(), Text = caseType.CaseType });
            ViewBag.CaseTypes = caseTypes;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.Initials, Text = employee.Initials });
            ViewBag.Employees = employees;

            List<SelectListItem> persons = new List<SelectListItem>();
            var personList = await _caseConnection.GetAllPersonsAtCase(id);
            foreach (var person in personList)
                persons.Add(new SelectListItem { Value = person.PersonId.ToString(), Text = person.LastName + " " + person.FirstName + ", " + person.PersonType });
            ViewBag.Persons = persons;

            return View(_currentCase);
        }

        // POST: CasesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            if (!collection.TryGetValue("CaseTypeId", out StringValues caseTypeId))
                return NoContent();

            if (!collection.TryGetValue("Title", out StringValues title))
                return NoContent();

            if (!collection.TryGetValue("Responsible", out StringValues responsible))
                return NoContent();

            if (!collection.TryGetValue("FinishedDate", out StringValues finishedDate))
                return NoContent();

            if (!collection.TryGetValue("Comment", out StringValues comment))
                return NoContent();

            if (!collection.TryGetValue("PersonId", out StringValues personId))
                return NoContent();

            bool active = false;
            string[] str = collection["Active"].ToArray();
            if (str.Length > 1)
            {
                active = true;
            }

            DateTime? inputFinishedDate = null;
            if (finishedDate != "")
                inputFinishedDate = DateTime.Parse(finishedDate);

            CasesViewModel updatedCase = await _caseConnection.UpdateCase(id, int.Parse(caseTypeId.ToString()), title.ToString(), responsible.ToString(), active, inputFinishedDate, comment.ToString(), int.Parse(personId.ToString()), _currentCase, UserName);
            if (updatedCase == null)
                return NoContent();
            else
            {
                string menuTitle = updatedCase.CaseType + "/" + updatedCase.CaseId;
                await _userConnection.CreateMenuItem(HttpContext.Request, menuTitle, updatedCase.CaseId, UserName);
            }

            return RedirectToAction("Details", new { id = updatedCase.CaseId });
        }

        // Get: CasesController/Reset
        public ActionResult Reset()
        {
            return RedirectToAction("Index", new { reset = 1 });
        }
    }
}
