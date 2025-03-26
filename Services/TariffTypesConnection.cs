using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrojWebApp.Models;


namespace TrojWebApp.Services
{
    public class TariffTypesConnection
    {
        private readonly TrojContext _context;

        public TariffTypesConnection(TrojContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TariffTypesModel>> GetTariffTypes()
        {
            return await _context.TariffTypes.OrderBy(t=>t.TariffType).ToListAsync();
        }

        public async Task<IEnumerable<BackgroundColorsModel>> GetBackgroundColors()
        {
            StringBuilder sql = new StringBuilder("SELECT DISTINCT BackgroundColor FROM TariffTypes ORDER BY BackgroundColor");
            return await _context.BackgroundColors.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<IEnumerable<ActiveTariffTypesModel>> GetActiveTariffTypeAndLevel()
        {
            StringBuilder sql = new StringBuilder("SELECT TariffTypes.TariffTypeId,  TariffTypes.TariffType, TariffLevels.TariffLevel");
            sql.Append(" FROM TariffTypes INNER JOIN TariffLevels ON TariffTypes.TariffTypeId = TariffLevels.TariffTypeId");
            sql.Append(" WHERE (TariffLevels.Valid = 1) AND (TariffTypes.NoLevel = 0)");
            sql.Append(" UNION");
            sql.Append(" SELECT TariffTypeId, TariffType, 0 AS TariffLevel");
            sql.Append(" FROM TariffTypes");
            sql.Append(" WHERE (TariffTypes.NoLevel = 1)");
            sql.Append(" ORDER BY TariffType");
            return await _context.ActiveTariffTypes.FromSqlRaw(sql.ToString()).ToListAsync();
        }

        public async Task<TariffTypesModel> GetTariffType(int id)
        {
            return await _context.TariffTypes.FindAsync(id);
        }

        public async Task<TariffTypesModel> CreateTariffType(string type, bool noLevel, bool invisible, string backgroundColor, string userName)
        {
            TariffTypesModel tariffType = new TariffTypesModel
            {
                TariffTypeId = 0,
                TariffType = type,
                NoLevel = noLevel,
                Invisible = invisible,
                BackgroundColor = backgroundColor,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.TariffTypes.Add(tariffType);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return tariffType;
            else
                return null;
        }

        public async Task<TariffTypesModel> UpdateTariffType(int tariffTypeId, string type, bool noLevel, bool invisible, string backgroundColor, string userName)
        {
            TariffTypesModel tariffType = new TariffTypesModel
            {
                TariffTypeId = tariffTypeId,
                TariffType = type,
                NoLevel = noLevel,
                Invisible = invisible,
                BackgroundColor = backgroundColor,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Entry(tariffType).State = EntityState.Modified;
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return tariffType;
            else
                return null;
        }


    }
}
