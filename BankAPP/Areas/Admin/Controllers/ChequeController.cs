using BankModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace BankAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = "AdminAuth")]
    public class ChequeController : Controller
    {
        private string urlCheque = "http://localhost:5032/api/Cheque/";
        private string urlContact = "http://localhost:5032/api/Contact/";
        private string urlAccount = "http://localhost:5032/api/Account/";

        private HttpClient client = new HttpClient();
        public void listContact()
        {
            var listContact = JsonConvert.DeserializeObject<IEnumerable<BankModel.Contact>>(client.GetStringAsync(urlContact).Result);
            Helper.Put(TempData, "listContact", listContact);
        }
        public IActionResult Index()
        {
            listContact();
            var model = JsonConvert.DeserializeObject<List<Cheque>>(client.GetStringAsync(urlCheque + "waiting").Result).OrderBy(x => x.Id);
            return View(model);
        }

        [HttpGet("CheckCheque")]
        public async Task<IActionResult> CheckCheque(int id)
        {
            var model = JsonConvert.DeserializeObject<Cheque>(client.GetStringAsync(urlCheque + "GetOne/" + id).Result);
            model.Status = "success";
            var updateStatus = await client.PutAsJsonAsync<Cheque>(urlCheque, model);
            if (updateStatus.IsSuccessStatusCode)
            {
                var cheque = JsonConvert.DeserializeObject<byte[]>(client.GetStringAsync(urlCheque + "printPdf/" + id).Result);
                return Json(new { status = true, msg = "Success", cheque = cheque });
            }
            else
            {
                return Json(new { status = false, msg = "Failed" });
            }
        }
    }
}
