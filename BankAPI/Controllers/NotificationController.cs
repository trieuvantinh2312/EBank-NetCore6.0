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
    public class NotificationController : Controller
    {
        private INotification _service;
        public NotificationController(INotification _service)
        {
            this._service = _service;
        }

        [HttpGet("{customerId}")]
        public  async Task<List<Notifications>> GetList(int customerId) {
            return await _service.GetNotifications(customerId);
        }

        [HttpGet]
        public async Task<List<Notifications>> GetListNoti()
        {
            return await _service.List();
        }

        [HttpPost]
        public async Task<Notifications> Post([FromBody]Notifications noti)
        {
            return await _service.PostNotification(noti);
        }
    }
}

