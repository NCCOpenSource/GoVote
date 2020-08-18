using System;
using System.ComponentModel.DataAnnotations;

namespace VotingApp.Models
{
    public class Session 
    {
        [Key]
        public int CurrentSessionID { get; set; }
        [Required]
        public bool IsActiveSession { get; set; }
        
        [Required,]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

    }
}
