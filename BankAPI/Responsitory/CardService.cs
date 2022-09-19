using System;
using BankAPI.DataContext;
using BankAPI.IResponsitory;
using BankModel;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Responsitory
{
	public class CardService :ICard
	{
        private ApplicationDbContext _db;
        public CardService(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        public async Task<List<Card>> listCard()
        {
            return await _db.Cards.ToListAsync();
        }

        public async Task<List<Card>> listCard(int accountId)
        {
            return await _db.Cards.Where(x => x.AccountId.Equals(accountId)).ToListAsync();
        }
        public async Task<Card> PostCard(Card card)
        {
            await _db.Cards.AddAsync(card);
            await _db.SaveChangesAsync();
            return card;
        }
    }
}

