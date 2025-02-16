using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
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
            StringBuilder sql = new StringBuilder("SELECT SubPages.*, Pages3.Title AS PageTitle");
            sql.Append(" FROM SubPages INNER JOIN Pages3 ON SubPages.PageId = Pages3.PageId");
            sql.Append(" WHERE Version = 3");
            sql.Append(" ORDER BY SubPages.Controller, SubPages.Position");
            return await _context.SubPagesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<Pages3Model>> GetPages()
        {
            return await _context.Pages3.OrderBy(s => s.Title).ToListAsync();
        }

        public async Task<IEnumerable<SubPagesModel>> GetOnlySubPages()
        {
            return await _context.SubPages.Where(s => s.Version==3).OrderBy(s => s.Controller).ThenBy(s => s.FileName).ToListAsync();
        }

        public async Task<IEnumerable<Pages3Model>> GetPagesWhichHaveChild()
        {
            return await _context.Pages3.OrderBy(s => s.Title).Where(s => s.HasChild == true).ToListAsync();
        }

        public async Task<IEnumerable<PageUsersViewModel>> GetPageUsers()
        {
            StringBuilder sql = new StringBuilder("SELECT PageUsers3.*, Pages3.Title, Pages3.Position, Employees.FirstName, Employees.LastName, Employees.Initials");
            sql.Append(" FROM PageUsers3 INNER JOIN");
            sql.Append(" Pages3 ON PageUsers3.PageId = Pages3.PageId INNER JOIN");
            sql.Append(" Employees ON PageUsers3.EmployeeId = Employees.EmployeeId");
            sql.Append(" ORDER BY Pages3.Position");
            return await _context.PageUsersView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<PageUsersViewModel> GetPageUser(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT PageUsers3.*, Pages3.Title, Pages3.Position, Employees.FirstName, Employees.LastName, Employees.Initials");
            sql.Append(" FROM PageUsers3 INNER JOIN");
            sql.Append(" Pages3 ON PageUsers3.PageId = Pages3.PageId INNER JOIN");
            sql.Append(" Employees ON PageUsers3.EmployeeId = Employees.EmployeeId");
            sql.AppendFormat(" WHERE PageUsers3.PageUserId = {0}", id);
            return await _context.PageUsersView.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<IEnumerable<SubPageUsersViewModel>> GetSubPageUsers()
        {
            StringBuilder sql = new StringBuilder("SELECT SubPageUsers.*, SubPages.Title, SubPages.Controller, SubPages.FileName AS Action, SubPages.Position, Employees.FirstName, Employees.LastName, Employees.Initials");
            sql.Append(" FROM SubPageUsers INNER JOIN");
            sql.Append(" SubPages ON SubPageUsers.SubPageId = SubPages.SubPageId INNER JOIN");
            sql.Append(" Employees ON SubPageUsers.EmployeeId = Employees.EmployeeId");
            sql.Append(" ORDER BY SubPages.Controller, SubPages.FileName");
            return await _context.SubPageUsersView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<SubPageUsersViewModel> GetSubPageUser(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT SubPageUsers.*, SubPages.Title, SubPages.Controller, SubPages.FileName AS Action, SubPages.Position, Employees.FirstName, Employees.LastName, Employees.Initials");
            sql.Append(" FROM SubPageUsers INNER JOIN");
            sql.Append(" SubPages ON SubPageUsers.SubPageId = SubPages.SubPageId INNER JOIN");
            sql.Append(" Employees ON SubPageUsers.EmployeeId = Employees.EmployeeId");
            sql.AppendFormat(" WHERE SubPageUsers.SubPageUserId = {0}", id);
            return await _context.SubPageUsersView.FromSqlRaw(sql.ToString()).FirstAsync();
        }

    }
}
