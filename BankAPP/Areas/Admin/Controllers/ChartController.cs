
﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
﻿using BankModel.ViewModel;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BankAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = "AdminAuth")]
    public class ChartController : Controller
    {        
        private string urlContact = "http://localhost:5032/api/Contact/";
        private string urlCustomer = "http://localhost:5032/api/Customer/";
        private string urlTransaction = "http://localhost:5032/api/Transaction/";
        private string urlCheque = "http://localhost:5032/api/Cheque/";
        private string urlReport = "http://localhost:5032/api/Reports/";
        private HttpClient client = new HttpClient();

        public void listContact()
        {
            var listContact = JsonConvert.DeserializeObject<IEnumerable<BankModel.Contact>>(client.GetStringAsync(urlContact).Result);
            Helper.Put(TempData, "listContact", listContact);
        }
        public IActionResult Index()
        {
            listContact();
            var model = JsonConvert.DeserializeObject<List<TopCustomer>>(client.GetStringAsync(urlCustomer + "TopCustomer").Result);
            //ViewBag.ListCustomer = model.OrderByDescending(c => c.Count);
            ViewBag.ListCustomer =  model.GroupBy(c => c.CustomerId).Select(cl => new TopCustomer
            {
                CustomerName = cl.First().CustomerName,
                Avatar = cl.First().Avatar,
                Count = cl.Count(),
                totalMoneyByTransaction = cl.Sum(c => c.Amount)
            }).OrderByDescending(c => c.Count).Take(5).ToList();

            var total = JsonConvert.DeserializeObject<List<BankModel.Customer>>(client.GetStringAsync(urlCustomer + "TotalCustomer").Result);
            var totalCus = total.Count();
            ViewBag.totalCus = totalCus;

            var totalMoney = JsonConvert.DeserializeObject<List<BankModel.Transaction>>(client.GetStringAsync(urlTransaction + "TotalMoneyByTransaction").Result);
            var totalMoneybyCus = totalMoney.Sum(x => x.Amount);
            ViewBag.totalMoneybyCus = totalMoneybyCus;

            var totalCon = JsonConvert.DeserializeObject<List<BankModel.Contact>>(client.GetStringAsync(urlContact).Result);
            var totalConTract = totalCon.Count();
            ViewBag.totalConTract = totalConTract;

            var Cheque = JsonConvert.DeserializeObject<List<BankModel.Cheque>>(client.GetStringAsync(urlCheque + "totalCheque").Result);
            var totalChe = Cheque.Count();
            ViewBag.totalCheque = totalChe;

            var contact = JsonConvert.DeserializeObject<List<BankModel.Contact>>(client.GetStringAsync(urlContact + "GetWaitingContact").Result);
            ViewBag.listWaitingContact = contact.Where(y => y.Status.Equals("waiting")).OrderByDescending(x => x.DateContact).Take(5).ToList();
            return View();
        }
        [HttpGet("PrintReport")]
        public async Task<IActionResult> Report()
        {

            var Report = JsonConvert.DeserializeObject<string>(client.GetStringAsync(urlReport).Result);
            return Json(Report);
        }
    }
}
