using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace VotingApp.Controllers
{
    public class AuthController : Controller
    {
        private IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("auth/signup")]
        public IActionResult SignUp()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "B2C_1_SignUpSignInWithLocalOrIDP");
        }

        [AllowAnonymous]
        [Route("reset-password")]
        public IActionResult ResetPassword()
        {
            string domainName = HttpContext.Request.Host.Value;
            string domain = _configuration.GetSection("AzureAdB2C").GetValue<string>("Domain");
            string resetPolicy = _configuration.GetSection("AzureAdB2C").GetValue<string>("ResetPasswordPolicyId");
            string clientId = _configuration.GetSection("AzureAdB2C").GetValue<string>("ClientId");
            string redirectUri = _configuration.GetSection("AzureAdB2C").GetValue<string>("redirectUri");
            string tenantId = _configuration.GetSection("AzureAdB2C").GetValue<string>("tenantId");

            string resetURL = $"https://login.microsoftonline.com/{domain}/oauth2/v2.0/authorize?p={resetPolicy}&client_id={clientId}&redirect_uri=https%3A%2F%2F{domainName}&scope=openid%20offline_access&response_type=code";

            return Redirect(resetURL);

        }

        [Route("auth/signout")]
        [HttpPost]
        public async Task SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var scheme = User.FindFirst("tfp").Value;
            await HttpContext.SignOutAsync(scheme);
        }
    }
}