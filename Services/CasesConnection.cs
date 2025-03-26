using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrojWebApp.Models;


namespace TrojWebApp.Services
{
    public class CasesConnection
    {
        private readonly TrojContext _context;
        private readonly string _cryKey;

        public CasesConnection(TrojContext context, string crykey)
        {
            _context = context;
            _cryKey = crykey;
        }

        public async Task<IEnumerable<CasesViewModel>> GetCases(int offset, int size)
        {
            StringBuilder sql = new StringBuilder("SELECT Cases.CaseId, Cases.CaseTypeId, CaseTypes.CaseType, CaseDate, Cases.Responsible, Cases.Active, Cases.Secrecy, FinishedDate, Cases.Changed, Cases.ChangedBy, PersonCases.PersonId, PersonCases.PersonCaseId");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', TitleCry, 1, CONVERT(varbinary, Cases.CaseId))) AS Title", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(4000), DecryptByPassphrase('{0}', CommentCry, 1, CONVERT(varbinary, Cases.CaseId))) AS Comment", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(" FROM Cases INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN PersonCases ON Cases.CaseId = PersonCases.CaseId INNER JOIN Persons ON PersonCases.PersonId = Persons.PersonId");
            sql.Append(" WHERE PersonCases.Responsible = 1");
            sql.Append(" ORDER BY Cases.CaseDate DESC");
            sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, size);
            return await _context.CasesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<int> GetNumberOfCases()
        {
            StringBuilder sql = new StringBuilder("SELECT Count(Cases.CaseId) AS NumberOf");
            sql.Append(" FROM Cases INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN PersonCases ON Cases.CaseId = PersonCases.CaseId INNER JOIN Persons ON PersonCases.PersonId = Persons.PersonId");
            sql.Append(" WHERE PersonCases.Responsible = 1");
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf;
        }

        public async Task<IEnumerable<CaseNumbersViewModel>> GetCaseNumbers(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT CaseNumbers.CaseNumberId, CaseNumbers.CaseId, CaseNumbers.CourtId, CaseNumbers.Changed, CaseNumbers.ChangedBy, Courts.CourtName");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', CaseNumberCry, 1, CONVERT(varbinary, CaseNumbers.CaseNumberId))) AS CaseNumber", _cryKey);
            sql.Append(" FROM CaseNumbers INNER JOIN Courts ON CaseNumbers.CourtId = Courts.CourtId");
            sql.AppendFormat(" WHERE CaseNumbers.CaseId = {0}", id);
            sql.AppendFormat(" ORDER BY CONVERT(nvarchar(256), DecryptByPassphrase('{0}', CaseNumberCry, 1, CONVERT(varbinary, CaseNumbers.CaseNumberId)))", _cryKey);
            return await _context.CaseNumbersView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<CaseLogsViewModel>> GetCaseLogs(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT CaseLogId, CaseId, FORMAT(CaseLogs.WhenDate,'yyyy-MM-dd') AS WhenDate, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', CommentCry, 1, CONVERT(varbinary, CaseLogId))) AS Comment", _cryKey);
            sql.Append(" FROM CaseLogs");
            sql.AppendFormat(" WHERE CaseId = {0}", id);
            sql.Append(" ORDER BY WhenDate DESC, Changed DESC");
            return await _context.CaseLogsView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<ClientFundingsViewModel>> GetClientFunding(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT ClientFundId, CaseId, FORMAT(ClientFundings.ClientFundDate,'yyyy-MM-dd') AS ClientFundDate,ClientSum, Comment, Changed, ChangedBy");
            sql.Append(" FROM ClientFundings");
            sql.AppendFormat(" WHERE CaseId = {0}", id);
            sql.Append(" ORDER BY ClientFundDate DESC, Changed DESC");
            return await _context.ClientFundingsView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<PersonsAtCaseViewModel>> GetOtherPersonsAtCase(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonCases.PersonCaseId, PersonCases.PersonId, PersonCases.CaseId, PersonCases.PersonTypeId, PersonCases.Responsible, PersonCases.Changed, PersonCases.ChangedBy, PersonTypes.PersonType, CaseTypes.CaseType, Cases.CaseDate, Cases.Active");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(" FROM PersonCases INNER JOIN PersonTypes ON PersonCases.PersonTypeId = PersonTypes.PersonTypeId INNER JOIN Persons ON PersonCases.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" Cases ON PersonCases.CaseId = Cases.CaseId INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId");
            sql.AppendFormat(" WHERE PersonCases.CaseId = {0}", id);
            sql.Append(" AND PersonCases.Responsible = 0");
            sql.Append(" ORDER BY LastName, FirstName");
            return await _context.PersonAtCasesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }
        public async Task<PersonsAtCaseViewModel> GetClientAtCase(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonCases.PersonCaseId, PersonCases.PersonId, PersonCases.CaseId, PersonCases.PersonTypeId, PersonCases.Responsible, PersonCases.Changed, PersonCases.ChangedBy, PersonTypes.PersonType, CaseTypes.CaseType, Cases.CaseDate, Cases.Active");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(" FROM PersonCases INNER JOIN PersonTypes ON PersonCases.PersonTypeId = PersonTypes.PersonTypeId INNER JOIN Persons ON PersonCases.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" Cases ON PersonCases.CaseId = Cases.CaseId INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId");
            sql.AppendFormat(" WHERE PersonCases.CaseId = {0}", id);
            sql.Append(" AND PersonCases.Responsible = 1");
            return await _context.PersonAtCasesView.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<IEnumerable<PersonsAtCaseViewModel>> GetAllPersonsAtCase(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonCases.PersonCaseId, PersonCases.PersonId, PersonCases.CaseId, PersonCases.PersonTypeId, PersonCases.Responsible, PersonCases.Changed, PersonCases.ChangedBy, PersonTypes.PersonType, CaseTypes.CaseType, Cases.CaseDate, Cases.Active");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(" FROM PersonCases INNER JOIN PersonTypes ON PersonCases.PersonTypeId = PersonTypes.PersonTypeId INNER JOIN Persons ON PersonCases.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" Cases ON PersonCases.CaseId = Cases.CaseId INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId");
            sql.AppendFormat(" WHERE PersonCases.CaseId = {0}", id);
            sql.Append(" ORDER BY LastName, FirstName");
            return await _context.PersonAtCasesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<CourtsModel>> GetAllCourtsAtCase(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT Courts.*");
            sql.Append(" FROM CaseNumbers INNER JOIN Courts ON CaseNumbers.CourtId = Courts.CourtId");
            sql.AppendFormat(" WHERE CaseNumbers.CaseId = {0}", id);
            sql.Append(" ORDER BY CourtName");
            return await _context.Courts.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<CasesClientViewModel>> GetCasesAndClient()
        {
            StringBuilder sql = new StringBuilder("SELECT Cases.CaseId, CaseTypes.CaseType,  Persons.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(" FROM Cases INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN PersonCases ON Cases.CaseId = PersonCases.CaseId INNER JOIN  Persons ON PersonCases.PersonId = Persons.PersonId");
            sql.Append(" WHERE (Cases.Active = 1) AND (PersonCases.Responsible = 1)");
            sql.Append(" ORDER BY LastName, FirstName, Cases.CaseDate DESC");
            return await _context.CasesClientView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<CasesClientViewModel>> GetFilteredCasesAndClient(string caseId, string caseTypeId, string title, string employeeId, string firstName, string lastName)
        {
            StringBuilder sql = new StringBuilder("SELECT Cases.CaseId, CaseTypes.CaseType,  Persons.PersonId");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(" FROM Cases INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN PersonCases ON Cases.CaseId = PersonCases.CaseId INNER JOIN  Persons ON PersonCases.PersonId = Persons.PersonId INNER JOIN Employees ON Cases.Responsible = Employees.Initials");
            sql.Append(" WHERE Cases.Active = 1 AND PersonCases.Responsible = 1");
            if (caseId != "")
                sql.AppendFormat(" AND Cases.CaseId = {0}", caseId);
            if (caseTypeId != "" && caseTypeId != "0")
                sql.AppendFormat(" AND Cases.CaseTypeId = {0}", caseTypeId);
            if (title != "")
                sql.AppendFormat(" AND CONVERT(nvarchar(512), DecryptByPassphrase('{0}', TitleCry, 1, CONVERT(varbinary, Cases.CaseId))) LIKE '{1}'", _cryKey, title);
            if (employeeId != "" && employeeId != "0")
                sql.AppendFormat(" AND Employees.EmployeeId = {0}", employeeId);
            if (firstName != "")
                sql.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, firstName);
            if (lastName != "")
                sql.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
            sql.Append(" ORDER BY LastName, FirstName, Cases.CaseDate DESC");
            return await _context.CasesClientView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<CasesViewModel> GetCase(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT Cases.CaseId, Cases.CaseTypeId, CaseTypes.CaseType, CaseDate, Cases.Responsible, Cases.Active, Cases.Secrecy, FinishedDate, Cases.Changed, Cases.ChangedBy, PersonCases.PersonId, PersonCases.PersonCaseId");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', TitleCry, 1, CONVERT(varbinary, Cases.CaseId))) AS Title", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(4000), DecryptByPassphrase('{0}', CommentCry, 1, CONVERT(varbinary, Cases.CaseId))) AS Comment", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(" FROM Cases INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN PersonCases ON Cases.CaseId = PersonCases.CaseId INNER JOIN Persons ON PersonCases.PersonId = Persons.PersonId");
            sql.AppendFormat(" WHERE Cases.CaseId = {0}", id);
            sql.Append(" AND PersonCases.Responsible = 1");
            return await _context.CasesView.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<IEnumerable<CasesViewModel>> GetFilteredCases(string caseId, string whenDate, string caseTypeId, string title, string initials, string firstName, string lastName, int offset, int size)
        {
            StringBuilder sql = new StringBuilder("SELECT Cases.CaseId, Cases.CaseTypeId, CaseTypes.CaseType, CaseDate, Cases.Responsible, Cases.Active, Cases.Secrecy, FinishedDate, Cases.Changed, Cases.ChangedBy, PersonCases.PersonId, PersonCases.PersonCaseId");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', TitleCry, 1, CONVERT(varbinary, Cases.CaseId))) AS Title", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(4000), DecryptByPassphrase('{0}', CommentCry, 1, CONVERT(varbinary, Cases.CaseId))) AS Comment", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(" FROM Cases INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN PersonCases ON Cases.CaseId = PersonCases.CaseId INNER JOIN Persons ON PersonCases.PersonId = Persons.PersonId");
            sql.Append(" WHERE PersonCases.Responsible = 1");
            if (caseId != "")
                sql.AppendFormat(" AND Cases.CaseId = {0}", caseId);
            if (whenDate != "")
                sql.AppendFormat(" AND Cases.CaseDate > '{0}'", whenDate);
            if (caseTypeId != "" && caseTypeId != "0")
                sql.AppendFormat(" AND Cases.CaseTypeId = {0}", caseTypeId);
            if (title != "")
                sql.AppendFormat(" AND CONVERT(nvarchar(512), DecryptByPassphrase('{0}', TitleCry, 1, CONVERT(varbinary, Cases.CaseId))) LIKE '{1}'", _cryKey, title);
            if (initials != "")
                sql.AppendFormat(" AND Cases.Responsible = '{0}'", initials);
            if (firstName != "")
                sql.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, firstName);
            if (lastName != "")
                sql.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
            sql.Append(" ORDER BY Cases.CaseDate DESC");
            sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, size);
            return await _context.CasesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<int> GetNumberOfFilteredCases(string caseId, string whenDate, string caseTypeId, string title, string initials, string firstName, string lastName)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(Cases.CaseId) AS NumberOf");
            sql.Append(" FROM Cases INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN PersonCases ON Cases.CaseId = PersonCases.CaseId INNER JOIN Persons ON PersonCases.PersonId = Persons.PersonId");
            sql.Append(" WHERE PersonCases.Responsible = 1");
            if (caseId != "")
                sql.AppendFormat(" AND Cases.CaseId = {0}", caseId);
            if (whenDate != "")
                sql.AppendFormat(" AND Cases.CaseDate > '{0}'", whenDate);
            if (caseTypeId != "" && caseTypeId != "0")
                sql.AppendFormat(" AND Cases.CaseTypeId = {0}", caseTypeId);
            if (title != "")
                sql.AppendFormat(" AND CONVERT(nvarchar(512), DecryptByPassphrase('{0}', TitleCry, 1, CONVERT(varbinary, Cases.CaseId))) LIKE '{1}'", _cryKey, title);
            if (initials != "")
                sql.AppendFormat(" AND Cases.Responsible = '{0}'", initials);
            if (firstName != "")
                sql.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, firstName);
            if (lastName != "")
                sql.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf;
        }

        public async Task<IEnumerable<ActiveCasesWorkingTimesViewModel>> GetActiveCases(string userName)
        {
            StringBuilder sql = new StringBuilder("SELECT Cases.CaseId, MAX(CaseTypes.CaseType) AS CaseType, MAX(WorkingTimes.Changed) AS Changed, MAX(Persons.PersonId) AS PersonId");
            sql.AppendFormat(", MAX(CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId)))) AS FirstName", _cryKey);
            sql.AppendFormat(", MAX(CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId)))) AS LastName", _cryKey);
            sql.Append(" FROM Cases INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" PersonCases ON Cases.CaseId = PersonCases.CaseId INNER JOIN");
            sql.Append(" Employees ON Cases.Responsible = Employees.Initials INNER JOIN");
            sql.Append(" Persons ON PersonCases.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" WorkingTimes ON Persons.PersonId = WorkingTimes.PersonId");
            sql.AppendFormat(" WHERE Employees.UserName3 = '{0}'", userName);
            sql.Append(" AND Cases.Active = 1");
            sql.Append(" AND PersonCases.Responsible = 1");
            sql.Append(" GROUP BY Cases.CaseId");
            sql.Append(" ORDER BY MAX(WorkingTimes.Changed) DESC");
            return await _context.ActiveCasesWorkingTimesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<ActiveCasesWorkingTimesViewModel>> GetActiveCasesWithWorkingtimes(string userName)
        {
            StringBuilder sql = new StringBuilder("SELECT Cases.CaseId, MAX(CaseTypes.CaseType) AS CaseType, MAX(WorkingTimes.Changed) AS Changed, MAX(Persons.PersonId) AS PersonId");
            sql.AppendFormat(", MAX(CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId)))) AS FirstName", _cryKey);
            sql.AppendFormat(", MAX(CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId)))) AS LastName", _cryKey);
            sql.Append(" FROM Cases INNER JOIN");
            sql.Append(" CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId INNER JOIN");
            sql.Append(" PersonCases ON Cases.CaseId = PersonCases.CaseId INNER JOIN");
            sql.Append(" Persons ON PersonCases.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" WorkingTimes ON Persons.PersonId = WorkingTimes.PersonId INNER JOIN");
            sql.Append(" Employees ON WorkingTimes.EmployeeId = Employees.EmployeeId");
            sql.AppendFormat(" WHERE Employees.UserName3 = '{0}'", userName);
            sql.Append(" AND Cases.Active = 1");
            sql.Append(" AND PersonCases.Responsible = 1");
            sql.Append(" GROUP BY Cases.CaseId");
            sql.Append(" ORDER BY MAX(WorkingTimes.Changed) DESC");
            return await _context.ActiveCasesWorkingTimesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<CasesViewModel> CreateCase(int caseTypeId, string title, string responsible, int personId, string userName)
        {
            CasesModel casesModel = new CasesModel
            {
                CaseId = 0,
                CaseTypeId = caseTypeId,
                Title = title,
                CaseDate = DateTime.Now,
                Responsible = responsible,
                Active = true,
                Secrecy = false,
                FinishedDate = null,
                Comment = "",
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Cases.Add(casesModel);
            int numberOfSaved = await _context.SaveChangesAsync();
            if (numberOfSaved != 1)
                return null;

            CasesModel newCase = await _context.Cases.Where(c => c.CaseTypeId == caseTypeId).Where(a => a.Active == true).OrderByDescending(s => s.CaseId).FirstAsync();
            if (newCase == null)
                return null;

            StringBuilder sql = new StringBuilder("DECLARE @Title nvarchar(512); ");
            sql.AppendFormat("SET @Title = '{0}'; ", title);
            sql.Append(" UPDATE Cases SET ");
            if (title == "")
                sql.Append(" TitleCry = Null");
            else
                sql.AppendFormat(" TitleCry = EncryptByPassPhrase('{0}', @Title, 1, CONVERT( varbinary, CaseId))", _cryKey);
            sql.Append(", Title = Null");
            sql.Append(", Comment = Null");
            sql.AppendFormat(" WHERE CaseId = {0}", newCase.CaseId);
            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            PersonCasesModel personCasesModel = new PersonCasesModel
            {
                PersonCaseId = 0,
                PersonId = personId,
                CaseId = newCase.CaseId,
                PersonTypeId = 1,
                Responsible = true,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.PersonCases.Add(personCasesModel);
            int numberOfSavedPersonsCases = await _context.SaveChangesAsync();
            if (numberOfSavedPersonsCases != 1)
                return null;

            return await GetCase(newCase.CaseId);
        }

        public async Task<CaseNumbersModel> CreateCaseNumber(int caseId, int courtId, string caseNumber, string userName)
        {
            CaseNumbersModel caseNumberModel = new CaseNumbersModel
            {
                CaseNumberId = 0,
                CaseId = caseId,
                CourtId = courtId,
                CaseNumber = caseNumber,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.CaseNumbers.Add(caseNumberModel);
            int numberOfSaved = await _context.SaveChangesAsync();
            if (numberOfSaved != 1)
                return null;

            CaseNumbersModel newCaseNumber = await _context.CaseNumbers.Where(c => c.CaseId == caseId).Where(a => a.CourtId == courtId).OrderByDescending(s => s.CaseNumberId).FirstAsync();
            if (newCaseNumber == null)
                return null;

            StringBuilder sql = new StringBuilder("DECLARE @CaseNumber nvarchar(256); ");
            sql.AppendFormat("SET @CaseNumber = '{0}'; ", caseNumber);
            sql.Append(" UPDATE CaseNumbers SET ");
            if (caseNumber == "")
                sql.Append(" CaseNumberCry = Null");
            else
                sql.AppendFormat(" CaseNumberCry = EncryptByPassPhrase('{0}', @CaseNumber, 1, CONVERT( varbinary, CaseNumberId))", _cryKey);
            sql.Append(", CaseNumber = Null");
            sql.AppendFormat(" WHERE CaseNumberId = {0}", newCaseNumber.CaseNumberId);
            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            return newCaseNumber;
        }

        public async Task<CaseLogsModel> CreateCaseLog(int caseId, DateTime whenDate, string comment, string userName)
        {
            CaseLogsModel caseLog = new CaseLogsModel
            {
                CaseLogId = 0,
                CaseId = caseId,
                WhenDate = whenDate,
                Comment = comment,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.CaseLogs.Add(caseLog);
            int numberOfSaved = await _context.SaveChangesAsync();
            if (numberOfSaved != 1)
                return null;

            CaseLogsModel newCaseLog = await _context.CaseLogs.Where(c => c.CaseId == caseId).OrderByDescending(s => s.CaseLogId).FirstAsync();
            if (newCaseLog == null)
                return null;

            StringBuilder sql = new StringBuilder("DECLARE @Comment nvarchar(2048); ");
            sql.AppendFormat("SET @Comment = '{0}'; ", comment);
            sql.Append(" UPDATE CaseLogs SET ");
            if (comment == "")
                sql.Append(" CommentCry = Null");
            else
                sql.AppendFormat(" CommentCry = EncryptByPassPhrase('{0}', @Comment, 1, CONVERT( varbinary, CaseLogId))", _cryKey);
            sql.Append(", Comment = Null");
            sql.AppendFormat(" WHERE CaseLogId = {0}", newCaseLog.CaseLogId);
            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            return newCaseLog;
        }

        public async Task<ClientFundingsModel> CreateClientFund(int caseId, double clientSum, DateTime clientFundDate, string comment, string userName)
        {
            ClientFundingsModel clientFunding = new ClientFundingsModel
            {
                ClientFundId = 0,
                CaseId = caseId,
                ClientSum = clientSum,
                ClientFundDate = clientFundDate,
                Comment = comment,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.ClientFundings.Add(clientFunding);
            int numberOfSaved = await _context.SaveChangesAsync();
            if (numberOfSaved != 1)
                return null;

            ClientFundingsModel newClientFunding = await _context.ClientFundings.Where(c => c.CaseId == caseId).OrderByDescending(s => s.ClientFundId).FirstAsync();
            if (newClientFunding == null)
                return null;

            return newClientFunding;
        }

        public async Task<int> CreatePersonCase(int personId, int caseId, int personTypeId, string userName)
        {
            PersonCasesModel personCasesModel = new PersonCasesModel
            {
                PersonCaseId = 0,
                PersonId = personId,
                CaseId = caseId,
                PersonTypeId = personTypeId,
                Responsible = false,
                Changed = DateTime.Now,
                ChangedBy = userName
            };
            _context.PersonCases.Add(personCasesModel);
            return await _context.SaveChangesAsync();
        }

        public async Task<CasesViewModel> UpdateCase(int id, int caseTypeId, string title, string responsible, bool active, bool secrecy, DateTime? finishedDate, string comment, int personId, CasesViewModel currentCase, string userName)
        {
            CasesModel casesModel = new CasesModel
            {
                CaseId = id,
                CaseTypeId = caseTypeId,
                Title = title,
                CaseDate = currentCase.CaseDate,
                Responsible = responsible,
                Active = active,
                Secrecy = secrecy,
                FinishedDate = finishedDate,
                Comment = comment,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Entry(casesModel).State = EntityState.Modified;
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return null;

            StringBuilder sql = new StringBuilder("DECLARE @Title nvarchar(512); ");
            sql.AppendFormat("SET @Title = '{0}'; ", title);
            sql.Append("DECLARE @Comment nvarchar(4000); ");
            sql.AppendFormat("SET @Comment = '{0}'; ", comment);
            sql.Append(" UPDATE Cases SET ");
            if (title == "")
                sql.Append(" TitleCry = Null");
            else
                sql.AppendFormat(" TitleCry = EncryptByPassPhrase('{0}', @Title, 1, CONVERT( varbinary, CaseId))", _cryKey);
            if (comment == "")
                sql.Append(", CommentCry = Null");
            else
                sql.AppendFormat(", CommentCry = EncryptByPassPhrase('{0}', @Comment, 1, CONVERT( varbinary, CaseId))", _cryKey);
            sql.Append(", Title = Null");
            sql.Append(", Comment = Null");
            sql.AppendFormat(" WHERE CaseId = {0}", id);
            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            PersonCasesModel personCasesModel = new PersonCasesModel
            {
                PersonCaseId = currentCase.PersonCaseId,
                PersonId = personId,
                CaseId = id,
                PersonTypeId = 1,
                Responsible = true,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Entry(personCasesModel).State = EntityState.Modified;
            int numberOfSavedPersonsCases = await _context.SaveChangesAsync();
            if (numberOfSavedPersonsCases != 1)
                return null;

            var personCases = await _context.PersonCases.Where(c => c.CaseId == id).Where(p => p.PersonId == personId).Where(r => r.Responsible == false).ToListAsync();
            if (personCases.Count > 0)
            {
                foreach (var personCase in personCases)
                {
                    await DeleteCasePerson(personCase.PersonCaseId);
                }
            }
            return await GetCase(id);
        }

        public async Task<CasesViewModel> ActivateCase(int id, string userName)
        {
            StringBuilder sql = new StringBuilder("UPDATE Cases SET");
            sql.Append(" Active = 1");
            sql.AppendFormat(" ,Changed = '{0}'", DateTime.Now);
            sql.AppendFormat(" ,ChangedBy = '{0}'", userName);
            sql.AppendFormat(" WHERE CaseId = {0}", id);
            await _context.Database.ExecuteSqlRawAsync(sql.ToString());
            return await GetCase(id);
        }

        public async Task<int> DeleteCaseNumber(int id)
        {
            StringBuilder sql = new StringBuilder("DELETE FROM CaseNumbers");
            sql.AppendFormat(" WHERE CaseNumberId = {0}", id);
            return await _context.Database.ExecuteSqlRawAsync(sql.ToString());
        }

        public async Task<int> DeleteCasePerson(int id)
        {
            StringBuilder sql = new StringBuilder("DELETE FROM PersonCases");
            sql.AppendFormat(" WHERE PersonCaseId = {0}", id);
            return await _context.Database.ExecuteSqlRawAsync(sql.ToString());
        }

        public async Task<int> DeleteCaseLog(int id)
        {
            StringBuilder sql = new StringBuilder("DELETE FROM CaseLogs");
            sql.AppendFormat(" WHERE CaseLogId = {0}", id);
            return await _context.Database.ExecuteSqlRawAsync(sql.ToString());
        }

        public async Task<int> DeleteClientFunding(int id)
        {
            StringBuilder sql = new StringBuilder("DELETE FROM ClientFundings");
            sql.AppendFormat(" WHERE ClientFundId = {0}", id);
            return await _context.Database.ExecuteSqlRawAsync(sql.ToString());
        }

        public async Task<TotalSumModel> GetTotalSum(int id)
        {
            TotalSumModel zeroTotalSum = new TotalSumModel
            {
                TotalSum = 0,
            };

            StringBuilder sql = new StringBuilder("SELECT Sum(ClientFundings.ClientSum) AS TotalSum");
            sql.Append(" FROM ClientFundings");
            sql.AppendFormat(" WHERE CaseId = {0}", id);

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
