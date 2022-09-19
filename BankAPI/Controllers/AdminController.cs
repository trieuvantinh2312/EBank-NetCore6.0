using BankAPI.IResponsitory;
using BankModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IAdmin service;
        public AdminController(IAdmin service)
        {
            this.service = service;
        }
       
        [HttpGet("{username}/{password}")]
        public async Task<Admin> Get(string username, string password)
        {
            return await service.CheckLogin(username,password);
        }


        [HttpGet]
        public async Task<Admin> GetOne()
        {
            return await service.Get();
        }

    }
}
