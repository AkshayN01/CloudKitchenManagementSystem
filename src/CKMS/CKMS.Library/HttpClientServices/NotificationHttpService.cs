using CKMS.Contracts.DTOs.Notification.Request;
using CKMS.Interfaces.HttpClientServices;
using CKMS.Library.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Library.Services
{
    public class NotificationHttpService : INotificationHttpService
    {
        private readonly HttpClient _httpClient;
        public NotificationHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> SendNotification(List<NotificationPayload> payload)
        {
            String endpoint = "/send-notification";
            if(payload == null)
                return false;

            String json = await Utility.SerialiseData<List<NotificationPayload>>(payload);

            StringContent jsonContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, jsonContent);

            // Ensure success status code
            response.EnsureSuccessStatusCode();

            // Deserialize and return the response content
            var content = await response.Content.ReadAsStringAsync();

            bool success = Convert.ToBoolean(content);

            return true;
        }
    }
}
