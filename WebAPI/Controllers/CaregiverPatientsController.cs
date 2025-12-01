using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CaregiverPatientsController : ControllerBase
    {
        private readonly ICaregiverPatientService _caregiverPatientService;

        public CaregiverPatientsController(ICaregiverPatientService caregiverPatientService)
        {
            _caregiverPatientService = caregiverPatientService;
        }

        /// <summary>
        /// Bir hasta yakınının (Caregiver) bir hastayı (Patient) takibe almasını sağlar.
        /// </summary>
        /// <param name="caregiverId">Takip eden kişinin ID'si</param>
        /// <param name="patientId">Takip edilecek hastanın ID'si</param>
        /// <returns></returns>
        [HttpPost("follow")]
        public IActionResult FollowPatient(int caregiverId, int patientId)
        {
            var result = _caregiverPatientService.FollowPatient(caregiverId, patientId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result); // Örn: Zaten takip ediyorsa hata döner
        }

        /// <summary>
        /// Bir hasta yakınının bir hastayı takipten bırakmasını sağlar.
        /// </summary>
        /// <param name="caregiverId"></param>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [HttpDelete("unfollow")]
        public IActionResult UnfollowPatient(int caregiverId, int patientId)
        {
            var result = _caregiverPatientService.UnfollowPatient(caregiverId, patientId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Bir hasta yakınının takip ettiği tüm HASTALARI listeler.
        /// (Flutter'da "Takip Ettiğim Hastalarım" ekranı)
        /// </summary>
        /// <param name="caregiverId">Hasta yakınının ID'si</param>
        /// <returns></returns>
        [HttpGet("getpatients/{caregiverId}")]
        public IActionResult GetPatientsOfCaregiver(int caregiverId)
        {
            var result = _caregiverPatientService.GetPatientsOfCaregiver(caregiverId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Bir hastayı takip eden tüm HASTA YAKINLARINI listeler.
        /// </summary>
        /// <param name="patientId">Hastanın ID'si</param>
        /// <returns></returns>
        [HttpGet("getcaregivers/{patientId}")]
        public IActionResult GetCaregiversOfPatient(int patientId)
        {
            var result = _caregiverPatientService.GetCaregiversOfPatient(patientId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}