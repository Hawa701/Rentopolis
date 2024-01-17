using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Rentopolis.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles = "Admin")]
        public IActionResult Home()
        {
            return View();
        }
    }
}
