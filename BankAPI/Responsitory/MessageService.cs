using System;
using BankAPI.DataContext;
using BankAPI.IResponsitory;
using BankModel;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Responsitory
{
	public class MessageService : IMessage
	{
        private ApplicationDbContext _db;
        public MessageService(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        public async Task<List<Message>> list()
        {
            return await _db.Messages.ToListAsync();
        }

        public async Task<Message> Post(Message mess)
        {
            await _db.Messages.AddAsync(mess);
            await _db.SaveChangesAsync();
            return mess;
        }
    }
}

