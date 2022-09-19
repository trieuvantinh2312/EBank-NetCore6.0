using BankModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = "AdminAuth")]
    public class TransactionController : Controller
    {
        private string urlTransaction = "http://localhost:5032/api/Transaction/";
        private string urlContact = "http://localhost:5032/api/Contact/";
        private string urlAccount = "http://localhost:5032/api/Account/";
        HttpClient client = new HttpClient();
        public void listContact()
        {
            var listContact = JsonConvert.DeserializeObject<IEnumerable<BankModel.Contact>>(client.GetStringAsync(urlContact).Result);
            Helper.Put(TempData, "listContact", listContact);
        }
        public IActionResult Index(string accountNo)
        {
            listContact();
            if (string.IsNullOrEmpty(accountNo))
            {
                var trans = JsonConvert.DeserializeObject<List<Transaction>>(client.GetStringAsync(urlTransaction).Result);
                var account = JsonConvert.DeserializeObject<List<Account>>(client.GetStringAsync(urlAccount).Result);
                ViewBag.account = account;
                ViewBag.accountNo = null;
                return View(trans);
            }
            else
            {
                var trans = JsonConvert.DeserializeObject<List<Transaction>>(client.GetStringAsync(urlTransaction + accountNo).Result);
                var account = JsonConvert.DeserializeObject<List<Account>>(client.GetStringAsync(urlAccount).Result);
                ViewBag.account = account;
                ViewBag.accountNo = accountNo;
                return View(trans);
            }
        }
        [HttpGet("PrintPdfAdmin")]
        public JsonResult GetPdfView(string accountNo)
        {
            if (!string.IsNullOrEmpty(accountNo))
            {
                var findTransaction = JsonConvert.DeserializeObject<List<Transaction>>(client.GetStringAsync(urlTransaction + accountNo).Result);
                if (findTransaction.Count() > 0)
                {
                    var model = JsonConvert.DeserializeObject<string>(client.GetStringAsync(urlTransaction + "printPdf/" + accountNo).Result);
                    return Json(new { status = true, data = model, msg = "Export success" });
                }
                else
                {
                    return Json(new { status = false, msg = "Don't have any transaction to print" });
                }
            }
            else
            {
                return Json(new { status = false, msg = "Please find account to print" });
            }
            
        }
    }
}
