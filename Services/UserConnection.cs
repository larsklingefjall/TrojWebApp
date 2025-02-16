using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

            StringBuilder sql = new("SELECT PageId, Title, Controller, Action, Tip, Position, HasChild, Changed, ChangedBy");
            sql.Append(" FROM Pages3");
            sql.AppendFormat(" WHERE Controller = '{0}'", controller);
            sql.AppendFormat(" AND Action = '{0}'", action);
            Pages3Model page;
            try
            {
                page = _context.Pages3.FromSqlRaw(sql.ToString()).FirstAsync().Result;
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


            sql = new("SELECT PageUserId, PageId, EmployeeId, Changed, ChangedBy");
            sql.Append(" FROM PageUsers3");
            sql.AppendFormat(" WHERE EmployeeId = {0}", employeeId);
            sql.AppendFormat(" AND PageId = {0}", pageId);
            PageUsers3Model permission;
            try
            {
                permission = _context.PageUsers3.FromSqlRaw(sql.ToString()).FirstAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get page user: '{ex.Message}'");
                return false;
            }
            if (permission == null) return false;

            return true;
        }

        public async Task<IEnumerable<Pages3Model>> AccessToIndexPages(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            StringBuilder sql = new("SELECT EmployeeId, FirstName, LastName, Initials, MailAddress, EmployeeTitle, SignatureLink, Represent, Active, ReadOnly, UserName, Changed, ChangedBy");
            sql.Append(" FROM Employees");
            sql.AppendFormat(" WHERE UserName = '{0}'", userName);
            EmployeesModel employee;
            try
            {
                employee = await _context.Employees.FromSqlRaw(sql.ToString()).FirstAsync();
            }
            catch (Exception)
            {
                return null;
            }
            if (employee == null) return null;
            int employeeId = employee.EmployeeId;

            sql = new("SELECT Pages3.PageId,Pages3.Title,Pages3.Controller,Pages3.Action,Pages3.Tip,Pages3.Position,Pages3.HasChild,Pages3.Changed,Pages3.ChangedBy");
            sql.Append(" FROM Pages3 INNER JOIN PageUsers3 ON Pages3.PageId = PageUsers3.PageId");
            sql.AppendFormat(" WHERE EmployeeId = {0}", employeeId);
            IEnumerable<Pages3Model> permissions;
            try
            {
                permissions = await _context.Pages3.FromSqlRaw(sql.ToString()).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }

            return permissions;
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

        public bool AccessToSubPage(HttpRequest request, string action, string userName)
        {
            string controller;
            if (request.RouteValues["controller"] != null)
                controller = request.RouteValues["controller"].ToString();
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


        public bool AccessToSubPage(string controller, string action, string userName)
        {
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

        public async Task<bool> MenuItemExist(int parentPageId, int childPageId)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(SubPageMenus.SubPageMenuId) AS NumberOf");
            sql.Append(" FROM SubPageMenus");
            sql.AppendFormat(" WHERE ParentPageId = {0}", parentPageId);
            sql.AppendFormat(" AND ChildPageId = {0}", childPageId);
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf > 0;
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

            IdModel page;
            StringBuilder sql = new StringBuilder("SELECT SubPageId AS Id");
            sql.Append(" FROM SubPages");
            sql.AppendFormat(" WHERE Controller = '{0}'", controller);
            sql.AppendFormat(" AND FileName = '{0}'", action);
            try
            {
                page = await _context.Id.FromSqlRaw(sql.ToString()).FirstAsync();
            }
            catch (Exception)
            {
                return null;
            }

            if (page != null)
            {
                sql = new StringBuilder("SELECT SubPageMenus.SubPageMenuId, SubPages.Controller, SubPages.FileName AS Action, SubPages.Title, SubPages.Tip");
                sql.Append(" FROM SubPages INNER JOIN ");
                sql.Append(" SubPageMenus ON SubPages.SubPageId = SubPageMenus.ChildPageId INNER JOIN ");
                sql.Append(" SubPageUsers ON SubPages.SubPageId = SubPageUsers.SubPageId INNER JOIN ");
                sql.Append(" Employees ON SubPageUsers.EmployeeId = Employees.EmployeeId ");
                sql.AppendFormat(" WHERE SubPageMenus.ParentPageId = {0}", page.Id);
                sql.AppendFormat(" AND Employees.UserName = '{0}'", userName);
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

        public async Task<int> GetMenuItemId(HttpRequest request, int id, string userName)
        {
            string controller;
            if (request.RouteValues["controller"] != null)
                controller = request.RouteValues["controller"].ToString();
            else
                return 0;

            IdModel menuId;
            StringBuilder sql = new StringBuilder("SELECT MenuPageId AS Id");
            sql.Append(" FROM MenuPages");
            sql.AppendFormat(" WHERE Controller = '{0}'", controller);
            sql.Append(" AND Action = 'Details'");
            sql.AppendFormat(" AND Id = {0}", id);
            sql.AppendFormat(" AND ChangedBy = '{0}'", userName);
            try
            {
                menuId = await _context.Id.FromSqlRaw(sql.ToString()).FirstAsync();
            }
            catch (Exception)
            {
                return 0;
            }
            return menuId.Id;
        }

        public async Task<bool> CreateMenuItem(HttpRequest request, string title, int id, string userName)
        {
            string controller;
            if (request.RouteValues["controller"] != null)
                controller = request.RouteValues["controller"].ToString();
            else
                return false;

            int menuPageId = await GetMenuItemId(request, id, userName);

            if (menuPageId > 0)
            {
                MenuPagesModel menu = new MenuPagesModel
                {
                    MenuPageId = menuPageId,
                    Controller = controller,
                    Action = "Details",
                    Title = title,
                    Id = id,
                    Changed = DateTime.Now,
                    ChangedBy = userName
                };
                _context.Entry(menu).State = EntityState.Modified;
                int numberOfSaves = await _context.SaveChangesAsync();
                if (numberOfSaves == 1)
                    return true;
                else
                    return false;
            }
            else
            {
                MenuPagesModel menu = new MenuPagesModel
                {
                    MenuPageId = 0,
                    Controller = controller,
                    Action = "Details",
                    Title = title,
                    Id = id,
                    Changed = DateTime.Now,
                    ChangedBy = userName
                };
                _context.MenuPages.Add(menu);
                int numberOfSaves = await _context.SaveChangesAsync();
                if (numberOfSaves == 1)
                    return true;
                else
                    return false;
            }
        }

        public async Task<IEnumerable<MenuPagesViewModel>> GetMenuItems(HttpRequest request, string userName)
        {
            string controller;
            if (request.RouteValues["controller"] != null)
                controller = request.RouteValues["controller"].ToString();
            else
                return null;

            StringBuilder sql = new StringBuilder("SELECT DISTINCT TOP 5 Title, Controller, Action, Id");
            sql.Append(" FROM MenuPages");
            sql.AppendFormat(" WHERE Controller = '{0}'", controller);
            sql.AppendFormat(" AND ChangedBy = '{0}'", userName);
            try
            {
                return await _context.MenuPagesView.FromSqlRaw(sql.ToString()).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<MenuPagesViewModel>> GetMenuItems(string controller, string userName)
        {
            StringBuilder sql = new StringBuilder("SELECT DISTINCT TOP 5 Title, Controller, Action, Id");
            sql.Append(" FROM MenuPages");
            sql.AppendFormat(" WHERE Controller = '{0}'", controller);
            sql.AppendFormat(" AND ChangedBy = '{0}'", userName);
            sql.Append(" ORDER BY Title");
            try
            {
                return await _context.MenuPagesView.FromSqlRaw(sql.ToString()).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddLoadTime(HttpRequest request, long loadTime, string userName)
        {
            if (request == null)
                return false;

            if (request.Host.Value == null)
                return false;

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

            int intValue;
            try
            {
                intValue = Convert.ToInt32(loadTime);
            }
            catch (Exception)
            {
                return false;
            }

            LoadTimesModel time = new LoadTimesModel
            {
                LoadTimeId = 0,
                Host = request.Host.Value,
                Controller = controller,
                Action = action,
                LoadTime = intValue,
                Changed = DateTime.Now,
                ChangedBy = userName
            };
            _context.LoadTimes.Add(time);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return true;
            else
                return false;
        }

    }
}
