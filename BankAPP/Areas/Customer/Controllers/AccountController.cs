using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using BankModel;
using BankModel.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using Twilio.TwiML.Voice;

namespace BankAPP.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private string urlCustomer = "http://localhost:5032/api/Customer/";
        private string urlNotification = "http://localhost:5032/api/Notification/";
        private string urlTransaction = "http://localhost:5032/api/Transaction/";
        private string urlAccount = "http://localhost:5032/api/Account/";
        private string urlCard = "http://localhost:5032/api/Card/";
        private string urlCheque = "http://localhost:5032/api/Cheque/";
        private string urlMessage = "http://localhost:5032/api/Message/";
        HttpClient client = new HttpClient();

        public void TemDataListNotification()
        {
            if (User.Identity.IsAuthenticated)
            {
                var clams = (ClaimsIdentity)User.Identity;
                var CustomerId = int.Parse(clams.FindFirst(ClaimTypes.NameIdentifier).Value);
                var listNotification = JsonConvert.DeserializeObject<IEnumerable<Notifications>>(client.GetStringAsync(urlNotification + CustomerId).Result).OrderByDescending(x => x.TransactionId);
                var customer = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + CustomerId).Result);
                var listMess = JsonConvert.DeserializeObject<IEnumerable<Message>>(client.GetStringAsync(urlMessage).Result);
                Helper.Put(TempData, "ListNotification", listNotification);
                Helper.Put(TempData, "CustomerAvatar", customer.Avatar);
                Helper.Put(TempData, "listMess", listMess);
            }
        }

        [HttpGet("login")]
        public IActionResult Login(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.url = returnUrl;
                return View();
            }
            else
            {
                ViewBag.url = "";
                return View();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(BankModel.Customer cus, string? returnUrl)
        {
            try
            {
                if (cus.UserName != null)
                {
                    var model = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + cus.UserName).Result);
                    if (model != null)
                    {
                        if (model.Status.Equals("normal"))
                        {
                            cus.Password = Helper.sha256_hash(cus.Password);
                            if (model.Password.Equals(cus.Password))
                            {
                                model.Attempt = 0;
                                await client.PutAsJsonAsync<BankModel.Customer>(urlCustomer, model);
                                var claim = new List<Claim>();
                                claim.Add(new Claim(ClaimTypes.Name, cus.UserName));
                                claim.Add(new Claim(ClaimTypes.NameIdentifier, model.CustomerId.ToString()));
                                claim.Add(new Claim(ClaimTypes.Role, "User"));
                                var claimIdentify = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                                var claimPrincipal = new ClaimsPrincipal(claimIdentify);
                                await HttpContext.SignInAsync(claimPrincipal);

                                return Json(new { status = true, msg = "Login Success", url = returnUrl });
                            }
                            else
                            {
                                model.Attempt += 1;
                                if (model.Attempt > 2)
                                {
                                    model.Status = "lock";
                                    var cussUpdate = client.PutAsJsonAsync(urlCustomer, model).Result;
                                    return Json(new { status = false, msg = "Your account had locked! Please contact Banking to unlock" });
                                }
                                var cusUpdate = client.PutAsJsonAsync(urlCustomer, model).Result;
                                return Json(new { status = false, msg = "Wrong password! Your can enter more " + (3 - model.Attempt) + " times" });
                            }
                        }
                        else
                        {
                            return Json(new { status = false, msg = "Your account had locked! Please contact Banking to unlock" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Can't find your username" });
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IActionResult> Logout()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            await HttpContext.SignOutAsync();
            return RedirectToAction("login");
        }

        public async Task<IActionResult> Profile(int pg = 1)
        {
            TemDataListNotification();
            try
            {
                var claim = (ClaimsIdentity)User.Identity;
                var id = int.Parse(claim.FindFirst(ClaimTypes.NameIdentifier).Value);
                var cus = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + id).Result);
                var account = JsonConvert.DeserializeObject<List<Account>>(client.GetStringAsync(urlAccount + id).Result);
                var card = JsonConvert.DeserializeObject<List<Card>>(client.GetStringAsync(urlCard + account.FirstOrDefault().AccountId).Result);
                ViewBag.acc = new SelectList(account, "AccountNo", "AccountNo");
                var trans = JsonConvert.DeserializeObject<List<Transaction>>(client.GetStringAsync(urlTransaction + account.FirstOrDefault().AccountNo).Result).OrderByDescending(x => x.TransactionId);
                const int pageSize = 5;
                if (pg < 1)
                {
                    pg = 1;
                }
                int resCount = trans.Count();
                var pager = new Paging(resCount, pg, pageSize);
                var recSkip = (pg - 1) * pageSize;
                var data = trans.Skip(recSkip).Take(pager.PageSize).ToList();
                ViewBag.page = pager;
                ProfileModel model = new ProfileModel
                {
                    Card = card,
                    Accounts = account,
                    Transactions = data,
                    Customer = cus
                };
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("NotFound", "Other");
            }

        }

        [HttpGet("DetailTransaction")]
        public async Task<IActionResult> DetailTransaction(int id)
        {
            TemDataListNotification();
            var model = JsonConvert.DeserializeObject<Transaction>(client.GetStringAsync(urlTransaction + id).Result);
            return View(model);
        }

        [HttpGet("TransactionRender")]
        public async Task<IActionResult> TransactionRender(string account, int pg)
        {
            var trans = JsonConvert.DeserializeObject<List<Transaction>>(client.GetStringAsync(urlTransaction + account).Result).OrderByDescending(x => x.TransactionId);
            var acc = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + account).Result);
            var card = JsonConvert.DeserializeObject<List<Card>>(client.GetStringAsync(urlCard + acc.AccountId).Result);
            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }
            int resCount = trans.Count();
            var pager = new Paging(resCount, pg, pageSize);
            var recSkip = (pg - 1) * pageSize;
            var data = trans.Skip(recSkip).Take(pager.PageSize).ToList();
            return Json(new { trans = data, pager = pager });
        }

        [HttpGet("CardRender")]
        public async Task<IActionResult> CardRender(string account)
        {
            var acc = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + account).Result);
            var card = JsonConvert.DeserializeObject<List<Card>>(client.GetStringAsync(urlCard + acc.AccountId).Result);
            return Json(new { card = card, account = acc });
        }

        [HttpGet("SearchByDate")]
        public async Task<IActionResult> SearchByDate(string fromDate, string toDate, string accountNo, int pg)
        {
            var model = JsonConvert.DeserializeObject<List<Transaction>>(client.GetStringAsync(urlTransaction + fromDate + "/" + toDate + "/" + accountNo).Result).OrderByDescending(x => x.TransactionId);
            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }
            int resCount = model.Count();
            var pager = new Paging(resCount, pg, pageSize);
            var recSkip = (pg - 1) * pageSize;
            var data = model.Skip(recSkip).Take(pager.PageSize).ToList();
            return Json(new { trans = data, pager = pager });
        }

        [HttpPost("UpdateAvatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            try
            {
                var extension = Path.GetExtension(file.FileName);
                if (extension.Equals(".png") || extension.Equals(".jpg") || extension.Equals(".jepg"))
                {
                    var claim = (ClaimsIdentity)User.Identity;
                    var id = int.Parse(claim.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var cus = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + id).Result);
                    var oldPath = Path.Combine("wwwroot/images/", cus.Avatar);
                    if (System.IO.File.Exists(oldPath) && cus.Avatar != "userdefault.png")
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    string newFileName = cus.UserName+extension;
                    var path = Path.Combine("wwwroot/images/", newFileName);                   
                    try
                    {
                        using (var stream = System.IO.File.Create(path))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        NoContent();
                    }

                    cus.Avatar = newFileName;
                    var model = client.PutAsJsonAsync<BankModel.Customer>(urlCustomer, cus).Result;
                    if (model.IsSuccessStatusCode)
                    {
                        return Json(new { status = true, msg = "Update avatar success" });
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Update avatar failed" });
                    }
                }
                else
                {
                    return Json(new { status = false, msg = "File must be png,jepg or jpg" });
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("NotFound", "Other");
            }
        }

        [HttpGet("PrintPdf")]
        public JsonResult GetPdfView(string accountNo, string? fromDate, string? toDate)
        {
            object model = "";
            var findTransaction = JsonConvert.DeserializeObject<List<Transaction>>(client.GetStringAsync(urlTransaction + accountNo).Result);
            if (findTransaction.Count() > 0)
            {
                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    model = JsonConvert.DeserializeObject(client.GetStringAsync(urlTransaction + "printPdf/" + fromDate + "/" + toDate + "/" + accountNo).Result);
                }
                else
                {
                    model = JsonConvert.DeserializeObject(client.GetStringAsync(urlTransaction + "printPdf/" + accountNo).Result);
                }
                return Json(new { status = true, data = model, msg = "Export success" });
            }
            else
            {
                return Json(new { status = false, msg = "Don't have any transaction to print" });
            }
        }

        //[HttpPost("RequestCheque")]
        //public async Task<IActionResult> PostCheque(Cheque cheque)
        //{
        //    var account = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + "GetAccountById/" + cheque.AccountId).Result);
        //    var model = client.PostAsJsonAsync<Cheque>(urlCheque, cheque).Result;
        //    if (model.IsSuccessStatusCode)
        //    {
        //        account.Balance -= cheque.Amount;
        //        var updateBalance = client.PutAsJsonAsync(urlCheque, account).Result;
        //        if (updateBalance.IsSuccessStatusCode)
        //        {
        //            return Json(new { msg = "Request Cheque Success", status = true });
        //        }
        //        else
        //        {
        //            return Json(new { msg = "Something wrong", status = false });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { msg = "Something wrong", status = false });
        //    }
        //}

        [HttpGet("RenderCheque")]
        public async Task<IActionResult> ListCheque(string account, int pg = 1)
        {
            var acc = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + account).Result);
            var model = JsonConvert.DeserializeObject<List<Cheque>>(client.GetStringAsync(urlCheque + "ListbyAccount/" + acc.AccountId).Result).OrderByDescending(x => x.Id);
            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }
            int resCount = model.Count();
            var pager = new Paging(resCount, pg, pageSize);
            var recSkip = (pg - 1) * pageSize;
            var data = model.Skip(recSkip).Take(pager.PageSize).ToList();
            return Json(new { cheque = data, pager = pager });
        }

        [HttpGet("DeleteCheque")]
        public async Task<IActionResult> DeleteCheque(int id)
        {
            var model = client.DeleteAsync(urlCheque + id).Result;
            if (model.IsSuccessStatusCode)
            {
                return Json(new { status = true, msg = "Delete Success" });
            }
            else
            {
                return Json(new { status = false, msg = "Something wrong" });
            }
        }

        [HttpGet("CancelCheque")]
        public async Task<IActionResult> CancelCheque(int id)
        {
            var model = JsonConvert.DeserializeObject<Cheque>(client.GetStringAsync(urlCheque + "GetOne/" + id).Result);
            var account = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + "GetAccountById/" + model.AccountId).Result);
            model.Status = "cancel";
            var updateCheque = client.PutAsJsonAsync(urlCheque, model).Result;
            if (updateCheque.IsSuccessStatusCode)
            {
                account.Balance += model.Amount;
                var updateBalance = client.PutAsJsonAsync<Account>(urlAccount, account).Result;
                if (updateBalance.IsSuccessStatusCode)
                {
                    return Json(new { status = true, msg = "Cancel Success" });
                }
                else
                {
                    return Json(new { status = false, msg = "Something wrong" });
                }
            }
            else
            {
                return Json(new { status = false, msg = "Something wrong" });
            }
        }

        [HttpPost("ReciveRequestCheque")]
        public async Task<IActionResult> GetRequest(Cheque cheque, string accountNo)
        {
            var account = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + accountNo).Result);
            cheque.AccountId = account.AccountId;
            if (account.Balance >= cheque.Amount)
            {
                if (!account.Status.Equals("lock"))
                {
                    var model = client.PostAsJsonAsync(urlCheque, cheque).Result;
                    if (model.IsSuccessStatusCode)
                    {
                        account.Balance -= cheque.Amount;
                        var updateAccount = client.PutAsJsonAsync(urlAccount, account).Result;
                        if (updateAccount.IsSuccessStatusCode)
                        {
                            return Json(new { status = true, msg = "Sent Request Success", balance = account.Balance });
                        }
                        else
                        {
                            return Json(new { status = false, msg = "Sent Request Fail" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Sent Request Fail" });
                    }
                }
                else
                {
                    return Json(new { status = false, msg = "Your Account has locked! Can't request cheque" });
                }
            }
            else
            {
                return Json(new { status = false, msg = "Your balance don't have enough" });
            }
        }

        [HttpGet("ChequeAccountRender")]
        public async Task<IActionResult> GetAccountCheque(string accountNo)
        {
            var account = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + accountNo).Result);
            return Json(account);
        }

        [HttpPost("ChangePinCode")]
        public async Task<IActionResult> ChangePinCode(string accountNo, string currentPin, string newPin)
        {
            var model = JsonConvert.DeserializeObject<Account>(client.GetStringAsync(urlAccount + accountNo).Result);
            if (!string.IsNullOrEmpty(currentPin) && !string.IsNullOrEmpty(newPin))
            {
                if (currentPin.Equals(model.PinCode))
                {
                    if (newPin != currentPin)
                    {
                        model.PinCode = newPin;
                        var updatePincode = client.PutAsJsonAsync(urlAccount, model);
                        return Json(new { status = true, msg = "Change Pin Code Success" });
                    }
                    else
                    {
                        return Json(new { status = false, msg = "New pin is same Current pin" });
                    }
                }
                else
                {
                    return Json(new { status = false, msg = "Current Pin Has Wrong" });
                }
            }
            else
            {
                return Json(new { status = false, msg = "Current Pin and New Pin not blank" });
            }

        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string customerId, string currentPassword, string newPassword, string confirmPassword)
        {
            var model = JsonConvert.DeserializeObject<BankModel.Customer>(client.GetStringAsync(urlCustomer + customerId).Result);
            if (!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword))
            {
                currentPassword = Helper.sha256_hash(currentPassword);
                if (currentPassword.Equals(model.Password))
                {
                    if (Helper.sha256_hash(newPassword) != currentPassword)
                    {
                        if (confirmPassword.Equals(newPassword))
                        {
                            model.Password = Helper.sha256_hash(newPassword);
                            var updatePassword = client.PutAsJsonAsync(urlCustomer, model).Result;
                            if (updatePassword.IsSuccessStatusCode)
                            {
                                return Json(new { status = true, msg = "Change password success" });
                            }
                            else
                            {
                                return Json(new { status = false, msg = "Change Password Fail" });
                            }
                        }
                        else
                        {
                            return Json(new { status = false, msg = "Confirm password is not same new Password" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, msg = "New Password is same current password" });
                    }
                }
                else
                {
                    return Json(new { status = false, msg = "Current password wrong" });
                }
            }
            else
            {
                return Json(new { status = false, msg = "All fill not blank" });
            }

        }
    }
}

