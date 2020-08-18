using System.Collections.Generic;
using System.Linq;
using VotingApp.Data;
using VotingApp.Models;
using VotingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace VotingApp.Services
{
    public class SqlVoteData : IVote
    {
        private readonly VotingAppDBContext _context;
        public SqlVoteData(VotingAppDBContext context)
        {
            _context = context;
        }
        public Vote Get(int id)
        {
            return _context.Votes.FirstOrDefault(r => r.VoteId == id);
        }

        public IEnumerable<Vote> GetAll(Ballot ballot)
        {
            IEnumerable<Vote> votes = _context.Votes.Include(s => s.Member).Where(r => r.Ballot.BallotId == ballot.BallotId);
            return votes;
        }

        public Vote CastVote(Ballot ballot, Member member, VoteOptions voteOptions, int seat)
        {
            Vote vote = new Vote
            {
                Ballot = ballot,
                CastVote = voteOptions,
                Member = member,
                Seat = seat
            };
            IEnumerable<Vote> votes = _context.Votes.Where(r => r.Ballot.BallotId == ballot.BallotId && r.Member == member);
            if (votes.Count() > 0)
            {
                _context.Votes.Remove(votes.First());
            }
            _context.Votes.Add(vote);
            _context.SaveChanges(true);
            return vote;
        }

        public Vote Get(Member member, Ballot ballot)
        {
            return _context.Votes.Where(r => r.Ballot == ballot && r.Member == member).FirstOrDefault();
        }
    }
}
