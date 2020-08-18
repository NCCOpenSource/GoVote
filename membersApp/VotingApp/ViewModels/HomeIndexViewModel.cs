using System.Collections.Generic;
using VotingApp.Models;

namespace VotingApp.ViewModels
{
    public class HomeIndexViewModel
    {
        public int Registered { get; set; }

        public Vote CurrentVote { get; set; }

        public Session Session{ get; set; }

        public Ballot Ballot { get; set; }

        public Member Member { get; set; }

        public int Seat { get; set; }
    }
}
