using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrojWebApp.Models;

namespace TrojWebApp.Services
{
    public class PhoneNumberTypesConnection
    {
        private readonly TrojContext _context;

        public PhoneNumberTypesConnection(TrojContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PhoneNumberTypesModel>> GetPhoneNumberTypes()
        {
            return await _context.PhoneNumberTypes.OrderBy(s => s.PhoneNumberType).ToListAsync();
        }

        public async Task<PhoneNumberTypesModel> GetPhoneNumberType(int id)
        {
            return await _context.PhoneNumberTypes.FindAsync(id);
        }

        public async Task<PhoneNumberTypesModel> CreatePhoneNumberType(string type, string userName = "")
        {
            PhoneNumberTypesModel phoneNumberType = new PhoneNumberTypesModel
            {
                PhoneNumberTypeId = 0,
                PhoneNumberType = type,
                Changed = DateTime.Now,
                ChangedBy = userName
            };
            _context.PhoneNumberTypes.Add(phoneNumberType);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return phoneNumberType;
            else
                return null;
        }
    }
}
