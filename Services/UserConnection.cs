using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrojWebApp.Models;

namespace TrojWebApp.Services
{
    public class UserConnection
    {
        private readonly TrojContext _context;

        public UserConnection(TrojContext context)
        {
            _context = context;
        }

        public bool AccessToIndexPage(HttpRequest request, string userName)
        {
            string controller;
            if (request.RouteValues["controller"] != null)
                controller = request.RouteValues["controller"].ToString();
            else
                return false;

            string action;
            if (request.RouteValues["action"] != null)
                action = request.RouteValues["action"].ToString();
            else
                return false;

            string fileName = "/" + controller + "/" + action;

            StringBuilder sql = new("SELECT PageId, Title, FileName, Tip, Link, Position, Hidden, HasChild, Changed, ChangedBy");
            sql.Append(" FROM Pages");
            sql.AppendFormat(" WHERE FileName = '{0}'", fileName);
            PagesModel page;
            try
            {
                page = _context.Pages.FromSqlRaw(sql.ToString()).FirstAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get page: '{ex.Message}'");
                return false;
            }
            if (page == null) return false;
            int pageId = page.PageId;


            sql = new("SELECT EmployeeId, FirstName, LastName, Initials, MailAddress, EmployeeTitle, SignatureLink, Represent, Active, ReadOnly, UserName, Changed, ChangedBy");
            sql.Append(" FROM Employees");
            sql.AppendFormat(" WHERE UserName = '{0}'", userName);
            EmployeesModel employee;
            try
            {
                employee = _context.Employees.FromSqlRaw(sql.ToString()).FirstAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to employee: '{ex.Message}'");
                return false;
            }
            if (employee == null) return false;
            int employeeId = employee.EmployeeId;


            sql = new("SELECT PageUserId, PageId, EmployeeId");
            sql.Append(" FROM PageUsers");
            sql.AppendFormat(" WHERE EmployeeId = {0}", employeeId);
            sql.AppendFormat(" AND PageId = {0}", pageId);
            PageUsersModel permission;
            try
            {
                permission = _context.PageUsers.FromSqlRaw(sql.ToString()).FirstAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get page user: '{ex.Message}'");
                return false;
            }
            if (permission == null) return false;

            return true;
        }

        public bool AccessToSubPage(HttpRequest request, string userName)
        {
            string controller;
            if (request.RouteValues["controller"] != null)
                controller = request.RouteValues["controller"].ToString();
            else
                return false;

            string action;
            if (request.RouteValues["action"] != null)
                action = request.RouteValues["action"].ToString();
            else
                return false;

            StringBuilder sql = new("SELECT SubPageId, PageId, Controller, Title, FileName, Tip, Position, Parameter, IsVisible, Version, Changed, ChangedBy");
            sql.Append(" FROM SubPages");
            sql.AppendFormat(" WHERE Controller = '{0}'", controller);
            sql.AppendFormat(" AND FileName = '{0}'", action);
            SubPagesModel subPage;
            try
            {
                subPage = _context.SubPages.FromSqlRaw(sql.ToString()).FirstAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get sub page: '{ex.Message}'");
                return false;
            }
            if (subPage == null) return false;
            int subPageId = subPage.SubPageId;

            sql = new("SELECT EmployeeId, FirstName, LastName, Initials, MailAddress, EmployeeTitle, SignatureLink, Represent, Active, ReadOnly, UserName, Changed, ChangedBy");
            sql.Append(" FROM Employees");
            sql.AppendFormat(" WHERE UserName = '{0}'", userName);
            EmployeesModel employee;
            try
            {
                employee = _context.Employees.FromSqlRaw(sql.ToString()).FirstAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to employee: '{ex.Message}'");
                return false;
            }
            if (employee == null) return false;
            int employeeId = employee.EmployeeId;

            sql = new("SELECT SubPageUserId, SubPageId, EmployeeId, Changed, ChangedBy");
            sql.Append(" FROM SubPageUsers");
            sql.AppendFormat(" WHERE EmployeeId = {0}", employeeId);
            sql.AppendFormat(" AND SubPageId = {0}", subPageId);
            SubPageUsersModel permission;
            try
            {
                permission = _context.SubPageUsers.FromSqlRaw(sql.ToString()).FirstAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get sub page user: '{ex.Message}'");
                return false;
            }
            if (permission == null) return false;

            return true;
        }

        public async Task<IEnumerable<SubPageMenusChildViewModel>> GetMenu(HttpRequest request, string userName)
        {
            string controller;
            if (request.RouteValues["controller"] != null)
                controller = request.RouteValues["controller"].ToString();
            else
                return null;

            string action;
            if (request.RouteValues["action"] != null)
                action = request.RouteValues["action"].ToString();
            else
                return null;

            PageIdModel page;
            StringBuilder sql = new StringBuilder("SELECT SubPageId AS Id");
            sql.Append(" FROM SubPages");
            sql.AppendFormat(" WHERE Controller = '{0}'", controller);
            sql.AppendFormat(" AND FileName = '{0}'", action);
            try
            {
                page = await _context.PageId.FromSqlRaw(sql.ToString()).FirstAsync();
            }
            catch (Exception)
            {
                return null;
            }

            if (page != null)
            {
                sql = new StringBuilder("SELECT SubPageMenus.SubPageMenuId, SubPages.Controller, SubPages.FileName AS Action, SubPages.Title, SubPages.Tip");
                sql.Append(" FROM SubPages INNER JOIN SubPageMenus ON SubPages.SubPageId = SubPageMenus.ChildPageId");
                sql.AppendFormat(" WHERE SubPageMenus.ParentPageId = {0}", page.Id);
                sql.Append(" ORDER BY SubPageMenus.Position");
                try
                {
                    return await _context.SubPageMenusChildView.FromSqlRaw(sql.ToString()).ToListAsync();
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<SubPageMenusViewModel>> GetMenus()
        {
            StringBuilder sql = new StringBuilder("SELECT SubPageMenus.SubPageMenuId, SubPageMenus.ParentPageId, SubPageMenus.ChildPageId, SubPageMenus.Changed, SubPageMenus.ChangedBy, SubPageMenus.Position, ");
            sql.Append(" SubPages_1.Controller, SubPages_1.FileName AS Action, SubPages_1.Title,");
            sql.Append(" SubPages.Controller AS ChildController, SubPages.FileName AS ChildAction, SubPages.Title AS ChildTitle");
            sql.Append(" FROM SubPageMenus INNER JOIN");
            sql.Append(" dbo.SubPages ON dbo.SubPageMenus.ChildPageId = dbo.SubPages.SubPageId INNER JOIN");
            sql.Append(" dbo.SubPages AS SubPages_1 ON dbo.SubPageMenus.ParentPageId = SubPages_1.SubPageId");
            sql.Append(" ORDER BY SubPages_1.Controller, SubPages_1.FileName, SubPageMenus.Position");
            return await _context.SubPageMenusView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<SubPageMenusViewModel> GetMenu(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT SubPageMenus.SubPageMenuId, SubPageMenus.ParentPageId, SubPageMenus.ChildPageId, SubPageMenus.Changed, SubPageMenus.ChangedBy, SubPageMenus.Position, ");
            sql.Append(" SubPages_1.Controller, SubPages_1.FileName AS Action, SubPages_1.Title,");
            sql.Append(" SubPages.Controller AS ChildController, SubPages.FileName AS ChildAction, SubPages.Title AS ChildTitle");
            sql.Append(" FROM SubPageMenus INNER JOIN");
            sql.Append(" dbo.SubPages ON dbo.SubPageMenus.ChildPageId = dbo.SubPages.SubPageId INNER JOIN");
            sql.Append(" dbo.SubPages AS SubPages_1 ON dbo.SubPageMenus.ParentPageId = SubPages_1.SubPageId");
            sql.AppendFormat(" WHERE SubPageMenus.SubPageMenuId = {0}", id);
            return await _context.SubPageMenusView.FromSqlRaw(sql.ToString()).FirstAsync();
        }

    }
}
