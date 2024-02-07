using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrojWebApp.Models;

namespace TrojWebApp.Services
{
    public class PersonTypesConnection
    {
        private readonly TrojContext _context;

        public PersonTypesConnection(TrojContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PersonTypesModel>> GetPersonTypes()
        {
            return await _context.PersonTypes.OrderBy(s => s.PersonType).ToListAsync();
        }

        public async Task<PersonTypesModel> GetPersonType(int id)
        {
            return await _context.PersonTypes.FindAsync(id);
        }

        public async Task<PersonTypesModel> CreatePersonTypes(string type, string userName = "")
        {
            PersonTypesModel personType = new PersonTypesModel
            {
                PersonTypeId = 0,
                PersonType = type,
                Changed = DateTime.Now,
                ChangedBy = userName
            };
            _context.PersonTypes.Add(personType);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return personType;
            else
                return null;
        }
    }
}
