using BankAPP.Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace BankAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = "AdminAuth")]
    public class AccountController : Controller
    {
        private string urlAccount = "http://localhost:5032/api/Account/";
        private string urlAdmin = "http://localhost:5032/api/Admin/";
        private string urlContact = "http://localhost:5032/api/Contact/";
        private string urlCustomer = "http://localhost:5032/api/Customer/";
        private HttpClient client = new HttpClient();
        Random random = new Random();
        private IMailService _mail;

        public AccountController(IMailService mail)
        {
            _mail = mail;
        }

        public void listContact()
        {
            var listContact = JsonConvert.DeserializeObject<IEnumerable<BankModel.Contact>>(client.GetStringAsync(urlContact).Result);
            Helper.Put(TempData, "listContact", listContact);
        }

        public IActionResult Index()
        {
            var acc = JsonConvert.DeserializeObject<List<BankModel.Account>>(client.GetStringAsync(urlAccount).Result);
            return View(acc);
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAccount(BankModel.Account newAccount)
        {
            try
            {
                var accounts = JsonConvert.DeserializeObject<List<BankModel.Account>>(client.GetStringAsync(urlAccount + newAccount.CustomerId).Result);
                if (accounts.Count() >= 2)
                {
                    return Json(new { status = false, message = "You are only have 2 Account" });
                }
                else
                {
                    var res = JsonConvert.DeserializeObject<BankModel.Account>(client.GetStringAsync(urlAccount + newAccount.AccountNo).Result);
                    if (res == null)
                    {
                        newAccount.PinCode = random.Next(0, 9999).ToString("D4");
                        var model = client.PostAsJsonAsync<BankModel.Account>(urlAccount, newAccount).Result;
                        if (model.IsSuccessStatusCode)
                        {
                            var customerId = newAccount.CustomerId;
                            var customer = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + customerId).Result);
                            string reciverEmail = customer.Email;
                            string subject = "Pincode for your new AccountNo: " + newAccount.AccountNo;
                            StringBuilder body = new StringBuilder();
                            body.AppendLine($"Welcome {customer.CustomerName}, you have been create new Acount in Ebank successfully. ");
                            body.AppendLine($"Your new AccountNo: {newAccount.AccountNo} and Pincode: {newAccount.PinCode}");
                            BankModel.Mail.MailRequest mailRequest = new BankModel.Mail.MailRequest()
                            {
                                UserName = customer.CustomerName,
                                UserContent = subject,
                                ToEmail = reciverEmail,
                                Body = body.ToString()
                            };
                            var status = await _mail.SendEmailAsync(mailRequest);
                            return RedirectToAction("Details", "Customer", new {id = newAccount.CustomerId});
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "Something went wrong, try again later");
                            listContact();
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Account is existed");
                        listContact();
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                listContact();
                return View();
            }
        }

        [HttpGet("/CreateAccount")]
        public async Task<IActionResult> Create(int CustomerId, string AccountNo, double Balance)
        {
            BankModel.Account account = new BankModel.Account
            {
                CustomerId = CustomerId,
                AccountNo = AccountNo,
                Balance = Balance,
            };
            try
            {
                var accounts = JsonConvert.DeserializeObject<List<BankModel.Account>>(client.GetStringAsync(urlAccount + account.CustomerId).Result);
                if (accounts.Count() >= 2)
                {
                    return Json(new { status = false, message = "You are only have 2 Account" });
                }
                else
                {
                    var res = JsonConvert.DeserializeObject<BankModel.Account>(client.GetStringAsync(urlAccount + account.AccountNo).Result);
                    if (res == null)
                    {
                        account.PinCode = random.Next(0, 9999).ToString("D4");
                        var model = client.PostAsJsonAsync<BankModel.Account>(urlAccount, account).Result;
                        if (model.IsSuccessStatusCode)
                        {
                            var customerId = account.CustomerId;
                            var customer = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + customerId).Result);
                            string reciverEmail = customer.Email;
                            string subject = "Pincode for your new AccountNo: " + account.AccountNo;
                            StringBuilder body = new StringBuilder();
                            body.AppendLine($"Welcome {customer.CustomerName}, you have been create new Acount in Ebank successfully. ");
                            body.AppendLine($"Your new AccountNo: {account.AccountNo} and Pincode: {account.PinCode}");
                            BankModel.Mail.MailRequest mailRequest = new BankModel.Mail.MailRequest()
                            {
                                UserName = customer.CustomerName,
                                UserContent = subject,
                                ToEmail = reciverEmail,
                                Body = body.ToString()
                            };
                            var status = await _mail.SendEmailAsync(mailRequest);

                            return Json(new { status = true, message = "Your Account have been created" });
                        }
                        else
                        {
                            return Json(new { status = false, message = "Something went wrong! please try again later" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, message = $"Account: {account.AccountNo} is existed" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Something went wrong! please try again later" });
            }
        }


        [HttpGet("loginAdmin")]
        [AllowAnonymous]
        public IActionResult LoginAdmin(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.returnUrdl = returnUrl;
                return View();
            }
            else
            {
            ViewBag.returnUrdl = "";
            return View();
            }

        }

        [HttpPost("loginAdmin")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAdminAsync(string UserName, string Password, string? returnUrl)
        {
            await HttpContext.SignOutAsync();
            var res = JsonConvert.DeserializeObject<BankModel.Admin>(client.GetStringAsync(urlAdmin + UserName + "/" + Password).Result);
            if (res != null)
            {
                var claim = new List<Claim>();
                claim.Add(new Claim(ClaimTypes.Name, res.UserName));
                claim.Add(new Claim(ClaimTypes.NameIdentifier, res.AdminId.ToString()));
                claim.Add(new Claim(ClaimTypes.Role, "Admin"));
                var claimIdentify = new ClaimsIdentity(claim, "AdminAuth");
                var claimPrincipal = new ClaimsPrincipal(claimIdentify);
                await HttpContext.SignInAsync("AdminAuth", claimPrincipal);
                if (returnUrl != null)
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Chart");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> logoutAdmin()
        {
            await HttpContext.SignOutAsync("AdminAuth");
            return RedirectToAction("loginAdmin", "Account");
        }


        #region "API_Calls"

        [HttpGet("isExitedAccount")]
        public bool isExited(string accountNo)
        {
            var account = JsonConvert.DeserializeObject<BankModel.Account>(client.GetStringAsync(urlAccount + accountNo).Result);
            if (account != null)
            {
                return true;
            }
            return false;
        }

        [HttpGet("disableAccount")]
        public async Task<IActionResult> disableAccount(string accountNo, bool status)
        {
            var account = JsonConvert.DeserializeObject<BankModel.Account>(client.GetStringAsync(urlAccount + accountNo).Result);
            if (account != null)
            {
                if (status)
                {
                    account.Status = "lock";
                    var model = client.PutAsJsonAsync<BankModel.Account>(urlAccount, account).Result;
                    if (model.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, status = "lock" , msg = "This account have been DISABLE !" });
                    }
                    return Json(new { success = false, msg = "Cannot enable/disable this account, try again later !" });
                }
                else
                {
                    account.Status = "normal";
                    var model = client.PutAsJsonAsync<BankModel.Account>(urlAccount, account).Result;
                    if (model.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, status = "normal", msg = "This account have been ENABLE !" });
                    }
                    return Json(new { success = false, msg = "Cannot enable/disable this account, try again later !" });
                }
            }
            else
            {
                return Json(new { success = false, msg = "Can not find this account, try again later !" });
            }

        }

        #endregion
    }
}
