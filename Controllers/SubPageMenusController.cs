using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class SubPageMenusController : IdenityController
    {
        private readonly TrojContext _context;
        private readonly PageConnection _pageConnection;
        private readonly UserConnection _userConnection;

        public SubPageMenusController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _context = context;
            _pageConnection = new PageConnection(context);
            _userConnection = new UserConnection(context);
        }

        // GET: SubPageMenus
        public async Task<IActionResult> Index()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            IEnumerable<SubPageMenusViewModel> menus = await _userConnection.GetMenus();
            return View(menus);
        }

        // GET: SubPageMenus/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            List<SelectListItem> parents = new List<SelectListItem>();
            var parentList = await _pageConnection.GetSubPages();
            foreach (var page in parentList)
                parents.Add(new SelectListItem { Value = page.SubPageId.ToString(), Text = page.Controller + "/" + page.Action });
            ViewBag.Parents = parents;

            List<SelectListItem> children = new List<SelectListItem>();
            var cildrenList = await _pageConnection.GetSubPages();
            foreach (var page in cildrenList)
                children.Add(new SelectListItem { Value = page.SubPageId.ToString(), Text = page.Controller + "/" + page.Action });
            ViewBag.Children = children;

            return View();
        }

        // POST: SubPageMenus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubPageMenuId,ParentPageId,ChildPageId,Position,Changed,ChangedBy")] SubPageMenusModel subPageMenusModel)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                bool exist = await _userConnection.MenuItemExist(subPageMenusModel.ParentPageId, subPageMenusModel.ChildPageId);
                if (exist == false)
                {
                    subPageMenusModel.Changed = DateTime.Now;
                    subPageMenusModel.ChangedBy = UserName;
                    _context.Add(subPageMenusModel);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subPageMenusModel);
        }

        // GET: SubPageMenus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (id == null || _context.SubPageMenus == null)
            {
                return NotFound();
            }
            SubPageMenusViewModel model = await _userConnection.GetMenu(id.Value);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: SubPageMenus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (_context.SubPageMenus == null)
            {
                return Problem("Entity set 'TrojContext.SubPageMenus'  is null.");
            }
            var subPageMenusModel = await _context.SubPageMenus.FindAsync(id);
            if (subPageMenusModel != null)
            {
                _context.SubPageMenus.Remove(subPageMenusModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: SubPageMenus/Sql
        public async Task<IActionResult> Sql()
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");
            var list = await _context.SubPageMenus.ToListAsync();
            return View(list);
        }

        private bool SubPageMenusModelExists(int id)
        {
          return _context.SubPageMenus.Any(e => e.SubPageMenuId == id);
        }
    }
}
