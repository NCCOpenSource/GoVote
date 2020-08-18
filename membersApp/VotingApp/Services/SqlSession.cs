using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VotingApp.Data;
using VotingApp.Models;

namespace VotingApp.Services
{
    public class SqlSession : ICouncilSession
    {
        private readonly VotingAppDBContext _context;

        public SqlSession(VotingAppDBContext context)
        {
            _context = context;
        }
        public Session EndSession(Session session)
        {
            session.IsActiveSession = false;
            session.EndTime = DateTime.Now;
            _context.Sessions.Update(session);
            _context.SaveChanges();
            return session;
        }

        public Session GetActiveSession()
        {
            try
            {
                return _context.Sessions.OrderByDescending(r => r.StartTime).First();
            }
            catch
            {
                return new Session {CurrentSessionID = 0, IsActiveSession = false, StartTime= DateTime.Now, EndTime = DateTime.Now };
            }
        }

        public Session GetSession(int sessionId)
        {
            return _context.Sessions.FirstOrDefault(r => r.CurrentSessionID == sessionId); 
        }

        public Session StartSession()
        {
            Session session = new Session
            {
                StartTime = DateTime.Now,
                IsActiveSession = true
            };

            _context.Sessions.Add(session);
            _context.SaveChanges();
            return session;
        }

     
    }
}
