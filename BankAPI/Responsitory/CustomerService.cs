using BankAPI.DataContext;
using BankAPI.IResponsitory;
using BankModel;
using BankModel.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Responsitory
{

    public class CustomerService : ICustomer
    {

        public ApplicationDbContext db;
        public CustomerService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<Customer> CheckLogin(string username, string password)
        {
            var model = await db.Customers.SingleOrDefaultAsync(c => c.UserName.Equals(username) && c.Password.Equals(password));
            if (model != null)
            {
                return model;
            }
            else
            {
                return null;
            }
        }

        public async Task<Customer> GetCustomer(string username)
        {
            return await db.Customers.SingleOrDefaultAsync(c => c.UserName.Equals(username));
        }

        public async Task<Customer> GetCustomerbyID(int Id)
        {
            return await db.Customers.FindAsync(Id);
        }

        public async Task<List<Customer>> GetCustomers()
        {
            return await db.Customers.ToListAsync();
        }

        public async Task<Customer> PostCustomer(Customer cus)
        {
            await db.Customers.AddAsync(cus);
            await db.SaveChangesAsync();
            return cus;
        }

        public async Task<Customer> UpdateAttempt(Customer cus)
        {
            var model = await db.Customers.SingleOrDefaultAsync(c => c.UserName.Equals(cus.UserName));
            if (!string.IsNullOrEmpty(cus.Password))
            {
                model.Password = cus.Password;
            }
            if (!string.IsNullOrEmpty(cus.Address))
            {
                model.Address = cus.Address;
            }
            if (!string.IsNullOrEmpty(cus.Email))
            {
                model.Email = cus.Email;
            }
            if (!string.IsNullOrEmpty(cus.Phone))
            {
                model.Phone = cus.Phone;
            }
            if (!string.IsNullOrEmpty(cus.CustomerName))
            {
                model.CustomerName = cus.CustomerName;
            }
            if (cus.Dob != null)
            {
                model.Dob = cus.Dob;
            }
            model.Attempt = cus.Attempt;
            model.Status = cus.Status;
            model.Avatar = cus.Avatar;
            
            await db.SaveChangesAsync();
            return model;
        }

        public async Task<List<TotalMonth>> GetTotalCustomerByMonth()
        {
            var Cus = from cus in db.Customers
                      group cus by new { month = cus.CreateDate.Month } into d
                      select new TotalMonth { Month =  d.Key.month, Count = d.Count() };
            return await  Cus.ToListAsync();
        }

        public async Task<List<TopCustomer>> getTopCustomerByTransaction()
        {
            var Cus = from cus in db.Customers
                      join Acc in db.Accounts
                      on cus.CustomerId equals Acc.CustomerId
                      join Tran in db.Transactions
                      on Acc.AccountNo equals Tran.FromAccount
                      select new TopCustomer
                      {
                          CustomerId = cus.CustomerId,
                          CustomerName = cus.CustomerName,
                          Avatar = cus.Avatar,
                          Amount = Tran.Amount,
                          Account = Tran.FromAccount
                      };
            return await Cus.ToListAsync();


        }

        public async Task<List<AddressByCustomer>> GetAverageAddressByCustomer()
        {
            var model = from cus in db.Customers
                        group cus by new
                        {
                            cus.City,
                        } into c
                        select new AddressByCustomer { Address = c.Key.City, Count = c.Count() };
            return await model.ToListAsync();

        }

        public async Task<List<Customer>> TotalCustomer()
        {
            var model = db.Customers.ToListAsync();
                        
            return await model;
        }
    }

}
