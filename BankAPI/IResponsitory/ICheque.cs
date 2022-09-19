using BankModel;

namespace BankAPI.IResponsitory
{
    public interface ICheque
    {
        Task<List<Cheque>> ListCheque(string Status);
        Task<Cheque> GetCheque(int id);
        Task<List<Cheque>> ListChequeByAccount(int  account);
        Task<Cheque> PostCheque(Cheque cheque);
        Task<Cheque> PutCheque(Cheque cheque);
        Task<Cheque> DeleteCheque(int id);

        Task<List<Cheque>> totalCheque();
    }
}
