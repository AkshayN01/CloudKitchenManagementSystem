using Microsoft.AspNetCore.SignalR;

namespace CKMS.NotificationService.API.Hubs
{
    public class AdminNotificationHub : Hub
    {
        // Admin users join a group based on their KitchenId
        public override async Task OnConnectedAsync()
        {
            var kitchenGroup = Context.User?.FindFirst("kitchenId")?.Value;

            if (!string.IsNullOrEmpty(kitchenGroup))
            {
                //here group is created to send noti to users bu their customerId
                await Groups.AddToGroupAsync(Context.ConnectionId, kitchenGroup);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var kitchenGroup = Context.User?.FindFirst("kitchenId")?.Value;

            if (!string.IsNullOrEmpty(kitchenGroup))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, kitchenGroup);
            }

            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessageToGroup(string groupName, string user, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveNotification", user, message);
        }
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
