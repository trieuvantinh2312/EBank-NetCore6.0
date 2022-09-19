using BankModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankService.IResponsitory
{
    public interface ICustomer
    {
        public Task<Customer> CheckLogin(string username, string password);
    }
}
