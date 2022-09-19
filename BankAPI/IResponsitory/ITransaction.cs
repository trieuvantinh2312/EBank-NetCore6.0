using System;
using BankModel;

namespace BankAPI.IResponsitory
{
    public interface ITransaction
    {
        Task<Transaction> postTransaction(Transaction sac);
        Task<List<Transaction>> searchTransByDate(string fromDate, string toDate, string accountNo);
        Task<List<Transaction>> listTransactions();
        Task<List<Transaction>> listTransactionsByAccount(string accountNo);
        Task<Transaction> listTransactionsByID(int transactionId);

        Task<List<Transaction>> TotalMoneyByTransaction();
    }
}

