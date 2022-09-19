using BankAPI.IResponsitory;
using BankModel;
using BankModel.ViewModel;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private ICustomer service;
        public CustomerController(ICustomer service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<List<Customer>> GetCustomers()
        {
            return await service.GetCustomers();
        }


        [HttpPost]
        public async Task<Customer> Post(Customer cus)
        {
            return await service.PostCustomer(cus);
        }
        [HttpGet("{username}")]
        public async Task<Customer> Get(string username)
        {
            return await service.GetCustomer(username);
        }
        [HttpGet("{Id:int}")]
        public async Task<Customer> GetById(int Id)
        {
            return await service.GetCustomerbyID(Id);
        }
        [HttpGet("{username}/{password}")]
        public async Task<Customer> Get(string username,string password)
        {
            return await service.CheckLogin(username, password);
        }
        [HttpPut]
        public async Task<Customer> Put(Customer cus)
        {
            return await service.UpdateAttempt(cus);
        }

        [HttpGet("total")]
        public async Task<List<TotalMonth>> total()
        {
            return await service.GetTotalCustomerByMonth();
        }
        [HttpGet("TopCustomer")]
        public async Task<List<TopCustomer>> topCustomer()
        {
            return await service.getTopCustomerByTransaction();
        }
        [HttpGet("AvgAddress")]
        public async Task<List<AddressByCustomer>> getAvgAddress()
        {
            return await service.GetAverageAddressByCustomer();
        }

        [HttpGet("TotalCustomer")]
        public async Task<List<Customer>> TotalCustomer()
        {
            return await service.TotalCustomer();
        }
    }
}
