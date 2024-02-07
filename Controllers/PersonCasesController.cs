using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class PersonCasesController : IdenityController
    {
        private readonly CasesConnection _caseConnection;
        private readonly PersonTypesConnection _personTypesConnection;
        private readonly PersonsConnection _personConnection;

        public PersonCasesController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _caseConnection = new CasesConnection(context, configuration["CryKey"]);
            _personTypesConnection = new PersonTypesConnection(context);
            _personConnection = new PersonsConnection(context, configuration["CryKey"]);
        }

        // GET: PersonCasesController/Create
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return NoContent();

            CasesViewModel currentCase = await _caseConnection.GetCase(id.Value);
            if (currentCase == null)
                return NoContent();

            ViewBag.CaseId = currentCase.CaseId.ToString();
            ViewBag.CaseLinkText = currentCase.CaseType + "/" + currentCase.CaseId.ToString();

            List<SelectListItem> personTypes = new List<SelectListItem>();
            var personTypesList = await _personTypesConnection.GetPersonTypes();
            foreach (var personType in personTypesList)
                personTypes.Add(new SelectListItem { Value = personType.PersonTypeId.ToString(), Text = personType.PersonType });
            ViewBag.PersonTypes = personTypes;

            List<SelectListItem> persons = new List<SelectListItem>();
            var personList = await _personConnection.GetActivePersonsNotInCase(id.Value);
            foreach (var person in personList)
                persons.Add(new SelectListItem { Value = person.PersonId.ToString(), Text = person.FirstName + " " + person.LastName });
            ViewBag.Persons = persons;

            return View();
        }

        // POST: PersonCasesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            if (!collection.TryGetValue("CaseId", out StringValues caseId))
                return NoContent();

            if (!collection.TryGetValue("PersonTypeId", out StringValues personTypeId))
                return NoContent();

            if (!collection.TryGetValue("PersonId", out StringValues personId))
                return NoContent();

            int created = await _caseConnection.CreatePersonCase(Int32.Parse(personId.ToString()), Int32.Parse(caseId.ToString()), Int32.Parse(personTypeId.ToString()), UserName);
            if (created == 0)
                return NoContent();

            return RedirectToAction("Details", "Cases", new { id = Int32.Parse(caseId.ToString()) });
        }

        // GET: PersonCasesController/Delete/5
        public async Task<ActionResult> Delete(int id, int caseId)
        {
            var response = await _caseConnection.DeleteCasePerson(id);
            if (response == 0)
                return NoContent();
            return RedirectToAction("Details", "Cases", new { id = caseId });
        }
    }
}
