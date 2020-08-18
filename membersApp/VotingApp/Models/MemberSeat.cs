using System.ComponentModel.DataAnnotations;

namespace VotingApp.Models
{
    public class MemberSeat
    {
        //AzureID of member who voted
        public int Id { get; set; }
        [Required]
        public Member Member { get; set; }

        [Required]
        //The vote itself
        public int Seat { get; set; }
    }
}
