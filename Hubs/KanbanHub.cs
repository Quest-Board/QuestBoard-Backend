using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using QuestBoard_Backend.Context;
using System;
using System.Threading.Tasks;

namespace QuestBoard_Backend.Hubs
{
    public class KanbanHub : Hub
    {
        private readonly QuestboardContext _context;

        private readonly IDistributedCache _cache;

        public KanbanHub(QuestboardContext context, IDistributedCache cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
