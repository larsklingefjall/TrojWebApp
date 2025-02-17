using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrojWebApp.Models;


namespace TrojWebApp.Services
{
    public class EmployeesConnection
    {
        private readonly TrojContext _context;

        public EmployeesConnection(TrojContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeesModel>> GetEmployees(bool initials = true)
        {
            if(initials)
                return await _context.Employees.OrderBy(s => s.Initials).ToListAsync();
            else
                return await _context.Employees.OrderBy(s => s.LastName).OrderBy(s => s.FirstName).ToListAsync();
        }

        public async Task<IEnumerable<EmployeesModel>> GetActiveEmployees(bool initials = true)
        {
            if (initials)
                return await _context.Employees.Where(e => e.Represent == true).Where(e => e.Active == true).OrderBy(s => s.Initials).ToListAsync();
            else
                return await _context.Employees.Where(e => e.Represent == true).Where(e => e.Active == true).OrderBy(s => s.LastName).OrderBy(s => s.FirstName).ToListAsync();
        }

        public async Task<IEnumerable<EmployeesModel>> GetOnlyActiveEmployees(bool initials = true)
        {
            if (initials)
                return await _context.Employees.Where(e => e.Active == true).OrderBy(s => s.Initials).ToListAsync();
            else
                return await _context.Employees.Where(e => e.Active == true).OrderBy(s => s.LastName).OrderBy(s => s.FirstName).ToListAsync();
        }

        public async Task<EmployeesModel> GetEmployee(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<EmployeesModel> GetEmployee(string userName)
        {
            return await _context.Employees.FirstOrDefaultAsync(m => m.UserName == userName);
        }

        public async Task<EmployeesModel> CreateEmployee(string firstName, string lastName, string initials, string mailAddress, string title, string userName = "")
        {
            EmployeesModel employee = new EmployeesModel
            {
                EmployeeId = 0,
                FirstName = firstName,
                LastName = lastName,
                Initials = initials,
                MailAddress = mailAddress,
                EmployeeTitle = title,
                Represent = true,
                Active = true,
                ReadOnly = false,
                UserName = mailAddress,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Employees.Add(employee);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return employee;
            else
                return null;
        }

        public async Task<EmployeesModel> UpdateEmployee(int id, string firstName, string lastName, string initials, string mailAddress, string link, string title, bool represent, bool active, bool readOnly, string userName = "")
        {
            EmployeesModel employee = new EmployeesModel
            {
                EmployeeId = id,
                FirstName = firstName,
                LastName = lastName,
                Initials = initials,
                MailAddress = mailAddress,
                EmployeeTitle = title,
                SignatureLink = link,
                Represent = represent,
                Active = active,
                ReadOnly = readOnly,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Entry(employee).State = EntityState.Modified;
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return employee;
            else
                return null;
        }
    }
}
