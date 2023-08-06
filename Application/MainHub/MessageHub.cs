using Microsoft.AspNetCore.SignalR;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace Application.MainHub
{
    public class MessageHub : Hub<IMessageHub>
    {
        public List<string> nameAndConnectedId = new List<string>();
        public async override Task OnConnectedAsync()
        {
            
            
            //string claimName = Context.User.Identity.Name;
            
            //nameAndConnectedId.Add(claimName);
            //nameAndConnectedId.Add(Context.ConnectionId);
            
            await base.OnConnectedAsync();
        }
        public async Task SendMesssageToAdmins(string message)
        {
            await Clients.Group("Admin").SendMesssageToAdmins(message);
        }
        public async Task AddAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
        }
        public async Task SendMesssageToUser(string message, string userId)
        {
            await Clients.Group(userId).SendMesssageToUser(message, userId);
        }
        public async Task AddGroup(string name)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, name);
        }
    }
}
