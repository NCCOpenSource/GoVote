using System;
using System.Collections.Generic;
using System.Linq;
using VotingApp.Data;
using VotingApp.Models;
using VotingApp.Services.Interfaces;

namespace VotingApp.Services
{
    public class SqlBallotData : IBallot
    {
        private ICouncilSession _session;
        private readonly VotingAppDBContext _context;
        public SqlBallotData(VotingAppDBContext context, ICouncilSession session)
        {
            _context = context;
            _session = session;
        }
        public bool CheckIfActiveBallot()
        {
            try
            {
                Ballot ballot = _context.Ballots.OrderByDescending(r => r.BallotStartTime).FirstOrDefault();
                if (ballot.BallotEndTime == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false; 
            }
        }
            
            
        public Ballot EndBallot(Ballot ballot)
        {
            ballot.BallotEndTime = DateTime.Now;
            ballot.IsActiveBallot = false;
            _context.Ballots.Update(ballot);
            _context.SaveChanges();
            return ballot;
        }

        public Ballot GetActiveOrLast()
        {
            try
            {
                return _context.Ballots.OrderByDescending(r => r.BallotStartTime).First();
            }
            catch
            {
                return new Ballot { BallotName = 0, BallotStartTime = DateTime.Now, BallotEndTime = DateTime.Now, };
            }
            
        }

        public int GetBallotsThisSession(Session session)
        {
            try
            {
                return _context.Ballots.Where(r => r.Session == session).Count();
            }
            catch
            {
                return 0;
            }
        }

        public int GetCurrentBallotID(int sessionId)
        {
            Ballot ballot =_context.Ballots.OrderByDescending(r => r.BallotStartTime).FirstOrDefault(r => r.IsActiveBallot == true);
            return ballot.BallotId;

        }

        public Ballot StartBallot(Session session, int ballotName)
        {
            Ballot ballot = new Ballot
            {
                Session = session,
                BallotStartTime = DateTime.Now,
                BallotName = ballotName,
                IsActiveBallot = true
            };

            _context.Ballots.Add(ballot);
            _context.SaveChanges();
            
            return ballot;
        }
    }
}
