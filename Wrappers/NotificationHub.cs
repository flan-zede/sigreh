
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace sigreh.Wrappers
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
