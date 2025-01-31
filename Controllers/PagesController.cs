using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class PagesController : IdenityController
    {
        private readonly TrojContext _context;
        private readonly UserConnection _userConnection;

        public PagesController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _context = context;
            _userConnection = new UserConnection(context);
        }

        // GET: Pages
        public async Task<IActionResult> Index()
        {
            var list = await _context.Pages.ToListAsync();
            var sortedList = list.OrderBy(item => item.Position).ToList();
            return View(sortedList);
        }

        // GET: Pages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PageId,Title,FileName,Tip,Link,Position,Hidden,HasChild,Changed,ChangedBy")] PagesModel pagesModel)
        {
            if (ModelState.IsValid)
            {
                pagesModel.Changed = DateTime.Now;
                pagesModel.ChangedBy = UserName;
                _context.Add(pagesModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pagesModel);
        }

        // GET: Pages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var pagesModel = await _context.Pages.FindAsync(id);
            if (pagesModel == null)
            {
                return NotFound();
            }
            return View(pagesModel);
        }

        // POST: Pages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PageId,Title,FileName,Tip,Link,Position,Hidden,HasChild,Changed,ChangedBy")] PagesModel pagesModel)
        {
            if (id != pagesModel.PageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    pagesModel.Changed = DateTime.Now;
                    pagesModel.ChangedBy = UserName;
                    _context.Update(pagesModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagesModelExists(pagesModel.PageId))
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
            return View(pagesModel);
        }

        private bool PagesModelExists(int id)
        {
          return _context.Pages.Any(e => e.PageId == id);
        }
    }
}
