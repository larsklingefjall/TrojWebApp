using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrojWebApp.Models;
using TrojWebApp.Services;


namespace TrojWebApp.Controllers
{
    [Authorize]
    public class CaseTypesController : IdenityController
    {
        private readonly CaseTypesConnection _connection;

        public CaseTypesController(TrojContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _connection = new CaseTypesConnection(context);
        }

        public async Task<IActionResult> Index()
        {
            var types = await _connection.GetCaseTypes();
            return View(types);
        }

        public async Task<IActionResult> Create(string CaseType)
        {
            if (CaseType == null)
                return NoContent();

            CaseTypesModel caseType = await _connection.CreateCaseTypes(CaseType, UserName);
            if (caseType == null)
                return NoContent();

            if (caseType.CaseType != CaseType)
                return NoContent();

            var types = await _connection.GetCaseTypes();
            return View("Index", types);
        }
    }
}
