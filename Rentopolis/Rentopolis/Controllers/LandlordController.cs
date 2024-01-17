using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Rentopolis.Controllers
{
    public class LandlordController : Controller
    {
        [Authorize(Roles = "Landlord")]
        public IActionResult Home()
        {
            return View();
        }
    }
}
