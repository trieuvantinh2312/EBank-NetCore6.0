using BankModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BankAPP.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OtherController : Controller
    {
        private string urlCustomer = "http://localhost:5032/api/Customer/";
        private string urlNotification = "http://localhost:5032/api/Notification/";
        private string urlMessage = "http://localhost:5032/api/Message/";
        private HttpClient client = new HttpClient();
        public void TemDataListNotification()
        {
            if (User.Identity.IsAuthenticated)
            {
                var clams = (ClaimsIdentity)User.Identity;
                var CustomerId = int.Parse(clams.FindFirst(ClaimTypes.NameIdentifier).Value);
                var listNotification = JsonConvert.DeserializeObject<IEnumerable<Notifications>>(client.GetStringAsync(urlNotification + CustomerId).Result).OrderByDescending(x => x.TransactionId);
                var customer = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + CustomerId).Result);
                var listMess = JsonConvert.DeserializeObject<IEnumerable<Message>>(client.GetStringAsync(urlMessage).Result);
                Helper.Put(TempData, "CustomerAvatar", customer.Avatar);
                Helper.Put(TempData, "ListNotification", listNotification);
                Helper.Put(TempData, "listMess", listMess);
            }
        }
        public IActionResult NotFound()
        {
            TemDataListNotification();
            return View();
        }
        
        public IActionResult PrivacyPolicy()
        {
            TemDataListNotification();
            return View();
        }
        
        public IActionResult TermsConditions()
        {
            TemDataListNotification();
            return View();
        }

        [HttpGet("Service")]
        public IActionResult Service()
        {
            TemDataListNotification();
            return View();
        }
    }
}
