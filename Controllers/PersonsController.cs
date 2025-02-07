using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class PersonsController : IdenityController
    {
        private readonly PersonsConnection _connection;
        private readonly UserConnection _userConnection;

        public PersonsController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _connection = new PersonsConnection(context, configuration["CryKey"]);
            _userConnection = new UserConnection(context);
        }

        // GET: PersonsController
        public async Task<ActionResult> Index(int? page, int? size, int? reset, IFormCollection collection)
        {
            var permission = _userConnection.AccessToIndexPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index", "Home");

            if (HttpContext.Session.GetInt32("TrojPersonSize").HasValue == false)
            {
                HttpContext.Session.SetInt32("TrojPersonSize", 20);
            }

            if (size != null)
            {
                HttpContext.Session.SetInt32("TrojPersonSize", size.Value);
            }

            int iSize = HttpContext.Session.GetInt32("TrojPersonSize").Value;
            ViewBag.Size = iSize.ToString();

            int iPage = 0;
            int iOffset = 0;
            if (page != null)
            {
                iPage = page.Value;
                iOffset = (iPage - 1) * iSize;
            }
            ViewBag.Page = iPage;

            HttpContext.Session.SetInt32("TrojPersonStart", 1);
            HttpContext.Session.SetInt32("TrojPersonEnd", 10);


            if (iPage > HttpContext.Session.GetInt32("TrojPersonEnd"))
            {
                HttpContext.Session.SetInt32("TrojPersonStart", iPage - 1);
                HttpContext.Session.SetInt32("TrojPersonEnd", iPage + 9);
            }

            if (HttpContext.Session.GetInt32("TrojPersonStart") == 1)
            {
                ViewBag.Start = 0;
            }
            else
            {
                ViewBag.Start = HttpContext.Session.GetInt32("TrojPersonStart") - 1;
            }

            int trojPersonStart = HttpContext.Session.GetInt32("TrojPersonStart").Value;
            ViewBag.TrojPersonStart = trojPersonStart;

            int trojPersonEnd = HttpContext.Session.GetInt32("TrojPersonEnd").Value;
            ViewBag.TrojPersonEnd = trojPersonEnd;

            if (collection.Count > 0)
            {
                HttpContext.Session.SetString("TrojPersonFilter", "Set");
                collection.TryGetValue("FirstName", out StringValues firstName);
                collection.TryGetValue("LastName", out StringValues lastName);
                collection.TryGetValue("MiddleName", out StringValues middleName);
                collection.TryGetValue("PersonNumber", out StringValues personNumber);
                collection.TryGetValue("MailAddress", out StringValues mailAddress);
                collection.TryGetValue("Active", out StringValues active);
                HttpContext.Session.SetString("TrojPersonFirstName", firstName.ToString());
                HttpContext.Session.SetString("TrojPersonLastName", lastName.ToString());
                HttpContext.Session.SetString("TrojPersonMiddleName", middleName.ToString());
                HttpContext.Session.SetString("TrojPersonPersonNumber", personNumber.ToString());
                HttpContext.Session.SetString("TrojPersonMailAddress", mailAddress.ToString());
                HttpContext.Session.SetString("TrojPersonActive", active.ToString());
            }

            if (reset != null)
            {
                HttpContext.Session.SetString("TrojPersonFilter", "Reset");
                HttpContext.Session.Remove("TrojPersonFirstName");
                HttpContext.Session.Remove("TrojPersonLastName");
                HttpContext.Session.Remove("TrojPersonMiddleName");
                HttpContext.Session.Remove("TrojPersonPersonNumber");
                HttpContext.Session.Remove("TrojPersonMailAddress");
                HttpContext.Session.Remove("TrojPersonActive");
            }

            IEnumerable<PersonsModel> persons;
            int numberOfPerson;
            if (HttpContext.Session.GetString("TrojPersonFilter") == "Set")
            {
                string firstName = HttpContext.Session.GetString("TrojPersonFirstName");
                string lastName = HttpContext.Session.GetString("TrojPersonLastName");
                string middleName = HttpContext.Session.GetString("TrojPersonMiddleName");
                string personNumber = HttpContext.Session.GetString("TrojPersonPersonNumber");
                string mailAddress = HttpContext.Session.GetString("TrojPersonMailAddress");
                string active = HttpContext.Session.GetString("TrojPersonActive");
                persons = await _connection.GetFilteredPersons(firstName, lastName, middleName, personNumber, mailAddress, active, iOffset, iSize);
                numberOfPerson = await _connection.GetNumberOfFilteredPersons(firstName, lastName, middleName, personNumber, mailAddress, active);
                ViewBag.FirstName = firstName;
                ViewBag.LastName = lastName;
                ViewBag.MiddleName = middleName;
                ViewBag.PersonNumber = personNumber;
                ViewBag.MailAddress = mailAddress;
                ViewBag.Active = active;
            }
            else
            {
                persons = await _connection.GetPersons(iOffset, iSize);
                numberOfPerson = await _connection.GetNumberOfPersons();
                ViewBag.FirstName = "";
                ViewBag.LastName = "";
                ViewBag.MiddleName = "";
                ViewBag.PersonNumber = "";
                ViewBag.MailAddress = "";
                ViewBag.Active = "";
            }

            ViewBag.NumberOfPersons = numberOfPerson;
            double dMaxPage = Math.Ceiling((double)numberOfPerson / iSize);
            int maxPage = Convert.ToInt32(dMaxPage);
            ViewBag.MaxPage = maxPage;

            ViewBag.ShowMaxPage = false;
            if (trojPersonEnd < maxPage - 1)
            {
                ViewBag.ShowMaxPage = true;
            }

            List<SelectListItem> listSizes = new List<SelectListItem>();
            listSizes.Add(new SelectListItem { Value = "5", Text = "5" });
            listSizes.Add(new SelectListItem { Value = "10", Text = "10" });
            listSizes.Add(new SelectListItem { Value = "20", Text = "20" });
            listSizes.Add(new SelectListItem { Value = "40", Text = "40" });
            listSizes.Add(new SelectListItem { Value = "80", Text = "80" });
            ViewBag.ListSizes = listSizes;

            return View(persons);
        }

        // GET: PersonsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            ViewBag.EditAddressPermission = _userConnection.AccessToSubPage("PersonAddresses", "Edit", UserName);
            ViewBag.DeleteAddressPermission = _userConnection.AccessToSubPage("PersonAddresses", "Delete", UserName);

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            PersonsModel person = await _connection.GetPerson(id);
            if (person == null)
                return NoContent();
            if (person.MiddleName != null)
                ViewBag.Person = person.FirstName + " " + person.MiddleName + " " + person.LastName;
            else
                ViewBag.Person = person.FirstName + " " + person.LastName;

            var addresses = await _connection.GetAddressesForPerson(id);
            if (addresses.Count() == 0)
            {
                ViewBag.Addresses = null;
            }
            else
            {
                ViewBag.Addresses = addresses;
            }

            var phoneNumbers = await _connection.GetPhoneNumbersForPerson(id);
            if (phoneNumbers.Count() == 0)
            {
                ViewBag.PhoneNumbers = null;
            }
            else
            {
                ViewBag.PhoneNumbers = phoneNumbers;
            }
            var cases = await _connection.GetPersonAtCases(id);
            if (cases.Count() == 0)
            {
                ViewBag.Cases = null;
            }
            else
            {
                ViewBag.Cases = cases;
            }
            return View(person);
        }

        // GET: PersonsController/Create
        public ActionResult Create()
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            return View();
        }

        // POST: PersonsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            var request = HttpContext.Request;
            var currentUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            ViewBag.Link = currentUrl;

            if (!collection.TryGetValue("FirstName", out StringValues firstName))
                return NoContent();

            if (!collection.TryGetValue("LastName", out StringValues lastName))
                return NoContent();

            if (!collection.TryGetValue("MiddleName", out StringValues middleName))
                return NoContent();

            if (!collection.TryGetValue("PersonNumber", out StringValues personNumber))
                return NoContent();

            if (!collection.TryGetValue("MailAddress", out StringValues mailAddress))
                return NoContent();

            PersonsModel personsModel = await _connection.CreatePerson(firstName.ToString(), lastName.ToString(), middleName.ToString(), personNumber.ToString(), mailAddress.ToString(), UserName);

            if (personsModel == null)
                return NoContent();

            return View("Edit", personsModel);
        }

        // GET: PersonsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Index");

            IEnumerable<SubPageMenusChildViewModel> menu = await _userConnection.GetMenu(HttpContext.Request, UserName);
            ViewBag.Menu = menu;

            var person = await _connection.GetPerson(id);
            return View(person);
        }

        // POST: PersonsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            if (!collection.TryGetValue("FirstName", out StringValues firstName))
                return NoContent();

            if (!collection.TryGetValue("LastName", out StringValues lastName))
                return NoContent();

            if (!collection.TryGetValue("MiddleName", out StringValues middleName))
                return NoContent();

            if (!collection.TryGetValue("PersonNumber", out StringValues personNumber))
                return NoContent();

            if (!collection.TryGetValue("MailAddress", out StringValues mailAddress))
                return NoContent();

            bool active = false;
            string[] str = collection["Active"].ToArray();
            if (str.Length > 1)
            {
                active = true;
            }

            PersonsModel person = await _connection.UpdatePerson(id, firstName.ToString(), lastName.ToString(), middleName.ToString(), personNumber.ToString(), mailAddress.ToString(), active, UserName);

            if (person == null)
                return NoContent();

            return RedirectToAction("Details", new { id = person.PersonId });
        }

        // Get: PersonsController/Reset
        public ActionResult Reset()
        {
            return RedirectToAction("Index", new { reset = 1 });
        }
    }
}
