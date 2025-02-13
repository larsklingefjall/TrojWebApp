using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TrojWebApp.Controllers;
using TrojWebApp.Models;

namespace TrojWebApp.Services
{
    public class InvoicesConnection
    {
        private readonly TrojContext _context;
        private readonly string _cryKey;
        private readonly ConfigurationsConnection _configurationConnection;
        private readonly EmployeesConnection _employeesConnection;
        private readonly TariffLevelsConnection _tariffLevelsConnection;
        private readonly InvoiceUnderlaysConnection _invoiceUnderlaysConnection;

        public InvoicesConnection(TrojContext context, string crykey)
        {
            _context = context;
            _cryKey = crykey;
            _configurationConnection = new ConfigurationsConnection(context);
            _employeesConnection = new EmployeesConnection(context);
            _tariffLevelsConnection = new TariffLevelsConnection(context);
            _invoiceUnderlaysConnection = new InvoiceUnderlaysConnection(context, crykey);
        }

        public async Task<IEnumerable<InvoicesViewModel>> GetInvoices(int offset, int size)
        {
            StringBuilder sql = new StringBuilder("SELECT Invoices.InvoiceId, Invoices.InvoiceUnderlayId, InvoiceUnderlays.UnderlayNumber, Invoices.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.FirstNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.LastNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(", Invoices.EmployeeId, Employees.Initials");
            sql.Append(", Invoices.InvoiceNumber, Invoices.InvoiceDate, Invoices.InvoicePlace, Invoices.ExpirationDate");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', Invoices.ReceiverNameCry, 1, CONVERT(varbinary, Invoices.InvoiceId))) AS ReceiverName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', Invoices.CareOfCry, 1, CONVERT(varbinary, Invoices.InvoiceId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Invoices.StreetNameCry, 1, CONVERT(varbinary, Invoices.InvoiceId))) AS StreetName", _cryKey);
            sql.Append(", Invoices.ContactName, Invoices.StreetNumber, Invoices.PostalCode, Invoices.PostalAddress, Invoices.Country");
            sql.Append(", Invoices.Headline1, Invoices.Headline2, Invoices.Text1");
            sql.Append(", Invoices.Vat, Invoices.Sum, Invoices.Division");
            sql.Append(", Invoices.Locked, Invoices.HideClientFunding");
            sql.Append(", InvoiceUnderlays.CaseId, CaseTypes.CaseType");
            sql.Append(", Invoices.Changed, Invoices.ChangedBy");
            sql.Append(" FROM Invoices INNER JOIN");
            sql.Append(" Employees ON Invoices.EmployeeId = Employees.EmployeeId INNER JOIN");
            sql.Append(" Persons ON Invoices.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" InvoiceUnderlays ON Invoices.InvoiceUnderlayId = InvoiceUnderlays.InvoiceUnderlayId INNER JOIN");
            sql.Append(" Cases ON InvoiceUnderlays.CaseId = Cases.CaseId INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId");
            sql.Append(" ORDER BY Invoices.InvoiceDate DESC");
            sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, size);
            return await _context.InvoicesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<InvoicesViewModel>> GetInvoices(int clientId)
        {
            StringBuilder sql = new StringBuilder("SELECT Invoices.InvoiceId, Invoices.InvoiceUnderlayId, InvoiceUnderlays.UnderlayNumber, Invoices.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.FirstNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.LastNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(", Invoices.EmployeeId, Employees.Initials");
            sql.Append(", Invoices.InvoiceNumber, Invoices.InvoiceDate, Invoices.InvoicePlace, Invoices.ExpirationDate");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', Invoices.ReceiverNameCry, 1, CONVERT(varbinary, Invoices.InvoiceId))) AS ReceiverName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', Invoices.CareOfCry, 1, CONVERT(varbinary, Invoices.InvoiceId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Invoices.StreetNameCry, 1, CONVERT(varbinary, Invoices.InvoiceId))) AS StreetName", _cryKey);
            sql.Append(", Invoices.ContactName, Invoices.StreetNumber, Invoices.PostalCode, Invoices.PostalAddress, Invoices.Country");
            sql.Append(", Invoices.Headline1, Invoices.Headline2, Invoices.Text1");
            sql.Append(", Invoices.Vat, Invoices.Sum, Invoices.Division");
            sql.Append(", Invoices.Locked, Invoices.HideClientFunding");
            sql.Append(", InvoiceUnderlays.CaseId, CaseTypes.CaseType");
            sql.Append(", Invoices.Changed, Invoices.ChangedBy");
            sql.Append(" FROM Invoices INNER JOIN");
            sql.Append(" Employees ON Invoices.EmployeeId = Employees.EmployeeId INNER JOIN");
            sql.Append(" Persons ON Invoices.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" InvoiceUnderlays ON Invoices.InvoiceUnderlayId = InvoiceUnderlays.InvoiceUnderlayId INNER JOIN");
            sql.Append(" Cases ON InvoiceUnderlays.CaseId = Cases.CaseId INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId");
            sql.AppendFormat(" WHERE Invoices.PersonId = {0}", clientId);
            sql.Append(" ORDER BY Invoices.InvoiceDate DESC");
            return await _context.InvoicesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<int> GetNumberOfInvoices()
        {
            StringBuilder sql = new StringBuilder("SELECT Count(Invoices.InvoiceId) AS NumberOf");
            sql.Append(" FROM Invoices INNER JOIN");
            sql.Append(" Employees ON Invoices.EmployeeId = Employees.EmployeeId INNER JOIN");
            sql.Append(" Persons ON Invoices.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" InvoiceUnderlays ON Invoices.InvoiceUnderlayId = InvoiceUnderlays.InvoiceUnderlayId");
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf;
        }

        public async Task<int> GetNumberOfInvoices(int clientId)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(Invoices.InvoiceId) AS NumberOf");
            sql.Append(" FROM Invoices INNER JOIN");
            sql.Append(" Employees ON Invoices.EmployeeId = Employees.EmployeeId INNER JOIN");
            sql.Append(" Persons ON Invoices.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" InvoiceUnderlays ON Invoices.InvoiceUnderlayId = InvoiceUnderlays.InvoiceUnderlayId");
            sql.AppendFormat(" WHERE Invoices.PersonId = {0}", clientId);
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf;
        }

        public async Task<int> GetNumberOfInvoices4Underlay(int underlayId)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(Invoices.InvoiceId) AS NumberOf");
            sql.Append(" FROM Invoices");
            sql.AppendFormat(" WHERE InvoiceUnderlayId = {0}", underlayId);
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf;
        }

        public async Task<IEnumerable<InvoicesViewModel>> GetFilyeredInvoices(string invoiceNumber, string underlayNumber, string invoiceDate, string receiverName, string employeeId, string firstName, string lastName, string locked, int offset, int size)
        {
            StringBuilder sql = new StringBuilder("SELECT Invoices.InvoiceId, Invoices.InvoiceUnderlayId, InvoiceUnderlays.UnderlayNumber, Invoices.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.FirstNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.LastNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(", Invoices.EmployeeId, Employees.Initials");
            sql.Append(", Invoices.InvoiceNumber, Invoices.InvoiceDate, Invoices.InvoicePlace, Invoices.ExpirationDate");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', Invoices.ReceiverNameCry, 1, CONVERT(varbinary, Invoices.InvoiceId))) AS ReceiverName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', Invoices.CareOfCry, 1, CONVERT(varbinary, Invoices.InvoiceId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Invoices.StreetNameCry, 1, CONVERT(varbinary, Invoices.InvoiceId))) AS StreetName", _cryKey);
            sql.Append(", Invoices.ContactName, Invoices.StreetNumber, Invoices.PostalCode, Invoices.PostalAddress, Invoices.Country");
            sql.Append(", Invoices.Headline1, Invoices.Headline2, Invoices.Text1");
            sql.Append(", Invoices.Vat, Invoices.Sum, Invoices.Division");
            sql.Append(", Invoices.Locked, Invoices.HideClientFunding");
            sql.Append(", InvoiceUnderlays.CaseId, CaseTypes.CaseType");
            sql.Append(", Invoices.Changed, Invoices.ChangedBy");
            sql.Append(" FROM Invoices INNER JOIN");
            sql.Append(" Employees ON Invoices.EmployeeId = Employees.EmployeeId INNER JOIN");
            sql.Append(" Persons ON Invoices.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" InvoiceUnderlays ON Invoices.InvoiceUnderlayId = InvoiceUnderlays.InvoiceUnderlayId INNER JOIN");
            sql.Append(" Cases ON InvoiceUnderlays.CaseId = Cases.CaseId INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId");
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
            if (invoiceNumber != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND Invoices.InvoiceNumber LIKE '{0}'", invoiceNumber);
                else
                    where.AppendFormat(" WHERE Invoices.InvoiceNumber LIKE '{0}'", invoiceNumber);
            }
            if (underlayNumber != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND InvoiceUnderlays.UnderlayNumber LIKE '{0}'", underlayNumber);
                else
                    where.AppendFormat(" WHERE InvoiceUnderlays.UnderlayNumber LIKE '{0}'", underlayNumber);
            }
            if (invoiceDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND Invoices.InvoiceDate = '{0}'", invoiceDate);
                else
                    where.AppendFormat(" WHERE Invoices.InvoiceDate = '{0}'", invoiceDate);
            }
            if (receiverName != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Invoices.ReceiverNameCry, 1 , CONVERT(varbinary, Invoices.InvoiceId))) LIKE '{1}'", _cryKey, receiverName);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Invoices.ReceiverNameCry, 1 , CONVERT(varbinary, Invoices.InvoiceId))) LIKE '{1}'", _cryKey, receiverName);
            }
            if (employeeId != "" && employeeId != "0")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND Invoices.EmployeeId = {0}", employeeId);
                else
                    where.AppendFormat(" WHERE Invoices.EmployeeId = {0}", employeeId);
            }
            if (locked != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND Invoices.Locked = {0}", locked);
                else
                    where.AppendFormat(" WHERE Invoices.Locked = {0}", locked);
            }
            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());
            sql.Append(" ORDER BY Invoices.InvoiceDate DESC");
            sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, size);
            return await _context.InvoicesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<int> GetNumberOfFilteredInvoices(string invoiceNumber, string underlayNumber, string invoiceDate, string receiverName, string employeeId, string firstName, string lastName, string locked)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(Invoices.InvoiceId) AS NumberOf");
            sql.Append(" FROM Invoices INNER JOIN");
            sql.Append(" Employees ON Invoices.EmployeeId = Employees.EmployeeId INNER JOIN");
            sql.Append(" Persons ON Invoices.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" InvoiceUnderlays ON Invoices.InvoiceUnderlayId = InvoiceUnderlays.InvoiceUnderlayId");
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
            if (invoiceNumber != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND Invoices.InvoiceNumber LIKE '{0}'", invoiceNumber);
                else
                    where.AppendFormat(" WHERE Invoices.InvoiceNumber LIKE '{0}'", invoiceNumber);
            }
            if (underlayNumber != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND InvoiceUnderlays.UnderlayNumber LIKE '{0}'", underlayNumber);
                else
                    where.AppendFormat(" WHERE InvoiceUnderlays.UnderlayNumber LIKE '{0}'", underlayNumber);
            }
            if (invoiceDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND Invoices.InvoiceDate = '{0}'", invoiceDate);
                else
                    where.AppendFormat(" WHERE Invoices.InvoiceDate = '{0}'", invoiceDate);
            }
            if (receiverName != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Invoices.ReceiverNameCry, 1 , CONVERT(varbinary, Invoices.InvoiceId))) LIKE '{1}'", _cryKey, receiverName);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Invoices.ReceiverNameCry, 1 , CONVERT(varbinary, Invoices.InvoiceId))) LIKE '{1}'", _cryKey, receiverName);
            }
            if (employeeId != "" && employeeId != "0")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND Invoices.EmployeeId = {0}", employeeId);
                else
                    where.AppendFormat(" WHERE Invoices.EmployeeId = {0}", employeeId);
            }
            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf;
        }

        public async Task<InvoicesModel> GetInvoice(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT InvoiceId, InvoiceUnderlayId, PersonId, PersonAddressId, EmployeeId, EmployeeTitle, SignatureLink, InvoiceNumber, InvoiceDate, InvoicePlace, ExpirationDate,ContactName, StreetNumber, PostalCode, PostalAddress, Country, Headline1, Headline2, Text1, Vat, [Sum], Division, Locked, HideClientFunding, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', ReceiverNameCry, 1, CONVERT(varbinary, InvoiceId))) AS ReceiverName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', CareOfCry, 1, CONVERT(varbinary, InvoiceId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', StreetNameCry, 1, CONVERT(varbinary, InvoiceId))) AS StreetName", _cryKey);
            sql.Append(" FROM Invoices");
            sql.AppendFormat(" WHERE InvoiceId = {0}", id);
            return await _context.Invoices.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<InvoicesViewModel> GetInvoiceView(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT Invoices.InvoiceId, Invoices.InvoiceUnderlayId, InvoiceUnderlays.UnderlayNumber, Invoices.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.FirstNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.LastNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(", Invoices.EmployeeId, Employees.Initials");
            sql.Append(", Invoices.InvoiceNumber, Invoices.InvoiceDate, Invoices.InvoicePlace, Invoices.ExpirationDate");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', Invoices.ReceiverNameCry, 1, CONVERT(varbinary, Invoices.InvoiceId))) AS ReceiverName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', Invoices.CareOfCry, 1, CONVERT(varbinary, Invoices.InvoiceId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Invoices.StreetNameCry, 1, CONVERT(varbinary, Invoices.InvoiceId))) AS StreetName", _cryKey);
            sql.Append(", Invoices.ContactName, Invoices.StreetNumber, Invoices.PostalCode, Invoices.PostalAddress, Invoices.Country");
            sql.Append(", Invoices.Headline1, Invoices.Headline2, Invoices.Text1");
            sql.Append(", Invoices.Vat, Invoices.Sum, Invoices.Division");
            sql.Append(", Invoices.Locked, Invoices.HideClientFunding");
            sql.Append(", InvoiceUnderlays.CaseId, CaseTypes.CaseType");
            sql.Append(", Invoices.Changed, Invoices.ChangedBy");
            sql.Append(" FROM Invoices INNER JOIN");
            sql.Append(" Employees ON Invoices.EmployeeId = Employees.EmployeeId INNER JOIN");
            sql.Append(" Persons ON Invoices.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" InvoiceUnderlays ON Invoices.InvoiceUnderlayId = InvoiceUnderlays.InvoiceUnderlayId INNER JOIN");
            sql.Append(" Cases ON InvoiceUnderlays.CaseId = Cases.CaseId INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId");
            sql.AppendFormat(" WHERE Invoices.InvoiceId = {0}", id);
            return await _context.InvoicesView.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<IEnumerable<InvoiceSummarysViewModel>> GetInvoiceSummeries(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT InvoiceSummaryId, InvoiceId, UnitCost, UnitCounts, [Sum]");
            sql.Append(", InvoiceSummarys.TariffTypeId, TariffTypes.BackgroundColor, TariffTypes.TariffType");
            sql.Append(", InvoiceSummarys.Changed, InvoiceSummarys.ChangedBy");
            sql.Append(" FROM InvoiceSummarys INNER JOIN");
            sql.Append(" TariffTypes ON InvoiceSummarys.TariffTypeId = TariffTypes.TariffTypeId");
            sql.AppendFormat(" WHERE InvoiceId = {0}", id);
            return await _context.InvoiceSummarysView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<SumOfModel> GetSumOfInvoiceSummeries(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT Sum(Sum) AS SumOf");
            sql.Append(" FROM InvoiceSummarys");
            sql.AppendFormat(" WHERE InvoiceId = {0}", id);
            return await _context.SumOf.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<TotalSumAndHoursModel> GetSumAndHoursOfInvoiceSummeries(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT Sum(Sum) AS TotalSum, Sum(UnitCounts) AS TotalHours");
            sql.Append(" FROM InvoiceSummarys");
            sql.AppendFormat(" WHERE InvoiceId = {0}", id);
            return await _context.TotalSumAndHours.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<InvoicesModel> CreateInvoice(int invoiceUnderlayId, int employeeId, string invoicePlace, DateTime invoiceDate, DateTime expirationDate, int vat, int division, double clientFundingTotalSum, string userName = "")
        {
            InvoiceUnderlaysViewModel underlay = await _invoiceUnderlaysConnection.GetUnderlay(invoiceUnderlayId);

            string headline1 = "";
            ConfigurationsModel configuration = await _configurationConnection.GetConfigurationWithkey("KostnadsRakningText1");
            if (configuration != null)
                headline1 = configuration.ConfigValue;

            string headline2 = "";
            configuration = await _configurationConnection.GetConfigurationWithkey("KostnadsRakningText2");
            if (configuration != null)
                headline2 = configuration.ConfigValue;

            string text1 = "";
            configuration = await _configurationConnection.GetConfigurationWithkey("FakturaText1");
            if (configuration != null)
                text1 = configuration.ConfigValue;

            string employeeTitle = "";
            string signatureLink = "";
            EmployeesModel employee = await _employeesConnection.GetEmployee(employeeId);
            if (employee != null)
            {
                employeeTitle = employee.EmployeeTitle;
                signatureLink = employee.SignatureLink;
            }

            string receiverName = underlay.ReceiverName;
            string careOf = underlay.CareOf;
            string streetName = underlay.StreetName;

            InvoicesModel invoice = new InvoicesModel
            {
                InvoiceId = 0,
                InvoiceUnderlayId = invoiceUnderlayId,
                PersonId = underlay.PersonId,
                PersonAddressId = underlay.PersonAddressId,
                EmployeeId = employeeId,
                EmployeeTitle = employeeTitle,
                SignatureLink = signatureLink,

                InvoiceDate = invoiceDate,
                ExpirationDate = expirationDate,
                InvoicePlace = invoicePlace,

                ReceiverName = "",
                CareOf = "",
                StreetName = "",
                StreetNumber = underlay.StreetNumber,
                PostalCode = underlay.PostalCode,
                PostalAddress = underlay.PostalAddress,
                Country = underlay.Country,

                ContactName = "",

                Headline1 = headline1,
                Headline2 = headline2,
                Text1 = text1,

                Vat = vat,
                Sum = 0,
                Division = division,

                Locked = false,
                HideClientFunding = false,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Invoices.Add(invoice);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return null;

            InvoicesModel newInvoice = await _context.Invoices.Where(i => i.InvoiceUnderlayId == invoiceUnderlayId).OrderByDescending(s => s.InvoiceUnderlayId).FirstAsync();
            if (newInvoice == null)
                return null;

            StringBuilder sql = new StringBuilder("DECLARE @ReceiverName nvarchar(512); ");
            sql.Append("DECLARE @CareOf nvarchar(512); ");
            sql.Append("DECLARE @StreetName nvarchar(256); ");
            sql.AppendFormat("SET @ReceiverName = '{0}'; ", receiverName);
            sql.AppendFormat("SET @CareOf = '{0}'; ", careOf);
            sql.AppendFormat("SET @StreetName = '{0}'; ", streetName);
            sql.Append(" UPDATE Invoices SET ");
            if (string.IsNullOrEmpty(receiverName))
                sql.Append(" ReceiverNameCry = Null");
            else
                sql.AppendFormat(" ReceiverNameCry = EncryptByPassPhrase('{0}', @ReceiverName, 1, CONVERT( varbinary, InvoiceId))", _cryKey);
            if (string.IsNullOrEmpty(careOf))
                sql.Append(", CareOfCry = Null");
            else
                sql.AppendFormat(", CareOfCry = EncryptByPassPhrase('{0}', @CareOf, 1, CONVERT( varbinary, InvoiceId))", _cryKey);
            if (string.IsNullOrEmpty(streetName))
                sql.Append(", StreetNameCry = Null");
            else
                sql.AppendFormat(", StreetNameCry = EncryptByPassPhrase('{0}', @StreetName, 1, CONVERT( varbinary, InvoiceId))", _cryKey);
            sql.Append(", ReceiverName = Null");
            sql.Append(", CareOf = Null");
            sql.Append(", StreetName = Null");
            sql.AppendFormat(" WHERE InvoiceId = {0}", newInvoice.InvoiceId);
            int updateInvoice = await _context.Database.ExecuteSqlRawAsync(sql.ToString());
            if (updateInvoice != 1)
            {
                return null;
            }

            bool result = true;
            IEnumerable<UnderlayWorkingTimeSummaryModel> summaries = await _invoiceUnderlaysConnection.GetUnderlayWorkingTimeSummary(underlay.InvoiceUnderlayId);
            foreach (UnderlayWorkingTimeSummaryModel summary in summaries)
            {
                double sumOfHours = summary.SumHours;
                double sumOfCosts = summary.SumCosts;
                if (division != 100)
                {
                    sumOfHours = (summary.SumHours * division) / 100;
                    sumOfCosts = (summary.SumCosts * division) / 100;
                }
                InvoiceSummarysModel invoiceSummary = new InvoiceSummarysModel
                {
                    InvoiceSummaryId = 0,
                    InvoiceId = newInvoice.InvoiceId,
                    TariffTypeId = summary.TariffTypeId,
                    UnitCost = summary.UnitCost,
                    UnitCounts = sumOfHours,
                    Sum = sumOfCosts,
                    Changed = DateTime.Now,
                    ChangedBy = userName
                };
                _context.InvoiceSummarys.Add(invoiceSummary);
                numberOfSaves = await _context.SaveChangesAsync();
                if (numberOfSaves != 1)
                {
                    result = false;
                    break;
                }
            }

            double invoiceSum = 0.0;
            SumOfModel totalSum = await GetSumOfInvoiceSummeries(newInvoice.InvoiceId);
            if (totalSum != null)
            {
                double addVat = 1.0 + (double)vat / 100;
                invoiceSum = (double)totalSum.SumOf * addVat;
            }
            InvoicesModel updatedInvoice = await _context.Invoices.Where(i => i.InvoiceId == newInvoice.InvoiceId).FirstAsync();
            if (updatedInvoice == null)
                return null;
            string invoiceNumber = underlay.PersonId.ToString() + "-" + underlay.CaseId.ToString() + "-" + newInvoice.InvoiceId.ToString();
            updatedInvoice.Sum = invoiceSum;
            updatedInvoice.InvoiceNumber = invoiceNumber;
            _context.Entry(updatedInvoice).State = EntityState.Modified;
            numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return null;

            bool updateLocked = await _invoiceUnderlaysConnection.SetLocked(invoiceUnderlayId);
            if (updateLocked == false)
            {
                result = false;
            }

            if(clientFundingTotalSum != 0)
            {
                ClientFundingsModel clientFunding = new ClientFundingsModel
                {
                    ClientFundId = 0,
                    CaseId = underlay.CaseId,
                    ClientSum = clientFundingTotalSum * -1,
                    Comment = invoiceNumber,
                    ClientFundDate = DateTime.Now,
                    Changed = DateTime.Now,
                    ChangedBy = userName
                };
                _context.ClientFundings.Add(clientFunding);
                numberOfSaves = await _context.SaveChangesAsync();
                if (numberOfSaves == 1)
                {
                    ClientFundingsModel newClientFunding = await _context.ClientFundings.Where(i => i.CaseId == underlay.CaseId).OrderByDescending(s => s.ClientFundId).FirstAsync();
                    if (newClientFunding != null)
                    {
                        InvoiceClientFundsModel invoiceClientFunding = new InvoiceClientFundsModel
                        {
                            InvoiceClientFundId = 0,
                            ClientFundId = newClientFunding.ClientFundId,
                            InvoiceId = newInvoice.InvoiceId,
                            Changed = DateTime.Now,
                            ChangedBy = userName
                        };
                        _context.InvoiceClientFunds.Add(invoiceClientFunding);
                        numberOfSaves = await _context.SaveChangesAsync();
                        if (numberOfSaves != 1)
                        {
                            result = false;
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }

            if (result)
                return invoice;
            else
                return null;
        }

        public async Task<int> UpdateInvoiceSum(int id)
        {
            InvoicesModel invoice = await _context.Invoices.Where(i => i.InvoiceId == id).FirstAsync();
            if (invoice == null)
                return 0;

            double vat = (double)invoice.Vat;
            double invoiceSum = 0.0;
            SumOfModel totalSum = await GetSumOfInvoiceSummeries(id);
            if (totalSum != null)
            {
                double addVat = 1.0 + (double)vat / 100;
                invoiceSum = (double)totalSum.SumOf * addVat;
            }

            StringBuilder sql = new StringBuilder("UPDATE Invoices SET ");
            sql.AppendFormat(" [Sum] = {0}", invoiceSum.ToString().Replace(',', '.'));
            sql.AppendFormat(" WHERE InvoiceId = {0}", id.ToString());
            return await _context.Database.ExecuteSqlRawAsync(sql.ToString());
        }

        public async Task<InvoicesModel> UpdateInvoice(int id, int employeeId, DateTime invoiceDate, DateTime expirationDate, string invoicePlace, string headline1, string headline2, string text1, bool locked, bool hideClientFunding, InvoicesModel currentInvoice, string userName = "")
        {
            EmployeesModel newEmployee = await _employeesConnection.GetEmployee(employeeId);
            InvoicesModel invoice = new InvoicesModel
            {
                InvoiceId = id,
                InvoiceUnderlayId = currentInvoice.InvoiceUnderlayId,
                PersonId = currentInvoice.PersonId,
                PersonAddressId = currentInvoice.PersonAddressId,
                EmployeeId = employeeId,
                EmployeeTitle = newEmployee.EmployeeTitle,
                SignatureLink = newEmployee.SignatureLink,
                InvoiceNumber = currentInvoice.InvoiceNumber,
                InvoiceDate = invoiceDate,
                ExpirationDate = expirationDate,
                InvoicePlace = invoicePlace,
                StreetNumber = currentInvoice.StreetNumber,
                PostalCode = currentInvoice.PostalCode,
                PostalAddress = currentInvoice.PostalAddress,
                Country = currentInvoice.Country,
                Headline1 = headline1,
                Headline2 = headline2,
                Text1 = text1,
                Vat = currentInvoice.Vat,
                Sum = currentInvoice.Sum,
                Division = currentInvoice.Division,
                Locked = locked,
                HideClientFunding = hideClientFunding,
                Changed = DateTime.Now,
                ChangedBy = userName
            };
            _context.Entry(invoice).State = EntityState.Modified;
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return null;

            StringBuilder sql = new StringBuilder("DECLARE @ReceiverName nvarchar(512); ");
            sql.AppendFormat("SET @ReceiverName = '{0}'; ", currentInvoice.ReceiverName);
            sql.Append("DECLARE @CareOf nvarchar(255); ");
            sql.AppendFormat("SET @CareOf = '{0}'; ", currentInvoice.CareOf);
            sql.Append("DECLARE @StreetName nvarchar(256); ");
            sql.AppendFormat("SET @StreetName = '{0}'; ", currentInvoice.StreetName);
            sql.Append(" UPDATE Invoices SET ");
            if (string.IsNullOrEmpty(currentInvoice.ReceiverName))
                sql.Append(" ReceiverNameCry = Null");
            else
                sql.AppendFormat(" ReceiverNameCry = EncryptByPassPhrase('{0}', @ReceiverName, 1, CONVERT( varbinary, InvoiceId))", _cryKey);
            if (string.IsNullOrEmpty(currentInvoice.CareOf))
                sql.Append(", CareOfCry = Null");
            else
                sql.AppendFormat(", CareOfCry = EncryptByPassPhrase('{0}', @CareOf, 1, CONVERT( varbinary, InvoiceId))", _cryKey);
            if (string.IsNullOrEmpty(currentInvoice.StreetName))
                sql.Append(", StreetNameCry = Null");
            else
                sql.AppendFormat(", StreetNameCry = EncryptByPassPhrase('{0}', @StreetName, 1, CONVERT( varbinary, InvoiceId))", _cryKey);
            sql.Append(", ReceiverName = Null");
            sql.Append(", CareOf = Null");
            sql.Append(", StreetName = Null");
            sql.AppendFormat(" WHERE InvoiceId = {0}", id);
            int numberOfUpdated = await _context.Database.ExecuteSqlRawAsync(sql.ToString());
            if (numberOfUpdated != 1)
            {
                return null;
            }

            return await GetInvoice(id);
        }

        public async Task<InvoiceSummarysModel> GetInvoiceSummary(int id)
        {
            return await _context.InvoiceSummarys.FindAsync(id);
        }

        public async Task<int> CreateInvoiceSummary(int invoiceId, int tariffTypeId, double unitCounts, string userName = "")
        {
            double level = 0;
            TariffLevelsModel tariffLevel = await _tariffLevelsConnection.GetValidTariffLevel(tariffTypeId);
            if (tariffLevel != null)
            {
                level = tariffLevel.TariffLevel;
            }

            double sum = level * unitCounts;

            InvoiceSummarysModel invoiceSummary = new InvoiceSummarysModel
            {
                InvoiceSummaryId = 0,
                InvoiceId = invoiceId,
                TariffTypeId = tariffTypeId,
                UnitCounts = unitCounts,
                UnitCost = level,
                Sum = sum,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.InvoiceSummarys.Add(invoiceSummary);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return 0;

            _context.Entry(invoiceSummary).State = EntityState.Detached;

            return await UpdateInvoiceSum(invoiceId);
        }

        public async Task<int> CreateInvoiceSummary(int invoiceId, int tariffTypeId, double unitCounts, double unitCost, string userName = "")
        {
            double sum = unitCost * unitCounts;

            InvoiceSummarysModel invoiceSummary = new InvoiceSummarysModel
            {
                InvoiceSummaryId = 0,
                InvoiceId = invoiceId,
                TariffTypeId = tariffTypeId,
                UnitCounts = unitCounts,
                UnitCost = unitCost,
                Sum = sum,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.InvoiceSummarys.Add(invoiceSummary);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return 0;

            _context.Entry(invoiceSummary).State = EntityState.Detached;

            return await UpdateInvoiceSum(invoiceId);
        }

        public async Task<int> UpdateInvoiceSummary(int id, double unitCounts, string userName = "")
        {
            InvoiceSummarysModel currentSummary = await _context.InvoiceSummarys.FindAsync(id);
            double sum = currentSummary.UnitCost * unitCounts;
            int invoiceId = currentSummary.InvoiceId;

            InvoiceSummarysModel invoiceSummary = new InvoiceSummarysModel
            {
                InvoiceSummaryId = id,
                InvoiceId = invoiceId,
                TariffTypeId = currentSummary.TariffTypeId,
                UnitCounts = unitCounts,
                UnitCost = currentSummary.UnitCost,
                Sum = sum,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Entry(currentSummary).State = EntityState.Detached;

            _context.Entry(invoiceSummary).State = EntityState.Modified;
            int numberOfUpdated = await _context.SaveChangesAsync();
            if (numberOfUpdated != 1)
                return 0;
            _context.Entry(invoiceSummary).State = EntityState.Detached;

            return await UpdateInvoiceSum(invoiceId);
        }

        public async Task<int> DeleteInvoiceSummary(int id)
        {
            InvoiceSummarysModel currentSummary = await _context.InvoiceSummarys.FindAsync(id);
            int invoiceId = currentSummary.InvoiceId;
            _context.Entry(currentSummary).State = EntityState.Detached;

            StringBuilder sql = new StringBuilder("DELETE FROM InvoiceSummarys");
            sql.AppendFormat(" WHERE InvoiceSummaryId = {0}", id);
            int numberOfDeleted = await _context.Database.ExecuteSqlRawAsync(sql.ToString());
            if (numberOfDeleted != 1)
                return 0;

            return await UpdateInvoiceSum(invoiceId);
        }

        public async Task<IEnumerable<InvoicesPartialViewModel>> GetInvoicesForUnderly(int invoiceUnderlayId)
        {
            StringBuilder sql = new StringBuilder("SELECT InvoiceId, InvoiceUnderlayId, InvoiceNumber, InvoiceDate, Vat, [Sum], Locked, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', Invoices.ReceiverNameCry, 1, CONVERT(varbinary, InvoiceId))) AS ReceiverName", _cryKey);
            sql.Append(" FROM Invoices");
            sql.AppendFormat(" WHERE InvoiceUnderlayId = {0}", invoiceUnderlayId);
            sql.Append(" ORDER BY Invoices.InvoiceDate DESC");
            return await _context.InvoicesPartialView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<TotalSumModel> GetClientFundTotalSum(int id)
        {
            TotalSumModel zeroTotalSum = new TotalSumModel
            {
                TotalSum = 0,
            };

            StringBuilder sql = new StringBuilder("SELECT Sum(ClientFundings.ClientSum) AS TotalSum");
            sql.Append(" FROM InvoiceClientFunds INNER JOIN ClientFundings ON InvoiceClientFunds.ClientFundId = ClientFundings.ClientFundId");
            sql.AppendFormat(" WHERE InvoiceClientFunds.InvoiceId = {0}", id);

            TotalSumModel totalSum;
            try
            {
                totalSum = await _context.ClientFundingTotalSum.FromSqlRaw(sql.ToString()).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return zeroTotalSum;
            }
            return totalSum;
        }

    }
}
