using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using VotingApp.Models;
using VotingApp.Services;
using VotingApp.Services.Interfaces;
using VotingApp.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace VotingApp.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private ICouncilSession _session;
        private IBallot _ballot;
        private IMember _member;
        private IMemberRegister _memberRegister;
        private ISeatService _seatService;
        private ILogger _logger;
        private AdminIndexViewModel model;

        private static IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public AdminController( ILogger<AdminController> logger, ICouncilSession session, IBallot ballot, 
                                IMember member, IMemberRegister memberRegister, IConfiguration configuration, 
                                IHostingEnvironment hostingEnvironment, ISeatService seatService)
        {
            _logger = logger;
            _session = session;
            _ballot = ballot;
            _member = member;
            
            _memberRegister = memberRegister;
            _seatService = seatService;

            _configuration = configuration;
            _env = hostingEnvironment;

            model = new AdminIndexViewModel
            {
                ActiveOrLastBallot = _ballot.GetActiveOrLast(),
                Members = _member.GetAllMembers(true),
                CouncilSession = _session.GetActiveSession(),
                Register = _memberRegister.GetStatusAllMember(),
                BallotsThisSession = _ballot.GetBallotsThisSession(_session.GetActiveSession())
            };

        }

        public IActionResult Index()
        {
            //Retunr the default model indicating the current session state
            return View(model);
        }

        public IActionResult StartSession()
        {
            //Only start a session if a council is not yet running
            if (!model.CouncilSession.IsActiveSession)
            {
                _logger.LogDebug("{0}- No Active Session", DateTime.Now);
               
                IEnumerable<Member> members = _member.GetAllMembers(true);
                _memberRegister.UpdateAttendance(members, 0);
                
                //Begin the council session
                _session.StartSession();
            }
            else
            {
                _logger.LogDebug("{0}- Council session already active", DateTime.Now);
            }
            return RedirectToAction(nameof(Index));

        }

        public IActionResult StartBallot()
        {
            if (model.CouncilSession.IsActiveSession && !model.ActiveOrLastBallot.IsActiveBallot)
            {
                _logger.LogDebug("{0}- No active ballot and active session so start ballot", DateTime.Now);
                
                //Create a Ballot in the database
                _ballot.StartBallot(model.CouncilSession, model.BallotsThisSession + 1);

            }
            return RedirectToAction(nameof(Index));

        }


        public IActionResult CloseSession()
        {
            if (model.CouncilSession.IsActiveSession)
            {
                if (model.ActiveOrLastBallot.IsActiveBallot)
                {
                    _logger.LogInformation("{0}- Ballot closing", DateTime.Now);

                    //End the ballot in the database
                    _ballot.EndBallot(model.ActiveOrLastBallot);

                }
                _logger.LogInformation("{0}- Session closing", DateTime.Now);

                //Remove all members from the register
                _memberRegister.ClearAllEntries();

                //End the session in the database
                _session.EndSession(model.CouncilSession);
            }
            return RedirectToAction(nameof(Index));

        }

        public IActionResult CloseBallot()
        {
            if (model.CouncilSession.IsActiveSession && model.ActiveOrLastBallot.IsActiveBallot)
            {
                _logger.LogInformation("{0}- Ballot closing", DateTime.Now);

                //End the ballot in the database
                _ballot.EndBallot(model.ActiveOrLastBallot);

            }
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Attend(string id)
        {
            //Sign in the member in the database
            _memberRegister.RegisterMemberByAzureID(id);
            _logger.LogInformation("{0}- Admin checked in member {1}", DateTime.Now, id);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Signout(string id)
        {
            //Sign out the member in the database
            _memberRegister.SignOutMemberByAzureID(id);
            _logger.LogInformation("{0}- Admin signed out member {1}", DateTime.Now, id);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost("UploadFiles")]
        public IActionResult UploadFiles(List<IFormFile> files)
        {
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var reader = new StreamReader(formFile.OpenReadStream()))
                    using (var csv = new CsvReader(reader))
                    {
                        csv.Configuration.HeaderValidated = null;
                        csv.Configuration.MissingFieldFound = null;
                        var records = csv.GetRecords<Member>();
                        //Clear old seating table
                        _seatService.ClearAllEntries();

                        //add members to the seat plan.
                        foreach (var record in records)
                        {
                            Member member = _member.GetMember(record.AzureId);
                            _seatService.AddMemberSeat(member, record.SeatNumber);
                        }
                        return RedirectToAction(nameof(UploadMemberSeatingPlan));
                    }
                
                }
            }

            return Ok(new { count = files.Count });
        }

        [HttpGet]
        public IActionResult ExportMemberList()
        {
            Dictionary<Member, int> seatList = _seatService.GetAllMemberSeats();
            
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter))
                {
                    csvWriter.WriteField("AzureId");
                    csvWriter.WriteField("DisplayName");
                    csvWriter.WriteField("FirstName");
                    csvWriter.WriteField("LastName");
                    csvWriter.WriteField("SeatNumber");
                    csvWriter.NextRecord();
                    foreach (var seat in seatList)
                    {
                        csvWriter.WriteField(seat.Key.AzureId);
                        csvWriter.WriteField(seat.Key.DisplayName);
                        csvWriter.WriteField(seat.Key.FirstName);
                        csvWriter.WriteField(seat.Key.LastName);
                        csvWriter.WriteField(seat.Value);
                        csvWriter.NextRecord();
                    }
                } // StreamWriter gets flushed here.

                return File(memoryStream.ToArray(), "application/octet-stream", "Reports.csv");
            }
        }

        public IActionResult RefreshMemberList()
        {
            try
            {
                //Access to Azure B2C users
                GetMembersFromAzure GetMember = new GetMembersFromAzure(_configuration);

                //Get a collection of just the members
                IGraphServiceUsersCollectionPage members = GetMember.GetCurrentMembers();

                foreach (User user in members)
                {
                    TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                    Member member = new Member
                    {
                        DisplayName = ti.ToTitleCase(user.DisplayName),
                        FirstName = ti.ToTitleCase(user.GivenName),
                        LastName = ti.ToTitleCase(user.Surname),
                        AzureId = user.Id,
                        SeatNumber = _seatService.GetSeatByAzureID(user.Id),
                        IsActiveMember = true
                    };

                    _logger.LogInformation("{0} Registering Member {1} - {2}, {3}", DateTime.Now, user.Id, user.Surname, user.GivenName);

                    //Add the member to the members table for longterm storage
                    _member.AddMember(member);
                }
                return RedirectToAction("UploadMemberSeatingPlan","Admin");
            }
            catch (Exception ex)
            {

                return RedirectToAction(nameof(UploadMemberSeatingPlan));
            }
         }

        [HttpPost("DeLinkMembers")]
        public IActionResult DeLinkMembers()
        {
            try
            {
                //Unregister all current members
                _member.SetAllMembersToInActive();
                return RedirectToAction(nameof(RefreshMemberList));
            }
            catch
            {
                return RedirectToAction(nameof(UploadMemberSeatingPlan));
            }
        }

        public IActionResult UploadMemberSeatingPlan()
        {
            model.SeatList = _seatService.GetAllMemberSeats();
            return View(model);
        }

        
    }
}