using System;
using BankModel;

namespace BankAPP.Mail
{
	public interface IMailService
	{
        Task<string> SendEmailAsync(BankModel.Mail.MailRequest mailRequest);
        Task<string> SendEmailAsyncTransaction(BankModel.Mail.MailRequest mailRequest,Transaction sac);
        Task<string> SendEmailAsyncCustomer(BankModel.Mail.MailRequest mailRequest, Customer cus);
    }
}

