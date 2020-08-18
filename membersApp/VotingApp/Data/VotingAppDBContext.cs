using System;
using Microsoft.EntityFrameworkCore;
using VotingApp.Models;

namespace VotingApp.Data
{
    public class VotingAppDBContext : DbContext
    {
        public VotingAppDBContext(DbContextOptions options) : base(options)
        {
           
        }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Ballot> Ballots { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Attendee> Attendeess { get; set; }
        public DbSet<MemberSeat> MembersSeats { get; set; }

    }
}

    