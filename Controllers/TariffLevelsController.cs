using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public TariffLevelsController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _tariffLevelsConnection = new TariffLevelsConnection(context);
            _tariffTypesConnection = new TariffTypesConnection(context);
        }

        // GET: TariffLevelsController
        public async Task<ActionResult> Index()
        {
            var tariffLevels = await _tariffLevelsConnection.GetTariffLevels();
            return View(tariffLevels);
        }

        // GET: TariffLevelsController/Create
        public async Task<ActionResult> Create()
        {
            List<SelectListItem> tariffTypes = new List<SelectListItem>();
            var tariffTypeList = await _tariffTypesConnection.GetTariffTypes();
            foreach (var tariffType in tariffTypeList)
                tariffTypes.Add(new SelectListItem { Value = tariffType.TariffTypeId.ToString(), Text = tariffType.TariffType });
            ViewBag.TariffTypes = tariffTypes;

            return View();
        }

        // POST: TariffLevelsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
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

            List<SelectListItem> tariffTypes = new List<SelectListItem>();
            var tariffTypeList = await _tariffTypesConnection.GetTariffTypes();
            foreach (var tariffType in tariffTypeList)
                tariffTypes.Add(new SelectListItem { Value = tariffType.TariffTypeId.ToString(), Text = tariffType.TariffType });
            ViewBag.TariffTypes = tariffTypes;

            return RedirectToAction("Edit", new { id = tariffLevelsModel.TariffLevelId });
        }

        // GET: TariffLevelsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            TariffLevelsModel tariffLevelsModel = await _tariffLevelsConnection.GetTariffLevel(id);
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

            TariffLevelsModel tariffLevelsModel = await _tariffLevelsConnection.UpdateTariffLevel(id, Int32.Parse(tariffTypeId.ToString()), Double.Parse(tariffLevel.ToString()), validFromDate, validToDate, valid);
            if (tariffLevelsModel == null)
                return NoContent();

            var tariffLevels = await _tariffLevelsConnection.GetTariffLevels();
            return View("Index", tariffLevels);
        }
    }
}
