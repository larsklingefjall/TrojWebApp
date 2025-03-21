﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class ConfigurationsController : IdenityController
    {
        private readonly ConfigurationsConnection _connection;
        private readonly UserConnection _userConnection;

        public ConfigurationsController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _connection = new ConfigurationsConnection(context);
            _userConnection = new UserConnection(context);
        }

        // GET: ConfigurationsController
        public async Task<ActionResult> Index()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            var configurations = await _connection.GetConfigurations();
            return View(configurations);
        }

        // GET: ConfigurationsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: ConfigurationsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            if (!collection.TryGetValue("ConfigKey", out StringValues configKey))
                return NoContent();

            if (!collection.TryGetValue("ConfigValue", out StringValues configValue))
                return NoContent();

            ConfigurationsModel configuration = await _connection.CreateConfiguration(configKey.ToString(), configValue.ToString(), UserName);

            if (configuration == null)
                return NoContent();

            return RedirectToAction("Index");
        }

        // GET: ConfigurationsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            var configuration = await _connection.GetConfiguration(id);
            return View(configuration);
        }

        // POST: ConfigurationsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            if (!collection.TryGetValue("ConfigKey", out StringValues configKey))
                return NoContent();

            if (!collection.TryGetValue("ConfigValue", out StringValues configValue))
                return NoContent();

            ConfigurationsModel configuration = await _connection.UpdateConfiguration(id, configKey.ToString(), configValue.ToString(), UserName);

            if (configuration == null)
                return NoContent();

            return RedirectToAction("Index");
        }

    }
}
