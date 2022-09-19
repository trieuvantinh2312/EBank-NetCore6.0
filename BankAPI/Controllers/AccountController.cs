using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankAPI.IResponsitory;
using BankModel;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private IAccount _service;
        public AccountController(IAccount _service)
        {
            this._service = _service;
        }

        [HttpGet("{accountNo}")]
        public async Task<Account> GetAccount(string accountNo) {
            var model = await _service.GetOneByAcNo(accountNo);
            if (model != null)
            {
                return model;
            }
            else {
                return null;
            }
        }

        [HttpGet("GetAccountById/{Id}")]
        public async Task<Account> GetAccountById(int Id)
        {
            return  await _service.GetOneByAcId(Id);
        }

        [HttpGet("{customerId:int}")]
        public async Task<List<Account>> GetAccountByID(int customerId)
        {
            var model = await _service.GetOneById(customerId);
            if (model != null)
            {
                return model;
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        public async Task<List<Account>> GetAccounts()
        {
            var model = await _service.ListAccounts();
            if (model != null)
            {
                return model;
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<Account> PostAccount([FromBody]Account ac)
        {
            return await _service.PostAccount(ac);
        }

        [HttpPut]
        public async Task<Account> PutAccount([FromBody]Account ac)
        {
            return await _service.PutAccount(ac);
        }

        [HttpGet("GetCustomerId/{Id}")]
        public async Task<Account> GetCustomerId(string Id)
        {
            return await _service.GetCustomerIdByAccountNo(Id);
        }
    }
}

