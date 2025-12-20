using Core.Utilities.Results;
using Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract
{
    public interface ICaregiverPatientService
    {
        /// Bir hasta yakınını (caregiver) bir hastayı (patient) takip etmesi için eşler.
        IResult FollowPatient(int caregiverId, int patientId);

        /// Takip ilişkisini sonlandırır.
        IResult UnfollowPatient(int caregiverId, int patientId);

        /// Bir hasta yakınının takip ettiği tüm HASTA (User) listesini getirir.
        IDataResult<List<User>> GetPatientsOfCaregiver(int caregiverId);

        /// Bir hastayı takip eden tüm HASTA YAKINI (User) listesini getirir.
        IDataResult<List<User>> GetCaregiversOfPatient(int patientId);

        // 👇 EKLENEN YENİ METOT 👇
        // Dedektif servisi, hastanın ID'sini verip "Bunun doktoru kim?" diye sorabilsin.
        IDataResult<CaregiverPatient> GetCaregiverByPatientId(int patientId);
    }
}