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
    public class SubPagesController : IdenityController
    {
        private readonly TrojContext _context;
        private readonly PageConnection _pageConnection;
        private readonly UserConnection _userConnection;

        public SubPagesController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _context = context;
            _pageConnection = new PageConnection(context);
            _userConnection = new UserConnection(context);
        }

        // GET: SubPages
        public async Task<IActionResult> Index()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            IEnumerable<SubPagesViewModel> subPages = await _pageConnection.GetSubPages();
            return View(subPages);
        }

        // GET: SubPages/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            List<SelectListItem> pages = new List<SelectListItem>();
            var pageList = await _pageConnection.GetPagesWhichHaveChild();
            foreach (var page in pageList)
                pages.Add(new SelectListItem { Value = page.PageId.ToString(), Text = page.Controller });
            ViewBag.Pages = pages;

            return View();
        }

        // POST: SubPages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubPageId,PageId,Controller,Title,FileName,Tip,Position,Parameter,IsVisible,Changed,ChangedBy")] SubPagesModel subPagesModel)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                subPagesModel.Changed = DateTime.Now;
                subPagesModel.ChangedBy = UserName;
                subPagesModel.Version = 3;
                _context.Add(subPagesModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subPagesModel);
        }

        // GET: SubPages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SubPages == null)
            {
                return NotFound();
            }

            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            List<SelectListItem> pages = new List<SelectListItem>();
            var pageList = await _pageConnection.GetPages();
            foreach (var page in pageList)
                pages.Add(new SelectListItem { Value = page.PageId.ToString(), Text = page.Title });
            ViewBag.Pages = pages;

            var subPagesModel = await _context.SubPages.FindAsync(id);
            if (subPagesModel == null)
            {
                return NotFound();
            }
            return View(subPagesModel);
        }

        // POST: SubPages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubPageId,PageId,Controller,Title,FileName,Tip,Position,Parameter,IsVisible,Version,Changed,ChangedBy")] SubPagesModel subPagesModel)
        {
            if (id != subPagesModel.SubPageId)
            {
                return NotFound();
            }

            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                try
                {
                    subPagesModel.Changed = DateTime.Now;
                    subPagesModel.ChangedBy = UserName;
                    _context.Update(subPagesModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubPagesModelExists(subPagesModel.SubPageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subPagesModel);
        }

        private bool SubPagesModelExists(int id)
        {
            return _context.SubPages.Any(e => e.SubPageId == id);
        }
    }
}
