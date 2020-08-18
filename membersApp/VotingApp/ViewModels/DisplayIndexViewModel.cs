using System.Collections.Generic;
using VotingApp.Models;

namespace VotingApp.ViewModels
{
    public class DisplayIndexViewModel
    {
        public IEnumerable<Vote> Votes { get; set; }
        public IEnumerable<Member> Members { get; set; }

        public Session Session { get; set; }

        public Ballot Ballot { get; set; }

        public Dictionary<int, string> MembersBySeat { get; set; }

        public int BallotId { get; set; }


    }
}
