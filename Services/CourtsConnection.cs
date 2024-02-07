using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrojWebApp.Models;


namespace TrojWebApp.Services
{
    public class CourtsConnection
    {
        private readonly TrojContext _context;

        public CourtsConnection(TrojContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourtsModel>> GetCourts()
        {
            return await _context.Courts.OrderBy(c => c.CourtName).ToListAsync();
        }

        public async Task<CourtsModel> GetCourt(int id)
        {
            return await _context.Courts.FindAsync(id);
        }

        public async Task<CourtsModel> CreateCourt(string name, string userName = "")
        {
            CourtsModel court = new CourtsModel
            {
                CourtId = 0,
                CourtName = name,
                StreetName = "",
                StreetNumber = "",
                PostalCode = "",
                PostalAddress = "",
                Country = "",
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Courts.Add(court);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return court;
            else
                return null;
        }

        public async Task<CourtsModel> UpdateCourt(int id, string name, string street, string number, string postalCode, string PostalAddress, string country, string userName = "")
        {
            CourtsModel court = new CourtsModel
            {
                CourtId = id,
                CourtName = name,
                StreetName = street,
                StreetNumber = number,
                PostalCode = postalCode,
                PostalAddress = PostalAddress,
                Country = country,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Entry(court).State = EntityState.Modified;
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return court;
            else
                return null;
        }
    }
}
