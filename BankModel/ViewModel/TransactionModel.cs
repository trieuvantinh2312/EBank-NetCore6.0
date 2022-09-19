using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankModel.ViewModel
{
    public class TransactionModel
    {
        public Customer? Customer { get; set; }
        public List<Transaction>? Transactions { get; set; }
        public Account Account { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

    }
}
