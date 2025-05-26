using Microsoft.AspNetCore.SignalR;

namespace PMSWebApp.Extensions
{
    public class SiganlRHUB : Hub
    {
        public async Task JoinGroup(string groupName)
        {
            Console.WriteLine($"Client {Context.ConnectionId} joined group {groupName}");
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task LeaveGroup(string groupName)
        {
            Console.WriteLine($"Client {Context.ConnectionId} left group {groupName}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task NotifyGroup(string groupName, string action, object data)
        {
            Console.WriteLine($"Broadcasting to group {groupName}: Action={action}, Data={data}");
            await Clients.Group(groupName).SendAsync("DataChanged", action, data);
        }
    }
}
