using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class TariffLevelsController : IdenityController
    {
        private readonly TariffLevelsConnection _tariffLevelsConnection;
        private readonly TariffTypesConnection _tariffTypesConnection;
        private readonly UserConnection _userConnection;

        public TariffLevelsController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _tariffLevelsConnection = new TariffLevelsConnection(context);
            _tariffTypesConnection = new TariffTypesConnection(context);
            _userConnection = new UserConnection(context);
        }

        // GET: TariffLevelsController
        public async Task<ActionResult> Index()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            ViewBag.CreatePermission = _userConnection.AccessToSubPage(HttpContext.Request, "Create", UserName);
            ViewBag.EditPermission = _userConnection.AccessToSubPage(HttpContext.Request, "Edit", UserName);

            @ViewBag.StartDate = DateTime.Now.Year + "-01-01";
            @ViewBag.EndDate = DateTime.Now.Year + "-12-31";

            List<SelectListItem> tariffTypes = new List<SelectListItem>();
            var tariffTypeList = await _tariffTypesConnection.GetTariffTypes();
            foreach (var tariffType in tariffTypeList)
                tariffTypes.Add(new SelectListItem { Value = tariffType.TariffTypeId.ToString(), Text = tariffType.TariffType });
            ViewBag.TariffTypes = tariffTypes;

            var tariffLevels = await _tariffLevelsConnection.GetTariffLevels();
            return View(tariffLevels);
        }

        // POST: TariffLevelsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (!collection.TryGetValue("TariffTypeId", out StringValues tariffTypeId))
                return NoContent();

            if (!collection.TryGetValue("TariffLevel", out StringValues tariffLevel))
                return NoContent();

            if (!collection.TryGetValue("ValidFrom", out StringValues validFrom))
                return NoContent();

            if (!collection.TryGetValue("ValidTo", out StringValues validTo))
                return NoContent();

            DateTime? validFromDate = null;
            if (validFrom != "")
                validFromDate = DateTime.Parse(validFrom);

            DateTime? validToDate = null;
            if (validTo != "")
                validToDate = DateTime.Parse(validTo);

            TariffLevelsModel tariffLevelsModel = await _tariffLevelsConnection.CreateTariffLevel(Int32.Parse(tariffTypeId.ToString()), Double.Parse(tariffLevel.ToString()), validFromDate, validToDate, UserName);
            if (tariffLevelsModel == null)
                return NoContent();

            return RedirectToAction("Index");
        }

        // GET: TariffLevelsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            TariffLevelsViewModel tariffLevelsModel = await _tariffLevelsConnection.GetTariffLevelView(id);
            if (tariffLevelsModel == null)
                return NoContent();

            List<SelectListItem> tariffTypes = new List<SelectListItem>();
            var tariffTypeList = await _tariffTypesConnection.GetTariffTypes();
            foreach (var tariffType in tariffTypeList)
                tariffTypes.Add(new SelectListItem { Value = tariffType.TariffTypeId.ToString(), Text = tariffType.TariffType });
            ViewBag.TariffTypes = tariffTypes;

            return View("Edit", tariffLevelsModel);
        }

        // POST: TariffLevelsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (!collection.TryGetValue("TariffTypeId", out StringValues tariffTypeId))
                return NoContent();

            if (!collection.TryGetValue("TariffLevel", out StringValues tariffLevel))
                return NoContent();

            if (!collection.TryGetValue("ValidFrom", out StringValues validFrom))
                return NoContent();

            if (!collection.TryGetValue("ValidTo", out StringValues validTo))
                return NoContent();

            bool valid = false;
            string[] str = collection["Valid"].ToArray();
            if (str.Length > 1)
            {
                valid = true;
            }

            DateTime? validFromDate = null;
            if (validFrom != "")
                validFromDate = DateTime.Parse(validFrom);

            DateTime? validToDate = null;
            if (validTo != "")
                validToDate = DateTime.Parse(validTo);

            TariffLevelsModel tariffLevelsModel = await _tariffLevelsConnection.UpdateTariffLevel(id, Int32.Parse(tariffTypeId.ToString()), Double.Parse(tariffLevel.ToString()), validFromDate, validToDate, valid, UserName);
            if (tariffLevelsModel == null)
                return NoContent();

            return RedirectToAction("Index");
        }

        // POST: CasesController/MakeValid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MakeValid(IFormCollection collection)
        {
            if (!collection.TryGetValue("TariffLevelId", out StringValues tariffLevelId))
                return NoContent();

            TariffLevelsViewModel tariffLevelsModel = await _tariffLevelsConnection.MakeTariffLevelValid(Int32.Parse(tariffLevelId.ToString()), UserName);
            if (tariffLevelsModel == null)
                return NoContent();

            return RedirectToAction("Edit", new { id = Int32.Parse(tariffLevelId.ToString()) });
        }
    }
}
