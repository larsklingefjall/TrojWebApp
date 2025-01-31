using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
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

            string fileName = "/" + controller + "/" + action;

            StringBuilder sql = new("SELECT SubPageId, PageId, Parent, Title, FileName, Tip, Position, Parameter, IsVisible, Changed, ChangedBy");
            sql.Append(" FROM SubPages");
            sql.AppendFormat(" WHERE FileName = '{0}'", fileName);
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

            sql = new("SELECT SubPageUserId, SubPageId, EmployeeId");
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

    }
}
