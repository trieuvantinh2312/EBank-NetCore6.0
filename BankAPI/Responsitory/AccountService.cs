using BankAPI.DataContext;
using BankAPI.IResponsitory;
using BankModel;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Responsitory
{
    public class AccountService : IAccount
    {
        private ApplicationDbContext _db;
        public AccountService(ApplicationDbContext _db)
        {
            this._db = _db;
        }

        public async Task<Account> GetCustomerIdByAccountNo(string accountNo)
        {
            return await _db.Accounts.SingleOrDefaultAsync(x => x.AccountNo.Equals(accountNo));
        }

        public async Task<Account> GetOneByAcId(int Id)
        {
            return await _db.Accounts.FindAsync(Id);
        }

        public async Task<Account> GetOneByAcNo(string accountNo)
        {
            return await _db.Accounts.SingleOrDefaultAsync(x => x.AccountNo.Equals(accountNo));
        }

        public async Task<List<Account>> GetOneById(int customerID)
        {
            return await _db.Accounts.Where(x => x.CustomerId.Equals(customerID)).ToListAsync();
        }

        public async Task<List<Account>> ListAccounts()
        {
            return await _db.Accounts.ToListAsync();
        }

        public async Task<Account> PostAccount(Account ac)
        {
            await _db.Accounts.AddAsync(ac);
            await _db.SaveChangesAsync();
            return ac;

        }

        public async Task<Account> PutAccount(Account ac)
        {
            var model = await _db.Accounts.FindAsync(ac.AccountId);
            if (model != null) {
                model.Attempt = ac.Attempt;
                model.Balance = ac.Balance;
                model.Status = ac.Status;
                model.PinCode = ac.PinCode;
                await _db.SaveChangesAsync();
            }
            return model;
        }
    }
}
