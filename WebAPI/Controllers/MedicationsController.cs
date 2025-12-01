using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicationsController : ControllerBase
    {
        private readonly IMedicationService _medicationService;

        public MedicationsController(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        /// Belirli bir kullanıcıya ait tüm ilaçları listeler.
        /// Flutter uygulamasında "İlaçlarım" sayfasında bu çağrılacak.
        /// <param name="userId">Kullanıcının ID'si</param>
        /// <returns></returns>
        [HttpGet("getbyuserid/{userId}")]
        public IActionResult GetByUserId(int userId)
        {
            var result = _medicationService.GetByUserId(userId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// ID'ye göre tek bir ilacı getirir.
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _medicationService.GetById(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// Yeni bir ilaç ekler.
        /// <param name="medication">Gövdede (body) gönderilen ilaç JSON'u</param>
        /// <returns></returns>
        [HttpPost("add")]
        public IActionResult Add(Medication medication)
        {
            var result = _medicationService.Add(medication);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// Mevcut bir ilacı günceller.
        /// <param name="medication"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public IActionResult Update(Medication medication)
        {
            var result = _medicationService.Update(medication);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// ID'si verilen ilacı siler.
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Önce ilacı bulmamız gerekiyor, çünkü Repository 'Delete' için entity istiyor.
            var medicationResult = _medicationService.GetById(id);
            if (!medicationResult.Success)
            {
                return BadRequest(medicationResult); // İlaç bulunamadı
            }

            var deleteResult = _medicationService.Delete(medicationResult.Data);
            if (deleteResult.Success)
            {
                return Ok(deleteResult);
            }
            return BadRequest(deleteResult);
        }
    }
}