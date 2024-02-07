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
    public class ConfigurationsConnection
    {
        private readonly TrojContext _context;

        public ConfigurationsConnection(TrojContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ConfigurationsModel>> GetConfigurations()
        {
            return await _context.Configurations.OrderBy(t=>t.ConfigKey).ToListAsync();
        }

        public async Task<ConfigurationsModel> GetConfiguration(int id)
        {
            return await _context.Configurations.FindAsync(id);
        }

        public async Task<ConfigurationsModel> GetConfigurationWithkey(string key)
        {
            return await _context.Configurations.FirstOrDefaultAsync(c => c.ConfigKey == key);
        }

        public async Task<ConfigurationsModel> CreateConfiguration(string key, string value, string userName = "")
        {
            ConfigurationsModel configuration = new ConfigurationsModel
            {
                ConfigurationId = 0,
                ConfigKey = key,
                ConfigValue = value,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Configurations.Add(configuration);
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return configuration;
            else
                return null;
        }

        public async Task<ConfigurationsModel> UpdateConfiguration(int id, string key, string value, string userName = "")
        {
            ConfigurationsModel configuration = new ConfigurationsModel
            {
                ConfigurationId = id,
                ConfigKey = key,
                ConfigValue = value,
                Changed = DateTime.Now,
                ChangedBy = userName
            };

            _context.Entry(configuration).State = EntityState.Modified;
            int numberOfSaves = await _context.SaveChangesAsync();
            if (numberOfSaves == 1)
                return configuration;
            else
                return null;
        }
    }
}
