using System.Collections.Generic;
using VotingApp.Models;

namespace VotingApp.Services.Interfaces
{
    public interface IBallot
    {
        Ballot StartBallot(Session session, int ballotName);    

        Ballot EndBallot(Ballot ballot);

        int GetCurrentBallotID(int sessionId);

        bool CheckIfActiveBallot();
        Ballot GetActiveOrLast();

        int GetBallotsThisSession(Session session);
    }
}
