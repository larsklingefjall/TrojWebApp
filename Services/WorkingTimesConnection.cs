using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrojWebApp.Models;


namespace TrojWebApp.Services
{
    public class WorkingTimesConnection
    {
        private readonly TrojContext _context;
        private readonly string _cryKey;

        public WorkingTimesConnection(TrojContext context, string crykey)
        {
            _context = context;
            _cryKey = crykey;
        }

        public String LastSqlCommand;

        public async Task<IEnumerable<WorkingTimesModel>> GetWorkingTimes()
        {
            StringBuilder sql = new StringBuilder("SELECT WorkingTimeId, PersonId ,CaseId ,TariffTypeId ,EmployeeId ,WhenDate ,TariffLevel ,NumberOfHours ,Cost ,Sum ,Billed ,Changed ,ChangedBy ");
            sql.AppendFormat(", CONVERT(nvarchar(2048), DecryptByPassphrase('{0}', CommentCry, 1, CONVERT(varbinary, WorkingTimeId))) AS Comment", _cryKey);
            sql.Append(" FROM WorkingTimes");
            return await _context.WorkingTimes.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<int> GetNumberOfWorkingTimesForCurrentDay(string currentDate, string nextDate)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(WorkingTimes.WorkingTimeId) AS NumberOf");
            sql.Append(" FROM WorkingTimes");
            sql.AppendFormat(" WHERE WorkingTimes.WhenDate >= '{0}'", currentDate);
            sql.AppendFormat(" AND  WorkingTimes.WhenDate < '{0}'", nextDate);

            NumberOfModel NumberOf;
            int response = 0;
            try
            {
                NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            }
            catch (Exception)
            {
                LastSqlCommand = sql.ToString();
                return response;
            }
            if(NumberOf == null)
                return response;
            return NumberOf.NumberOf;
        }

        public async Task<IEnumerable<WorkingTimesViewModel>> GetWorkingTimesForCurrentDay(string currentDate, string nextDate, int offset, int size)
        {
            StringBuilder where = new StringBuilder("");
            if (currentDate != "")
            {
                where.AppendFormat(" WHERE WorkingTimes.WhenDate >= '{0}'", currentDate);
            }
            if (nextDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.WhenDate < '{0}'", nextDate);
                else
                    where.AppendFormat(" WHERE WorkingTimes.WhenDate < '{0}'", nextDate);
            }

            StringBuilder sql = new StringBuilder("SELECT WorkingTimes.WorkingTimeId, WorkingTimes.CaseId, WorkingTimes.TariffTypeId, WorkingTimes.EmployeeId, FORMAT(WorkingTimes.WhenDate,'yyyy-MM-dd') AS WhenDate");
            sql.Append(" ,WorkingTimes.TariffLevel, WorkingTimes.NumberOfHours, WorkingTimes.Cost, WorkingTimes.[Sum]");
            sql.Append(" ,WorkingTimes.Billed, WorkingTimes.Changed, WorkingTimes.ChangedBy, CaseTypes.CaseType, TariffTypes.TariffType, TariffTypes.NoLevel, TariffTypes.BackgroundColor, Employees.Initials, Persons.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(2048), DecryptByPassphrase('{0}', WorkingTimes.CommentCry, 1, CONVERT(varbinary, WorkingTimes.WorkingTimeId))) AS Comment", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Cases.TitleCry, 1 , CONVERT(varbinary, Cases.CaseId))) AS Title", _cryKey);
            sql.Append(" FROM WorkingTimes INNER JOIN");
            sql.Append(" Persons ON WorkingTimes.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" Cases ON WorkingTimes.CaseId = Cases.CaseId INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" TariffTypes ON WorkingTimes.TariffTypeId = TariffTypes.TariffTypeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");

            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());

            sql.Append(" ORDER BY WorkingTimes.WhenDate DESC,  WorkingTimes.Changed DESC");
            sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, size);

            IEnumerable<WorkingTimesViewModel> response = null;
            try
            {
                response = await _context.WorkingTimesView.FromSqlRaw(sql.ToString()).AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                LastSqlCommand = sql.ToString();
            }
            return response;
        }

        public async Task<int> GetNumberOfFilteredWorkingTimes(string caseId, string whenDate, string caseTypeId, string title, string employeeId, string firstName, string lastName)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(WorkingTimes.WorkingTimeId) AS NumberOf");
            sql.Append(" FROM WorkingTimes INNER JOIN");
            sql.Append(" Persons ON WorkingTimes.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" Cases ON WorkingTimes.CaseId = Cases.CaseId INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" TariffTypes ON WorkingTimes.TariffTypeId = TariffTypes.TariffTypeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");
            StringBuilder where = new StringBuilder("");
            if (firstName != "")
            {
                where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, firstName);
            }
            if (lastName != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
            }
            if (caseId != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.CaseId = {0}", caseId);
                else
                    where.AppendFormat(" WHERE WorkingTimes.CaseId = {0}", caseId);
            }
            if (whenDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.WhenDate = '{0}'", whenDate);
                else
                    where.AppendFormat(" WHERE WorkingTimes.WhenDate = '{0}'", whenDate);
            }
            if (title != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', TitleCry, 1 , CONVERT(varbinary, Cases.CaseId))) LIKE '{1}'", _cryKey, title);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', TitleCry, 1 , CONVERT(varbinary, Cases.CaseId))) LIKE '{1}'", _cryKey, title);
            }
            if (caseTypeId != "" && caseTypeId != "0")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND Cases.CaseTypeId = {0}", caseTypeId);
                else
                    where.AppendFormat(" WHERE Cases.CaseTypeId = {0}", caseTypeId);
            }
            if (employeeId != "" && employeeId != "0")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.EmployeeId = {0}", employeeId);
                else
                    where.AppendFormat(" WHERE WorkingTimes.EmployeeId = {0}", employeeId);
            }
            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf;
        }

        public async Task<IEnumerable<WorkingTimesViewModel>> GetFilteredWorkingTimes(string caseId, string whenDate, string caseTypeId, string title, string employeeId, string firstName, string lastName, int offset, int size)
        {
            StringBuilder sql = new StringBuilder("SELECT WorkingTimes.WorkingTimeId, WorkingTimes.CaseId, WorkingTimes.TariffTypeId, WorkingTimes.EmployeeId, FORMAT(WorkingTimes.WhenDate,'yyyy-MM-dd') AS WhenDate");
            sql.Append(" ,WorkingTimes.TariffLevel, WorkingTimes.NumberOfHours, WorkingTimes.Cost, WorkingTimes.[Sum]");
            sql.Append(" ,WorkingTimes.Billed, WorkingTimes.Changed, WorkingTimes.ChangedBy, CaseTypes.CaseType, TariffTypes.TariffType, TariffTypes.NoLevel, TariffTypes.BackgroundColor, Employees.Initials, Persons.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(2048), DecryptByPassphrase('{0}', WorkingTimes.CommentCry, 1, CONVERT(varbinary, WorkingTimes.WorkingTimeId))) AS Comment", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Cases.TitleCry, 1 , CONVERT(varbinary, Cases.CaseId))) AS Title", _cryKey);
            sql.Append(" FROM WorkingTimes INNER JOIN");
            sql.Append(" Persons ON WorkingTimes.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" Cases ON WorkingTimes.CaseId = Cases.CaseId INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" TariffTypes ON WorkingTimes.TariffTypeId = TariffTypes.TariffTypeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");
            StringBuilder where = new StringBuilder("");
            if (firstName != "")
            {
                where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, firstName);
            }
            if (lastName != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
            }
            if (caseId != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.CaseId = {0}", caseId);
                else
                    where.AppendFormat(" WHERE WorkingTimes.CaseId = {0}", caseId);
            }
            if (whenDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.WhenDate = '{0}'", whenDate);
                else
                    where.AppendFormat(" WHERE WorkingTimes.WhenDate = '{0}'", whenDate);
            }
            if (title != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', TitleCry, 1 , CONVERT(varbinary, Cases.CaseId))) LIKE '{1}'", _cryKey, title);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', TitleCry, 1 , CONVERT(varbinary, Cases.CaseId))) LIKE '{1}'", _cryKey, title);
            }
            if (caseTypeId != "" && caseTypeId != "0")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND Cases.CaseTypeId = {0}", caseTypeId);
                else
                    where.AppendFormat(" WHERE Cases.CaseTypeId = {0}", caseTypeId);
            }
            if (employeeId != "" && employeeId != "0")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.EmployeeId = {0}", employeeId);
                else
                    where.AppendFormat(" WHERE WorkingTimes.EmployeeId = {0}", employeeId);
            }
            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());
            sql.Append(" ORDER BY WorkingTimes.WhenDate DESC,  WorkingTimes.Changed DESC");
            sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, size);
            return await _context.WorkingTimesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<WorkingTimesViewModel>> GetWorkingTimesForCase(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT WorkingTimes.WorkingTimeId, WorkingTimes.CaseId, WorkingTimes.TariffTypeId, WorkingTimes.EmployeeId, FORMAT(WorkingTimes.WhenDate,'yyyy-MM-dd') AS WhenDate");
            sql.Append(" ,WorkingTimes.TariffLevel, WorkingTimes.NumberOfHours, WorkingTimes.Cost, WorkingTimes.[Sum]");
            sql.Append(" ,WorkingTimes.Billed, WorkingTimes.Changed, WorkingTimes.ChangedBy, CaseTypes.CaseType, TariffTypes.TariffType, TariffTypes.NoLevel, TariffTypes.BackgroundColor, Employees.Initials, Persons.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(2048), DecryptByPassphrase('{0}', WorkingTimes.CommentCry, 1, CONVERT(varbinary, WorkingTimes.WorkingTimeId))) AS Comment", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('kalle', WorkingTimes.CommentCry, 1, CONVERT(varbinary, WorkingTimes.WorkingTimeId))) AS Title", _cryKey);
            sql.Append(" FROM WorkingTimes INNER JOIN");
            sql.Append(" Persons ON WorkingTimes.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" Cases ON WorkingTimes.CaseId = Cases.CaseId INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" TariffTypes ON WorkingTimes.TariffTypeId = TariffTypes.TariffTypeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");
            sql.AppendFormat(" WHERE (WorkingTimes.CaseId = {0})", id);
            sql.Append(" ORDER BY WorkingTimes.WhenDate DESC,  WorkingTimes.Changed DESC");
            return await _context.WorkingTimesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<WorkingTimesViewModel>> GetWorkingTimesForCaseNotBilled(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT WorkingTimes.WorkingTimeId, WorkingTimes.CaseId, WorkingTimes.TariffTypeId, WorkingTimes.EmployeeId, FORMAT(WorkingTimes.WhenDate,'yyyy-MM-dd') AS WhenDate");
            sql.Append(" ,WorkingTimes.TariffLevel, WorkingTimes.NumberOfHours, WorkingTimes.Cost, WorkingTimes.[Sum]");
            sql.Append(" ,WorkingTimes.Billed, WorkingTimes.Changed, WorkingTimes.ChangedBy, CaseTypes.CaseType, TariffTypes.TariffType, TariffTypes.NoLevel, TariffTypes.BackgroundColor, Employees.Initials, Persons.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(2048), DecryptByPassphrase('{0}', WorkingTimes.CommentCry, 1, CONVERT(varbinary, WorkingTimes.WorkingTimeId))) AS Comment", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', TitleCry, 1 , CONVERT(varbinary, Cases.CaseId))) AS Title", _cryKey);
            sql.Append(" FROM WorkingTimes INNER JOIN");
            sql.Append(" Persons ON WorkingTimes.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" Cases ON WorkingTimes.CaseId = Cases.CaseId INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" TariffTypes ON WorkingTimes.TariffTypeId = TariffTypes.TariffTypeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");
            sql.AppendFormat(" WHERE (WorkingTimes.CaseId = {0})", id);
            sql.Append(" AND WorkingTimes.Billed = 0");
            sql.Append(" ORDER BY WorkingTimes.WhenDate DESC,  WorkingTimes.Changed DESC");
            IEnumerable<WorkingTimesViewModel> list;
            try
            {
                list = await _context.WorkingTimesView.FromSqlRaw(sql.ToString()).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SQL: {0} Exception: {1}", sql.ToString(), ex.Message);
                return null;
            }
            return list;
        }

        public async Task<WorkingTimesViewModel> GetWorkingTimeNotBilled(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT WorkingTimes.WorkingTimeId, WorkingTimes.CaseId, WorkingTimes.TariffTypeId, WorkingTimes.EmployeeId, FORMAT(WorkingTimes.WhenDate,'yyyy-MM-dd') AS WhenDate");
            sql.Append(" ,WorkingTimes.TariffLevel, WorkingTimes.NumberOfHours, WorkingTimes.Cost, WorkingTimes.[Sum]");
            sql.Append(" ,WorkingTimes.Billed, WorkingTimes.Changed, WorkingTimes.ChangedBy, CaseTypes.CaseType, TariffTypes.TariffType, TariffTypes.NoLevel, TariffTypes.BackgroundColor, Employees.Initials, Persons.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(2048), DecryptByPassphrase('{0}', WorkingTimes.CommentCry, 1, CONVERT(varbinary, WorkingTimes.WorkingTimeId))) AS Comment", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', TitleCry, 1 , CONVERT(varbinary, Cases.CaseId))) AS Title", _cryKey);
            sql.Append(" FROM WorkingTimes INNER JOIN");
            sql.Append(" Persons ON WorkingTimes.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" Cases ON WorkingTimes.CaseId = Cases.CaseId INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" TariffTypes ON WorkingTimes.TariffTypeId = TariffTypes.TariffTypeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");
            sql.AppendFormat(" WHERE WorkingTimes.WorkingTimeId = {0}", id);
            sql.Append(" AND WorkingTimes.Billed = 0");
            WorkingTimesViewModel item;
            try
            {
                item = await _context.WorkingTimesView.FromSqlRaw(sql.ToString()).FirstAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SQL: {0} Exception: {1}", sql.ToString(), ex.Message);
                return null;
            }
            return item;
        }

        public async Task<IEnumerable<WorkingTimesViewModel>> GetWorkingTimeForClient(int clientId, string startDate, string endDate)
        {
            StringBuilder where = new StringBuilder("");
            if (clientId != 0)
            {
                where.AppendFormat(" WHERE WorkingTimes.PersonId = {0}", clientId);
            }
            if (startDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.WhenDate >= '{0}'", startDate);
                else
                    where.AppendFormat(" WHERE WorkingTimes.WhenDate >= '{0}'", startDate);
            }
            if (endDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.WhenDate <= '{0}'", endDate);
                else
                    where.AppendFormat(" WHERE WorkingTimes.WhenDate <= '{0}'", endDate);
            }

            StringBuilder sql = new StringBuilder("SELECT WorkingTimes.WorkingTimeId, WorkingTimes.CaseId, WorkingTimes.TariffTypeId, WorkingTimes.EmployeeId, FORMAT(WorkingTimes.WhenDate,'yyyy-MM-dd') AS WhenDate");
            sql.Append(" ,WorkingTimes.TariffLevel, WorkingTimes.NumberOfHours, WorkingTimes.Cost, WorkingTimes.[Sum]");
            sql.Append(" ,WorkingTimes.Billed, WorkingTimes.Changed, WorkingTimes.ChangedBy, CaseTypes.CaseType, TariffTypes.TariffType, TariffTypes.NoLevel, TariffTypes.BackgroundColor, Persons.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(2048), DecryptByPassphrase('{0}', WorkingTimes.CommentCry, 1, CONVERT(varbinary, WorkingTimes.WorkingTimeId))) AS Comment", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', TitleCry, 1 , CONVERT(varbinary, Cases.CaseId))) AS Title", _cryKey);
            sql.Append(", Employees.Initials");

            sql.Append(" FROM TariffTypes INNER JOIN");
            sql.Append(" CaseTypes INNER JOIN");
            sql.Append(" Persons INNER JOIN");
            sql.Append(" Cases INNER JOIN");
            sql.Append(" WorkingTimes ON Cases.CaseId = WorkingTimes.CaseId ON Persons.PersonId = WorkingTimes.PersonId ON CaseTypes.CaseTypeId = Cases.CaseTypeId ON");
            sql.Append(" TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId INNER JOIN");
            sql.Append(" InvoiceWorkingTimes ON WorkingTimes.WorkingTimeId = InvoiceWorkingTimes.WorkingTimeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");

            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());

            sql.Append(" UNION");

            sql.Append(" SELECT WorkingTimes.WorkingTimeId, WorkingTimes.CaseId, WorkingTimes.TariffTypeId, WorkingTimes.EmployeeId, FORMAT(WorkingTimes.WhenDate,'yyyy-MM-dd') AS WhenDate");
            sql.Append(" ,WorkingTimes.TariffLevel, WorkingTimes.NumberOfHours, WorkingTimes.Cost, WorkingTimes.[Sum]");
            sql.Append(" ,WorkingTimes.Billed, WorkingTimes.Changed, WorkingTimes.ChangedBy, CaseTypes.CaseType, TariffTypes.TariffType, TariffTypes.NoLevel, TariffTypes.BackgroundColor, Persons.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(2048), DecryptByPassphrase('{0}', WorkingTimes.CommentCry, 1, CONVERT(varbinary, WorkingTimes.WorkingTimeId))) AS Comment", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', TitleCry, 1 , CONVERT(varbinary, Cases.CaseId))) AS Title", _cryKey);
            sql.Append(", Employees.Initials");

            sql.Append(" FROM TariffTypes INNER JOIN");
            sql.Append(" CaseTypes INNER JOIN");
            sql.Append(" Persons INNER JOIN");
            sql.Append(" Cases INNER JOIN");
            sql.Append(" WorkingTimes ON Cases.CaseId = WorkingTimes.CaseId ON Persons.PersonId = WorkingTimes.PersonId ON CaseTypes.CaseTypeId = Cases.CaseTypeId ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");

            if (where.Length > 0)
            {
                sql.AppendFormat(" {0}", where.ToString());
                sql.Append(" AND WorkingTimes.WorkingTimeId NOT IN (SELECT WorkingTimeId FROM InvoiceWorkingTimes)");
            }

            sql.Append(" ORDER BY WhenDate DESC");
            return await _context.WorkingTimesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<int> GetNumberOfWorkingTimeForClient(int clientId, string startDate, string endDate)
        {
            StringBuilder where = new StringBuilder("");
            if (clientId != 0)
            {
                where.AppendFormat(" WHERE WorkingTimes.PersonId = {0}", clientId);
            }
            if (startDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.WhenDate >= '{0}'", startDate);
                else
                    where.AppendFormat(" WHERE WorkingTimes.WhenDate >= '{0}'", startDate);
            }
            if (endDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.WhenDate <= '{0}'", endDate);
                else
                    where.AppendFormat(" WHERE WorkingTimes.WhenDate <= '{0}'", endDate);
            }

            StringBuilder sql = new StringBuilder("SELECT Count(WorkingTimes.WorkingTimeId) AS NumberOf");
            sql.Append(" FROM TariffTypes INNER JOIN");
            sql.Append(" CaseTypes INNER JOIN");
            sql.Append(" Persons INNER JOIN");
            sql.Append(" Cases INNER JOIN");
            sql.Append(" WorkingTimes ON Cases.CaseId = WorkingTimes.CaseId ON Persons.PersonId = WorkingTimes.PersonId ON CaseTypes.CaseTypeId = Cases.CaseTypeId ON");
            sql.Append(" TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId INNER JOIN");
            sql.Append(" InvoiceWorkingTimes ON WorkingTimes.WorkingTimeId = InvoiceWorkingTimes.WorkingTimeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");

            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());

            sql.Append(" UNION");

            sql.Append(" SELECT Count(WorkingTimes.WorkingTimeId) AS NumberOf");
            sql.Append(" FROM TariffTypes INNER JOIN");
            sql.Append(" CaseTypes INNER JOIN");
            sql.Append(" Persons INNER JOIN");
            sql.Append(" Cases INNER JOIN");
            sql.Append(" WorkingTimes ON Cases.CaseId = WorkingTimes.CaseId ON Persons.PersonId = WorkingTimes.PersonId ON CaseTypes.CaseTypeId = Cases.CaseTypeId ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");

            if (where.Length > 0)
            {
                sql.AppendFormat(" {0}", where.ToString());
                sql.Append(" AND WorkingTimes.WorkingTimeId NOT IN (SELECT WorkingTimeId FROM InvoiceWorkingTimes)");
            }

            IEnumerable<NumberOfModel> list = await _context.NumberOf.FromSqlRaw(sql.ToString()).ToListAsync();
            int numberOf = 0;
            foreach (NumberOfModel item in list)
            {
                numberOf += item.NumberOf;
            }
            return numberOf;
        }

        public async Task<MaxMinDateModel> GetMaxMinDateWorkingTimeForClient(int clientId, string startDate, string endDate)
        {
            StringBuilder where = new StringBuilder("");
            if (clientId != 0)
            {
                where.AppendFormat(" WHERE WorkingTimes.PersonId = {0}", clientId);
            }
            if (startDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.WhenDate >= '{0}'", startDate);
                else
                    where.AppendFormat(" WHERE WorkingTimes.WhenDate >= '{0}'", startDate);
            }
            if (endDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.WhenDate <= '{0}'", endDate);
                else
                    where.AppendFormat(" WHERE WorkingTimes.WhenDate <= '{0}'", endDate);
            }

            StringBuilder sql = new StringBuilder("SELECT 1 AS Id, Max(WorkingTimes.WhenDate) AS MaxDate, Min(WorkingTimes.WhenDate) AS MinDate");
            sql.Append(" FROM TariffTypes INNER JOIN");
            sql.Append(" CaseTypes INNER JOIN");
            sql.Append(" Persons INNER JOIN");
            sql.Append(" Cases INNER JOIN");
            sql.Append(" WorkingTimes ON Cases.CaseId = WorkingTimes.CaseId ON Persons.PersonId = WorkingTimes.PersonId ON CaseTypes.CaseTypeId = Cases.CaseTypeId ON");
            sql.Append(" TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId INNER JOIN");
            sql.Append(" InvoiceWorkingTimes ON WorkingTimes.WorkingTimeId = InvoiceWorkingTimes.WorkingTimeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");

            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());

            sql.Append(" UNION");

            sql.Append(" SELECT 2 AS Id, Max(WorkingTimes.WhenDate) AS MaxDate, Min(WorkingTimes.WhenDate) AS MinDate");
            sql.Append(" FROM TariffTypes INNER JOIN");
            sql.Append(" CaseTypes INNER JOIN");
            sql.Append(" Persons INNER JOIN");
            sql.Append(" Cases INNER JOIN");
            sql.Append(" WorkingTimes ON Cases.CaseId = WorkingTimes.CaseId ON Persons.PersonId = WorkingTimes.PersonId ON CaseTypes.CaseTypeId = Cases.CaseTypeId ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");

            if (where.Length > 0)
            {
                sql.AppendFormat(" {0}", where.ToString());
                sql.Append(" AND WorkingTimes.WorkingTimeId NOT IN (SELECT WorkingTimeId FROM InvoiceWorkingTimes)");
            }

            IEnumerable<MaxMinDateModel> list = await _context.MaxMinDate.FromSqlRaw(sql.ToString()).ToListAsync();
            MaxMinDateModel maxMinDateModel = new MaxMinDateModel()
            {
                MaxDate = new DateTime(1900, 1, 1),
                MinDate = new DateTime(2050, 1, 1)
            };
            foreach (MaxMinDateModel date in list)
            {
                if (date.MaxDate > maxMinDateModel.MaxDate)
                    maxMinDateModel.MaxDate = date.MaxDate;
                if (date.MinDate < maxMinDateModel.MinDate)
                    maxMinDateModel.MinDate = date.MinDate;
            }
            return maxMinDateModel;
        }

        public async Task<WorkingTimesModel> GetWorkingTime(int workingTimeId)
        {
            StringBuilder sql = new StringBuilder("SELECT WorkingTimeId, PersonId ,CaseId ,TariffTypeId ,EmployeeId ,WhenDate ,TariffLevel ,NumberOfHours ,Cost ,Sum ,Billed ,Changed ,ChangedBy ");
            sql.AppendFormat(", CONVERT(nvarchar(2048), DecryptByPassphrase('{0}', CommentCry, 1, CONVERT(varbinary, WorkingTimeId))) AS Comment", _cryKey);
            sql.Append(" FROM WorkingTimes");
            sql.AppendFormat(" WHERE WorkingTimeId = {0}", workingTimeId);
            return await _context.WorkingTimes.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<WorkingTimesModel> CreateWorkingTime(int caseId, int tariffTypeId, int employeeId, double numberOfHours, double? cost, string comment, DateTime? whenDate, string userName = "")
        {
            double level;
            double workingTimeSum;
            double inputCost;

            TariffTypesModel tariffType = await _context.TariffTypes.Where(t => t.TariffTypeId == tariffTypeId).FirstAsync();
            if (tariffType.NoLevel)
            {
                level = 0;
                double tmpCost = cost ?? 0;
                workingTimeSum = tmpCost * numberOfHours;
                inputCost = tmpCost;
            }
            else
            {
                TariffLevelsModel tariffLevel = await _context.TariffLevels.Where(t => t.TariffTypeId == tariffTypeId).Where(v => v.Valid == true).FirstAsync();
                level = tariffLevel.TariffLevel;
                workingTimeSum = level * numberOfHours;
                inputCost = level;
            }

            DateTime inputWhenDate = whenDate ?? DateTime.Now;
            PersonCasesModel person = await _context.PersonCases.Where(c => c.CaseId == caseId).Where(r => r.Responsible == true).FirstAsync();

            WorkingTimesModel workingTime = new WorkingTimesModel
            {
                WorkingTimeId = 0,
                PersonId = person.PersonId,
                CaseId = caseId,
                TariffTypeId = tariffTypeId,
                EmployeeId = employeeId,
                WhenDate = inputWhenDate,
                TariffLevel = level,
                NumberOfHours = numberOfHours,
                Cost = inputCost,
                Sum = workingTimeSum,
                Billed = false,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.WorkingTimes.Add(workingTime);
            int numberOfSaved = await _context.SaveChangesAsync();
            if (numberOfSaved != 1)
                return null;

            WorkingTimesModel newWorkingTime = await _context.WorkingTimes.Where(c => c.CaseId == caseId).OrderByDescending(c => c.WorkingTimeId).FirstAsync();
            if (newWorkingTime == null)
                return null;

            StringBuilder sql = new StringBuilder("DECLARE @Comment nvarchar(2048); ");
            sql.AppendFormat("SET @Comment = '{0}'; ", comment);
            sql.Append(" UPDATE WorkingTimes SET ");
            if (comment == "")
                sql.Append(" CommentCry = Null");
            else
                sql.AppendFormat(" CommentCry = EncryptByPassPhrase('{0}', @Comment, 1, CONVERT( varbinary, WorkingTimeId))", _cryKey);
            sql.Append(", Comment = Null");
            sql.AppendFormat(" WHERE WorkingTimeId = {0}", newWorkingTime.WorkingTimeId);
            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            return await GetWorkingTime(newWorkingTime.WorkingTimeId);
        }

        public async Task<int> UpdateWorkingTime(int id, double numberOfHours, string comment, string userName = "")
        {
            WorkingTimesModel workingTime = await GetWorkingTime(id);
            workingTime.NumberOfHours = numberOfHours;
            workingTime.Sum = numberOfHours * workingTime.Cost;
            workingTime.Changed = DateTime.Now;
            workingTime.ChangedBy = userName;

            _context.Entry(workingTime).State = EntityState.Modified;
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return 0;

            StringBuilder sql = new StringBuilder("DECLARE @Comment nvarchar(2048); ");
            sql.AppendFormat("SET @Comment = '{0}'; ", comment);
            sql.Append(" UPDATE WorkingTimes SET ");
            if (comment == "")
                sql.Append(" CommentCry = Null");
            else
                sql.AppendFormat(" CommentCry = EncryptByPassPhrase('{0}', @Comment, 1, CONVERT( varbinary, WorkingTimeId))", _cryKey);
            sql.Append(", Comment = Null");
            sql.AppendFormat(" WHERE WorkingTimeId = {0}", id);
            return await _context.Database.ExecuteSqlRawAsync(sql.ToString());
        }

        public async Task<int> DeleteWorkingTime(int workingTimeId)
        {
            StringBuilder sql = new StringBuilder("DELETE FROM WorkingTimes");
            sql.AppendFormat(" WHERE WorkingTimeId = {0}", workingTimeId);
            return await _context.Database.ExecuteSqlRawAsync(sql.ToString());
        }

        public async Task<TotalSumAndHoursModel> GetTotalSum(string currentDate, string nextDate)
        {
            TotalSumAndHoursModel zeroTotalSum = new TotalSumAndHoursModel
            {
                TotalSum = 0,
                TotalHours = 0
            };

            StringBuilder sql = new StringBuilder("SELECT Sum(WorkingTimes.Sum) AS TotalSum, Sum(WorkingTimes.NumberOfHours) AS TotalHours");
            sql.Append(" FROM WorkingTimes");
            sql.AppendFormat(" WHERE (WhenDate >= '{0}')", currentDate);
            sql.AppendFormat(" AND  (WhenDate < '{0}')", nextDate);

            TotalSumAndHoursModel totalSum;
            try
            {
                totalSum = await _context.TotalSumAndHours.FromSqlRaw(sql.ToString()).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return zeroTotalSum;
            }
            return totalSum;
        }

        public async Task<TotalSumAndHoursModel> GetFilteredTotalSum(string caseId, string whenDate, string caseTypeId, string title, string employeeId, string firstName, string lastName)
        {
            TotalSumAndHoursModel zeroTotalSum = new TotalSumAndHoursModel
            {
                TotalSum = 0,
                TotalHours = 0
            };

            StringBuilder sql = new StringBuilder("SELECT Sum(WorkingTimes.Sum) AS TotalSum, Sum(WorkingTimes.NumberOfHours) AS TotalHours");
            sql.Append(" FROM WorkingTimes INNER JOIN");
            sql.Append(" Persons ON WorkingTimes.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" Cases ON WorkingTimes.CaseId = Cases.CaseId INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" TariffTypes ON WorkingTimes.TariffTypeId = TariffTypes.TariffTypeId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");

            StringBuilder where = new StringBuilder("");
            if (firstName != "")
            {
                where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, firstName);
            }
            if (lastName != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
            }
            if (caseId != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.CaseId = {0}", caseId);
                else
                    where.AppendFormat(" WHERE WorkingTimes.CaseId = {0}", caseId);
            }
            if (whenDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.WhenDate = '{0}'", whenDate);
                else
                    where.AppendFormat(" WHERE WorkingTimes.WhenDate = '{0}'", whenDate);
            }
            if (title != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', TitleCry, 1 , CONVERT(varbinary, Cases.CaseId))) LIKE '{1}'", _cryKey, title);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', TitleCry, 1 , CONVERT(varbinary, Cases.CaseId))) LIKE '{1}'", _cryKey, title);
            }
            if (caseTypeId != "" && caseTypeId != "0")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND Cases.CaseTypeId = {0}", caseTypeId);
                else
                    where.AppendFormat(" WHERE Cases.CaseTypeId = {0}", caseTypeId);
            }
            if (employeeId != "" && employeeId != "0")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND WorkingTimes.EmployeeId = {0}", employeeId);
                else
                    where.AppendFormat(" WHERE WorkingTimes.EmployeeId = {0}", employeeId);
            }
            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());

            TotalSumAndHoursModel totalSum;
            try
            {
                totalSum = await _context.TotalSumAndHours.FromSqlRaw(sql.ToString()).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return zeroTotalSum;
            }
            return totalSum;
        }

        public async Task<TotalSumAndHoursModel> GetTotalSumForCase(int id)
        {
            TotalSumAndHoursModel zeroTotalSum = new TotalSumAndHoursModel
            {
                TotalSum = 0,
                TotalHours = 0
            };

            StringBuilder sql = new StringBuilder("SELECT Sum(WorkingTimes.Sum) AS TotalSum, Sum(WorkingTimes.NumberOfHours) AS TotalHours");
            sql.Append(" FROM WorkingTimes");
            sql.AppendFormat(" WHERE WorkingTimes.CaseId = {0}", id);

            TotalSumAndHoursModel totalSum;
            try
            {
                totalSum = await _context.TotalSumAndHours.FromSqlRaw(sql.ToString()).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return zeroTotalSum;
            }
            return totalSum;
        }

        public async Task<IEnumerable<WorkingTimesSummarysModel>> GetUnderlaySummariesOfYear(DateTime date, string userName)
        {
            int year = date.Year;
            StringBuilder sql = new StringBuilder("SELECT TariffTypes.TariffType, WorkingTimes.Cost AS UnitCost, Sum(WorkingTimes.Sum) AS SumCosts, Sum(WorkingTimes.NumberOfHours) AS SumHours, Max(TariffTypes.BackgroundColor) AS BackgroundColor");
            sql.Append(" FROM TariffTypes INNER JOIN(Employees INNER JOIN WorkingTimes ON Employees.EmployeeId = WorkingTimes.EmployeeId) ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId");
            sql.AppendFormat(" WHERE DatePart(yyyy, WorkingTimes.WhenDate) = {0}", year);
            sql.AppendFormat(" AND Employees.UserName = '{0}'", userName);
            sql.Append(" AND TariffTypes.NoLevel = 0");
            sql.Append(" GROUP BY TariffTypes.TariffType, WorkingTimes.TariffLevel, WorkingTimes.Cost");
            sql.Append(" UNION");
            sql.Append(" SELECT TariffTypes.TariffType, WorkingTimes.Cost AS UnitCost, Sum(WorkingTimes.Sum) AS SumCosts, Sum(WorkingTimes.NumberOfHours) AS SumHours, Max(TariffTypes.BackgroundColor) AS BackgroundColor");
            sql.Append(" FROM TariffTypes INNER JOIN(Employees INNER JOIN WorkingTimes ON Employees.EmployeeId = WorkingTimes.EmployeeId) ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId");
            sql.AppendFormat(" WHERE DatePart(yyyy, WorkingTimes.WhenDate) = {0}", year);
            sql.AppendFormat(" AND Employees.UserName = '{0}'", userName);
            sql.Append(" AND TariffTypes.NoLevel = 1");
            sql.Append(" GROUP BY TariffTypes.TariffType, WorkingTimes.TariffLevel, WorkingTimes.Cost");
            sql.Append(" ORDER BY TariffTypes.TariffType");
            return await _context.WorkingTimesSummaries.FromSqlRaw(sql.ToString()).AsNoTracking().ToListAsync();
        }

        public async Task<double> GetUnderlaySummariesOfYearSum(DateTime date, string userName)
        {
            int year = date.Year;
            StringBuilder sql = new StringBuilder("SELECT Sum(WorkingTimes.Sum) AS SumOf");
            sql.Append(" FROM TariffTypes INNER JOIN(Employees INNER JOIN WorkingTimes ON Employees.EmployeeId = WorkingTimes.EmployeeId) ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId");
            sql.AppendFormat(" WHERE DatePart(yyyy, WorkingTimes.WhenDate) = {0}", year);
            sql.AppendFormat(" AND Employees.UserName = '{0}'", userName);
            SumOfModel sumOf;
            try
            {
                sumOf = await _context.SumOf.FromSqlRaw(sql.ToString()).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return 0;
            }
            return sumOf.SumOf;
        }

        public async Task<IEnumerable<WorkingTimesSummarysModel>> GetUnderlaySummariesOfWeek(DateTime date, string userName)
        {
            DateTime thisWeekStart = date.AddDays(-(int)date.DayOfWeek + 1);
            DateTime thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            StringBuilder sql = new StringBuilder("SELECT TariffTypes.TariffType, WorkingTimes.Cost AS UnitCost, Sum(WorkingTimes.Sum) AS SumCosts, Sum(WorkingTimes.NumberOfHours) AS SumHours, Max(TariffTypes.BackgroundColor) AS BackgroundColor");
            sql.Append(" FROM TariffTypes INNER JOIN(Employees INNER JOIN WorkingTimes ON Employees.EmployeeId = WorkingTimes.EmployeeId) ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId");
            sql.AppendFormat($" WHERE WorkingTimes.WhenDate >= '{thisWeekStart.Year}-{thisWeekStart.Month}-{thisWeekStart.Day}'");
            sql.AppendFormat($" AND WorkingTimes.WhenDate <= '{thisWeekEnd.Year}-{thisWeekEnd.Month}-{thisWeekEnd.Day}'");
            sql.AppendFormat(" AND Employees.UserName = '{0}'", userName);
            sql.Append(" AND TariffTypes.NoLevel = 0");
            sql.Append(" GROUP BY TariffTypes.TariffType, WorkingTimes.TariffLevel, WorkingTimes.Cost");
            sql.Append(" UNION");
            sql.Append(" SELECT TariffTypes.TariffType, WorkingTimes.Cost AS UnitCost, Sum(WorkingTimes.Sum) AS SumCosts, Sum(WorkingTimes.NumberOfHours) AS SumHours, Max(TariffTypes.BackgroundColor) AS BackgroundColor");
            sql.Append(" FROM TariffTypes INNER JOIN(Employees INNER JOIN WorkingTimes ON Employees.EmployeeId = WorkingTimes.EmployeeId) ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId");
            sql.AppendFormat($" WHERE WorkingTimes.WhenDate >= '{thisWeekStart.Year}-{thisWeekStart.Month}-{thisWeekStart.Day}'");
            sql.AppendFormat($" AND WorkingTimes.WhenDate <= '{thisWeekEnd.Year}-{thisWeekEnd.Month}-{thisWeekEnd.Day}'");
            sql.AppendFormat(" AND Employees.UserName = '{0}'", userName);
            sql.Append(" AND TariffTypes.NoLevel = 1");
            sql.Append(" GROUP BY TariffTypes.TariffType, WorkingTimes.TariffLevel, WorkingTimes.Cost");
            sql.Append(" ORDER BY TariffTypes.TariffType");
            IEnumerable<WorkingTimesSummarysModel> response = null;
            try
            {
                response = await _context.WorkingTimesSummaries.FromSqlRaw(sql.ToString()).AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                LastSqlCommand = sql.ToString();
            }
            return response;
        }

        public async Task<double> GetUnderlaySummariesOfWeekSum(DateTime date, string userName)
        {
            DateTime thisWeekStart = date.AddDays(-(int)date.DayOfWeek + 1);
            DateTime thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            StringBuilder sql = new StringBuilder("SELECT Sum(WorkingTimes.Sum) AS SumOf");
            sql.Append(" FROM TariffTypes INNER JOIN(Employees INNER JOIN WorkingTimes ON Employees.EmployeeId = WorkingTimes.EmployeeId) ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId");
            sql.AppendFormat(" WHERE WorkingTimes.WhenDate >= '{0}'", thisWeekStart);
            sql.AppendFormat(" AND WorkingTimes.WhenDate <= '{0}'", thisWeekEnd);
            sql.AppendFormat(" AND Employees.UserName = '{0}'", userName);
            SumOfModel sumOf;
            try
            {
                sumOf = await _context.SumOf.FromSqlRaw(sql.ToString()).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return 0;
            }
            return sumOf.SumOf;
        }

        public async Task<IEnumerable<WorkingTimesSummarysModel>> GetUnderlaySummariesOfDay(DateTime date, string userName)
        {
            DateOnly currentDate = DateOnly.FromDateTime(date);
            StringBuilder sql = new StringBuilder("SELECT TariffTypes.TariffType, WorkingTimes.Cost AS UnitCost, Sum(WorkingTimes.Sum) AS SumCosts, Sum(WorkingTimes.NumberOfHours) AS SumHours, Max(TariffTypes.BackgroundColor) AS BackgroundColor");
            sql.Append(" FROM TariffTypes INNER JOIN(Employees INNER JOIN WorkingTimes ON Employees.EmployeeId = WorkingTimes.EmployeeId) ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId");
            sql.AppendFormat($" WHERE WorkingTimes.WhenDate = '{currentDate.Year}-{currentDate.Month}-{currentDate.Day}'");
            sql.AppendFormat(" AND Employees.UserName = '{0}'", userName);
            sql.Append(" AND TariffTypes.NoLevel = 0");
            sql.Append(" GROUP BY TariffTypes.TariffType, WorkingTimes.TariffLevel, WorkingTimes.Cost");
            sql.Append(" UNION");
            sql.Append(" SELECT TariffTypes.TariffType, WorkingTimes.Cost AS UnitCost, Sum(WorkingTimes.Sum) AS SumCosts, Sum(WorkingTimes.NumberOfHours) AS SumHours, Max(TariffTypes.BackgroundColor) AS BackgroundColor");
            sql.Append(" FROM TariffTypes INNER JOIN(Employees INNER JOIN WorkingTimes ON Employees.EmployeeId = WorkingTimes.EmployeeId) ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId");
            sql.AppendFormat($" WHERE WorkingTimes.WhenDate = '{currentDate.Year}-{currentDate.Month}-{currentDate.Day}'");
            sql.AppendFormat(" AND Employees.UserName = '{0}'", userName);
            sql.Append(" AND TariffTypes.NoLevel = 1");
            sql.Append(" GROUP BY TariffTypes.TariffType, WorkingTimes.TariffLevel, WorkingTimes.Cost");
            sql.Append(" ORDER BY TariffTypes.TariffType");
            IEnumerable<WorkingTimesSummarysModel> response = null;
            try
            {
                response = await _context.WorkingTimesSummaries.FromSqlRaw(sql.ToString()).AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                LastSqlCommand = sql.ToString();
            }
            return response;
        }

        public async Task<double> GetUnderlaySummariesOfDaySum(DateTime date, string userName)
        {
            DateOnly currentDate = DateOnly.FromDateTime(date);
            StringBuilder sql = new StringBuilder("SELECT Sum(WorkingTimes.Sum) AS SumOf");
            sql.Append(" FROM TariffTypes INNER JOIN(Employees INNER JOIN WorkingTimes ON Employees.EmployeeId = WorkingTimes.EmployeeId) ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId");
            sql.AppendFormat(" WHERE WorkingTimes.WhenDate = '{0}'", currentDate);
            sql.AppendFormat(" AND Employees.UserName = '{0}'", userName);
            SumOfModel sumOf;
            try
            {
                sumOf = await _context.SumOf.FromSqlRaw(sql.ToString()).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return 0;
            }
            return sumOf.SumOf;

        }

    }
}
