using BankAPI.DataContext;
using BankAPI.IResponsitory;
using BankModel;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Responsitory
{
    public class ChequeService : ICheque
    {
        private ApplicationDbContext db;
        public ChequeService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<List<Cheque>> ListCheque(string Status)
        {
            return await db.Cheques.Where(x => x.Status.Equals(Status)).ToListAsync();
        }

        public async Task<List<Cheque>> ListChequeByAccount(int account)
        {
            return await db.Cheques.Where(x => x.AccountId.Equals(account)).ToListAsync();
        }

        public async Task<Cheque> PostCheque(Cheque cheque)
        {
            await db.Cheques.AddAsync(cheque);
            await db.SaveChangesAsync();
            return cheque;
        }

        public async Task<Cheque> PutCheque(Cheque cheque)
        {
            var model = await db.Cheques.Where(x => x.Id.Equals(cheque.Id)).SingleOrDefaultAsync();
            model.Status = cheque.Status;
            await db.SaveChangesAsync();
            return model;
        }

        public async Task<Cheque> DeleteCheque(int id)
        {
            var model = await db.Cheques.FindAsync(id);
            db.Remove(model);
            await db.SaveChangesAsync();
            return model;
        }

        public async Task<Cheque> GetCheque(int id)
        {
            return await db.Cheques.FindAsync(id);
        }

        public async Task<List<Cheque>> totalCheque()
        {
            return await db.Cheques.ToListAsync();
        }
    }
}
