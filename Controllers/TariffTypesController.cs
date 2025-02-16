using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class TariffTypesController : IdenityController
    {
        private readonly TariffTypesConnection _tariffTypesConnection;
        private readonly UserConnection _userConnection;

        public TariffTypesController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _tariffTypesConnection = new TariffTypesConnection(context);
            _userConnection = new UserConnection(context);
        }

        // GET: TariffTypesController
        public async Task<ActionResult> Index()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            ViewBag.CreatePermission = _userConnection.AccessToSubPage(HttpContext.Request, "Create", UserName);
            ViewBag.EditPermission = _userConnection.AccessToSubPage(HttpContext.Request, "Edit", UserName);

            List<SelectListItem> colors = new List<SelectListItem>();
            var backgroundColors = await _tariffTypesConnection.GetBackgroundColors();
            foreach (BackgroundColorsModel backgroundColor in backgroundColors)
                colors.Add(new SelectListItem { Value = backgroundColor.BackgroundColor, Text = backgroundColor.BackgroundColor });
            ViewBag.BackgroundColors = colors;

            var types = await _tariffTypesConnection.GetTariffTypes();
            return View(types);
        }

        // GET: TariffTypesController/Create
        public ActionResult Create()
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            return View();
        }

        // POST: TariffTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (!collection.TryGetValue("TariffType", out StringValues type))
                return NoContent();

            collection.TryGetValue("BackgroundColor", out StringValues BackgroundColor);
            collection.TryGetValue("NewBackgroundColor", out StringValues NewBackgroundColor);

            string backgroundColor = BackgroundColor.ToString();
            if (BackgroundColor.ToString() == "None")
            {
                backgroundColor = NewBackgroundColor.ToString();
            }


            bool noLevel = false;
            if (collection["NoLevel"].ToArray().Length > 1)
            {
                noLevel = true;
            }

            bool invisible = false;
            if (collection["Invisible"].ToArray().Length > 1)
            {
                invisible = true;
            }

            TariffTypesModel tariffType = await _tariffTypesConnection.CreateTariffType(type, noLevel, invisible, backgroundColor, UserName);

            if (tariffType == null)
                return NoContent();

            return RedirectToAction("Edit", new { id = tariffType.TariffTypeId });
        }

        // GET: TariffTypesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            var tariffType = await _tariffTypesConnection.GetTariffType(id);
            if (tariffType == null) return NoContent();

            return View(tariffType);
        }

        // POST: TariffTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (!collection.TryGetValue("TariffType", out StringValues type))
                return NoContent();

            if (!collection.TryGetValue("BackgroundColor", out StringValues backgroundColor))
                return NoContent();

            bool noLevel = false;
            if (collection["NoLevel"].ToArray().Length > 1)
            {
                noLevel = true;
            }

            bool invisible = false;
            if (collection["Invisible"].ToArray().Length > 1)
            {
                invisible = true;
            }

            TariffTypesModel tariffType = await _tariffTypesConnection.UpdateTariffType(id, type, noLevel, invisible, backgroundColor, UserName);

            if (tariffType == null)
                return NoContent();

            return RedirectToAction("Index");
        }

    }
}
