using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingApp.Services;
using VotingApp.Services.Interfaces;
using VotingApp.ViewModels;

namespace VotingApp.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class DisplayController : Controller
    {
        private IBallot _ballot;
        private IMember _member;
        private ICouncilSession _session;
        private IVote _vote;
        private ISeatService _seatService;

        public DisplayController(IBallot ballot, ISeatService seatService, IMember member, ICouncilSession session, IVote vote)
        {
            _ballot = ballot;
            _member = member;
            _session = session;
            _vote = vote;
            _seatService = seatService;
        }
        public IActionResult Index()
        {

            var model = new DisplayIndexViewModel
            {
                Members = _member.GetAllMembers(true),
                Session = _session.GetActiveSession(),
                Ballot = _ballot.GetActiveOrLast(),
                Votes = _vote.GetAll(_ballot.GetActiveOrLast()),
                MembersBySeat = _seatService.MembersBySeat(),
                BallotId = _ballot.GetBallotsThisSession(_session.GetActiveSession())
            };
            return View(model);
        }

    }
}