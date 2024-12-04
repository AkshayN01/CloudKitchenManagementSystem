using Microsoft.AspNetCore.SignalR;

namespace CKMS.NotificationService.API.Hubs
{
    public class AdminNotificationHub : Hub
    {
        // Admin users join a group based on their KitchenId
        public async Task JoinGroup(string kitchenId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, kitchenId);
        }

        public async Task LeaveGroup(string kitchenId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, kitchenId);
        }
    }
}
