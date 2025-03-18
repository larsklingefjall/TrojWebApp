using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class SubPageUsersController : IdenityController
    {
        private readonly TrojContext _context;
        private readonly PageConnection _pageConnection;
        private readonly EmployeesConnection _employeeConnection;
        private readonly UserConnection _userConnection;

        public SubPageUsersController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _context = context;
            _pageConnection = new PageConnection(context);
            _employeeConnection = new EmployeesConnection(context);
            _userConnection = new UserConnection(context);
        }

        // GET: SubPageUsers
        public async Task<IActionResult> Index(int? reset, IFormCollection collection)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            if (collection.Count > 0)
            {
                HttpContext.Session.SetString("TrojSubPageUserFilter", "Set");
                collection.TryGetValue("EmployeeId", out StringValues employeeId);
                HttpContext.Session.SetString("TrojSubPageUser", employeeId.ToString());
            }

            if (reset != null)
            {
                HttpContext.Session.SetString("TrojSubPageUserFilter", "Reset");
                HttpContext.Session.Remove("TrojSubPageUser");
            }

            IEnumerable<SubPageUsersView3Model> subPageUsers;
            int numberItems;
            if (HttpContext.Session.GetString("TrojSubPageUserFilter") == "Set")
            {
                string employeeId = HttpContext.Session.GetString("TrojSubPageUser");
                subPageUsers = await _pageConnection.GetSubPageUsers(Int32.Parse(employeeId.ToString()));
                numberItems = subPageUsers.Count();
                ViewBag.Initials = employeeId;
            }
            else
            {
                subPageUsers = await _pageConnection.GetSubPageUsers();
                numberItems = subPageUsers.Count();
                ViewBag.Initials = "";
            }
            ViewBag.NumberOfItems = numberItems;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            employees.Add(new SelectListItem { Value = "", Text = "" });
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.EmployeeId.ToString(), Text = employee.FirstName + " " + employee.LastName });
            ViewBag.Employees = employees;

            return View(subPageUsers);
        }

        // GET: SubPageUsers/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            List<SelectListItem> pages = new List<SelectListItem>();
            var pageList = await _pageConnection.GetOnlySubPages();
            foreach (var page in pageList)
                pages.Add(new SelectListItem { Value = page.SubPageId.ToString(), Text = page.Controller + "/" + page.Action });
            ViewBag.Pages = pages;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            employees.Add(new SelectListItem { Value = "", Text = "" });
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.EmployeeId.ToString(), Text = employee.FirstName + " " + employee.LastName });
            ViewBag.Employees = employees;

            return View();
        }

        // POST: SubPageUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubPageUserId,SubPageId,EmployeeId")] SubPageUsers3Model subPageUsersModel)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                subPageUsersModel.Changed = DateTime.Now;
                subPageUsersModel.ChangedBy = UserName;
                _context.Add(subPageUsersModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subPageUsersModel);
        }

        // GET: SubPageUsers/Copy
        public async Task<IActionResult> Copy()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            employees.Add(new SelectListItem { Value = "", Text = "" });
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.EmployeeId.ToString(), Text = employee.FirstName + " " + employee.LastName });
            ViewBag.Employees = employees;

            return View();
        }

        // POST: PageUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Copy(int fromEmployeeId, int toEmployeeId)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");
            bool succeed = await _pageConnection.CopySubPageUsers(fromEmployeeId, toEmployeeId, UserName);
            return RedirectToAction(nameof(Index));
        }

        // GET: SubPageUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SubPageUsers3 == null)
            {
                return NotFound();
            }

            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            var subPageUsersModel = await _pageConnection.GetSubPageUser(id.Value);
            if (subPageUsersModel == null)
            {
                return NotFound();
            }

            return View(subPageUsersModel);
        }

        // POST: SubPageUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SubPageUsers3 == null)
            {
                return Problem("Entity set 'TrojContext.SubPageUsers3'  is null.");
            }

            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            var subPageUsersModel = await _context.SubPageUsers3.FindAsync(id);
            if (subPageUsersModel != null)
            {
                _context.SubPageUsers3.Remove(subPageUsersModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Sql()
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");
            var list = await _context.SubPageUsers3.ToListAsync();
            return View(list);
        }

        private bool SubPageUsersModelExists(int id)
        {
            return _context.SubPageUsers3.Any(e => e.SubPageUserId == id);
        }

        public ActionResult Reset()
        {
            return RedirectToAction("Index", new { reset = 1 });
        }
    }
}
