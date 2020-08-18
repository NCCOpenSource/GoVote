using System.ComponentModel.DataAnnotations;

namespace VotingApp.Models
{
    public class Attendee
    {
        public int Id { get; set; }

        [Required]
        public virtual Member Member { get; set; }

        [Required]
        public int Status { get; set; }

    }
}
