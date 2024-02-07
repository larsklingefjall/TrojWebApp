using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrojWebApp.Models;

namespace TrojWebApp.Services
{
    public class EconomyConnection
    {

        private readonly TrojContext _context;
        private readonly string _cryKey;

        public EconomyConnection(TrojContext context, string crykey)
        {
            _context = context;
            _cryKey = crykey;
        }

        public async Task<IEnumerable<WorkingTimesEconomyModel>> GetEconomyWorkingTimes(string startDate, string endDate, string employeeId, int offset, int size)
        {
            StringBuilder sql = new StringBuilder("SELECT WorkingTimes.CaseId, SUM(WorkingTimes.Sum) AS WorkingTimesSum, MAX(FORMAT(Cases.FinishedDate, 'yyyy-MM-dd')) AS FinishedDate, MAX(Persons.PersonId) AS PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);

            sql.Append(" FROM WorkingTimes INNER JOIN");
            sql.Append(" Persons ON WorkingTimes.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" Cases ON WorkingTimes.CaseId = Cases.CaseId");

            sql.AppendFormat(" WHERE WorkingTimes.WhenDate >= '{0}'", startDate);
            sql.AppendFormat(" AND WorkingTimes.WhenDate <= '{0}'", endDate);
            if (employeeId != "" && employeeId != "0")
            {
                sql.AppendFormat(" AND WorkingTimes.EmployeeId = {0}", employeeId);
            }
            sql.Append(" GROUP BY WorkingTimes.CaseId");
            sql.AppendFormat(" , CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.FirstNameCry, 1, CONVERT(varbinary, Persons.PersonId)))", _cryKey);
            sql.AppendFormat(" , CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.LastNameCry, 1, CONVERT(varbinary, Persons.PersonId)))", _cryKey);
            sql.Append(" ORDER BY LastName, FirstName, WorkingTimes.CaseId");
            sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, size);
            return await _context.WorkingTimesEconomy.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<WorkingTimesPeriodEconomyModel>> GetEconomyTimePeriod(string startDate, string endDate, string employeeId, int offset, int size)
        {
            StringBuilder sql = new StringBuilder("SELECT WorkingTimes.CaseId, MIN(FORMAT(WorkingTimes.WhenDate, 'yyyy-MM-dd')) AS MinDate, MAX(FORMAT(WorkingTimes.WhenDate, 'yyyy-MM-dd')) AS MaxDate");
            sql.Append(" FROM WorkingTimes");
            sql.Append(" WHERE WorkingTimes.Sum > 0");
            sql.AppendFormat(" AND WorkingTimes.WhenDate >= '{0}'", startDate);
            sql.AppendFormat(" AND WorkingTimes.WhenDate <= '{0}'", endDate);
            if (employeeId != "" && employeeId != "0")
            {
                sql.AppendFormat(" AND WorkingTimes.EmployeeId = {0}", employeeId);
            }
            sql.Append(" GROUP BY WorkingTimes.CaseId");
            sql.Append(" ORDER BY WorkingTimes.CaseId");
            sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, size);
            return await _context.WorkingTimesPeriodEconomy.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<SumOfWorkingTimesModel>> GetSumOfWorkingTimesBefore(string startDate, string endDate, string employeeId)
        {
            StringBuilder sql = new StringBuilder("SELECT Sum(WorkingTimes.Sum) AS SumOfSum, CaseId");
            sql.Append(" FROM WorkingTimes");
            sql.AppendFormat(" WHERE WhenDate < '{0}'", startDate);
            sql.Append(" AND CaseId IN (");
            sql.Append(" SELECT DISTINCT CaseId");
            sql.Append(" FROM WorkingTimes");
            sql.AppendFormat(" WHERE WhenDate >= '{0}'", startDate);
            sql.AppendFormat(" AND WhenDate <= '{0}'", endDate);
            if (employeeId != "" && employeeId != "0")
            {
                sql.AppendFormat(" AND WorkingTimes.EmployeeId = {0}", employeeId);
            }
            sql.Append(" )");
            sql.Append(" GROUP BY CaseId");
            return await _context.SumOfWorkingTimes.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<SumOfWorkingTimesModel>> GetSumOfUnderlay(string startDate, string endDate, string employeeId)
        {
            StringBuilder sql = new StringBuilder("SELECT Sum(InvoiceWorkingTimes.Sum) AS SumOfSum, CaseId");
            sql.Append(" FROM WorkingTimes INNER JOIN InvoiceWorkingTimes ON WorkingTimes.WorkingTimeId = InvoiceWorkingTimes.WorkingTimeId");
            sql.Append(" WHERE CaseId IN (");
            sql.Append(" SELECT DISTINCT CaseId");
            sql.Append(" FROM WorkingTimes");
            sql.AppendFormat(" WHERE WhenDate >= '{0}'", startDate);
            sql.AppendFormat(" AND WhenDate <= '{0}'", endDate);
            if (employeeId != "" && employeeId != "0")
            {
                sql.AppendFormat(" AND WorkingTimes.EmployeeId = {0}", employeeId);
            }
            sql.Append(" )");
            sql.Append(" GROUP BY CaseId");
            return await _context.SumOfWorkingTimes.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<InvoiceEconomyModel>> GetInvoices(string startDate, string endDate, string employeeId)
        {
            StringBuilder sql = new StringBuilder("SELECT DISTINCT Invoices.InvoiceId, InvoiceUnderlays.CaseId, Invoices.InvoiceNumber, Invoices.Sum, Invoices.Vat");
            sql.Append(" FROM Invoices INNER JOIN");
            sql.Append(" InvoiceUnderlays ON Invoices.InvoiceUnderlayId = InvoiceUnderlays.InvoiceUnderlayId INNER JOIN");
            sql.Append(" InvoiceWorkingTimes ON InvoiceUnderlays.InvoiceUnderlayId = InvoiceWorkingTimes.InvoiceUnderlayId");
            sql.Append(" WHERE InvoiceWorkingTimes.WorkingTimeId IN (");
            sql.Append(" SELECT DISTINCT WorkingTimeId");
            sql.Append(" FROM WorkingTimes");
            sql.AppendFormat(" WHERE WhenDate >= '{0}'", startDate);
            sql.AppendFormat(" AND WhenDate <= '{0}'", endDate);
            if (employeeId != "" && employeeId != "0")
            {
                sql.AppendFormat(" AND WorkingTimes.EmployeeId = {0}", employeeId);
            }
            sql.Append(" )");
            sql.Append(" ORDER BY Invoices.InvoiceNumber");
            return await _context.InvoiceEconomy.FromSqlRaw(sql.ToString()).ToListAsync();
        }


    }
}
