using BankAPI.DataContext;
using BankAPI.IResponsitory;
using BankModel;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Responsitory
{
    public class AdminService : IAdmin
    {
        private ApplicationDbContext db;
        public AdminService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<Admin> CheckLogin(string username,string password)
        {
            return await db.Admins.SingleOrDefaultAsync(a => a.UserName.Equals(username) && a.Password.Equals(password));
        }

        public async Task<Admin> Get()
        {
            return await db.Admins.FirstOrDefaultAsync();
        }
    }
}
