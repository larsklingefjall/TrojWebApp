using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using TrojWebApp.Models;


namespace TrojWebApp.Services
{
    public class TariffLevelsConnection
    {
        private readonly TrojContext _context;

        public TariffLevelsConnection(TrojContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TariffLevelsViewModel>> GetTariffLevels()
        {
            StringBuilder sql = new StringBuilder("SELECT TariffLevels.*, TariffTypes.TariffType");
            sql.Append(" FROM TariffLevels INNER JOIN TariffTypes ON TariffLevels.TariffTypeId = TariffTypes.TariffTypeId");
            sql.Append(" ORDER BY TariffTypes.TariffType, TariffLevels.ValidFrom DESC");
            return await _context.TariffLevelsView.FromSqlRaw(sql.ToString()).ToListAsync();
        }
        public async Task<TariffLevelsModel> GetTariffLevel(int id)
        {
            return await _context.TariffLevels.FindAsync(id);
        }

        public async Task<TariffLevelsModel> GetValidTariffLevel(int tariffTypeId)
        {
            StringBuilder sql = new StringBuilder("SELECT TariffLevels.*");
            sql.Append(" FROM TariffLevels");
            sql.AppendFormat(" WHERE TariffTypeId = {0}", tariffTypeId);
            sql.Append(" AND Valid = 1");
            return await _context.TariffLevels.FromSqlRaw(sql.ToString()).FirstAsync();
        }

        public async Task<TariffLevelsModel> CreateTariffLevel(int tariffTypeId, double level, DateTime? from, DateTime? to, string userName = "")
        {
            TariffLevelsModel tariffLevel = new TariffLevelsModel
            {
                TariffLevelId = 0,
                TariffTypeId = tariffTypeId,
                TariffLevel = level,
                ValidFrom = from,
                ValidTo = to,
                Valid = true,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.TariffLevels.Add(tariffLevel);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return tariffLevel;
            else
                return null;
        }

        public async Task<TariffLevelsModel> UpdateTariffLevel(int id, int tariffTypeId, double level, DateTime? from, DateTime? to, bool valid, string userName = "")
        {
            TariffLevelsModel tariffLevel = new TariffLevelsModel
            {
                TariffLevelId = id,
                TariffTypeId = tariffTypeId,
                TariffLevel = level,
                ValidFrom = from,
                ValidTo = to,
                Valid = valid,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Entry(tariffLevel).State = EntityState.Modified;
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return tariffLevel;
            else
                return null;
        }
    }
}
