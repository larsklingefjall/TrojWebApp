using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrojWebApp.Models;

namespace TrojWebApp.Services
{
    public class PageConnection
    {
        private readonly TrojContext _context;

        public PageConnection(TrojContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubPagesViewModel>> GetSubPages()
        {
            StringBuilder sql = new StringBuilder("SELECT SubPages.*, Pages.Title AS PageTitle");
            sql.Append(" FROM SubPages INNER JOIN Pages ON SubPages.PageId = Pages.PageId");
            sql.Append(" ORDER BY SubPages.Controller, SubPages.Position");
            return await _context.SubPagesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<PagesModel>> GetPages()
        {
            return await _context.Pages.OrderBy(s => s.Title).ToListAsync();
        }

        public async Task<IEnumerable<PagesModel>> GetPagesWhichHaveChild()
        {
            return await _context.Pages.OrderBy(s => s.Title).Where(s => s.HasChild == true).ToListAsync();
        }

        public async Task<IEnumerable<PageUsersViewModel>> GetPageUsers()
        {
            StringBuilder sql = new StringBuilder("SELECT PageUsers.*, Pages.Title, Pages.Position, Employees.FirstName, Employees.LastName, Employees.Initials");
            sql.Append(" FROM PageUsers INNER JOIN");
            sql.Append(" Pages ON PageUsers.PageId = Pages.PageId INNER JOIN");
            sql.Append(" Employees ON PageUsers.EmployeeId = Employees.EmployeeId");
            sql.Append(" ORDER BY Pages.Position");
            return await _context.PageUsersView.FromSqlRaw(sql.ToString()).ToListAsync();
        }
    }
}
