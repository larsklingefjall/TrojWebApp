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
    public class EmployeesController : IdenityController
    {
        private readonly EmployeesConnection _employeesConnection;
        private readonly UserConnection _userConnection;

        public EmployeesController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _employeesConnection = new EmployeesConnection(context);
            _userConnection = new UserConnection(context);
        }

        // GET: EmployeesController
        public async Task<ActionResult> Index()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            ViewBag.EditPermission = _userConnection.AccessToSubPage(HttpContext.Request, "Edit", UserName);

            var request = HttpContext.Request;
            ViewBag.Url = $"{request.Scheme}://{request.Host}/images/";

            var employees = await _employeesConnection.GetEmployees();
            return View(employees);
        }

        // GET: EmployeesController/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            return View();
        }

        // POST: EmployeesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (!collection.TryGetValue("FirstName", out StringValues firstName))
                return NoContent();

            if (!collection.TryGetValue("LastName", out StringValues lastName))
                return NoContent();

            if (!collection.TryGetValue("Initials", out StringValues initials))
                return NoContent();

            if (!collection.TryGetValue("MailAddress", out StringValues mailAddress))
                return NoContent();

            if (!collection.TryGetValue("EmployeeTitle", out StringValues title))
                return NoContent();

            EmployeesModel employeesModel = await _employeesConnection.CreateEmployee(
                firstName.ToString(), 
                lastName.ToString(), 
                initials.ToString(), 
                mailAddress.ToString(), 
                title.ToString(),
                UserName);

            if (employeesModel == null)
                return NoContent();

            return View("Edit", employeesModel);
        }

        // GET: EmployeesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            var employeesModel = await _employeesConnection.GetEmployee(id);
            return View(employeesModel);
        }

        // POST: EmployeesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            if (!collection.TryGetValue("FirstName", out StringValues firstName))
                return NoContent();

            if (!collection.TryGetValue("LastName", out StringValues lastName))
                return NoContent();

            if (!collection.TryGetValue("Initials", out StringValues initials))
                return NoContent();

            if (!collection.TryGetValue("MailAddress", out StringValues mailAddress))
                return NoContent();

            if (!collection.TryGetValue("EmployeeTitle", out StringValues title))
                return NoContent();

            if (!collection.TryGetValue("SignatureLink", out StringValues signatureLink))
                return NoContent();

            bool active = false;
            if (collection["Active"].ToArray().Length > 1)
            {
                active = true;
            }

            bool represent = false;
            if (collection["Represent"].ToArray().Length > 1)
            {
                represent = true;
            }

            bool readOnly = false;
            if (collection["ReadOnly"].ToArray().Length > 1)
            {
                readOnly = true;
            }

            EmployeesModel employeesModel = await _employeesConnection.UpdateEmployee(
                    id, 
                    firstName.ToString(), 
                    lastName.ToString(), 
                    initials.ToString(), 
                    mailAddress.ToString(),
                    signatureLink.ToString(),
                    title.ToString(), 
                    represent, 
                    active,
                    readOnly,
                    UserName);

            if (employeesModel == null)
                return NoContent();

            return RedirectToAction("Index");
        }
    }
}
