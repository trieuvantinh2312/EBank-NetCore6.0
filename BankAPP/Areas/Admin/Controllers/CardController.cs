using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace BankAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = "AdminAuth")]
    public class CardController : Controller
    {
        private HttpClient client = new HttpClient();
        private string urlCard = "http://localhost:5032/api/Card/";
        private string urlAccount = "http://localhost:5032/api/Account/";
        private string urlContact = "http://localhost:5032/api/Contact/";
        private string urlCustomer = "http://localhost:5032/api/Customer/";

        public void listContact()
        {
            var listContact = JsonConvert.DeserializeObject<IEnumerable<BankModel.Contact>>(client.GetStringAsync(urlContact).Result);
            Helper.Put(TempData, "listContact", listContact);
        }

        public IActionResult Index()
        {
            listContact();
            return View();
        }
        [HttpGet]
        public IActionResult Cards()
        {
            listContact();
            return View();
        }

        [HttpPost("CreateCard")]
        public IActionResult CreateCard(string AccountNo ,string CardNo, string month , string year , string cvv)
        {
            listContact();
            var account = JsonConvert.DeserializeObject<BankModel.Account>(client.GetStringAsync(urlAccount + AccountNo).Result);
            if (account != null)
            {
                BankModel.Card card = new BankModel.Card()
                {
                    CardNo = CardNo,
                    Cvv = cvv ,
                    ExpirationCard = string.Concat(month,"/",year)
                };
                card.AccountId = account.AccountId;
                var model = client.PostAsJsonAsync<BankModel.Card>(urlCard, card).Result;
                if (model.IsSuccessStatusCode)
                {
                    var customerReceiver = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + account.CustomerId).Result);

                    return Json(new { status = true, msg = "Saved sussess",customerId = customerReceiver.CustomerId });
                } else
                {
                    return Json(new { status = false, msg = "Saved sussess" });
                }

            } else
            {
                return Json(new { status = false, msg = "Account no not found" });

            }
        }
        [HttpGet("CreateCard")]
        public IActionResult CreateCard(string accountNo)
        {
            listContact();
            ViewBag.accountNo = accountNo;
            return View();
        }

        [HttpGet("FindCustomerByAccount")]
        public IActionResult FindCustomerByAccount(string toAccount)
        {
            var account = JsonConvert.DeserializeObject<BankModel.Account>(client.GetStringAsync(urlAccount + toAccount).Result);
            if (account != null)
            {
                var customerReceiver = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + account.CustomerId).Result);
                return Json(new { success = true, data = customerReceiver.CustomerName});
            }
            else
            {
                return Json(new { success = false, data = "Not found" });
            }
        }

    }
}
