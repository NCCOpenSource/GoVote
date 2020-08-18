using System;
using System.ComponentModel.DataAnnotations;

namespace VotingApp.Models
{
    public class Ballot
    {
        //Unique ID of the ballot
        public int BallotId { get; set; }

        [Required]
        //Id of the Council Session that the vote is a part of.
        public Session Session { get; set; }

        [Required]
        public bool IsActiveBallot { get; set; }
        [Required]
        public DateTime BallotStartTime { get; set; }

        public DateTime? BallotEndTime { get; set; }

        [Required]
        //Description of what the vote is for 
        public int BallotName { get; set; }

    }
}
