namespace BankAPP.Areas.Admin.Models
{
	public class CustomerViewModel
	{
        public IEnumerable<BankModel.Customer> Customers { get; set; }

        public IEnumerable<BankModel.Transaction> Transactions { get; set; }
    }
}
