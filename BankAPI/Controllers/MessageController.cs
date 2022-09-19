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
    public class MessageController : Controller
    {
        private IMessage _service;
        public MessageController(IMessage _service)
        {
            this._service = _service;
        }

        [HttpGet]
        public async Task<List<Message>> Gets() {
            return await _service.list();
        }

        [HttpPost]
        public async Task<Message> Post([FromBody]Message mess)
        {
            return await _service.Post(mess);
        }
    }
}

