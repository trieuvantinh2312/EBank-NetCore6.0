using System.Security.Claims;
using BankAPP.Mail;
using BankAPP.SignalRServer;
using BankModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace BankAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = "AdminAuth")]
    public class HomeController : Controller
    {
        private string urlContact = "http://localhost:5032/api/Contact/";
        private string urlCheque = "http://localhost:5032/api/Cheque/";
        private string urlNews = "http://localhost:5032/api/News/";
        private string urlCustomer = "http://localhost:5032/api/Customer/";
        private string urlMessage = "http://localhost:5032/api/Message/";
        private string urlAdmin = "http://localhost:5032/api/Admin/";
        private HttpClient client = new HttpClient();
        private IHubContext<Notification> hubContext;
        private IMailService _mail;
        public HomeController(IMailService _mail, IHubContext<Notification> hubContext)
        {
            this._mail = _mail;
            this.hubContext = hubContext;
        }
        public void listContact()
        {
            var listContact = JsonConvert.DeserializeObject<IEnumerable<BankModel.Contact>>(client.GetStringAsync(urlContact).Result);
            Helper.Put(TempData, "listContact", listContact);
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Chart");
        }

        public IActionResult CreateProduct()
        {
            listContact();
            return View();
        }


        public IActionResult ShowCustomers()
        {
            listContact();
            return View();
        }

        public IActionResult Contact()
        {
            listContact();
            var listContacts = JsonConvert.DeserializeObject<IEnumerable<BankModel.Contact>>(client.GetStringAsync(urlContact).Result);
            ViewBag.listContWaitting = listContacts.Where(x => x.Status == "waiting");
            ViewBag.listContSuccess = listContacts.Where(x => x.Status != "waiting");
            return View();
        }

        [HttpGet("Replay_Contact")]
        public IActionResult Replay_Contact(int contactId)
        {
            listContact();
            var contact = JsonConvert.DeserializeObject<BankModel.Contact>(client.GetStringAsync(urlContact + contactId).Result);
            return View(contact);
        }

        [HttpGet]
        public async Task<IActionResult> NewsIndex()
        {
            listContact();
            var model = JsonConvert.DeserializeObject<List<News>>(client.GetStringAsync(urlNews).Result);
            return View(model);
        }

        [HttpGet("CreateNews")]
        public async Task<IActionResult> CreateNews()
        {
            listContact();
            return View();
        }

        [HttpPost("CreateNews")]
        public async Task<IActionResult> CreateNews(News news, IFormFile mainImage, IFormFile slideImage, int pg = 1)
        {
            listContact();
            string mainImagePath = "";
            string slideImagePath = "";
            if (mainImage != null)
            {
                if (slideImage != null)
                {
                    var mainImageExtension = Path.GetExtension(mainImage.FileName);
                    var slideImageExtension = Path.GetExtension(slideImage.FileName);
                    if (mainImageExtension.Equals(".png") || mainImageExtension.Equals(".jpg") || mainImageExtension.Equals(".jepg"))
                    {
                        if (slideImageExtension.Equals(".png") || slideImageExtension.Equals(".jpg") || slideImageExtension.Equals(".jepg"))
                        {

                            mainImagePath = Path.Combine("wwwroot", "images/NewsImage/MainImage", mainImage.FileName);
                            slideImagePath = Path.Combine("wwwroot", "images/NewsImage/SlideImage", slideImage.FileName);
                            news.ImageMain = "/images/NewsImage/MainImage/" + mainImage.FileName;
                            news.ImageSlide = "/images/NewsImage/SlideImage/" + slideImage.FileName;
                            news.CreatedDate = DateTime.Now;
                            news.Status = "Active";
                            var model = await client.PostAsJsonAsync(urlNews, news);
                            if (model.IsSuccessStatusCode)
                            {
                                var mainFs = new FileStream(mainImagePath, FileMode.Create);
                                var slideFs = new FileStream(slideImagePath, FileMode.Create);
                                await mainImage.CopyToAsync(mainFs);
                                await slideImage.CopyToAsync(slideFs);
                                mainFs.Close();
                                slideFs.Close();
                                var listNews = JsonConvert.DeserializeObject<List<News>>(client.GetStringAsync(urlNews).Result).Where(x => x.Status.Equals("Active")).OrderByDescending(x => x.NewId);
                                const int pageSize = 6;
                                if (pg < 1)
                                {
                                    pg = 1;
                                }
                                int resCount = listNews.Count();
                                var pager = new Paging(resCount, pg, pageSize);
                                var recSkip = (pg - 1) * pageSize;
                                var data = listNews.Skip(recSkip).Take(pager.PageSize).ToList();
                                await hubContext.Clients.All.SendAsync("reloadNews", data, pager);
                                return RedirectToAction("NewsIndex");
                            }
                            else
                            {
                                ViewBag.msg = "Create Failed";
                                return View();
                            }
                        }
                        else
                        {
                            ViewBag.slide = "Slide Image must be png,jpg or jepg";
                            return View();
                        }

                    }
                    else
                    {
                        ViewBag.main = "Main Image must be png,jpg or jepg";
                        return View();
                    }
                }
                else
                {
                    ViewBag.slide = "Slide Image is required";
                    return View();
                }
            }
            else
            {
                ViewBag.main = "Main Image is required";
                ViewBag.slide = "Slide Image is required";
                return View();
            }

        }

        public IActionResult Message()
        {
            listContact();
            var listCustomer = JsonConvert.DeserializeObject<IEnumerable<BankModel.Customer>>(client.GetStringAsync(urlCustomer).Result);

            var admin = JsonConvert.DeserializeObject<BankModel.Admin>(client.GetStringAsync(urlAdmin).Result);
            ViewBag.adminId = admin.AdminId;
            return View(listCustomer);
        }




        [HttpPost("updateNewsStatus")]
        public async Task<IActionResult> UpdateStatusNews(bool active, int id, int pg = 1)
        {
            var model = JsonConvert.DeserializeObject<News>(client.GetStringAsync(urlNews + id).Result);
            if (active == true)
            {
                model.Status = "Active";
            }
            else
            {
                model.Status = "Unactive";
            }
            var putNews = await client.PutAsJsonAsync<News>(urlNews, model);
            if (putNews.IsSuccessStatusCode)
            {
                var listNews = JsonConvert.DeserializeObject<List<News>>(client.GetStringAsync(urlNews).Result).Where(x => x.Status.Equals("Active")).OrderByDescending(x => x.NewId);
                const int pageSize = 6;
                if (pg < 1)
                {
                    pg = 1;
                }
                int resCount = listNews.Count();
                var pager = new Paging(resCount, pg, pageSize);
                var recSkip = (pg - 1) * pageSize;
                var data = listNews.Skip(recSkip).Take(pager.PageSize).ToList();
                await hubContext.Clients.All.SendAsync("reloadNews", data, pager);
                return Json(new { status = true });
            }
            else
            {
                return Json(new { status = false });
            }
        }
        #region
        [HttpPost("/Replay_Contact_Send")]

        public async Task<IActionResult> Replay_Contact_Send(int idContact, string customer, string cusEmail, string cusContent, string content_mess)
        {

            var mail = new BankModel.Mail.MailRequest
            {
                UserContent = cusContent,
                ToEmail = cusEmail,
                Body = content_mess,
                UserName = customer
            };
            var status = await _mail.SendEmailAsync(mail);
            if (status == "success")
            {
                var Contact = JsonConvert.DeserializeObject<BankModel.Contact>(client.GetStringAsync(urlContact + idContact).Result);
                Contact.Status = "success";
                await client.PutAsJsonAsync<BankModel.Contact>(urlContact, Contact);
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }

        }
        [HttpPost("sendToUser")]
        public async Task<IActionResult> sendToUser(int cusId, string message_to_user)
        {
            var clams = (ClaimsIdentity)User.Identity;
            var AdminId = int.Parse(clams.FindFirst(ClaimTypes.NameIdentifier).Value);
            Message mess = new Message
            {
                Sender = AdminId,
                Receiver = cusId,
                MessageChat = message_to_user
            };
            var check = client.PostAsJsonAsync<Message>(urlMessage, mess).Result;
            if (check.IsSuccessStatusCode)
            {
                var listMess = JsonConvert.DeserializeObject<IEnumerable<Message>>(client.GetStringAsync(urlMessage).Result);
                await hubContext.Clients.All.SendAsync("listMess", listMess, cusId);
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        [HttpGet("getListMessage")]
        public IActionResult getListMessage(int cusId)
        {
            var listMess = JsonConvert.DeserializeObject<IEnumerable<Message>>(client.GetStringAsync(urlMessage).Result).Where(x => x.Sender.Equals(cusId) || x.Receiver.Equals(cusId));
            var customer = JsonConvert.DeserializeObject<IEnumerable<BankModel.Customer>>(client.GetStringAsync(urlCustomer).Result).SingleOrDefault(x => x.CustomerId.Equals(cusId));
            return Json(new { listMess = listMess, customer = customer });
        }
        #endregion
    }
}
