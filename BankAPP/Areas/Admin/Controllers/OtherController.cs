using Microsoft.AspNetCore.Mvc;

namespace BankAPP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OtherController : Controller
    {
        [HttpGet("NotFound")]
        public IActionResult NotFound()
        {
            return View();
        }

        [HttpGet("Error")]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet("Denied")]
        public IActionResult Denied()
        {
            return View();
        }
    }
}
