using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankModel.ViewModel
{
    public class ReportTotalTranAndCus
    {
        public int TotalCustomer { get; set; }
        public float TotalMoneyByTransaction { get; set; }
        public float ReportCompleted { get; set; }
        public int TotalRequestReport { get; set; }

    }
}
