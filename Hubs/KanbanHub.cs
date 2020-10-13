using Microsoft.AspNetCore.SignalR;
using QuestBoard_Backend.Context;
using System;
using System.Threading.Tasks;

namespace QuestBoard_Backend.Hubs
{
    public class KanbanHub : Hub
    {
        private readonly QuestboardContext _context;

        public KanbanHub(QuestboardContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
