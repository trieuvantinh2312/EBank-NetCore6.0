using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using BankAPP.Mail;
using BankAPP.SignalRServer;
using BankModel;
using BankModel.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace BankAPP.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private string urlAccount = "http://localhost:5032/api/Account/";
        private string urlCustomer = "http://localhost:5032/api/Customer/";
        private string urlTransaction = "http://localhost:5032/api/Transaction/";
        private string urlContact = "http://localhost:5032/api/Contact/";
        private string urlNotification = "http://localhost:5032/api/Notification/";
        private string urlNews = "http://localhost:5032/api/News/";
        private string urlConfiguration = "http://localhost:5032/api/Configuration/";
        private string urlMessage = "http://localhost:5032/api/Message/";
        private string urlAdmin = "http://localhost:5032/api/Admin/";
        private HttpClient client = new HttpClient();
        private IHubContext<Notification> hubContext;
        private IMailService _mail;
        public HomeController(IHubContext<Notification> hubContext, IMailService _mail)
        {
            this.hubContext = hubContext;
            this._mail = _mail;
        }
        public void TemDataListNotification() {
            if (User.Identity.IsAuthenticated)
            {
                var clams = (ClaimsIdentity)User.Identity;
                var CustomerId = int.Parse(clams.FindFirst(ClaimTypes.NameIdentifier).Value);
                var listNotification = JsonConvert.DeserializeObject<IEnumerable<Notifications>>(client.GetStringAsync(urlNotification + CustomerId).Result).OrderByDescending(x=>x.TransactionId);
                var listMess = JsonConvert.DeserializeObject<IEnumerable<Message>>(client.GetStringAsync(urlMessage).Result);
                var customer = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + CustomerId).Result);
                Helper.Put(TempData, "CustomerAvatar", customer.Avatar);
                Helper.Put(TempData, "ListNotification", listNotification);
                Helper.Put(TempData, "listMess", listMess);
            }
        }
        public IActionResult Index()
        {
            TemDataListNotification();
            var model = JsonConvert.DeserializeObject<List<News>>(client.GetStringAsync(urlNews).Result).TakeLast(10);
            if(model != null)
            {
                return View(model);
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult TransferMoney()
        {
            var clams = (ClaimsIdentity)User.Identity;
            var CustomerId = clams.FindFirst(ClaimTypes.NameIdentifier).Value;
            var model = JsonConvert.DeserializeObject<IEnumerable<Account>>(client.GetStringAsync(urlAccount + int.Parse(CustomerId)).Result);
            //ViewBag.Account = new SelectList(model, "AccountNo", "AccountNo");
            ViewBag.Account = model;
            ViewBag.Customer = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer+CustomerId).Result).CustomerName;
            TemDataListNotification();
            var configuration = JsonConvert.DeserializeObject<ConfigurationTransfer>(client.GetStringAsync(urlConfiguration).Result);
            ViewBag.configuration = configuration;
            return View();
        }



        [HttpGet]
        public IActionResult AboutUs()
        {
            TemDataListNotification();
            return View();
        }

        [HttpGet("ContactUs")]
        public IActionResult ContactUs()
        {
            TemDataListNotification();
            if (User.Identity.IsAuthenticated)
            {
                var claim = (ClaimsIdentity)User.Identity;
                var id = int.Parse(claim.FindFirst(ClaimTypes.NameIdentifier).Value);
                var cus = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + id).Result);
                return View(cus);
            }
            else
            {
                return View();
            }
        }
        [HttpPost("ContactUs")]
        public async Task<IActionResult> ContactUs(Contact contact)
        {
            string[] autoReply = { "Account", "Money", "Help", "Interest" };
            foreach (var item in autoReply)
            {
                if (contact.Description.Contains(item))
                {
                    var mail = new BankModel.Mail.MailRequest
                    {
                        UserContent = contact.Description,
                        ToEmail = contact.Email,
                        Body = "Please go to bank for your issue! Thank You",
                        UserName = contact.UserContact
                    };
                    var status = await _mail.SendEmailAsync(mail);
                    if (status == "success")
                    {
                        return Json(new { success = true, msg = "Please check your mail for information" });
                    }
                    else
                    {
                        return Json(new { success = false, msg = "Something wrong contact with Bank" });
                    }
                }
                else
                {
                    var claim = (ClaimsIdentity)User.Identity;
                    var id = int.Parse(claim.FindFirst(ClaimTypes.NameIdentifier).Value);
                    contact.CustomerId = id;
                    var model = client.PostAsJsonAsync<Contact>(urlContact, contact).Result;
                    var listContact = JsonConvert.DeserializeObject<IEnumerable<Contact>>(client.GetStringAsync(urlContact).Result);
                    hubContext.Clients.All.SendAsync("sendContact", listContact);
                    if (model.IsSuccessStatusCode)
                    {
                        return Json(new { status = true, msg = "Sent success" });
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Something wrong contact with Bank" });
                    }
                }
            }
            return View();
        }

        [HttpGet("Blog")]
        public IActionResult Blog(int pg = 1)
        {
            TemDataListNotification();
            BlogRender(pg);
            return View();
        }

        [HttpGet("BlogRender")]
        public IActionResult BlogRender(int pg)
        {
            const int pageSize = 6;
            var model = JsonConvert.DeserializeObject<List<News>>(client.GetStringAsync(urlNews).Result).Where(x => x.Status.Equals("Active")).OrderByDescending(x => x.NewId);
            if (pg < 1)
            {
                pg = 1;
            }
            int resCount = model.Count();
            var pager = new Paging(resCount, pg, pageSize);
            var recSkip = (pg - 1) * pageSize;
            var data = model.Skip(recSkip).Take(pager.PageSize).ToList();
            return Json(new { render = data, pager = pager });
        }

        [HttpGet("NewsDetail")]
        public IActionResult BlogDetails(int id)
        {
            try
            {
                TemDataListNotification();
                var model = JsonConvert.DeserializeObject<News>(client.GetStringAsync(urlNews + id).Result);
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("NotFound", "Other");
            }
        }

        [HttpGet("Tranfers_success")]
        public IActionResult Transfer_Success(int transactionId)
        {
            TemDataListNotification();
            var transaction = JsonConvert.DeserializeObject<Transaction>(client.GetStringAsync(urlTransaction + transactionId).Result);
            return View(transaction);
        }
        
        #region API CALLS
        [HttpGet("BindingToAccount")]
        public IActionResult BindingToAccount(string toAccount)
        {
            var clams = (ClaimsIdentity)User.Identity;
            var CustomerId = int.Parse(clams.FindFirst(ClaimTypes.NameIdentifier).Value);
            var customerSender = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + CustomerId).Result);
            var listAccount = JsonConvert.DeserializeObject<IEnumerable<Account>>(client.GetStringAsync(urlAccount+customerSender.CustomerId).Result);
            var accountCheck = listAccount.SingleOrDefault(x => x.AccountNo.Equals(toAccount));
            if (accountCheck==null)
            {
                var account = JsonConvert.DeserializeObject<BankModel.Account>(client.GetStringAsync(urlAccount + toAccount).Result);
                if (account != null)
                {
                    var customerReceiver = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + account.CustomerId).Result);
                    return Json(new { success = true, data = customerReceiver.CustomerName });
                }
                else
                {
                    return Json(new { success = false, data = "Not found" });
                }
            }
            else {
                return Json(new { success = false, data = "Cannot transfer to your account" });
            }
            
        }
        [HttpGet("checkAmount")]
        [Authorize]
        public IActionResult checkAmount(string fromAccount, double amount)
        {
            var Account = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + fromAccount).Result);
            var configuration = JsonConvert.DeserializeObject<ConfigurationTransfer>(client.GetStringAsync(urlConfiguration).Result);
            if (amount >= configuration.MinValue && amount <= configuration.MaxValue)
            {
                if (amount < Account.Balance)
                {
                    return Json(new { success = true });
                }
                else
                {
                    var msg = "Your balance not enough!";
                    return Json(new { success = false, msg = msg });
                }
                
            }
            else
            {
                var msg = "A per transfer min " + configuration.FormatMin + ", max " + configuration.FormatMax;
                return Json(new { success = false, msg = msg });
            }
            

        }
        [HttpPost("PostTransaction")]
        [Authorize]
        public async Task<IActionResult> PostTransaction(Transaction transaction)
        {
            var model = client.PostAsJsonAsync<Transaction>(urlTransaction, transaction).Result;
            var FromAccount = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + transaction.FromAccount).Result);
            var ToAccount = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + transaction.ToAccount).Result);
            //var getPhoneFrom = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + FromAccount.CustomerId).Result);
            //var getPhoneTo= JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + ToAccount.CustomerId).Result);
            if (model.IsSuccessStatusCode)

            {
                FromAccount.Balance -= transaction.Amount;
                await client.PutAsJsonAsync<Account>(urlAccount, FromAccount);
                ToAccount.Balance += transaction.Amount;
                client.PutAsJsonAsync<Account>(urlAccount, ToAccount); 
                var respons = await model.Content.ReadAsStringAsync();
                var modelTransaction = JsonConvert.DeserializeObject<Transaction>(respons);
                Notifications noti = new Notifications {
                    FromCustomerId = FromAccount.CustomerId,
                    ToCustomerId = ToAccount.CustomerId,
                    FromAccountId = FromAccount.AccountNo,
                    ToAccountId = ToAccount.AccountNo,
                    TransactionId = modelTransaction.TransactionId,
                    Amount = transaction.Amount
                    
                };
                
                await client.PostAsJsonAsync<Notifications>(urlNotification, noti);
                var listNotification = JsonConvert.DeserializeObject<IEnumerable<Notifications>>(client.GetStringAsync(urlNotification).Result);
                await hubContext.Clients.All.SendAsync("sendNotification",listNotification);
                await hubContext.Clients.All.SendAsync("sendAccountBalance", FromAccount);
                //string accountSid = "AC5915bd862f44e2cdaf42a0ac64a5f314";
                //string authToken ="5186e1b5fd8459bb21f71642fa294d60";
                //TwilioClient.Init(accountSid, authToken);
                //var messageFrom = MessageResource.Create(
                //    body: "Transaction Code: "+modelTransaction.TransactionCode+" in "+modelTransaction.FormatDate+" you've transfer to "+transaction.ToAccount+" - " +transaction.FormatAmount+" Your balance: "+FromAccount.FormatAmount,
                //    from: new Twilio.Types.PhoneNumber("+14632413886"),
                //    to: new Twilio.Types.PhoneNumber("+84"+getPhoneFrom.Phone)
                //);
                //var messageTo = MessageResource.Create(
                //    body: "Transaction Code: " + modelTransaction.TransactionCode +" in " + modelTransaction.FormatDate +" you've received from "+transaction.FromAccount +" + " + transaction.FormatAmount +" with description: "+transaction.Description+". Your balance " + ToAccount.FormatAmount,
                //    from: new Twilio.Types.PhoneNumber("+14632413886"),
                //    to: new Twilio.Types.PhoneNumber("+84"+getPhoneTo.Phone)
                //);
                return Json(new { success = true,url= "/Tranfers_success?transactionId="+ modelTransaction.TransactionId });    
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpGet("CheckPinCode")]
        [Authorize]
        public IActionResult CheckPinCode(string pincode, string account)
        {
            var Account = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + account).Result);
            if (Account.Status.Equals("normal"))
            {
                if (pincode == Account.PinCode)
                {
                    return Json(new { success = true });
                }
                else
                {
                    Account.Attempt += 1;
                    if (Account.Attempt > 2)
                    {
                        Account.Status = "block";
                    }

                    var model = client.PutAsJsonAsync<Account>(urlAccount, Account).Result;
                    return Json(new { success = false, msg = "Wrong pincode! You can enter more " + (3 - Account.Attempt) + " times" });
                }
            }
            else
            {
                return Json(new { success = false, msg = "Your Account had locked!" });

            }
        }

        [HttpGet("checkEmail")]
        public async Task<IActionResult> checkEmail(string email) {
            var apiUrl = "https://apps.emaillistverify.com/api/verifyEmail";
            var apiKey = "5rdbWl02VyZSACKq1SaHW";
            var requestUrl = $"{apiUrl}?secret={apiKey}&email={email}";

            var request = WebRequest.Create(requestUrl);
            var response = await request.GetResponseAsync();

            string emailStatus;
            using (var responseStream = response.GetResponseStream())
            {
                var responseReader = new StreamReader(responseStream);
                emailStatus = responseReader.ReadToEnd();
            }
            if (emailStatus == "ok")
            {
                return Json(new { success = true });
            }
            else {
                return Json(new { success = false });
            }
        }
        [HttpGet("Save_Transaction")]
        public async Task<IActionResult> Save_Transaction(int transactionId)
        {
            var clams = (ClaimsIdentity)User.Identity;
            var CustomerId = int.Parse(clams.FindFirst(ClaimTypes.NameIdentifier).Value);
            var customer = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + CustomerId).Result);
            var transaction = JsonConvert.DeserializeObject<Transaction>(client.GetStringAsync(urlTransaction + transactionId).Result);
            MailRequest mailRequest = new MailRequest
            {
                UserName = customer.UserName,
                ToEmail = customer.Email,
                UserContent = "",
                Body = ""
            };
            var status = await _mail.SendEmailAsyncTransaction(mailRequest, transaction);
            if (status == "success")
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost("PostMessage")]
        public async Task<IActionResult> PostMessage(string text_messasge)
        {
            var clams = (ClaimsIdentity)User.Identity;
            var CustomerId = int.Parse(clams.FindFirst(ClaimTypes.NameIdentifier).Value);
            //var customer = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + CustomerId).Result);
            var admin = JsonConvert.DeserializeObject<BankModel.Admin>(client.GetStringAsync(urlAdmin).Result);
            Message message = new Message {
                Sender = CustomerId,
                Receiver = admin.AdminId,
                MessageChat = text_messasge
            };
            var postMess = client.PostAsJsonAsync<BankModel.Message>(urlMessage, message).Result;
            if (postMess.IsSuccessStatusCode)
            {
                var listMess = JsonConvert.DeserializeObject<IEnumerable<Message>>(client.GetStringAsync(urlMessage).Result).Where(x => x.Sender.Equals(CustomerId) || x.Receiver.Equals(CustomerId));
                await hubContext.Clients.All.SendAsync("listMess",listMess, CustomerId);
                return Json(new { success=true});
            }
            else {
                return Json(new { success = false });
            }
            
        }
        [HttpGet("bindingBalance")]
        public IActionResult bindingBalance(string accountNo) {
            var account = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + accountNo).Result);
            if (account != null)
            {
                return Json(new { success = true, balance = account.FormatAmount });
            }
            else {
                return Json(new { success = false});
            }

        }
        #endregion
    }
}
