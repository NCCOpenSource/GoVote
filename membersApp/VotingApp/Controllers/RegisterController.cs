using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using VotingApp.Services;
using VotingApp.Services.Interfaces;
using VotingApp.ViewModels;

//
//  Register  Member as present in council chamber to allow voting
//            and unregister Member when leaves the  council chamber to disable voting.
//
//

namespace VotingApp.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class RegisterController : Controller
    {
        private ICouncilSession _session;
        private IMemberRegister _memberRegister;
        private static IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        private RegisterIndexViewModel model;
        private ILogger _logger;

        public RegisterController(ILogger<RegisterController> logger, IMember member, ICouncilSession session, IMemberRegister memberRegister, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _session = session;
            _memberRegister = memberRegister;
            _configuration = configuration;
            _env = hostingEnvironment;
            _logger = logger;

            model = new RegisterIndexViewModel
            {
                CouncilSession = _session.GetActiveSession(),
                Register = _memberRegister.GetStatusAllMember()
            };

        }

        public IActionResult Index()
        {
            return View(model);

        }

        public IActionResult RegisterAttendance(string id)
        {
            if (model.CouncilSession.IsActiveSession)
            {
                //If active session then allow the Member to sign-in
                _logger.LogInformation("{0}- Registering Member {1} signed in", DateTime.Now, id);
                _memberRegister.RegisterMemberByAzureID(id);
            }
            return RedirectToAction(nameof(Index));

        }

        public IActionResult UnregisterMember(string id)
        {
            if (model.CouncilSession.IsActiveSession)
            {
                //If active session then allow the Member to sign-out
                _logger.LogInformation("{0}- Registering Member {1} signed out", DateTime.Now, id);
                _memberRegister.SignOutMemberByAzureID(id);
            }
            return RedirectToAction(nameof(Index));

        }

        
    }
}