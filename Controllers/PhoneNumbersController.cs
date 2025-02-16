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
    public class PhoneNumbersController : IdenityController
    {
        private readonly PersonsConnection _personConnection;
        private readonly PhoneNumberTypesConnection _phoneNumberTypesConnection;
        private readonly UserConnection _userConnection;
        private static int _currentPersonId;

        public PhoneNumbersController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _personConnection = new PersonsConnection(context, configuration["CryKey"]);
            _phoneNumberTypesConnection = new PhoneNumberTypesConnection(context);
            _userConnection = new UserConnection(context);
        }

        // GET: PhoneNumbersController/Create
        public async Task<ActionResult> Create(int? id)
        {
            if (id == null)
                return NoContent();
            _currentPersonId = id.Value;

            ViewBag.IndexPermissions = await _userConnection.AccessToIndexPages(UserName);
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "Persons", new { id.Value });

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            ViewBag.PersonId = id.Value.ToString();

            PersonsModel currentPerson = await _personConnection.GetPerson(id.Value);
            if (currentPerson == null)
                return NoContent();
            ViewBag.PersonName = currentPerson.FirstName + " " + currentPerson.LastName;

            List<SelectListItem> phoneNumberTypes = new List<SelectListItem>();
            var phoneNumberList = await _phoneNumberTypesConnection.GetPhoneNumberTypes();
            foreach (var type in phoneNumberList)
                phoneNumberTypes.Add(new SelectListItem { Value = type.PhoneNumberTypeId.ToString(), Text = type.PhoneNumberType });
            ViewBag.PhoneNumberTypes = phoneNumberTypes;

            return View();
        }

        // POST: PhoneNumbersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "Persons", new { id = _currentPersonId });

            if (!collection.TryGetValue("PersonId", out StringValues personId))
                return NoContent();

            if (!collection.TryGetValue("PhoneNumberTypeId", out StringValues phoneNumberTypeId))
                return NoContent();

            if (!collection.TryGetValue("PhoneNumber", out StringValues phoneNumber))
                return NoContent();

            PhoneNumbersViewModel newNumber = await _personConnection.CreatePhoneNumber(Int32.Parse(personId.ToString()), Int32.Parse(phoneNumberTypeId.ToString()), phoneNumber.ToString(), UserName);
            if (newNumber == null)
                return NoContent();

            return RedirectToAction("Details", "Persons", new { id = newNumber.PersonId });
        }

        // GET: PhoneNumbersController/Delete/5
        public async Task<ActionResult> Delete(int id, int personId)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "Persons", new { id });

            var response = await _personConnection.DeletePhoneNumber(id);
            if (response == 0)
                return NoContent();
            return RedirectToAction("Details", "Persons", new { id = personId });
        }
    }
}
