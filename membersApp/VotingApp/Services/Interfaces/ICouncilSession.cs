using VotingApp.Models;
namespace VotingApp.Services
{
    public interface ICouncilSession
    {
        Session StartSession();

        Session EndSession(Session session);

        Session GetSession(int sessionId);

        Session GetActiveSession();


    }
}
