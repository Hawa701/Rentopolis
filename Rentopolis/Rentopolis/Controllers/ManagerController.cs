using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Rentopolis.Controllers
{
    public class ManagerController : Controller
    {
        [Authorize(Roles = "Manager")]
        public IActionResult Home()
        {
            return View();
        }
    }
}
