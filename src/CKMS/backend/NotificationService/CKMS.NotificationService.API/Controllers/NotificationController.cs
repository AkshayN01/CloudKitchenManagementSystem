using CKMS.Contracts.DTOs.AdminUser.Request;
using CKMS.Contracts.DTOs.Notification.Request;
using CKMS.Interfaces.Repository;
using CKMS.Library.UserNotification;
using CKMS.NotificationService.Blanket;
using Microsoft.AspNetCore.Mvc;

namespace CKMS.NotificationService.API.Controllers
{
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationBlanket _notificationBlanket;
        public NotificationController(INotificationUnitOfWork notificationUnitOfWork, NotificationHandler notificationHandler) 
        {
            _notificationBlanket = new NotificationBlanket(notificationUnitOfWork, notificationHandler);
        }

        [HttpPost]
        [Route("/api/notification/add-notification")]
        public async Task<IActionResult> AddNotification([FromBody] List<NotificationPayload> payload)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); };

            try
            {
                var httpResponse = await _notificationBlanket.SendNotification(payload);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
