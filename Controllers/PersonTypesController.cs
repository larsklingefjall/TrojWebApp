using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrojWebApp.Models;
using TrojWebApp.Services;

namespace TrojWebApp.Controllers
{
    [Authorize]
    public class PersonTypesController : IdenityController
    {
        private readonly PersonTypesConnection _connection;

        public PersonTypesController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _connection = new PersonTypesConnection(context);
        }

        // GET: PersonTypesController
        public async Task<IActionResult> Index()
        {
            var types = await _connection.GetPersonTypes();
            return View(types);
        }

        // GET: PersonTypesController/Create
        public async Task<IActionResult> Create(string PersonType)
        {
            if (PersonType == null)
                return NoContent();

            PersonTypesModel personType = await _connection.CreatePersonTypes(PersonType, UserName);
            if (personType == null)
                return NoContent();

            if (personType.PersonType != PersonType)
                return NoContent();

            var types = await _connection.GetPersonTypes();
            return View("Index", types);
        }
    }
}
