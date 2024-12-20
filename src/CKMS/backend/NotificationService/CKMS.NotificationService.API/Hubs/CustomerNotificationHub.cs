using Microsoft.AspNetCore.SignalR;

namespace CKMS.NotificationService.API.Hubs
{
    public class CustomerNotificationHub: Hub
    {
        public override async Task OnConnectedAsync()
        {
            var customerId = Context.User?.FindFirst("id")?.Value;

            if (!string.IsNullOrEmpty(customerId))
            {
                //here group is created to send noti to users bu their customerId
                await Groups.AddToGroupAsync(Context.ConnectionId, customerId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var customerId = Context.User?.FindFirst("id")?.Value;

            if (!string.IsNullOrEmpty(customerId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, customerId);
            }

            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessageToGroup(string groupName, string user, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveNotification", user, message);
        }
    }
}
