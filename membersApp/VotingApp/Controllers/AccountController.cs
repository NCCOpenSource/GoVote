using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace VotingApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            ClaimsPrincipal currentUser = this.User;
            var currentUserName = currentUser.FindFirst(ClaimTypes.GivenName).Value;
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}