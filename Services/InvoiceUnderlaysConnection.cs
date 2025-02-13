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
using TrojWebApp.Models;

namespace TrojWebApp.Services
{
    public class InvoiceUnderlaysConnection
    {
        private readonly TrojContext _context;
        private readonly string _cryKey;
        private readonly ConfigurationsConnection _configurationConnection;
        private readonly EmployeesConnection _employeesConnection;
        private readonly PersonsConnection _personsConnection;
        private readonly CourtsConnection _courtsConnection;
        private readonly CasesConnection _casesConnection;
        private readonly WorkingTimesConnection _workingTimesConnection;


        public InvoiceUnderlaysConnection(TrojContext context, string crykey)
        {
            _context = context;
            _cryKey = crykey;
            _configurationConnection = new ConfigurationsConnection(context);
            _employeesConnection = new EmployeesConnection(context);
            _personsConnection = new PersonsConnection(context, crykey);
            _courtsConnection = new CourtsConnection(context);
            _casesConnection = new CasesConnection(context, crykey);
            _workingTimesConnection = new WorkingTimesConnection(context, crykey);
        }

        public async Task<IEnumerable<InvoiceUnderlaysViewModel>> GetUnderlays(int offset, int size)
        {
            StringBuilder sql = new StringBuilder("SELECT InvoiceUnderlayId, InvoiceUnderlays.Title, UnderlayNumber, InvoiceUnderlays.PersonId, InvoiceUnderlays.CaseId, InvoiceUnderlays.PersonAddressId, InvoiceUnderlays.EmployeeId, InvoiceUnderlays.EmployeeTitle, InvoiceUnderlays.SignatureLink, UnderlayDate, UnderlayPlace, StreetNumber, PostalCode, PostalAddress, Country, Headline1, Headline2, WorkingReport, Vat, [Sum], Locked, InvoiceUnderlays.Changed, InvoiceUnderlays.ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', ReceiverNameCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS ReceiverName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', InvoiceUnderlays.CareOfCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', StreetNameCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS StreetName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.FirstNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.LastNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(" ,Initials, CaseType");
            sql.Append(" FROM InvoiceUnderlays INNER JOIN Cases ON InvoiceUnderlays.CaseId = Cases.CaseId INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" Persons ON InvoiceUnderlays.PersonId = Persons.PersonId INNER JOIN Employees ON InvoiceUnderlays.EmployeeId = Employees.EmployeeId");
            sql.Append(" ORDER BY UnderlayDate DESC");
            sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, size);
            return await _context.InvoiceUnderlaysView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<int> GetNumberOfUnderlays()
        {
            StringBuilder sql = new StringBuilder("SELECT Count(InvoiceUnderlays.InvoiceUnderlayId) AS NumberOf");
            sql.Append(" FROM InvoiceUnderlays INNER JOIN Cases ON InvoiceUnderlays.CaseId = Cases.CaseId INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" Persons ON InvoiceUnderlays.PersonId = Persons.PersonId INNER JOIN Employees ON InvoiceUnderlays.EmployeeId = Employees.EmployeeId");
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf;
        }

        public async Task<IEnumerable<InvoiceWorkingTimesViewModel>> GetUnderlayWorkingTimes(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT InvoiceWorkingTimes.InvoiceWorkingTimeId, InvoiceWorkingTimes.InvoiceUnderlayId, InvoiceWorkingTimes.WorkingTimeId, WorkingTimes.WhenDate, WorkingTimes.Billed, TariffTypes.TariffType, TariffTypes.BackgroundColor, InvoiceWorkingTimes.UnitCost, InvoiceWorkingTimes.NumberOfHours, InvoiceWorkingTimes.[Sum], Employees.Initials");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', CommentCry, 1, CONVERT(varbinary, WorkingTimes.WorkingTimeId))) AS Comment", _cryKey);
            sql.Append(", InvoiceWorkingTimes.Changed, InvoiceWorkingTimes.ChangedBy");
            sql.Append(" FROM WorkingTimes INNER JOIN TariffTypes ON WorkingTimes.TariffTypeId = TariffTypes.TariffTypeId INNER JOIN InvoiceWorkingTimes ON WorkingTimes.WorkingTimeId = InvoiceWorkingTimes.WorkingTimeId INNER JOIN Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");
            sql.AppendFormat(" WHERE InvoiceWorkingTimes.InvoiceUnderlayId = {0}", id);
            sql.Append(" ORDER BY WorkingTimes.WhenDate DESC");
            return await _context.InvoiceWorkingTimesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<InvoiceWorkingTimesViewModel> GetUnderlayWorkingTime(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT InvoiceWorkingTimes.InvoiceWorkingTimeId, InvoiceWorkingTimes.InvoiceUnderlayId, InvoiceWorkingTimes.WorkingTimeId, WorkingTimes.WhenDate, WorkingTimes.Billed, TariffTypes.TariffType, TariffTypes.BackgroundColor, InvoiceWorkingTimes.UnitCost, InvoiceWorkingTimes.NumberOfHours, InvoiceWorkingTimes.[Sum], Employees.Initials");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', CommentCry, 1, CONVERT(varbinary, WorkingTimes.WorkingTimeId))) AS Comment", _cryKey);
            sql.Append(", InvoiceWorkingTimes.Changed, InvoiceWorkingTimes.ChangedBy");
            sql.Append(" FROM WorkingTimes INNER JOIN TariffTypes ON WorkingTimes.TariffTypeId = TariffTypes.TariffTypeId INNER JOIN InvoiceWorkingTimes ON WorkingTimes.WorkingTimeId = InvoiceWorkingTimes.WorkingTimeId INNER JOIN Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");
            sql.AppendFormat(" WHERE InvoiceWorkingTimes.InvoiceWorkingTimeId = {0}", id);
            return await _context.InvoiceWorkingTimesView.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<IEnumerable<InvoiceUnderlaysViewModel>> GetFilteredUnderlays(string caseId, string underlayNumber, string underlayDate, string caseTypeId, string employeeId, string firstName, string lastName, int offset, int size)
        {
            StringBuilder sql = new StringBuilder("SELECT InvoiceUnderlayId, InvoiceUnderlays.Title, UnderlayNumber, InvoiceUnderlays.PersonId, InvoiceUnderlays.CaseId, InvoiceUnderlays.PersonAddressId, InvoiceUnderlays.EmployeeId, InvoiceUnderlays.EmployeeTitle, InvoiceUnderlays.SignatureLink, UnderlayDate, UnderlayPlace, StreetNumber, PostalCode, PostalAddress, Country, Headline1, Headline2, WorkingReport, Vat, [Sum], Locked, InvoiceUnderlays.Changed, InvoiceUnderlays.ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', ReceiverNameCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS ReceiverName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', InvoiceUnderlays.CareOfCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', StreetNameCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS StreetName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.FirstNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.LastNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(" ,Initials, CaseType");
            sql.Append(" FROM InvoiceUnderlays INNER JOIN Cases ON InvoiceUnderlays.CaseId = Cases.CaseId INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" Persons ON InvoiceUnderlays.PersonId = Persons.PersonId INNER JOIN Employees ON InvoiceUnderlays.EmployeeId = Employees.EmployeeId");
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
                    where.AppendFormat(" AND InvoiceUnderlays.CaseId = {0}", caseId);
                else
                    where.AppendFormat(" WHERE InvoiceUnderlays.CaseId = {0}", caseId);
            }
            if (underlayNumber != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND UnderlayNumber LIKE '{0}'", underlayNumber);
                else
                    where.AppendFormat(" WHERE UnderlayNumber LIKE '{0}'", underlayNumber);
            }
            if (underlayDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND UnderlayDate = '{0}'", underlayDate);
                else
                    where.AppendFormat(" WHERE UnderlayDate = '{0}'", underlayDate);
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
                    where.AppendFormat(" AND InvoiceUnderlays.EmployeeId = {0}", employeeId);
                else
                    where.AppendFormat(" WHERE InvoiceUnderlays.EmployeeId = {0}", employeeId);
            }
            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());
            sql.Append(" ORDER BY UnderlayDate DESC");
            sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, size);
            return await _context.InvoiceUnderlaysView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<int> GetNumberOfFilteredUnderlays(string caseId, string underlayNumber, string underlayDate, string caseTypeId, string employeeId, string firstName, string lastName)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(InvoiceUnderlays.InvoiceUnderlayId) AS NumberOf");
            sql.Append(" FROM InvoiceUnderlays INNER JOIN Cases ON InvoiceUnderlays.CaseId = Cases.CaseId INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" Persons ON InvoiceUnderlays.PersonId = Persons.PersonId INNER JOIN Employees ON InvoiceUnderlays.EmployeeId = Employees.EmployeeId");
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
                    where.AppendFormat(" AND InvoiceUnderlays.CaseId = {0}", caseId);
                else
                    where.AppendFormat(" WHERE InvoiceUnderlays.CaseId = {0}", caseId);
            }
            if (underlayNumber != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND UnderlayNumber LIKE '{0}'", underlayNumber);
                else
                    where.AppendFormat(" WHERE UnderlayNumber LIKE '{0}'", underlayNumber);
            }
            if (underlayDate != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND UnderlayDate = '{0}'", underlayDate);
                else
                    where.AppendFormat(" WHERE UnderlayDate = '{0}'", underlayDate);
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
                    where.AppendFormat(" AND InvoiceUnderlays.EmployeeId = {0}", employeeId);
                else
                    where.AppendFormat(" WHERE InvoiceUnderlays.EmployeeId = {0}", employeeId);
            }
            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf;
        }

        public async Task<InvoiceUnderlaysViewModel> GetUnderlay(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT InvoiceUnderlayId, InvoiceUnderlays.Title, UnderlayNumber, InvoiceUnderlays.PersonId, InvoiceUnderlays.CaseId, InvoiceUnderlays.PersonAddressId, InvoiceUnderlays.EmployeeId, InvoiceUnderlays.EmployeeTitle, InvoiceUnderlays.SignatureLink, UnderlayDate, UnderlayPlace, StreetNumber, PostalCode, PostalAddress, Country, Headline1, Headline2, WorkingReport, Vat, [Sum], Locked, InvoiceUnderlays.Changed, InvoiceUnderlays.ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', ReceiverNameCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS ReceiverName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', InvoiceUnderlays.CareOfCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', StreetNameCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS StreetName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.FirstNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', Persons.LastNameCry, 1, CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(" ,Initials, CaseType");
            sql.Append(" FROM InvoiceUnderlays INNER JOIN Cases ON InvoiceUnderlays.CaseId = Cases.CaseId INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" Persons ON InvoiceUnderlays.PersonId = Persons.PersonId INNER JOIN Employees ON InvoiceUnderlays.EmployeeId = Employees.EmployeeId");
            sql.AppendFormat(" WHERE InvoiceUnderlayId = {0}", id);
            return await _context.InvoiceUnderlaysView.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<InvoiceUnderlaysModel> GetUnderlay2(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT InvoiceUnderlayId, InvoiceUnderlays.Title, UnderlayNumber, InvoiceUnderlays.PersonId, InvoiceUnderlays.CaseId, InvoiceUnderlays.PersonAddressId, InvoiceUnderlays.EmployeeId, InvoiceUnderlays.EmployeeTitle, InvoiceUnderlays.SignatureLink, UnderlayDate, UnderlayPlace, StreetNumber, PostalCode, PostalAddress, Country, Headline1, Headline2, WorkingReport, Vat, [Sum], Locked, InvoiceUnderlays.Changed, InvoiceUnderlays.ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', ReceiverNameCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS ReceiverName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', InvoiceUnderlays.CareOfCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', StreetNameCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS StreetName", _cryKey);
            sql.Append(" FROM InvoiceUnderlays ");
            sql.AppendFormat(" WHERE InvoiceUnderlayId = {0}", id);
            return await _context.InvoiceUnderlays.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<IEnumerable<InvoiceUnderlaySummarysModel>> GetUnderlaySummaries(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT TariffTypes.TariffType, InvoiceWorkingTimes.UnitCost, Sum(InvoiceWorkingTimes.Sum) AS SumCosts, Sum(InvoiceWorkingTimes.NumberOfHours) AS SumHours, Max(TariffTypes.BackgroundColor) AS BackgroundColor");
            sql.Append(" FROM (TariffTypes INNER JOIN WorkingTimes ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId) INNER JOIN InvoiceWorkingTimes ON WorkingTimes.WorkingTimeId = InvoiceWorkingTimes.WorkingTimeId");
            sql.AppendFormat(" WHERE TariffTypes.NoLevel = 0 AND InvoiceWorkingTimes.InvoiceUnderlayId = {0}", id);
            sql.Append(" GROUP BY TariffTypes.TariffType, InvoiceWorkingTimes.UnitCost");
            sql.Append(" UNION");
            sql.Append(" SELECT TariffTypes.TariffType, null AS 'UnitCost',  Sum(InvoiceWorkingTimes.Sum) AS SumCosts, Sum(InvoiceWorkingTimes.NumberOfHours) AS SumHours, Max(TariffTypes.BackgroundColor) AS BackgroundColor");
            sql.Append(" FROM (TariffTypes INNER JOIN WorkingTimes ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId) INNER JOIN InvoiceWorkingTimes ON WorkingTimes.WorkingTimeId = InvoiceWorkingTimes.WorkingTimeId");
            sql.AppendFormat(" WHERE TariffTypes.NoLevel = 1 AND InvoiceWorkingTimes.InvoiceUnderlayId = {0}", id);
            sql.Append(" GROUP BY TariffTypes.TariffType");
            sql.Append(" ORDER BY TariffTypes.TariffType");
            return await _context.InvoiceUnderlaySummaries.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<int> GetNumberOfUnderlays4Case(int caseIdd)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(InvoiceUnderlayId) AS NumberOf");
            sql.Append(" FROM InvoiceUnderlays ");
            sql.AppendFormat(" WHERE CaseId = {0}", caseIdd);
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf;
        }

        public async Task<TotalSumModel> GetUnderlaySummariesTotalSum(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(InvoiceWorkingTimes.InvoiceWorkingTimeId) AS NumberOf");
            sql.Append(" FROM WorkingTimes INNER JOIN InvoiceWorkingTimes ON WorkingTimes.WorkingTimeId = InvoiceWorkingTimes.WorkingTimeId");
            sql.AppendFormat(" WHERE InvoiceWorkingTimes.InvoiceUnderlayId = {0}", id);
            NumberOfModel numberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            if (numberOf != null)
            {
                if (numberOf.NumberOf == 0)
                {
                    TotalSumModel totalSumModel = new TotalSumModel();
                    totalSumModel.TotalSum = 0;
                    return totalSumModel;
                }
            }
            sql = new StringBuilder("SELECT Sum(InvoiceWorkingTimes.Sum) AS TotalSum");
            sql.Append(" FROM WorkingTimes INNER JOIN InvoiceWorkingTimes ON WorkingTimes.WorkingTimeId = InvoiceWorkingTimes.WorkingTimeId");
            sql.AppendFormat(" WHERE InvoiceWorkingTimes.InvoiceUnderlayId = {0}", id);
            return await _context.UnderlaySummariesTotalSum.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<IEnumerable<InvoiceUnderlaysPartialViewModel>> GetUnderlayForCase(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT InvoiceUnderlayId, InvoiceUnderlays.CaseId, CaseTypes.CaseType, UnderlayNumber, UnderlayDate, Locked, InvoiceUnderlays.Changed, InvoiceUnderlays.ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', ReceiverNameCry, 1, CONVERT(varbinary, InvoiceUnderlayId))) AS ReceiverName", _cryKey);
            sql.Append(" FROM InvoiceUnderlays INNER JOIN Cases ON InvoiceUnderlays.CaseId = Cases.CaseId INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId");
            sql.AppendFormat(" WHERE InvoiceUnderlays.CaseId = {0}", id);
            return await _context.InvoiceUnderlaysPartialView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<InvoiceUnderlaysModel> CreateUnderlay(int caseId, int employeeId, int courtId, int personId, string underlayPlace, DateTime underlayDate, int vat, string userName = "")
        {
            string headline1 = "";
            ConfigurationsModel configuration = await _configurationConnection.GetConfigurationWithkey("KostnadsRakningText1");
            if (configuration != null)
                headline1 = configuration.ConfigValue;

            string headline2 = "";
            configuration = await _configurationConnection.GetConfigurationWithkey("KostnadsRakningText2");
            if (configuration != null)
                headline2 = configuration.ConfigValue;

            string employeeTitle = "";
            string signatureLink = "";
            EmployeesModel employee = await _employeesConnection.GetEmployee(employeeId);
            if (employee != null)
            {
                employeeTitle = employee.EmployeeTitle;
                signatureLink = employee.SignatureLink;
            }

            string receiverName = "";
            int personAddressId = 0;
            string careOf = "";
            string streetName = "";
            string streetNumber = "";
            string postalCode = "";
            string postalAddress = "";
            string country = "";

            if (personId != 0)
            {
                PersonsModel person = await _personsConnection.GetPerson(personId);
                if (person != null)
                {
                    receiverName = person.FirstName + " " + person.LastName;
                }
                PersonAddressesModel address = await _personsConnection.GetValidAddressesForPerson(personId);
                if (address != null)
                {
                    personAddressId = address.PersonAddressId;
                    careOf = address.CareOf;
                    streetName = address.StreetName;
                    streetNumber = address.StreetNumber;
                    postalCode = address.PostalCode;
                    postalAddress = address.PostalAddress;
                    country = address.Country;
                }
            }

            if (courtId != 0)
            {
                CourtsModel court = await _courtsConnection.GetCourt(courtId);
                if (court != null)
                {
                    receiverName = court.CourtName;
                    streetName = court.StreetName;
                    streetNumber = court.StreetNumber;
                    postalCode = court.PostalCode;
                    postalAddress = court.PostalAddress;
                    country = court.Country;
                }
            }

            PersonsAtCaseViewModel primaryClient = await _casesConnection.GetClientAtCase(caseId);
            if (primaryClient == null)
                return null;

            InvoiceUnderlaysModel underlay = new InvoiceUnderlaysModel
            {
                InvoiceUnderlayId = 0,
                CaseId = caseId,
                PersonId = primaryClient.PersonId,
                PersonAddressId = personAddressId,
                EmployeeId = employeeId,
                EmployeeTitle = employeeTitle,
                Title = "Underlag",
                SignatureLink = signatureLink,
                UnderlayPlace = underlayPlace,
                ReceiverName = receiverName,
                CareOf = careOf,
                StreetName = streetName,
                StreetNumber = streetNumber,
                PostalCode = postalCode,
                PostalAddress = postalAddress,
                Headline1 = headline1,
                Headline2 = headline2,
                WorkingReport = "",
                Vat = vat,
                Sum = 0,
                Locked = false,
                UnderlayDate = underlayDate,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.InvoiceUnderlays.Add(underlay);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return null;

            InvoiceUnderlaysModel newUnderlay = await _context.InvoiceUnderlays.Where(c => c.CaseId == caseId).OrderByDescending(s => s.InvoiceUnderlayId).FirstAsync();
            if (newUnderlay == null)
                return null;

            string underlayNumber = primaryClient.PersonId.ToString() + "-" + caseId.ToString() + "-" + newUnderlay.InvoiceUnderlayId.ToString(); //TODO: Remove InvoiceUnderlayId?

            StringBuilder sql = new StringBuilder("DECLARE @ReceiverName nvarchar(512); ");
            sql.Append("DECLARE @CareOf nvarchar(512); ");
            sql.Append("DECLARE @StreetName nvarchar(256); ");
            sql.AppendFormat("SET @ReceiverName = '{0}'; ", receiverName);
            sql.AppendFormat("SET @CareOf = '{0}'; ", careOf);
            sql.AppendFormat("SET @StreetName = '{0}'; ", streetName);
            sql.Append(" UPDATE InvoiceUnderlays SET ");
            if (receiverName == "")
                sql.Append(" ReceiverNameCry = Null");
            else
                sql.AppendFormat(" ReceiverNameCry = EncryptByPassPhrase('{0}', @ReceiverName, 1, CONVERT( varbinary, InvoiceUnderlayId))", _cryKey);
            if (careOf == "")
                sql.Append(", CareOfCry = Null");
            else
                sql.AppendFormat(", CareOfCry = EncryptByPassPhrase('{0}', @CareOf, 1, CONVERT( varbinary, InvoiceUnderlayId))", _cryKey);
            if (streetName == "")
                sql.Append(", StreetNameCry = Null");
            else
                sql.AppendFormat(", StreetNameCry = EncryptByPassPhrase('{0}', @StreetName, 1, CONVERT( varbinary, InvoiceUnderlayId))", _cryKey);
            sql.Append(", ReceiverName = Null");
            sql.Append(", CareOf = Null");
            sql.Append(", StreetName = Null");
            sql.AppendFormat(", UnderlayNumber = '{0}'", underlayNumber);
            sql.AppendFormat(" WHERE InvoiceUnderlayId = {0}", newUnderlay.InvoiceUnderlayId);
            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            return underlay;
        }

        public async Task<InvoiceUnderlaysModel> UpdateUnderlay(int id, int employeeId, string title, DateTime underlayDate, string underlayPlace, string headline1, string headline2, string workingReport, bool locked, InvoiceUnderlaysModel currentUnderlay, string userName = "")
        {
            EmployeesModel employee = await _employeesConnection.GetEmployee(employeeId);
            InvoiceUnderlaysModel underlay = new InvoiceUnderlaysModel
            {
                InvoiceUnderlayId = id,
                CaseId = currentUnderlay.CaseId,
                PersonId = currentUnderlay.PersonId,
                PersonAddressId = currentUnderlay.PersonAddressId,
                EmployeeId = employeeId,
                EmployeeTitle = employee.EmployeeTitle,
                SignatureLink = employee.SignatureLink,
                Title = title,
                UnderlayNumber = currentUnderlay.UnderlayNumber,
                UnderlayDate = underlayDate,
                UnderlayPlace = underlayPlace,
                ReceiverName = currentUnderlay.ReceiverName,
                StreetNumber = currentUnderlay.StreetNumber,
                PostalCode = currentUnderlay.PostalCode,
                PostalAddress = currentUnderlay.PostalAddress,
                Country = currentUnderlay.Country,
                Headline1 = headline1,
                Headline2 = headline2,
                WorkingReport = workingReport,
                Vat = currentUnderlay.Vat,
                Sum = currentUnderlay.Sum,
                Locked = locked,
                Changed = DateTime.Now,
                ChangedBy = userName
            };
            _context.Entry(underlay).State = EntityState.Modified;
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return null;

            StringBuilder sql = new StringBuilder("DECLARE @ReceiverName nvarchar(512); ");
            sql.AppendFormat("SET @ReceiverName = '{0}'; ", currentUnderlay.ReceiverName);
            sql.Append("DECLARE @CareOf nvarchar(255); ");
            sql.AppendFormat("SET @CareOf = '{0}'; ", currentUnderlay.CareOf);
            sql.Append("DECLARE @StreetName nvarchar(256); ");
            sql.AppendFormat("SET @StreetName = '{0}'; ", currentUnderlay.StreetName);
            sql.Append(" UPDATE InvoiceUnderlays SET ");
            if (string.IsNullOrEmpty(currentUnderlay.ReceiverName))
                sql.Append(" ReceiverNameCry = Null");
            else
                sql.AppendFormat(" ReceiverNameCry = EncryptByPassPhrase('{0}', @ReceiverName, 1, CONVERT( varbinary, InvoiceUnderlayId))", _cryKey);
            if (string.IsNullOrEmpty(currentUnderlay.CareOf))
                sql.Append(", CareOfCry = Null");
            else
                sql.AppendFormat(", CareOfCry = EncryptByPassPhrase('{0}', @CareOf, 1, CONVERT( varbinary, InvoiceUnderlayId))", _cryKey);
            if (string.IsNullOrEmpty(currentUnderlay.StreetName))
                sql.Append(", StreetNameCry = Null");
            else
                sql.AppendFormat(", StreetNameCry = EncryptByPassPhrase('{0}', @StreetName, 1, CONVERT( varbinary, InvoiceUnderlayId))", _cryKey);
            sql.Append(", ReceiverName = Null");
            sql.Append(", CareOf = Null");
            sql.Append(", StreetName = Null");
            sql.AppendFormat(" WHERE InvoiceUnderlayId = {0}", id);
            int numberOfUpdated = await _context.Database.ExecuteSqlRawAsync(sql.ToString());
            if (numberOfUpdated != 1)
            {
                return null;
            }

            return await GetUnderlay2(id);
        }

        public async Task<bool> MoveAllWorkingTimes(int caseId, int invoiceUnderlayId, string userName = "")
        {
            IEnumerable<WorkingTimesViewModel> workingTimes = await _workingTimesConnection.GetWorkingTimesForCaseNotBilled(caseId);
            bool result = true;
            foreach (var workingTime in workingTimes)
            {
                double cost = 0;
                double sum = 0;
                if (workingTime.NoLevel)
                {
                    //This working time have no tariff level
                    cost = workingTime.Cost.Value;
                }
                else
                {
                    //Get the latest valid tariff level
                    TariffLevelsModel tariffLevel = await _context.TariffLevels.Where(t => t.TariffTypeId == workingTime.TariffTypeId).Where(v => v.Valid == true).OrderByDescending(i => i.TariffLevelId).FirstAsync();
                    cost = tariffLevel.TariffLevel;
                }
                sum = cost * workingTime.NumberOfHours;

                InvoiceWorkingTimesModel invoiceWorkingTime = new InvoiceWorkingTimesModel
                {
                    InvoiceWorkingTimeId = 0,
                    InvoiceUnderlayId = invoiceUnderlayId,
                    WorkingTimeId = workingTime.WorkingTimeId,
                    UnitCost = cost,
                    NumberOfHours = workingTime.NumberOfHours,
                    Sum = sum,
                    Changed = DateTime.Now,
                    ChangedBy = userName
                };
                _context.InvoiceWorkingTimes.Add(invoiceWorkingTime);
                int numberOfSaves = await _context.SaveChangesAsync();
                if (numberOfSaves != 1)
                {
                    result = false;
                    break;
                }

                StringBuilder sql = new StringBuilder("UPDATE WorkingTimes SET ");
                sql.Append(" Billed = 1");
                sql.AppendFormat(" WHERE WorkingTimeId = {0}", workingTime.WorkingTimeId);
                int update = await _context.Database.ExecuteSqlRawAsync(sql.ToString());
                if (numberOfSaves != 1)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public async Task<bool> MoveWorkingTime(int workingTimeId, int invoiceUnderlayId, string userName = "")
        {
            WorkingTimesViewModel workingTime = await _workingTimesConnection.GetWorkingTimeNotBilled(workingTimeId);
            double cost = 0;
            double sum = 0;
            if (workingTime.NoLevel)
            {
                //This working time have no tariff level
                cost = workingTime.Cost.Value;
            }
            else
            {
                //Get the latest valid tariff level
                TariffLevelsModel tariffLevel = await _context.TariffLevels.Where(t => t.TariffTypeId == workingTime.TariffTypeId).Where(v => v.Valid == true).OrderByDescending(i => i.TariffLevelId).FirstAsync();
                cost = tariffLevel.TariffLevel;
            }
            sum = cost * workingTime.NumberOfHours;

            InvoiceWorkingTimesModel invoiceWorkingTime = new InvoiceWorkingTimesModel
            {
                InvoiceWorkingTimeId = 0,
                InvoiceUnderlayId = invoiceUnderlayId,
                WorkingTimeId = workingTime.WorkingTimeId,
                UnitCost = cost,
                NumberOfHours = workingTime.NumberOfHours,
                Sum = sum,
                Changed = DateTime.Now,
                ChangedBy = userName
            };
            _context.InvoiceWorkingTimes.Add(invoiceWorkingTime);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
            {
                return false;
            }

            StringBuilder sql = new StringBuilder("UPDATE WorkingTimes SET ");
            sql.Append(" Billed = 1");
            sql.AppendFormat(" WHERE WorkingTimeId = {0}", workingTime.WorkingTimeId);
            int numberOfUpdated = await _context.Database.ExecuteSqlRawAsync(sql.ToString());
            if (numberOfUpdated != 1)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> RemoveWorkingTime(int invoiceWorkingTimeId)
        {
            InvoiceWorkingTimesViewModel invoiceWorkingTime = await GetUnderlayWorkingTime(invoiceWorkingTimeId);
            if (invoiceWorkingTime == null)
                return false;

            StringBuilder sql = new StringBuilder("UPDATE WorkingTimes SET ");
            sql.Append(" Billed = 0");
            sql.AppendFormat(" WHERE WorkingTimeId = {0}", invoiceWorkingTime.WorkingTimeId);
            int numberOfUpdated = await _context.Database.ExecuteSqlRawAsync(sql.ToString());
            if (numberOfUpdated != 1)
            {
                return false;
            }

            sql = new StringBuilder("DELETE FROM InvoiceWorkingTimes");
            sql.AppendFormat(" WHERE InvoiceWorkingTimeId = {0}", invoiceWorkingTimeId);
            int numberOfDeleted = await _context.Database.ExecuteSqlRawAsync(sql.ToString());
            if (numberOfDeleted != 1)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> RemoveAllWorkingTimes(int invoiceUnderlayId)
        {
            IEnumerable<InvoiceWorkingTimesViewModel> invoiceWorkingTimes = await GetUnderlayWorkingTimes(invoiceUnderlayId);
            if (invoiceWorkingTimes.Count() == 0)
                return false;
            bool result = true;
            foreach (var invoiceWorkingTime in invoiceWorkingTimes)
            {
                StringBuilder sql = new StringBuilder("UPDATE WorkingTimes SET ");
                sql.Append(" Billed = 0");
                sql.AppendFormat(" WHERE WorkingTimeId = {0}", invoiceWorkingTime.WorkingTimeId);
                int numberOfUpdated = await _context.Database.ExecuteSqlRawAsync(sql.ToString());
                if (numberOfUpdated != 1)
                {
                    result = false;
                    break;
                }
                sql = new StringBuilder("DELETE FROM InvoiceWorkingTimes");
                sql.AppendFormat(" WHERE InvoiceWorkingTimeId = {0}", invoiceWorkingTime.InvoiceWorkingTimeId);
                int numberOfDeleted = await _context.Database.ExecuteSqlRawAsync(sql.ToString());
                if (numberOfDeleted != 1)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public async Task<IEnumerable<UnderlayWorkingTimeSummaryModel>> GetUnderlayWorkingTimeSummary(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT TariffTypes.TariffTypeId, WorkingTimes.TariffLevel, InvoiceWorkingTimes.UnitCost, Sum(InvoiceWorkingTimes.Sum) AS SumCosts, Sum(InvoiceWorkingTimes.NumberOfHours) AS SumHours");
            sql.Append(" FROM TariffTypes INNER JOIN");
            sql.Append(" WorkingTimes ON TariffTypes.TariffTypeId = WorkingTimes.TariffTypeId INNER JOIN");
            sql.Append(" InvoiceWorkingTimes ON WorkingTimes.WorkingTimeId = InvoiceWorkingTimes.WorkingTimeId");
            sql.AppendFormat(" WHERE InvoiceWorkingTimes.InvoiceUnderlayId = {0}", id);
            sql.Append(" GROUP BY TariffTypes.TariffTypeId, WorkingTimes.TariffLevel, InvoiceWorkingTimes.UnitCost");
            return await _context.UnderlayWorkingTimeSummary.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<bool> SetLocked(int id)
        {
            StringBuilder sql = new StringBuilder("UPDATE InvoiceUnderlays SET ");
            sql.Append(" Locked =1");
            sql.AppendFormat(" WHERE InvoiceUnderlayId = {0}", id);
            int numberOfUpdated = await _context.Database.ExecuteSqlRawAsync(sql.ToString());
            if (numberOfUpdated != 1)
            {
                return false;
            }
            return true;
        }
    }
}
