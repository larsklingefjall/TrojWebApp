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

        public async Task<IEnumerable<SubPagesView3Model>> GetSubPages()
        {
            StringBuilder sql = new StringBuilder("SELECT SubPages3.*, Pages3.Title AS PageTitle");
            sql.Append(" FROM SubPages3 INNER JOIN Pages3 ON SubPages3.PageId = Pages3.PageId");
            sql.Append(" ORDER BY SubPages3.Controller, SubPages3.Position");
            return await _context.SubPagesView3.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<Pages3Model>> GetPages()
        {
            return await _context.Pages3.OrderBy(s => s.Title).ToListAsync();
        }

        public async Task<IEnumerable<SubPages3Model>> GetOnlySubPages()
        {
            return await _context.SubPages3.OrderBy(s => s.Controller).ThenBy(s => s.Action).ToListAsync();
        }

        public async Task<IEnumerable<Pages3Model>> GetPagesWhichHaveChild()
        {
            return await _context.Pages3.OrderBy(s => s.Title).Where(s => s.HasChild == true).ToListAsync();
        }

        public async Task<IEnumerable<PageUsersView3Model>> GetPageUsers()
        {
            StringBuilder sql = new StringBuilder("SELECT PageUsers3.*, Pages3.Title, Pages3.Position, Employees.FirstName, Employees.LastName, Employees.Initials");
            sql.Append(" FROM PageUsers3 INNER JOIN");
            sql.Append(" Pages3 ON PageUsers3.PageId = Pages3.PageId INNER JOIN");
            sql.Append(" Employees ON PageUsers3.EmployeeId = Employees.EmployeeId");
            sql.Append(" ORDER BY Pages3.Position");
            return await _context.PageUsersView3.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<PageUsersView3Model> GetPageUser(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT PageUsers3.*, Pages3.Title, Pages3.Position, Employees.FirstName, Employees.LastName, Employees.Initials");
            sql.Append(" FROM PageUsers3 INNER JOIN");
            sql.Append(" Pages3 ON PageUsers3.PageId = Pages3.PageId INNER JOIN");
            sql.Append(" Employees ON PageUsers3.EmployeeId = Employees.EmployeeId");
            sql.AppendFormat(" WHERE PageUsers3.PageUserId = {0}", id);
            return await _context.PageUsersView3.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<IEnumerable<SubPageUsersView3Model>> GetSubPageUsers()
        {
            StringBuilder sql = new StringBuilder("SELECT SubPageUsers3.*, SubPages3.Title, SubPages3.Controller, SubPages3.Action, SubPages3.Position, Employees.FirstName, Employees.LastName, Employees.Initials");
            sql.Append(" FROM SubPageUsers3 INNER JOIN");
            sql.Append(" SubPages3 ON SubPageUsers3.SubPageId = SubPages3.SubPageId INNER JOIN");
            sql.Append(" Employees ON SubPageUsers3.EmployeeId = Employees.EmployeeId");
            sql.Append(" ORDER BY SubPages3.Controller, SubPages3.Action");
            return await _context.SubPageUsersView3.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<SubPageUsersView3Model> GetSubPageUser(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT SubPageUsers3.*, SubPages3.Title, SubPages3.Controller, SubPages3.Action, SubPages3.Position, Employees.FirstName, Employees.LastName, Employees.Initials");
            sql.Append(" FROM SubPageUsers3 INNER JOIN");
            sql.Append(" SubPages3 ON SubPageUsers3.SubPageId = SubPages3.SubPageId INNER JOIN");
            sql.Append(" Employees ON SubPageUsers3.EmployeeId = Employees.EmployeeId");
            sql.AppendFormat(" WHERE SubPageUsers3.SubPageUserId = {0}", id);
            return await _context.SubPageUsersView3.FromSqlRaw(sql.ToString()).FirstAsync();
        }

    }
}
