using BankAPI.IResponsitory;
using BankModel;
using BankModel.ViewModel;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private IContact service;
        public ContactController(IContact service)
        {
            this.service = service;
        }
        [HttpGet]
        public async Task<IEnumerable<Contact>> Get()
        {
            return await service.ListContact();
        }

        [HttpGet("{contactId}")]
        public async Task<Contact> Get(int contactId)
        {
            return await service.GetOne(contactId);
        }
        [HttpPost]
        public async Task<Contact> Post(Contact contact)
        {
            return await service.PostContact(contact);
        }
        [HttpPut]
        public async Task<Contact> Put([FromBody]Contact contact)
        {
            return await service.PutContact(contact);
        }

        [HttpGet("GetWaitingContact")]
        public async Task<List<Contact>> GetWaitingContact()
        {
            return await service.GetWaitingContact();
        }

        [HttpGet("GetAll")]
        public async Task<List<TotalMonth>> GetAll()
        {
            return await service.GetAll();
        }
    }
}
