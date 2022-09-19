using BankModel;

namespace BankAPI.IResponsitory
{
    public interface IAdmin
    {
        public Task<Admin> CheckLogin(string username, string password);
        Task<Admin> Get();
    }
}
