using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Rentopolis.Controllers
{
    public class TenantController : Controller
    {
        [Authorize(Roles = "Tenant")]
        public IActionResult Home()
        {
            return View();
        }
    }
}
