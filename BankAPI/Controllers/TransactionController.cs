using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankAPI.IResponsitory;
using BankModel;
using BankModel.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Wkhtmltopdf.NetCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private ITransaction _service;
        private ICustomer custom;
        private IAccount account;
        private IGeneratePdf generatePdf;
        public TransactionController(ITransaction _service, ICustomer custom, IAccount account, IGeneratePdf generatePdf)
        {
            this._service = _service;
            this.custom = custom;
            this.account = account;
            this.generatePdf = generatePdf;
        }
        [HttpGet("printPdf/{accountNo}")]
        public async Task<byte[]> PrintPdf(string accountNo)
        {
            var acc = await account.GetOneByAcNo(accountNo);
            var cus =  await custom.GetCustomerbyID(acc.CustomerId);
            var model = await _service.listTransactionsByAccount(accountNo);
            var PageModel = new TransactionModel
            {
                Account = acc,
                Customer = cus,
                Transactions = model,
                FromDate = "",
                ToDate = ""
            };
            return await generatePdf.GetByteArray("Views/Transaction/Transaction.cshtml", PageModel);
        }
        [HttpGet("printPdf/{fromDate}/{toDate}/{accountNo}")]
        public async Task<byte[]> PrintPdf(string fromDate, string toDate, string accountNo)
        {
            var acc = await account.GetOneByAcNo(accountNo);
            var cus = await custom.GetCustomerbyID(acc.CustomerId);
            var model = await _service.searchTransByDate(fromDate, toDate, accountNo);
            var PageModel = new TransactionModel
            {
                Account = acc,
                Customer = cus,
                Transactions = model,
                FromDate = fromDate,
                ToDate = toDate
            };
            return await generatePdf.GetByteArray("Views/Transaction/Transaction.cshtml", PageModel);
        }
        //GET
        [HttpGet]
        public async Task<List<Transaction>> listTransaction() {
            return await _service.listTransactions();
        }

        [HttpGet("{fromDate}/{toDate}/{accountNo}")]
        public async Task<List<Transaction>> SearchTransaction(string fromDate, string toDate, string accountNo)
        {
            return await _service.searchTransByDate(fromDate, toDate, accountNo);
        }

        [HttpGet("{AccountNo}")]
        public async Task<List<Transaction>> listTransactionByAccount(string AccountNo)
        {
            return await _service.listTransactionsByAccount(AccountNo);
        }

        [HttpGet("{transactionId:int}")]
        public async Task<Transaction> listTransactionById(int transactionId)
        {
            return await _service.listTransactionsByID(transactionId);
        }

        [HttpPost]
        public async Task<Transaction> postTransaction([FromBody]Transaction sac)
        {
            return await _service.postTransaction(sac);
        }

        [HttpGet("TotalMoneyByTransaction")]
        public async Task<List<Transaction>> TotalMoneyByTransaction ()
        {
            return await _service.TotalMoneyByTransaction();
        }
    }
}

