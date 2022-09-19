using System;
using BankAPI.DataContext;
using BankAPI.IResponsitory;
using BankModel;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Responsitory
{
	public class TransactionService : ITransaction
	{
		private ApplicationDbContext _db;
		public TransactionService(ApplicationDbContext _db)
		{
			this._db = _db;
		}

        public async Task<List<Transaction>> listTransactions()
        {
            return await _db.Transactions.ToListAsync();
        }

        public async Task<List<Transaction>> listTransactionsByAccount(string accountNo)
        {
            return await _db.Transactions.Where(x=>x.FromAccount.Equals(accountNo) || x.ToAccount.Equals(accountNo)).ToListAsync();
        }

        public async Task<Transaction> listTransactionsByID(int transactionId)
        {
            return await _db.Transactions.FindAsync(transactionId);
        }

        public async Task<Transaction> postTransaction(Transaction sac)
        {
           await _db.Transactions.AddAsync(sac);
           await _db.SaveChangesAsync();
           return sac;
        }

        public async Task<List<Transaction>> searchTransByDate(string fromDate, string toDate,string accountNo)
        {
            var model = await _db.Transactions.Where(x => x.TransactionDate >= DateTime.Parse(fromDate) && x.TransactionDate <= DateTime.Parse(toDate))
                                              .Where(x => x.FromAccount.Equals(accountNo) || x.ToAccount.Equals(accountNo)).ToListAsync();
            return model;
        }

        public async Task<List<Transaction>> TotalMoneyByTransaction()
        {
            var model = _db.Transactions.ToListAsync();
            return await model;
        }
    }
}

