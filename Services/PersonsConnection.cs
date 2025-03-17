using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TrojWebApp.Models;

namespace TrojWebApp.Services
{
    public class PersonsConnection
    {
        private readonly TrojContext _context;
        private readonly string _cryKey;

        public PersonsConnection(TrojContext context, string crykey)
        {
            _context = context;
            _cryKey = crykey;
        }

        public async Task<IEnumerable<PersonsModel>> GetActivePersons()
        {
            StringBuilder sql = new StringBuilder("SELECT PersonId, Active, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MiddleNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS MiddleName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PersonNumberCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS PersonNumber", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', MailAddressCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS MailAddress", _cryKey);
            sql.Append(" FROM Persons");
            sql.Append(" WHERE Active = 1");
            sql.Append(" ORDER BY LastName, FirstName");
            return await _context.Persons.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<PersonsModel>> GetFilteredActivePersons(string firstName, string lastName)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonId, Active, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MiddleNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS MiddleName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PersonNumberCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS PersonNumber", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', MailAddressCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS MailAddress", _cryKey);
            sql.Append(" FROM Persons");
            sql.Append(" WHERE Active = 1");

            StringBuilder where = new StringBuilder("");
            if (firstName != "")
            {
                where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, firstName);
            }
            if (lastName != "")
            {
                where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
            }

            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());

            sql.Append(" ORDER BY LastName, FirstName");
            return await _context.Persons.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<PersonsModel>> GetActivePersonsNotInCase(int caseId)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonId, Active, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MiddleNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS MiddleName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PersonNumberCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS PersonNumber", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', MailAddressCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS MailAddress", _cryKey);
            sql.Append(" FROM Persons");
            sql.Append(" WHERE Active = 1");
            sql.AppendFormat(" AND PersonId NOT IN (SELECT PersonId FROM PersonCases WHERE PersonCases.CaseId = {0})", caseId);
            sql.Append(" ORDER BY LastName, FirstName");
            return await _context.Persons.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<PersonsModel>> GetPersons(int offset, int size)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonId, Active, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MiddleNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS MiddleName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PersonNumberCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS PersonNumber", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', MailAddressCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS MailAddress", _cryKey);
            sql.Append(" FROM Persons");
            sql.Append(" ORDER BY LastName, FirstName");
            sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, size);
            return await _context.Persons.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<int> GetNumberOfPersons()
        {
            StringBuilder sql = new StringBuilder("SELECT Count(PersonId) AS NumberOf");
            sql.Append(" FROM Persons");
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf;
        }

        public async Task<IEnumerable<PersonsModel>> GetFilteredPersons(string firstName, string lastName, string middleName, string personNumber, string mailAddress, string active, int offset, int size)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonId, Active, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MiddleNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS MiddleName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PersonNumberCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS PersonNumber", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', MailAddressCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS MailAddress", _cryKey);
            sql.Append(" FROM Persons");
            StringBuilder where = new StringBuilder("");
            if (firstName != "")
                where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, firstName);
            if (lastName != "")
            {
                if(where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
            }
            if (middleName != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MiddleNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, middleName);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MiddleNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, middleName);
            }
            if (personNumber != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PersonNumberCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, personNumber);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PersonNumberCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, personNumber);
            }
            if (mailAddress != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MailAddressCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, mailAddress);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MailAddressCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, mailAddress);
            }
            if (active != "-1")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND Active = {0}", active);
                else
                    where.AppendFormat(" WHERE Active = {0}", active);
            }
            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());
            sql.Append(" ORDER BY LastName, FirstName");
            sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, size);
            return await _context.Persons.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<int> GetNumberOfFilteredPersons(string firstName, string lastName, string middleName, string personNumber, string mailAddress, string active)
        {
            StringBuilder sql = new StringBuilder("SELECT Count(PersonId) AS NumberOf");
            sql.Append(" FROM Persons");
            StringBuilder where = new StringBuilder("");
            if (firstName != "")
                where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, firstName);
            if (lastName != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, lastName);
            }
            if (middleName != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MiddleNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, middleName);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MiddleNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, middleName);
            }
            if (personNumber != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PersonNumberCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, personNumber);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PersonNumberCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, personNumber);
            }
            if (mailAddress != "")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MailAddressCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, mailAddress);
                else
                    where.AppendFormat(" WHERE CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MailAddressCry, 1 , CONVERT(varbinary, Persons.PersonId))) LIKE '{1}'", _cryKey, mailAddress);
            }
            if (active != "-1")
            {
                if (where.Length > 0)
                    where.AppendFormat(" AND Active = {0}", active);
                else
                    where.AppendFormat(" WHERE Active = {0}", active);
            }
            if (where.Length > 0)
                sql.AppendFormat(" {0}", where.ToString());
            NumberOfModel NumberOf = await _context.NumberOf.FromSqlRaw(sql.ToString()).FirstAsync();
            return NumberOf.NumberOf;
        }

        public async Task<IEnumerable<PersonAddressesModel>> GetAddresses()
        {
            StringBuilder sql = new StringBuilder("SELECT PersonAddressId, PersonId, Valid, StreetNumber, PostalCode, PostalAddress, Country, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', CareOfCry, 1 , CONVERT(varbinary, PersonAddressId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', StreetNameCry, 1 , CONVERT(varbinary, PersonAddressId))) AS StreetName", _cryKey);
            sql.Append(" FROM PersonAddresses");
            sql.Append(" ORDER BY Changed DESC");
            return await _context.PersonAddresses.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<PersonAddressesModel>> GetAddressesForPerson(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonAddressId, PersonId, Valid, StreetNumber, PostalCode, PostalAddress, Country, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', CareOfCry, 1 , CONVERT(varbinary, PersonAddressId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', StreetNameCry, 1 , CONVERT(varbinary, PersonAddressId))) AS StreetName", _cryKey);
            sql.Append(" FROM PersonAddresses");
            sql.AppendFormat(" WHERE PersonId = {0}", id.ToString());
            sql.Append(" ORDER BY Valid DESC, Changed DESC");
            return await _context.PersonAddresses.FromSqlRaw(sql.ToString()).ToListAsync();
        }
        public async Task<PersonAddressesModel> GetValidAddressesForPerson(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonAddressId, PersonId, Valid, StreetNumber, PostalCode, PostalAddress, Country, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', CareOfCry, 1 , CONVERT(varbinary, PersonAddressId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', StreetNameCry, 1 , CONVERT(varbinary, PersonAddressId))) AS StreetName", _cryKey);
            sql.Append(" FROM PersonAddresses");
            sql.AppendFormat(" WHERE PersonId = {0}", id.ToString());
            sql.Append(" AND Valid = 1");
            sql.Append(" ORDER BY Valid DESC, Changed DESC");
            return await _context.PersonAddresses.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<IEnumerable<PhoneNumbersViewModel>> GetPhoneNumbersForPerson(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT PhoneNumbers.PhoneNumberId, PhoneNumbers.PersonId, PhoneNumbers.PhoneNumberTypeId,  PhoneNumberType, PhoneNumbers.Changed, PhoneNumbers.ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PhoneNumberCry, 1 , CONVERT(varbinary, PhoneNumbers.PhoneNumberId))) AS PhoneNumber", _cryKey);
            sql.Append(" FROM PhoneNumberTypes INNER JOIN PhoneNumbers ON PhoneNumberTypes.PhoneNumberTypeId = PhoneNumbers.PhoneNumberTypeId");
            sql.AppendFormat(" WHERE PhoneNumbers.PersonId = {0}", id.ToString());
            sql.Append(" ORDER BY PhoneNumberTypes.PhoneNumberType");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PhoneNumberCry, 1 , CONVERT(varbinary, PhoneNumbers.PhoneNumberId)))", _cryKey);
            return await _context.PhoneNumbersView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<MailAddressesModel>> GeMailsForPerson(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT MailAddressId, PersonId, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MailAddressCry, 1 , CONVERT(varbinary, MailAddressId))) AS MailAddress", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', CommentCry, 1 , CONVERT(varbinary, MailAddressId))) AS Comment", _cryKey);
            sql.Append(" FROM MailAddresses");
            sql.AppendFormat(" WHERE PersonId = {0}", id.ToString());
            return await _context.MailAddresses.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<PersonCasesViewModel>> GetCasesForPerson(int personId)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonCaseId, PersonCases.PersonId, PersonCases.CaseId, PersonCases.PersonTypeId, PersonCases.Responsible, PersonCases.Changed, PersonCases.ChangedBy");
            sql.Append(", Cases.CaseDate, CaseTypes.CaseType, Cases.Active");
            sql.Append(" FROM  PersonCases INNER JOIN Cases ON PersonCases.CaseId = Cases.CaseId INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId");
            sql.Append(" WHERE PersonCases.Responsible = 1");
            sql.AppendFormat(" AND PersonCases.PersonId = {0}", personId);
            sql.Append(" ORDER BY Cases.Active, Cases.CaseDate DESC");
            return await _context.PersonCasesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<PersonsAtCaseViewModel>> GetPersonAtCases(int personId)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonCases.PersonCaseId, PersonCases.PersonId, PersonCases.CaseId, PersonCases.PersonTypeId, PersonCases.Responsible, PersonCases.Changed, PersonCases.ChangedBy, PersonTypes.PersonType, CaseTypes.CaseType, Cases.CaseDate, Cases.Active");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.Append(" FROM PersonCases INNER JOIN PersonTypes ON PersonCases.PersonTypeId = PersonTypes.PersonTypeId INNER JOIN Persons ON PersonCases.PersonId = Persons.PersonId INNER JOIN");
            sql.Append(" Cases ON PersonCases.CaseId = Cases.CaseId INNER JOIN CaseTypes ON Cases.CaseTypeId = CaseTypes.CaseTypeId");
            sql.AppendFormat(" WHERE PersonCases.PersonId = {0}", personId);
            sql.Append(" ORDER BY Cases.Active, Cases.CaseDate DESC");
            return await _context.PersonAtCasesView.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<PersonsModel> GetPerson(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonId, Active, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', FirstNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS FirstName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', LastNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS LastName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MiddleNameCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS MiddleName", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PersonNumberCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS PersonNumber", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', MailAddressCry, 1 , CONVERT(varbinary, Persons.PersonId))) AS MailAddress", _cryKey);
            sql.Append(" FROM Persons");
            sql.AppendFormat(" WHERE PersonId = {0}", id);
            return await _context.Persons.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<PersonAddressesModel> GetAddress(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT PersonAddressId, PersonId, Valid, StreetNumber, PostalCode, PostalAddress, Country, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(512), DecryptByPassphrase('{0}', CareOfCry, 1 , CONVERT(varbinary, PersonAddressId))) AS CareOf", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', StreetNameCry, 1 , CONVERT(varbinary, PersonAddressId))) AS StreetName", _cryKey);
            sql.Append(" FROM PersonAddresses");
            sql.AppendFormat(" WHERE PersonAddressId = {0}", id);
            return await _context.PersonAddresses.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<PhoneNumbersViewModel> GetPhoneNumber(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT PhoneNumbers.PhoneNumberId, PhoneNumbers.PersonId, PhoneNumbers.PhoneNumberTypeId,  PhoneNumberType, PhoneNumbers.Changed, PhoneNumbers.ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', PhoneNumberCry, 1 , CONVERT(varbinary, PhoneNumbers.PhoneNumberId))) AS PhoneNumber", _cryKey);
            sql.Append(" FROM PhoneNumberTypes INNER JOIN PhoneNumbers ON PhoneNumberTypes.PhoneNumberTypeId = PhoneNumbers.PhoneNumberTypeId");
            sql.AppendFormat(" WHERE PhoneNumbers.PhoneNumberId = {0}", id.ToString());
            return await _context.PhoneNumbersView.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<MailAddressesModel> GetMailAddress(int id)
        {
            StringBuilder sql = new StringBuilder("SELECT MailAddressId, PersonId, Changed, ChangedBy");
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', MailAddressCry, 1 , CONVERT(varbinary, MailAddressId))) AS MailAddress", _cryKey);
            sql.AppendFormat(", CONVERT(nvarchar(256), DecryptByPassphrase('{0}', CommentCry, 1 , CONVERT(varbinary, MailAddressId))) AS Comment", _cryKey);
            sql.Append(" FROM MailAddresses");
            sql.AppendFormat(" WHERE MailAddressId = {0}", id.ToString());
            return await _context.MailAddresses.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<PersonsModel> CreatePerson(string firstName, string lastName, string middleName, string personNumber, string mailAddress, string userName = "")
        {
            PersonsModel person = new PersonsModel
            {
                PersonId = 0,
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName,
                PersonNumber = personNumber,
                MailAddress = mailAddress,
                Active = true,
                Changed = DateTime.Now,
                ChangedBy = userName
            };
            _context.Persons.Add(person);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return null;

            List<PersonsModel> list = await _context.Persons.ToListAsync();
            PersonsModel newPerson = list.OrderByDescending(s => s.PersonId).FirstOrDefault();

            if (newPerson == null)
                return null;

            StringBuilder sql = new StringBuilder("DECLARE @FirstName nvarchar(256); ");
            sql.Append("DECLARE @LastName nvarchar(256); ");
            sql.Append("DECLARE @MiddleName nvarchar(256); ");
            sql.Append("DECLARE @PersonNumber nvarchar(256); ");
            sql.Append("DECLARE @MailAddress nvarchar(512); ");

            sql.AppendFormat("SET @FirstName = '{0}'; ", firstName);
            sql.AppendFormat("SET @LastName = '{0}'; ", lastName);
            sql.AppendFormat("SET @MiddleName = '{0}'; ", middleName);
            sql.AppendFormat("SET @PersonNumber = '{0}'; ", personNumber);
            sql.AppendFormat("SET @MailAddress = '{0}'; ", mailAddress);

            sql.Append(" UPDATE Persons SET ");
            if (firstName == "")
                sql.Append(" FirstNameCry = Null");
            else
                sql.AppendFormat(" FirstNameCry = EncryptByPassPhrase('{0}', @FirstName, 1, CONVERT( varbinary, PersonId))", _cryKey);

            if (lastName == "")
                sql.Append(" ,LastNameCry = Null");
            else
                sql.AppendFormat(" ,LastNameCry = EncryptByPassPhrase('{0}', @LastName, 1, CONVERT( varbinary, PersonId))", _cryKey);

            if (middleName == "")
                sql.Append(" ,MiddleNameCry = Null");
            else
                sql.AppendFormat(" ,MiddleNameCry = EncryptByPassPhrase('{0}', @MiddleName, 1, CONVERT( varbinary, PersonId))", _cryKey);

            if (personNumber == "")
                sql.Append(" ,PersonNumberCry = Null");
            else
                sql.AppendFormat(" ,PersonNumberCry = EncryptByPassPhrase('{0}', @PersonNumber, 1, CONVERT( varbinary, PersonId))", _cryKey);

            if (mailAddress == "")
                sql.Append(" ,MailAddressCry = Null");
            else
                sql.AppendFormat(" ,MailAddressCry = EncryptByPassPhrase('{0}', @MailAddress, 1, CONVERT( varbinary, PersonId))", _cryKey);

            sql.Append(", FirstName = Null");
            sql.Append(", LastName = Null");
            sql.Append(", MiddleName = Null");
            sql.Append(", PersonNumber = Null");
            sql.Append(", MailAddress = Null");
            sql.AppendFormat(" WHERE PersonId = {0}", newPerson.PersonId);
            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            return await GetPerson(newPerson.PersonId);
        }

        public async Task<PersonAddressesModel> CreateAddress(int personId, string careOf, string streetName, string streetNumber, string postalCode, string postalAddress, string country, string userName = "")
        {
            PersonAddressesModel address = new PersonAddressesModel
            {
                PersonAddressId = 0,
                PersonId = personId,
                StreetNumber = streetNumber,
                PostalCode = postalCode,
                PostalAddress = postalAddress,
                Country = country,
                Valid = true,
                Changed = DateTime.Now,
                ChangedBy = userName
            };
            _context.PersonAddresses.Add(address);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return null;

            PersonAddressesModel newAddress = await _context.PersonAddresses.Where(c => c.PersonId == personId).Where(a => a.Valid == true).OrderByDescending(s => s.PersonAddressId).FirstAsync();
            if (newAddress == null)
                return null;

            //Set all other address to not valid.
            StringBuilder sqlUpdate = new StringBuilder("UPDATE PersonAddresses SET Valid = 0");
            sqlUpdate.AppendFormat(" WHERE PersonId = {0}; ", personId);
            await _context.Database.ExecuteSqlRawAsync(sqlUpdate.ToString());

            StringBuilder sql = new StringBuilder("DECLARE @CareOf nvarchar(512); ");
            sql.Append("DECLARE @StreetName nvarchar(256); ");
            sql.AppendFormat("SET @CareOf = '{0}'; ", careOf);
            sql.AppendFormat("SET @StreetName = '{0}'; ", streetName);
            sql.Append(" UPDATE PersonAddresses SET ");
            if (careOf == "")
                sql.Append(" CareOfCry = Null");
            else
                sql.AppendFormat(" CareOfCry = EncryptByPassPhrase('{0}', @CareOf, 1, CONVERT( varbinary, PersonAddressId))", _cryKey);
            if (streetName == "")
                sql.Append(" ,StreetNameCry = Null");
            else
                sql.AppendFormat(" ,StreetNameCry = EncryptByPassPhrase('{0}', @StreetName, 1, CONVERT( varbinary, PersonAddressId))", _cryKey);
            sql.Append(", CareOf = Null");
            sql.Append(", StreetName = Null");
            sql.Append(", Valid = 1");
            sql.AppendFormat(" WHERE PersonAddressId = {0}", newAddress.PersonAddressId);
            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            return await GetAddress(newAddress.PersonAddressId);
        }

        public async Task<PhoneNumbersViewModel> CreatePhoneNumber(int personId, int phoneNumberTypeId, string number, string userName = "")
        {
            PhoneNumbersModel phoneNumber = new PhoneNumbersModel
            {
                PhoneNumberId = 0,
                PersonId = personId,
                PhoneNumberTypeId = phoneNumberTypeId,
                PhoneNumber = number,
                Changed = DateTime.Now,
                ChangedBy = userName
            };
            _context.PhoneNumbers.Add(phoneNumber);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return null;

            PhoneNumbersModel newNumber = _context.PhoneNumbers.OrderByDescending(p => p.PhoneNumberId).FirstOrDefault();
            if (newNumber == null)
                return null;

            StringBuilder sql = new StringBuilder("DECLARE @PhoneNumber nvarchar(256); ");
            sql.AppendFormat("SET @PhoneNumber = '{0}'; ", number);
            sql.Append(" UPDATE PhoneNumbers SET ");
            if (number == "")
                sql.Append(" PhoneNumberCry = Null");
            else
                sql.AppendFormat(" PhoneNumberCry = EncryptByPassPhrase('{0}', @PhoneNumber, 1, CONVERT( varbinary, PhoneNumberId))", _cryKey);
            sql.Append(", PhoneNumber = Null");
            sql.AppendFormat(" WHERE PhoneNumberId = {0}", newNumber.PhoneNumberId);
            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            return await GetPhoneNumber(newNumber.PhoneNumberId);
        }

        public async Task<MailAddressesModel> CreateMailAddress(int personId, string mailAddress, string comment, string userName)
        {
            MailAddressesModel mail = new MailAddressesModel
            {
                MailAddressId = 0,
                PersonId = personId,
                Comment = "",
                MailAddress = "",
                Changed = DateTime.Now,
                ChangedBy = userName
            };
            _context.MailAddresses.Add(mail);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return null;

            MailAddressesModel newMail = _context.MailAddresses.OrderByDescending(p => p.MailAddressId).FirstOrDefault();
            if (newMail == null)
                return null;

            StringBuilder sql = new StringBuilder("");
            sql.Append("DECLARE @MailAddress nvarchar(1024); ");
            sql.Append("DECLARE @Comment nvarchar(1024); ");
            sql.AppendFormat("SET @MailAddress = '{0}'; ", mailAddress);
            sql.AppendFormat("SET @Comment = '{0}'; ", comment);
            sql.Append(" UPDATE MailAddresses SET ");
            if (string.IsNullOrEmpty(mailAddress))
                sql.Append(" MailAddressCry = Null");
            else
                sql.AppendFormat(" MailAddressCry = EncryptByPassPhrase('{0}', @MailAddress, 1, CONVERT( varbinary, MailAddressId))", _cryKey);
            if (string.IsNullOrEmpty(comment))
                sql.Append(" ,CommentCry = Null");
            else
                sql.AppendFormat(" ,CommentCry = EncryptByPassPhrase('{0}', @Comment, 1, CONVERT( varbinary, MailAddressId))", _cryKey);
            sql.Append(", MailAddress = Null");
            sql.Append(", Comment = Null");
            sql.AppendFormat(" WHERE MailAddressId = {0}", newMail.MailAddressId);
            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            return await GetMailAddress(newMail.MailAddressId);
        }

        public async Task<PersonsModel> UpdatePerson(int id, string firstName, string lastName, string middleName, string personNumber, string mailAddress, bool active, string userName = "")
        {
            PersonsModel person = new PersonsModel
            {
                PersonId = id,
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName,
                PersonNumber = personNumber,
                MailAddress = mailAddress,
                Active = active,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Entry(person).State = EntityState.Modified;
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return null;

            StringBuilder sql = new StringBuilder("DECLARE @FirstName nvarchar(256); ");
            sql.Append("DECLARE @LastName nvarchar(256); ");
            sql.Append("DECLARE @MiddleName nvarchar(256); ");
            sql.Append("DECLARE @PersonNumber nvarchar(256); ");
            sql.Append("DECLARE @MailAddress nvarchar(512); ");

            sql.AppendFormat("SET @FirstName = '{0}'; ", firstName);
            sql.AppendFormat("SET @LastName = '{0}'; ", lastName);
            sql.AppendFormat("SET @MiddleName = '{0}'; ", middleName);
            sql.AppendFormat("SET @PersonNumber = '{0}'; ", personNumber);
            sql.AppendFormat("SET @MailAddress = '{0}'; ", mailAddress);

            sql.Append(" UPDATE Persons SET ");
            if (firstName == "")
                sql.Append(" FirstNameCry = Null");
            else
                sql.AppendFormat(" FirstNameCry = EncryptByPassPhrase('{0}', @FirstName, 1, CONVERT( varbinary, PersonId))", _cryKey);

            if (lastName == "")
                sql.Append(" ,LastNameCry = Null");
            else
                sql.AppendFormat(" ,LastNameCry = EncryptByPassPhrase('{0}', @LastName, 1, CONVERT( varbinary, PersonId))", _cryKey);

            if (middleName == "")
                sql.Append(" ,MiddleNameCry = Null");
            else
                sql.AppendFormat(" ,MiddleNameCry = EncryptByPassPhrase('{0}', @MiddleName, 1, CONVERT( varbinary, PersonId))", _cryKey);

            if (personNumber == "")
                sql.Append(" ,PersonNumberCry = Null");
            else
                sql.AppendFormat(" ,PersonNumberCry = EncryptByPassPhrase('{0}', @PersonNumber, 1, CONVERT( varbinary, PersonId))", _cryKey);

            if (mailAddress == "")
                sql.Append(" ,MailAddressCry = Null");
            else
                sql.AppendFormat(" ,MailAddressCry = EncryptByPassPhrase('{0}', @MailAddress, 1, CONVERT( varbinary, PersonId))", _cryKey);

            sql.Append(", FirstName = Null");
            sql.Append(", LastName = Null");
            sql.Append(", MiddleName = Null");
            sql.Append(", PersonNumber = Null");
            sql.Append(", MailAddress = Null");
            sql.AppendFormat(" WHERE PersonId = {0}", id);
            await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            return await GetPerson(id);
        }

        public async Task<PersonAddressesModel> UpdateAddress(int id, int personId, string careOf, string streetName, string streetNumber, string postalCode, string postalAddress, string country, bool valid, string userName = "")
        {
            PersonAddressesModel address = new PersonAddressesModel
            {
                PersonAddressId = id,
                PersonId = personId,
                CareOf = careOf,
                StreetNumber = streetNumber,
                StreetName = streetName,
                PostalCode = postalCode,
                PostalAddress = postalAddress,
                Country = country,
                Valid = valid,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Entry(address).State = EntityState.Modified;
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves != 1)
                return null;

            StringBuilder sql = new StringBuilder("DECLARE @CareOf nvarchar(512); ");
            sql.Append("DECLARE @StreetName nvarchar(256); ");
            sql.AppendFormat("SET @CareOf = '{0}'; ", careOf);
            sql.AppendFormat("SET @StreetName = '{0}'; ", streetName);
            sql.Append(" UPDATE PersonAddresses SET ");
            if (careOf == "")
                sql.Append(" CareOfCry = Null");
            else
                sql.AppendFormat(" CareOfCry = EncryptByPassPhrase('{0}', @CareOf, 1, CONVERT( varbinary, PersonAddressId))", _cryKey);
            if (streetName == "")
                sql.Append(" ,StreetNameCry = Null");
            else
                sql.AppendFormat(" ,StreetNameCry = EncryptByPassPhrase('{0}', @StreetName, 1, CONVERT( varbinary, PersonAddressId))", _cryKey);
            sql.Append(", CareOf = Null");
            sql.Append(", StreetName = Null");
            sql.AppendFormat(" WHERE PersonAddressId = {0}", id);
            int numberOfUpdated = await _context.Database.ExecuteSqlRawAsync(sql.ToString());
            if (numberOfUpdated != 1)
                return null;
            return address;
        }

        public async Task<int> DeleteAddress(int id)
        {
            StringBuilder sql = new StringBuilder("DELETE FROM PersonAddresses");
            sql.AppendFormat(" WHERE PersonAddressId = {0}", id);
            int response = await _context.Database.ExecuteSqlRawAsync(sql.ToString());

            IEnumerable<PersonAddressesModel> list = await GetAddresses();
            if (list != null)
            {
                if (list.Any())
                {
                    PersonAddressesModel address = list.FirstOrDefault();
                    if(address != null)
                    {
                        StringBuilder sqlUpdate = new StringBuilder("UPDATE PersonAddresses SET Valid = 1");
                        sqlUpdate.AppendFormat(" WHERE PersonAddressId = {0}; ", address.PersonAddressId);
                        await _context.Database.ExecuteSqlRawAsync(sqlUpdate.ToString());
                    }
                }
            }
            return response;
        }

        public async Task<int> DeletePhoneNumber(int id)
        {
            StringBuilder sql = new StringBuilder("DELETE FROM PhoneNumbers");
            sql.AppendFormat(" WHERE PhoneNumberId = {0}", id);
            return await _context.Database.ExecuteSqlRawAsync(sql.ToString());
        }

        public async Task<int> DeleteMail(int id)
        {
            StringBuilder sql = new StringBuilder("DELETE FROM MailAddresses");
            sql.AppendFormat(" WHERE MailAddressId = {0}", id);
            return await _context.Database.ExecuteSqlRawAsync(sql.ToString());
        }

    }
}
