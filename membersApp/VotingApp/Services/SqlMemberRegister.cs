using System.Collections.Generic;
using VotingApp.Data;
using System.Linq;
using VotingApp.Models;
using VotingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace VotingApp.Services
{
    public class SqlMemberRegister : IMemberRegister
    {
        private readonly VotingAppDBContext _context;

        public SqlMemberRegister(VotingAppDBContext context)
        {
            _context = context;
        }
        public void ClearAllEntries()
        {
            _context.Attendeess.RemoveRange(_context.Attendeess);
            _context.SaveChanges(true);
        }

        public int GetMemberStatus(string AzureId)
        {
            try
            {
               return _context.Attendeess.FirstOrDefault(r => r.Member.AzureId == AzureId).Status;
            }
            catch
            {
                return 2;
            }
        }

        public SortedDictionary<Member, int> GetStatusAllMember()
        {
            SortedDictionary<Member, int> allMembers = new SortedDictionary<Member, int>(new MembComparer());
            if (_context.Attendeess.Count() > 0)
            {
                IEnumerable<Attendee> attendees = _context.Attendeess.Include(s => s.Member);
                foreach (var att in attendees)
                {
                    try
                    {
                        allMembers.Add(att.Member, att.Status);
                    }
                    catch {
                    }
                }
            }
            return allMembers;
        }

        public Member RegisterMemberByAzureID(string id)
        {
            Member member = _context.Members.FirstOrDefault(r => r.AzureId == id);
            UpdateAttendance(member, 1);
            return member;

        }

        public Member SignOutMemberByAzureID(string id)
        {
            Member member = _context.Members.FirstOrDefault(r => r.AzureId == id);
            UpdateAttendance(member, 0);
            return member;
        }

        public Member UpdateAttendance(Member member, int status)
        {
            if (_context.Attendeess.Count() > 0)
            {
                IEnumerable<Attendee> members = _context.Attendeess.Where(r => r.Member.AzureId == member.AzureId);
                if (members.Count() > 0)
                {
                    _context.Attendeess.Remove(members.First());
                    _context.SaveChanges(true);
                }
            }
            Attendee attendee = new Attendee
            {
                //Member = _context.Members.Where(r=> r.AzureId == member.AzureId).FirstOrDefault(),
                Member = member,
                Status = status
            };
            _context.Attendeess.Add(attendee);
            _context.SaveChanges();
            return attendee.Member;
        }

        public int UpdateAttendance(IEnumerable<Member> members, int status)
        {
            foreach (var member in members)
            {
                if (_context.Attendeess.Count() > 0)
                {
                    IEnumerable<Attendee> memberList = _context.Attendeess.Where(r => r.Member.AzureId == member.AzureId);
                    if (members.Count() > 0)
                    {
                        _context.Attendeess.Remove(memberList.First());
                        _context.SaveChanges(true);
                    }
                }
                Attendee attendee = new Attendee
                {
                    //Member = _context.Members.Where(r=> r.AzureId == member.AzureId).FirstOrDefault(),
                    Member = member,
                    Status = status
                };
                _context.Attendeess.Add(attendee);
            }
            
            _context.SaveChanges();
            return 0;
        }

    }


    internal class MembComparer : IComparer<Member>
    {
        public int Compare(Member x, Member y)
        {
            var xname = x.LastName + x.FirstName + x.AzureId;
            var yname = y.LastName + y.FirstName + y.AzureId;
            return xname.CompareTo(yname);
        }
    }
}
