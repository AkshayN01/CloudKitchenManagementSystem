using Microsoft.AspNetCore.SignalR;

namespace CKMS.NotificationService.API.Hubs
{
    public class CustomerNotificationHub: Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Assume CustomerId is passed as a query string or obtained from claims
            var customerId = Context.User?.FindFirst("customerId")?.Value ?? Context.GetHttpContext()?.Request.Query["customerId"];

            if (!string.IsNullOrEmpty(customerId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, customerId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var customerId = Context.User?.FindFirst("customerId")?.Value ?? Context.GetHttpContext()?.Request.Query["customerId"];

            if (!string.IsNullOrEmpty(customerId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, customerId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
