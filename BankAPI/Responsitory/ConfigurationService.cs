using System;
using BankAPI.DataContext;
using BankAPI.IResponsitory;
using BankModel;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Responsitory
{
	public class ConfigurationService :IConfigurationTransfer
	{
        private ApplicationDbContext _db;
        public ConfigurationService(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        public async Task<ConfigurationTransfer> GetConfiguration()
        {
            return await _db.ConfigurationTransfers.FirstOrDefaultAsync();
        }

        public async Task<ConfigurationTransfer> PutConfiguration(ConfigurationTransfer confi)
        {
            var model = await _db.ConfigurationTransfers.FindAsync(confi.Id);
            if (model != null) {
                model.MaxValue = confi.MaxValue;
                model.MinValue = confi.MinValue;
            }
            _db.ConfigurationTransfers.Update(model);
            await _db.SaveChangesAsync();
            return model;
        }
    }
}

