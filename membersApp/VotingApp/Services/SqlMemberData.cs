using System;
using System.Collections.Generic;
using System.Linq;
using VotingApp.Data;
using VotingApp.Models;
using VotingApp.Services.Interfaces;

namespace VotingApp.Services
{
    internal class SqlMemberData : IMember
    {
        private readonly VotingAppDBContext _context;

        public SqlMemberData(VotingAppDBContext context)
        {
            _context = context;
            
        }

        public Member AddMember(Member member)
        {
            if (!_context.Members.Contains(member))
            {
                _context.Members.Add(member);
                _context.SaveChanges();
            }
            else
            {
                Member tmpMember = _context.Members.FirstOrDefault(r => r.AzureId == member.AzureId);
                tmpMember.DisplayName = member.DisplayName;
                tmpMember.FirstName = member.FirstName;
                tmpMember.LastName = member.LastName;
                tmpMember.IsActiveMember = member.IsActiveMember;
                _context.Members.Update(tmpMember);
                _context.SaveChanges();
            }

            return member;
        }

        public IEnumerable<Member> GetAllMembers(bool active)
        {
            if (active)
            { 
                return _context.Members.Where(r=>r.IsActiveMember);
            }
            else
            {
                return _context.Members;
            }
            
        }

        public Member GetMember(string AzureID)
        {
            return _context.Members.FirstOrDefault(r => r.AzureId == AzureID);
        }

        public void SetAllMembersToInActive()
        {
            IEnumerable<Member> members = _context.Members.Where(r => r.IsActiveMember);

            foreach (var member in members)
            {
                member.IsActiveMember = false;
                _context.Members.Update(member);
            }

            _context.SaveChanges(true);
            
        }

        
    }
}