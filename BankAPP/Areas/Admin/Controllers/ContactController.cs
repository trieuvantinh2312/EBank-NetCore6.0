using BankModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankAPP.Areas.Admin.Controllers
{
    public class ContactController : Controller
    {
        private string urlContact = "http://localhost:5032/api/Contact/";

        private HttpClient client = new HttpClient();
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var model = JsonConvert.DeserializeObject<List<Contact>>(client.GetStringAsync(urlContact + "GetAll").Result);
            return Json(model);
        }
    }
}
