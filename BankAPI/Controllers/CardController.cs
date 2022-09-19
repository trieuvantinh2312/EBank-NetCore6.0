using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankAPI.IResponsitory;
using BankModel;
using Microsoft.AspNetCore.Mvc;



namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    public class CardController : Controller
    {
        private ICard _service;
        public CardController(ICard _service)
        {
            this._service = _service;
        }

        //GET
        [HttpGet]
        public async Task<List<Card>> List() {
            return await _service.listCard();
        }

        //GET
        [HttpPost]
        public async Task<Card> Post([FromBody]Card card)
        {
            return await _service.PostCard(card);
        }

        [HttpGet("{accountId}")]
        public async Task<List<Card>> List(int accountId)
        {
            return await _service.listCard(accountId);
        }
    }
}

