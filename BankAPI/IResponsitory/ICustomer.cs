using BankModel;
using BankModel.ViewModel;

namespace BankAPI.IResponsitory
{
    public interface ICustomer
    {
        public Task<List<Customer>> GetCustomers();
        public Task<Customer> CheckLogin(string username, string password);
        public Task<Customer> GetCustomer(string username);
        public Task<Customer> GetCustomerbyID(int Id);
        public Task<Customer> UpdateAttempt(Customer cus);
        public Task<Customer> PostCustomer(Customer cus);

        public Task<List<TotalMonth>> GetTotalCustomerByMonth();

        public Task<List<TopCustomer>> getTopCustomerByTransaction();

        public Task<List<AddressByCustomer>> GetAverageAddressByCustomer();

        public Task<List<Customer>> TotalCustomer();

    }
}
