using BankAPI.IResponsitory;
using BankModel;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private INews service;
        public NewsController(INews service)
        {
            this.service = service;
        }
        [HttpGet]
        public async Task<List<News>> Get()
        {
            return await service.ListNews();
        }
        [HttpGet("{id}")]
        public async Task<News> GetOne(int id)
        {
            return await service.GetNew(id);
        }
        [HttpPost]
        public async Task<News> Post(News news)
        {
            return await service.PostNews(news);
        }

        [HttpPut]
        public async Task<News> Put(News news)
        {
            return await service.UpdateNews(news);
        }
    }
}
