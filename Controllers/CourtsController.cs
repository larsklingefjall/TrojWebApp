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
    public class CourtsController : IdenityController
    {
        private readonly CourtsConnection _connection;

        public CourtsController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _connection = new CourtsConnection(context);
        }

        // GET: CourtController
        public async Task<ActionResult> Index()
        {
            var courts = await _connection.GetCourts();
            return View(courts);
        }

        // GET: CourtController/Create
        public async Task<IActionResult> Create(string CourtName)
        {
            if (CourtName == null)
                return NoContent();

            CourtsModel court = await _connection.CreateCourt(CourtName, UserName);
            if (court == null)
                return NoContent();

            if (court.CourtName != CourtName)
                return NoContent();

            return View("Edit", court);
        }

        // POST: CourtController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            if (!collection.TryGetValue("CourtName", out StringValues courtName))
                return NoContent();

            CourtsModel court = await _connection.CreateCourt(courtName, UserName);
            if (court == null)
                return NoContent();

            if (court.CourtName != courtName)
                return NoContent();

            return RedirectToAction("Edit", new { id = court.CourtId });
        }

        // GET: CourtController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var court = await _connection.GetCourt(id);
            return View(court);
        }

        // POST: CourtController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            if (!collection.TryGetValue("CourtName", out StringValues courtName))
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

            CourtsModel updatedCourt = await _connection.UpdateCourt(id, courtName.ToString(), streetName.ToString(), streetNumber.ToString(), postalCode.ToString(), postalAddress.ToString(), country.ToString(), UserName);
            if (updatedCourt == null)
                return NoContent();

            return RedirectToAction("Index");
        }
    }
}
