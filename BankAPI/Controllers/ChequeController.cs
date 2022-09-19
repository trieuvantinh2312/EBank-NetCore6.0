using BankAPI.IResponsitory;
using BankAPI.Responsitory;
using BankModel;
using BankModel.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using Wkhtmltopdf.NetCore;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChequeController : ControllerBase
    {
        private ICheque service;
        private ICustomer cusService;
        private IGeneratePdf generatePdf;
        public ChequeController(ICheque service, IGeneratePdf generatePdf, ICustomer cusService)
        {
            this.service = service;
            this.generatePdf = generatePdf;
            this.cusService = cusService;
        }

        [HttpGet("printPdf/{Id}")]
        public async Task<byte[]> PrintPdf(int Id)
        {
            var cheque = await service.GetCheque(Id);
            var customer = await cusService.GetCustomerbyID(cheque.CustomerId);
            var model = new ChequeModel
            {
                Customer = customer,
                Cheque = cheque
            };
            return await generatePdf.GetByteArray("Views/Cheque/Cheque.cshtml", model);
        }

        [HttpGet("{status}")]
        public async Task<List<Cheque>> Get(string status)
        {
            return await service.ListCheque(status);
        }

        [HttpGet("GetOne/{id}")]
        public async Task<Cheque> GetOne(int id)
        {
            return await service.GetCheque(id);
        }

        [HttpGet("ListbyAccount/{account}")]
        public async Task<List<Cheque>> Get(int account)
        {
            return await service.ListChequeByAccount(account);
        }

        [HttpPost]
        public async Task<Cheque> Post(Cheque cheque)
        {
            return await service.PostCheque(cheque);
        }

        [HttpPut]
        public async Task<Cheque> Put(Cheque cheque)
        {
            return await service.PutCheque(cheque);
        }

        [HttpDelete("{id}")]
        public async Task<Cheque> Delete(int id)
        {
            return await service.DeleteCheque(id);
        }

        [HttpGet("totalCheque")]
        public async Task<List<Cheque>> totalCheque()
        {
            return await service.totalCheque();
        }
    }
}
