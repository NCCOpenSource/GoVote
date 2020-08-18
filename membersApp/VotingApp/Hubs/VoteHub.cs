using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;


namespace VotingApp.Hubs
{
    public class VoteHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task ClosePoll(string pollid)
        {
            //Sent from Admin controller
            //Received by Home controller
            await Clients.All.SendAsync("ClosePoll", pollid);
        }
        public async Task OpenPoll(string pollid)
        {
            await Clients.All.SendAsync("OpenPoll", pollid);
        }
        public async Task Vote(string vote, string seatno)
        {
            await Clients.All.SendAsync("DisplayVote", seatno, vote);
            await Clients.All.SendAsync("CastVote", vote, seatno);
        }

        public async Task ClearDisplay()
        {
            await Clients.All.SendAsync("ClearDisplay");
        }
        public async Task BlankDisplay()
        {
            await Clients.All.SendAsync("BlankScreen");
        }
        
    }
}
