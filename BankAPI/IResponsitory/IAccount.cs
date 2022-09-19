using BankModel;

namespace BankAPI.IResponsitory
{
    public interface IAccount
    {
        Task<List<Account>> ListAccounts();
        Task<Account> GetOneByAcNo(string accountNo);
        Task<Account> GetOneByAcId(int Id);
        Task<List<Account>> GetOneById(int customerID);
        Task<Account> PostAccount(Account ac);
        Task<Account> PutAccount(Account ac);

        Task<Account> GetCustomerIdByAccountNo(string accountNo);
    }
}
