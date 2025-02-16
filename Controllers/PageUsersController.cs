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
    public class PageUsersController : IdenityController
    {
        private readonly TrojContext _context;
        private readonly PageConnection _pageConnection;
        private readonly EmployeesConnection _employeeConnection;
        private readonly UserConnection _userConnection;

        public PageUsersController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _context = context;
            _pageConnection = new PageConnection(context);
            _employeeConnection = new EmployeesConnection(context);
            _userConnection = new UserConnection(context);
        }

        // GET: PageUsers
        public async Task<IActionResult> Index()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            IEnumerable<PageUsersViewModel> pageUsers = await _pageConnection.GetPageUsers();
            return View(pageUsers);
        }

        // GET: PageUsers/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);

            List<SelectListItem> pages = new List<SelectListItem>();
            var pageList = await _pageConnection.GetPages();
            foreach (var page in pageList)
                pages.Add(new SelectListItem { Value = page.PageId.ToString(), Text = page.Title });
            ViewBag.Pages = pages;

            List<SelectListItem> employees = new List<SelectListItem>();
            var employeesList = await _employeeConnection.GetActiveEmployees();
            employees.Add(new SelectListItem { Value = "", Text = "" });
            foreach (var employee in employeesList)
                employees.Add(new SelectListItem { Value = employee.EmployeeId.ToString(), Text = employee.Initials });
            ViewBag.Employees = employees;

            return View();
        }

        // POST: PageUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PageUserId,PageId,EmployeeId")] PageUsersModel pageUsersModel)
        {
            if (ModelState.IsValid)
            {
                pageUsersModel.Changed = DateTime.Now;
                pageUsersModel.ChangedBy = UserName;
                _context.Add(pageUsersModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pageUsersModel);
        }

        // GET: PageUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PageUsers == null)
            {
                return NotFound();
            }

            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);

            var pageUsersModel = await _pageConnection.GetPageUser(id.Value);
            if (pageUsersModel == null)
            {
                return NotFound();
            }

            return View(pageUsersModel);
        }

        // POST: PageUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PageUsers == null)
            {
                return Problem("Entity set 'TrojContext.PageUsers'  is null.");
            }

            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);

            var pageUsersModel = await _context.PageUsers.FindAsync(id);
            if (pageUsersModel != null)
            {
                _context.PageUsers.Remove(pageUsersModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PageUsersModelExists(int id)
        {
          return _context.PageUsers.Any(e => e.PageUserId == id);
        }
    }
}
