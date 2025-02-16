using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            IEnumerable<SubPageUsersViewModel> subPageUsers = await _pageConnection.GetSubPageUsers();
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
                pages.Add(new SelectListItem { Value = page.SubPageId.ToString(), Text = page.Controller + "/" + page.FileName });
            ViewBag.Pages = pages;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            employees.Add(new SelectListItem { Value = "", Text = "" });
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.EmployeeId.ToString(), Text = employee.Initials });
            ViewBag.Employees = employees;


            return View();
        }

        // POST: SubPageUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubPageUserId,SubPageId,EmployeeId")] SubPageUsersModel subPageUsersModel)
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

        // GET: SubPageUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SubPageUsers == null)
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
            if (_context.SubPageUsers == null)
            {
                return Problem("Entity set 'TrojContext.SubPageUsers'  is null.");
            }

            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            var subPageUsersModel = await _context.SubPageUsers.FindAsync(id);
            if (subPageUsersModel != null)
            {
                _context.SubPageUsers.Remove(subPageUsersModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubPageUsersModelExists(int id)
        {
          return _context.SubPageUsers.Any(e => e.SubPageUserId == id);
        }
    }
}
