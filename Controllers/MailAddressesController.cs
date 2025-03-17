using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class MailAddressesController : IdenityController
    {
        private readonly PersonsConnection _personConnection;
        private readonly UserConnection _userConnection;
        private readonly TrojContext _context;
        private static int _currentPersonId;


        public MailAddressesController(TrojContext context, IConfiguration configuration, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _context = context;
            _personConnection = new PersonsConnection(context, configuration["CryKey"]);
            _userConnection = new UserConnection(context);
        }

        // GET: MailAddresses/Create
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

            return View();
        }

        // POST: MailAddresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MailAddressId,PersonId,MailAddress,Comment,Changed,ChangedBy")] MailAddressesModel mailAddressesModel)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "Persons", new { id = _currentPersonId });

            if (ModelState.IsValid)
            {
                MailAddressesModel newMail = await _personConnection.CreateMailAddress(mailAddressesModel.PersonId, mailAddressesModel.MailAddress.ToString(), mailAddressesModel.Comment, UserName);
                if (newMail == null)
                    return NoContent();
            }
            return RedirectToAction("Details", "Persons", new { id = _currentPersonId });
        }

        // GET: MailAddresses/Delete/5
        public async Task<IActionResult> Delete(int id, int personId)
        {
            var permission = _userConnection.AccessToSubPage(HttpContext.Request, UserName);
            if (!permission) return RedirectToAction("Details", "Persons", new { id = personId });

            var mailAddressesModel = await _context.MailAddresses.FirstOrDefaultAsync(m => m.MailAddressId == id);
            if (mailAddressesModel == null)
            {
                return NotFound();
            }
            else
            {
                await _personConnection.DeleteMail(id);
            }
            return RedirectToAction("Details", "Persons", new { id = personId });
        }

    }
}
