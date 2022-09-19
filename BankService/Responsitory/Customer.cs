using BankService.IResponsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankService.Responsitory
{
    public class Customer:ICustomer
    {
        public BankContext db;
        public CustomerService(BankContext db)
        {
            this.db = db;
        }
        public async Task<Customer> CheckLogin(string username, string password)
        {
            var model = await db.Customers.SingleOrDefaultAsync(c => c.Username.Equals(username));

            if (model != null)
            {
                if (model.Status == false)
                {
                    var encryptPass = Security.encryptData(password);
                    if (model.Password.Equals(password))
                    {
                        return model;
                    }
                    else
                    {
                        if (model.Attemp < 2)
                        {
                            model.Attemp = model.Attemp + 1;
                            db.SaveChanges();
                            return null;
                        }
                        else
                        {
                            model.Status = true;
                            db.SaveChanges();
                            return null;
                        }
                    }
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }
    }
}
