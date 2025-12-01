// WebAPI/Controllers/IntakesController.cs
using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IntakesController : ControllerBase
    {
        private readonly IIntakeService _intakeService;

        public IntakesController(IIntakeService intakeService)
        {
            _intakeService = intakeService;
        }

        /// <summary>
        /// Bir zamanlamaya (schedule) bağlı tüm ilaç alım kayıtlarını (geçmiş ve gelecek) getirir.
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <returns></returns>
        [HttpGet("getbyschedule/{scheduleId}")]
        public IActionResult GetByScheduleId(int scheduleId)
        {
            var result = _intakeService.GetByScheduleId(scheduleId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Planlanmış bir ilaç alımını 'Alındı' (Taken) olarak işaretler.
        /// (Flutter'daki "Aldım" butonu)
        /// </summary>
        /// <param name="intakeId">İlaç alım kaydının ID'si</param>
        /// <returns></D</returns>
        [HttpPut("marktaken/{intakeId}")]
        public IActionResult MarkAsTaken(int intakeId)
        {
            var result = _intakeService.MarkAsTaken(intakeId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Planlanmış bir ilaç alımını 'Atlandı' (Skipped) olarak işaretler.
        /// (Flutter'daki "Atla" butonu)
        /// </summary>
        /// <param name="intakeId">İlaç alım kaydının ID'si</param>
        /// <returns></returns>
        [HttpPut("markskipped/{intakeId}")]
        public IActionResult MarkAsSkipped(int intakeId)
        {
            var result = _intakeService.MarkAsSkipped(intakeId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Manuel olarak yeni bir ilaç alım kaydı ekler.
        /// (Not: Bu genelde arka plan servisiyle yapılır ama manuel ekleme için de durabilir)
        /// </summary>
        /// <param name="intake"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public IActionResult Add(Intake intake)
        {
            var result = _intakeService.Add(intake);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}