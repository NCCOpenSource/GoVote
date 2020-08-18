using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VotingApp.Data;
using VotingApp.Models;
using VotingApp.Services.Interfaces;

namespace VotingApp.Services
{
    public class SqlMemberSeatService : ISeatService
    {
        private readonly VotingAppDBContext _context;

        public SqlMemberSeatService(VotingAppDBContext context)
        {
            _context = context;
        }
        public Member AddMemberSeat(Member member, int seat)
        {
            IEnumerable<MemberSeat> seats = _context.MembersSeats.Where(r => r.Member == member);
            if (seats.Count() > 0)
            {
                _context.MembersSeats.Remove(seats.First());
                _context.SaveChanges(true);
            }
            MemberSeat memSeat = new MemberSeat
            {
                Member = member,
                Seat = seat
            };

            _context.MembersSeats.Add(memSeat);
            _context.SaveChanges(true);
            return member;
        }

        public Dictionary<Member, int> GetAllMemberSeats()
        {
            Dictionary<Member, int> _seatList = new Dictionary<Member, int>();
            try
            {
                foreach (var member in _context.Members.Where(r=>r.IsActiveMember))
                {
                    _seatList.Add(member, GetSeatByAzureID(member.AzureId));
                }
                return _seatList;
            }
            catch { 
                 return new Dictionary<Member, int>();
            }
        }

        public Dictionary<int,string> MembersBySeat()
        {
            Dictionary<int, string> _seatList = new Dictionary<int, string>();
            try
            {
                foreach (var member in _context.Members.Where(r => r.IsActiveMember))
                {
                    string memname = "Cllr " + member.FirstName.Substring(0, 1) + member.LastName;
                    _seatList.Add(GetSeatByAzureID(member.AzureId), memname);
                }
                return _seatList;
            }
            catch
            {
                return new Dictionary<int, String>();
            }
        }

        public int GetSeatByAzureID(string azureID)
        {
            try
            {
                return _context.MembersSeats.FirstOrDefault(r => r.Member.AzureId == azureID).Seat;
            }
            catch
            {
                return 0;
            }
        }

        public void ClearAllEntries()
        {
            _context.MembersSeats.RemoveRange(_context.MembersSeats);
            _context.SaveChanges(true);
        }
    }
}
