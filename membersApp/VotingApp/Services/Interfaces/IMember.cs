using System.Collections.Generic;
using VotingApp.Models;

namespace VotingApp.Services.Interfaces
{
    public interface IMember
    {
        Member GetMember(string AzureID);

        IEnumerable<Member> GetAllMembers(bool active);

        Member AddMember(Member member);

        void SetAllMembersToInActive();

        
    }
}
