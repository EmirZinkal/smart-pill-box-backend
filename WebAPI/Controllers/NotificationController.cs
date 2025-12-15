using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("add")]
        public IActionResult Add(Notification notification)
        {
            var result = _notificationService.Add(notification);
            return Ok(result);
        }

        [HttpGet("bypatient/{id}")]
        public IActionResult GetByPatient(int id)
        {
            var result = _notificationService.GetByPatient(id);
            return Ok(result);
        }
    }
}
