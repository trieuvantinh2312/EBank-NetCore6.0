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
    public class ConfigurationController : Controller
    {
        private IConfigurationTransfer _service;
        public ConfigurationController(IConfigurationTransfer _service)
        {
            this._service = _service;
        }
        [HttpGet]
        public async Task<ConfigurationTransfer> get() {
            return await _service.GetConfiguration();
        }

        [HttpPut]
        public async Task<ConfigurationTransfer> Put(ConfigurationTransfer confi) 
        {
            return await _service.PutConfiguration(confi);
        }
    }
}

