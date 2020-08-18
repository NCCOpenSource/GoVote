using System.Collections.Generic;
using VotingApp.Models;

namespace VotingApp.Services.Interfaces
{
    public interface IVote
    {
        Vote CastVote(Ballot ballot, Member member, VoteOptions voteOptions, int seat);

        Vote Get(int id);

        Vote Get(Member member, Ballot ballot);

        IEnumerable<Vote> GetAll(Ballot ballot);

    }
}
