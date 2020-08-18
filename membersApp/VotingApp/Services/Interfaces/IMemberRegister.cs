using System.Collections.Generic;
using VotingApp.Models;

namespace VotingApp.Services.Interfaces
{
    public interface IMemberRegister   
    {
        int GetMemberStatus(string AzureId);

        SortedDictionary<Member, int> GetStatusAllMember();

        Member UpdateAttendance(Member member, int id);
        int UpdateAttendance(IEnumerable<Member> members, int status);

        Member RegisterMemberByAzureID(string id);

        Member SignOutMemberByAzureID(string id);
        void ClearAllEntries();
        
    }
}
