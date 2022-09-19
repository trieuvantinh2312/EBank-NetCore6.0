using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankModel.ViewModel
{
    public class ProfileModel
    {
        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Card> Card { get; set; }
        public Customer Customer { get; set; }
    }
}
