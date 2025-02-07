using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrojWebApp.Models;

namespace TrojWebApp.Services
{
    public class CaseTypesConnection
    {
        private readonly TrojContext _context;

        public CaseTypesConnection(TrojContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CaseTypesModel>> GetCaseTypes()
        {
            return await _context.CaseTypes.OrderBy(s => s.CaseType).ToListAsync();
        }

        public async Task<CaseTypesModel> GetCaseType(int id)
        {
            return await _context.CaseTypes.FindAsync(id);
        }

        public async Task<CaseTypesModel> CreateCaseTypes(string type, string userName)
        {
            CaseTypesModel caseType = new CaseTypesModel
            {
                CaseTypeId = 0,
                CaseType = type,
                Changed = DateTime.Now,
                ChangedBy = userName
            };
            _context.CaseTypes.Add(caseType);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return caseType;
            else
                return null;
        }
    }
}
