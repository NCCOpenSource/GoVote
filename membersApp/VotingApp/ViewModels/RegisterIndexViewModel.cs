using System.Collections.Generic;
using VotingApp.Models;

namespace VotingApp.ViewModels
{
    public class RegisterIndexViewModel
    {
        public SortedDictionary<Member, int> Register { get; set; }
        public Session CouncilSession { get; set; }

    }
}
