using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VotingApp.Models;
using Microsoft.AspNetCore.Authorization;
using VotingApp.Services.Interfaces;
using VotingApp.ViewModels;
using VotingApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace VotingApp.Controllers
{
    [Authorize(Policy = "MemberOnly")]
    public class HomeController : Controller
    {
        private ILogger _logger;
        private ISeatService _seatService;
        private IConfiguration _configuration;
        private IMemberRegister _memberRegister;
        private ICouncilSession _session;
        private IBallot _ballot;
        private IMember _member;
        private IVote _vote;
        private HomeIndexViewModel model;

        public HomeController(ILogger<HomeController> logger, IMemberRegister memberRegister, ICouncilSession session, IBallot ballot, IMember member, IVote vote, ISeatService seatService, IConfiguration configuration)
        {
            _configuration = configuration;
            _memberRegister = memberRegister;
            _session = session;
            _ballot = ballot;
            _member = member;
            _vote = vote;
            _logger = logger;
            _seatService = seatService;

            model = new HomeIndexViewModel
            {
                Session = _session.GetActiveSession(),
                Ballot = _ballot.GetActiveOrLast()
                
            };
            
        }
        public IActionResult Index()
        {
            //Check to see if there is an AzureID in the session
            string userID ="";
            foreach (var claim in User.Claims)
            {
                if (claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                {
                    userID = claim.Value;
                }
            }
            //No claim is available
            if (userID == "")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            else
            {
                //Fetch the members information from the database 
                model.Member = _member.GetMember(userID);
                model.Seat = _seatService.GetSeatByAzureID(userID);
                //Get the members attendance status from the register.  If it's 1 they can log in, else they can't
                int status = _memberRegister.GetMemberStatus(userID);
                if (status == 1)
                {
                    model.Registered = 1;
                    model.CurrentVote = _vote.Get(model.Member, model.Ballot);
                }
                else
                {
                    model.Registered = status;
                }

                return View(model);

            }
        }

       
        public IActionResult Vote(string id)
        {
            string userID = "";
            foreach (var claim in User.Claims)
            {
                //Check to see if there is an AzureID in the session
                if (claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                {
                    userID = claim.Value;
                }
            }
            if (userID == "")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            else
            {
                model.Member = _member.GetMember(userID);
                model.Seat = _seatService.GetSeatByAzureID(userID);
            }
            
            //Create a Register the vote using enum vote object
            VoteOptions vote;
                
            if (id.Equals("Yes"))
            {
                vote = VoteOptions.Yes;
            }
            else if (id.Equals("No"))
            {
                vote = VoteOptions.No;
            }
            else if (id.Equals("Abstain"))
            {
                vote = VoteOptions.Abstain;
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

            //Check that the user us currently registered to vote and hasn't been signed out before they press send.
            if (_memberRegister.GetMemberStatus(userID) == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("{0}- Registered Member {1} - Voted {2} in poll {3} in seat {4}", DateTime.Now,  model.Member.AzureId, vote, model.Ballot.BallotName, model.Seat);
            if (_ballot.CheckIfActiveBallot()) {
                _logger.LogInformation("{0}- Registered Member {1} - Voted {2} in poll {3} in seat {4}", DateTime.Now, model.Member.AzureId, vote, model.Ballot.BallotName, model.Seat);
                //Cast vote into database
                _vote.CastVote(model.Ballot, model.Member, vote, model.Seat);
                //Send vote via SignalR to Display
                Send_Vote(id, model.Seat, model.Member.AzureId);
            }
            else
            {
                _logger.LogInformation("{0}- Attempted vote in closed Ballot. Registered Member {1} - Voted {2} in poll {3} in seat {4}", DateTime.Now, model.Member.AzureId, vote, model.Ballot.BallotName, model.Seat);
            }
            //Redirect user back to the vote homescreen
            return RedirectToAction(nameof(Index));
        }

        async void Send_Vote(string vote, int seatno, string azureID)
        {
            HubConnection connection;
            try
            {
                string domainName = HttpContext.Request.Host.Value;
                string scheme = HttpContext.Request.Scheme;
                string hub = _configuration.GetValue<string>("HubString");
                hub = hub.StartsWith("/") ? hub.Remove(0, 1) : hub ; 

                string hubPath = $"{scheme}://{domainName}/{hub}";
                connection = new HubConnectionBuilder()
                    .WithUrl(hubPath)
                    .Build();

                await connection.StartAsync();
                await connection.InvokeAsync("Vote", vote, seatno);
                await connection.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}- Member- {1} in seat {2} voted {3}", DateTime.Now, azureID, seatno, vote);
                _logger.LogError(ex.ToString());
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            
            return View();
        }

        [Route("error/404")]
        [AllowAnonymous]
        public IActionResult Error404()
        {
            return View("404");
        }
        [Route("error/{code:int}")]
        [AllowAnonymous]
        public IActionResult Error(int code)
        {
            // handle different codes or just return the default error view
            return View();
        }
    }
}

