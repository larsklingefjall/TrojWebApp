using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class EmployeesController : IdenityController
    {
        private readonly EmployeesConnection _connection;

        public EmployeesController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _connection = new EmployeesConnection(context);
        }

        // GET: EmployeesController
        public async Task<ActionResult> Index()
        {
            var employees = await _connection.GetEmployees();
            return View(employees);
        }

        // GET: EmployeesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
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

            EmployeesModel employeesModel = await _connection.CreateEmployee(
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
            var employeesModel = await _connection.GetEmployee(id);
            return View(employeesModel);
        }

        // POST: EmployeesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
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

            EmployeesModel employeesModel = await _connection.UpdateEmployee(
                    id, 
                    firstName.ToString(), 
                    lastName.ToString(), 
                    initials.ToString(), 
                    mailAddress.ToString(), 
                    title.ToString(), 
                    represent, 
                    active,
                    readOnly,
                    UserName);

            if (employeesModel == null)
                return NoContent();

            return View("Edit", employeesModel);
        }
    }
}
