using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class PersonAddressesController : IdenityController
    {
        private readonly PersonsConnection _personConnection;
        private readonly UserConnection _userConnection;

        public PersonAddressesController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _personConnection = new PersonsConnection(context, configuration["CryKey"]);
            _userConnection = new UserConnection(context);
        }

        // GET: PersonAddressesController
        public async Task<ActionResult> Index(int? id)
        {         
            if(id == null)
                return NoContent();

            PersonsModel currentPerson = await _personConnection.GetPerson(id.Value);
            ViewBag.FirstName = currentPerson.FirstName;
            ViewBag.LastName = currentPerson.LastName;
            var adresses = await _personConnection.GetAddressesForPerson(id.Value);
            return View(adresses);
        }

        // GET: PersonAddressesController/Create
        public async Task<ActionResult> Create(int? id)
        {
            if (id == null)
                return NoContent();

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            ViewBag.PersonId = id.Value.ToString();
            PersonsModel currentPerson = await _personConnection.GetPerson(id.Value);
            if (currentPerson == null)
                return NoContent();
            ViewBag.PersonName = currentPerson.FirstName + " " + currentPerson.LastName;

            return View();
        }

        // POST: PersonAddressesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            if (!collection.TryGetValue("PersonId", out StringValues personId))
                return NoContent();

            if (!collection.TryGetValue("CareOf", out StringValues careOf))
                return NoContent();

            if (!collection.TryGetValue("StreetName", out StringValues streetName))
                return NoContent();

            if (!collection.TryGetValue("StreetNumber", out StringValues streetNumber))
                return NoContent();

            if (!collection.TryGetValue("PostalCode", out StringValues postalCode))
                return NoContent();

            if (!collection.TryGetValue("PostalAddress", out StringValues postalAddress))
                return NoContent();

            if (!collection.TryGetValue("Country", out StringValues country))
                return NoContent();

            PersonAddressesModel newAddress = await _personConnection.CreateAddress(Int32.Parse(personId.ToString()), careOf.ToString(), streetName.ToString(), streetNumber.ToString(), postalCode.ToString(), postalAddress.ToString(), country.ToString(), UserName);
            if (newAddress == null)
                return NoContent();

            List<SelectListItem> persons = new List<SelectListItem>();
            var personList = await _personConnection.GetActivePersons();
            foreach (var person in personList)
                persons.Add(new SelectListItem { Value = person.PersonId.ToString(), Text = person.LastName + " " + person.FirstName });
            ViewBag.Persons = persons;
            var address = await _personConnection.GetAddress(newAddress.PersonAddressId);
            ViewBag.PersonId = address.PersonId.ToString();

            return RedirectToAction("Details", "Persons", new { id = newAddress.PersonId });
        }

        // GET: PersonAddressesController/Edit/5
        public async Task<ActionResult> Edit(int? id, int? personId)
        {
            if (id == null)
                return NoContent();

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            ViewBag.PersonId = personId.Value.ToString();

            PersonsModel currentPerson = await _personConnection.GetPerson(personId.Value);
            if (currentPerson == null)
                return NoContent();
            ViewBag.PersonName = currentPerson.FirstName + " " + currentPerson.LastName;

            var address = await _personConnection.GetAddress(id.Value);
            return View(address);
        }

        // POST: PersonAddressesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            if (!collection.TryGetValue("PersonId", out StringValues personId))
                return NoContent();

            if (!collection.TryGetValue("CareOf", out StringValues careOf))
                return NoContent();

            if (!collection.TryGetValue("StreetName", out StringValues streetName))
                return NoContent();

            if (!collection.TryGetValue("StreetNumber", out StringValues streetNumber))
                return NoContent();

            if (!collection.TryGetValue("PostalCode", out StringValues postalCode))
                return NoContent();

            if (!collection.TryGetValue("PostalAddress", out StringValues postalAddress))
                return NoContent();

            if (!collection.TryGetValue("Country", out StringValues country))
                return NoContent();

            bool valid = false;
            string[] str = collection["Valid"].ToArray();
            if (str.Length > 1)
            {
                valid = true;
            }

            PersonAddressesModel updatedAddress = await _personConnection.UpdateAddress(id, Int32.Parse(personId.ToString()), careOf.ToString(), streetName.ToString(), streetNumber.ToString(), postalCode.ToString(), postalAddress.ToString(), country.ToString(), valid);
            if (updatedAddress == null)
                return NoContent();

            return RedirectToAction("Details", "Persons", new { id = updatedAddress.PersonId });
        }

        // GET: PersonAddressesController/Delete/5
        public async Task<ActionResult> Delete(int id, int personId)
        {
            var response = await _personConnection.DeleteAddress(id);
            if (response == 0)
                return NoContent();
            return RedirectToAction("Details", "Persons", new { id = personId });
        }
    }
}
