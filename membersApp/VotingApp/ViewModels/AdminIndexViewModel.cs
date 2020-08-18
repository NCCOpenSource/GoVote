using System.Collections.Generic;
using VotingApp.Models;

namespace VotingApp.ViewModels
{
    public class AdminIndexViewModel
    {
        public Ballot ActiveOrLastBallot { get; set; }

        public IEnumerable<Member> Members { get; set; }

        public Session CouncilSession { get; set; }

        public SortedDictionary<Member, int> Register { get; set; }

        public int BallotsThisSession { get; set; }

        public Dictionary<Member, int> SeatList { get; set; }

    }
}
