using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using BankModel;
using BankModel.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;
using System;
using BankModel.Mail;
using Twilio.TwiML.Messaging;
using BankAPP.Mail;
using System.Text.RegularExpressions;

namespace BankAPP.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = "AdminAuth")]
    public class CustomerController : Controller
    {
        private string urlCustomer = "http://localhost:5032/api/Customer/";
        private string urlAccount = "http://localhost:5032/api/Account/";
        private string urlContact = "http://localhost:5032/api/Contact/";
        private string urlCard = "http://localhost:5032/api/Card/";
        private HttpClient client = new HttpClient();
        Random random = new Random();
        IMailService _mail;
        public CustomerController(IMailService _mail)
        {
            this._mail = _mail;
        }

        public void listContact()
        {
            var listContact = JsonConvert.DeserializeObject<IEnumerable<BankModel.Contact>>(client.GetStringAsync(urlContact).Result);
            Helper.Put(TempData, "listContact", listContact);
        }
        public class TotoalCustomerByMonth
        {
            public int Month { get; set; }
            public int Count { get; set; }

        }
        [HttpGet]
        public IActionResult Index()
        {
            listContact();
            var customers = JsonConvert.DeserializeObject<IEnumerable<BankModel.Customer>>(client.GetStringAsync(urlCustomer).Result);
            return View(customers);

        }
        [HttpGet]
        public IActionResult EditCustomer(string id)
        {
            listContact();
            var customer = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + int.Parse(id)).Result);
            return View(customer);

        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(BankModel.Customer customer)
        {
            try
            {
                var res = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + customer.CustomerId).Result);
                if (res != null)
                {
                    var model = client.PutAsJsonAsync<BankModel.Customer>(urlCustomer, customer).Result;
                    if (model.IsSuccessStatusCode)
                    {
                        customer.Password = "*******";
                        MailRequest mailRequest = new MailRequest()
                        {
                            UserName = customer.CustomerName,
                            UserContent = null,
                            ToEmail = customer.Email,
                            Body = null
                        };
                        var status = await _mail.SendEmailAsyncCustomer(mailRequest, customer);
                        return RedirectToAction("Details", "Customer", new { id = customer.CustomerId });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error, please try again later !");
                        listContact();
                        return View();
                    }
                }
                ModelState.AddModelError(string.Empty, "Error, please try again later !");
                listContact();
                return View();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Failed !");
                listContact();
                return View();
            }
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            listContact();
            var customer = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + int.Parse(id)).Result);
            if (customer != null)
            {
                customer.Accounts = JsonConvert.DeserializeObject<List<BankModel.Account>>(client.GetStringAsync(urlAccount + int.Parse(id)).Result);
                foreach (var item in customer.Accounts)
                {
                    item.Cards = JsonConvert.DeserializeObject<List<BankModel.Card>>(client.GetStringAsync(urlCard + item.AccountId).Result);
                }
                return View(customer);
            }
            return NotFound();

        }

        [HttpGet]
        public IActionResult CreateCustomer()
        {
            listContact();
            try
            {
                return View();

            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(BankModel.Customer customer)
        {
            try
            {
                string name = Helper.RemoveUnicode(customer.CustomerName);

                customer.UserName = name.Trim().Replace(" ", "").ToLower() + random.Next(0, 999).ToString("D3");
                customer.Password = random.Next(0, 999999).ToString("D6");
                var currentPass = customer.Password;
                customer.Password = Helper.sha256_hash(customer.Password);
                var res = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + customer.UserName).Result);
                if (res == null)
                {
                    customer.Avatar = "userdefault.png";
                    var model = client.PostAsJsonAsync<BankModel.Customer>(urlCustomer, customer).Result;
                    if (model.IsSuccessStatusCode)
                    {
                        customer.Password = currentPass;
                        MailRequest mailRequest = new MailRequest()
                        {
                            UserName = customer.CustomerName,
                            UserContent = null,
                            ToEmail = customer.Email,
                            Body = null
                        };
                        var status = await _mail.SendEmailAsyncCustomer(mailRequest, customer);
                        return RedirectToAction("Index", "Customer");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Post error");
                        listContact();
                        return View();
                    }
                }
                ModelState.AddModelError(string.Empty, "Customer is existed");
                listContact();
                return View();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Failed !");
                listContact();
                return View();
            }
        }

        [HttpGet("updateStatus")]
        public IActionResult updateStatus(int id, string status)
        {
            var customer = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + id).Result);
            if (customer != null)
            {
                if (status == "lock")
                {
                    customer.Status = "normal";
                }
                else
                {
                    customer.Status = "lock";
                }
                customer.Attempt = 0;
                var model = client.PutAsJsonAsync(urlCustomer, customer).Result;
                if (model.IsSuccessStatusCode)
                {

                    return Json(new { success = true, status = customer.Status });
                }
                return Json(new { success = false });
            }
            else
            {

                return Json(new { success = false });
            }

        }


        [HttpGet("total")]
        public IActionResult getTotal()
        {
            var model = JsonConvert.DeserializeObject<List<TotalMonth>>(client.GetStringAsync(urlCustomer + "total").Result);
            return Json(model);
        }

        [HttpGet("AvgAddress")]
        public IActionResult getAvgAddress()
        {
            var model = JsonConvert.DeserializeObject<List<AddressByCustomer>>(client.GetStringAsync(urlCustomer + "AvgAddress").Result);
            return Json(model);
        }

    }
}
