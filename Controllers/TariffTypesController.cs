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
        private readonly TariffTypesConnection _connection;

        public TariffTypesController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _connection = new TariffTypesConnection(context);
        }

        // GET: TariffTypesController
        public async Task<ActionResult> Index()
        {
            List<SelectListItem> colors = new List<SelectListItem>();
            var backgroundColors = await _connection.GetBackgroundColors();
            foreach (BackgroundColorsModel backgroundColor in backgroundColors)
                colors.Add(new SelectListItem { Value = backgroundColor.BackgroundColor, Text = backgroundColor.BackgroundColor });
            ViewBag.BackgroundColors = colors;

            var types = await _connection.GetTariffTypes();
            return View(types);
        }

        // GET: TariffTypesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TariffTypesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
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

            TariffTypesModel tariffType = await _connection.CreateTariffType(type, noLevel, invisible, backgroundColor, UserName);

            if (tariffType == null)
                return NoContent();

            return RedirectToAction("Edit", new { id = tariffType.TariffTypeId });
        }

        // GET: TariffTypesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var tariffType = await _connection.GetTariffType(id);
            return View(tariffType);
        }

        // POST: TariffTypesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
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

            TariffTypesModel tariffType = await _connection.UpdateTariffType(id, type, noLevel, invisible, backgroundColor, UserName);

            if (tariffType == null)
                return NoContent();

            return RedirectToAction("Index");
        }

    }
}
