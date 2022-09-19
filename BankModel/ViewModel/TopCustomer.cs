using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankModel.ViewModel
{
    public class TopCustomer
    {
        public int CustomerId { get; set; }
        public string Avatar { get; set; }
        public string CustomerName { get; set; }

        public string Address { get; set; }

        public int Count { get; set; }

        public int top { get; set; }

        public string Account { get; set; }

        public double totalMoneyByTransaction { get; set; }
        public double Amount { get; set; }


    }
}
