using System.ComponentModel.DataAnnotations;

namespace VotingApp.Models
{
    public class Member
    {
        //Azure ID of the member
        [Required]
        [Key]
        public string AzureId { get; set; }

        //Name of member who voted
        [Required]
        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int SeatNumber { get; set; }

        public bool IsActiveMember { get; set; }


    }
}
