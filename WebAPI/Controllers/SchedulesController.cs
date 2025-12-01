// WebAPI/Controllers/SchedulesController.cs
using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public SchedulesController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// Bir ilaca bağlı tüm zamanlamaları getirir.
        /// </summary>
        /// <param name="medicationId"></param>
        /// <returns></returns>
        [HttpGet("getbymedication/{medicationId}")]
        public IActionResult GetByMedicationId(int medicationId)
        {
            var result = _scheduleService.GetByMedicationId(medicationId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Yeni bir zamanlama ekler.
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public IActionResult Add(Schedule schedule)
        {
            var result = _scheduleService.Add(schedule);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}