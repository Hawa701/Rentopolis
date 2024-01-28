using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;

namespace Rentopolis.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly IManagerServices _services;
        public ManagerController(IManagerServices managerServices)
        {
            this._services = managerServices;
        }

        // Home page
        public IActionResult Home()
        {
            return View();
        }


        // Users list
        [HttpGet]
        public async Task<IActionResult> AllUsers(string role)
        {
            var users = await _services.GetUsersByRole(role);

            if (users == null)
                return RedirectToAction("AccessDenied", "Account");
            
            ViewData["role"] = role;
            return View(users);
        }


        //Ban users
        [HttpGet]
        public async Task<IActionResult> BanUser(string id, string role)
        {
            Status returnedStatus = await _services.BanUser(id);

            if (returnedStatus.StatusCode == 0)
            {
                ViewBag.Success = false;
                ViewBag.Message = returnedStatus.StatusMessage;
            }
            else ViewBag.Success = true;

            return RedirectToAction("AllUsers", "Manager", new { role = role });
        }
        

        //Unban users
        [HttpGet]
        public async Task<IActionResult> UnbanUser(string id, string role)
        {
            Status returnedStatus = await _services.UnbanUser(id);

            if (returnedStatus.StatusCode == 0)
            {
                ViewBag.Success = false;
                ViewBag.Message = returnedStatus.StatusMessage;
            }
            else ViewBag.Success = true;

            return RedirectToAction("AllUsers", "Manager", new { role = role });
        }
    }
}
