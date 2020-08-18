
using System.Collections.Generic;
using VotingApp.Models;

namespace VotingApp.Services.Interfaces
{
    public interface ISeatService
    {
        Dictionary<Member,int> GetAllMemberSeats();

        int GetSeatByAzureID(string azureID);

        Member AddMemberSeat(Member member, int seat);

        Dictionary<int, string> MembersBySeat();

        void ClearAllEntries();
    }
}
