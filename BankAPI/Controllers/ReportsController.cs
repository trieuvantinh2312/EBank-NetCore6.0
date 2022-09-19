using BankModel.ViewModel;
using BankModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wkhtmltopdf.NetCore;
using BankAPI.IResponsitory;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private ITransaction _service;
        private ICustomer custom;
        private IAccount account;
        private IGeneratePdf generatePdf;
        public ReportsController(ITransaction _service, ICustomer custom, IAccount account, IGeneratePdf generatePdf)
        {
            this._service = _service;
            this.custom = custom;
            this.account = account;
            this.generatePdf = generatePdf;
        }

        [HttpGet]
        public async Task<byte[]> PrintPdf()
        {
            var model = await custom.getTopCustomerByTransaction();
            var ListCustomer = model.GroupBy(c => c.CustomerId).Select(cl => new TopCustomer
            {
                CustomerName = cl.First().CustomerName,
                Avatar = cl.First().Avatar,
                Count = cl.Count(),
                totalMoneyByTransaction = cl.Sum(c => c.Amount)
            }).OrderByDescending(c => c.Count).ToList();
            return await generatePdf.GetByteArray("Views/Report/Report.cshtml", ListCustomer);
        }
    }
}
