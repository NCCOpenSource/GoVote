using System.ComponentModel.DataAnnotations;

namespace VotingApp.Models
{
    public class Vote
    {
        //Unique ID of the vote cast in a poll/ballot
        [Key]
        public int VoteId { get; set; }

        [Required]
        //Id of the ballot that the vote is a part of.
        public Ballot Ballot { get; set; }

        //AzureID of member who voted
        [Required]
        public Member Member { get; set; }

        [Required]
        //The vote itself
        public VoteOptions CastVote { get; set; }

        [Required]
        public int Seat { get; set; }

    }
}
