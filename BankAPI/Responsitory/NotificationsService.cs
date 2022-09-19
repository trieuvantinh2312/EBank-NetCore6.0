using System;
using BankAPI.DataContext;
using BankAPI.IResponsitory;
using BankModel;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Responsitory
{
	public class NotificationsService : INotification
	{
        private ApplicationDbContext _db;
        public NotificationsService(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        public async Task<List<Notifications>> GetNotifications(int CustomerId)
        {
            return await _db.Notifications.Include(x=>x.Transactions).Where(x=>x.FromCustomerId == CustomerId || x.ToCustomerId == CustomerId).ToListAsync();
        }

        public async Task<List<Notifications>> List()
        {
            return await _db.Notifications.Include(x => x.Transactions).ToListAsync();
        }

        public async Task<Notifications> PostNotification(Notifications noti)
        {
            await _db.Notifications.AddAsync(noti);
            await _db.SaveChangesAsync();
            return noti;
        }
    }
}

