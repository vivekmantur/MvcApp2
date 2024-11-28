using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Models;

namespace WebApplication1.Hubs
{
    public class ChatHub : Hub
    {
        // User joins a group based on their user ID
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        // Admin sends a message to the user
        public async Task SendMessage(string user, string message)
        {
            await Clients.Group(user).SendAsync("ReceiveMessage", "Admin", message);
        }

        // Allow the user to send a message to admin
        public async Task SendMessageToAdmin(string userName, string message)
        {
            await Clients.Group("Admin").SendAsync("ReceiveMessage", userName, message);
        }

        // Allow admin to join a group (admin can listen to all users)
        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "admin");
        }
    }
}
