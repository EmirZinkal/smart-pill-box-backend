using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class CaregiverPatientManager : ICaregiverPatientService
    {
        private readonly ICaregiverPatientDal _caregiverPatientDal;
        private readonly IUserDal _userDal; // Hastaları/Yakınlarını 'User' olarak getirmek için

        public CaregiverPatientManager(ICaregiverPatientDal caregiverPatientDal, IUserDal userDal)
        {
            _caregiverPatientDal = caregiverPatientDal;
            _userDal = userDal;
        }

        public IResult FollowPatient(int caregiverId, int patientId)
        {
            // Kural: Aynı ilişki tekrar eklenemez.
            var existing = _caregiverPatientDal.Get(cp =>
                cp.CaregiverId == caregiverId && cp.PatientId == patientId);

            if (existing != null)
            {
                return new ErrorResult(Messages.PatientIsAlreadyFollowed);
            }

            var newFollow = new CaregiverPatient
            {
                CaregiverId = caregiverId,
                PatientId = patientId
            };
            _caregiverPatientDal.Add(newFollow);
            return new SuccessResult(Messages.UserIsNowFollowing);
        }

        public IDataResult<List<User>> GetCaregiversOfPatient(int patientId)
        {
            // 1. Bu hastaya ait tüm ilişkileri (ilişki ID'lerini) bul
            var relationships = _caregiverPatientDal.GetList(cp => cp.PatientId == patientId);

            // 2. İlişkilerden hasta yakını ID'lerini (CaregiverId) ayıkla
            var caregiverIds = relationships.Select(r => r.CaregiverId).ToList();

            // 3. Bu ID'lere sahip User nesnelerini getir
            var caregivers = _userDal.GetList(u => caregiverIds.Contains(u.Id)).ToList();

            return new SuccessDataResult<List<User>>(caregivers, Messages.CaregiversListed);
        }

        public IDataResult<List<User>> GetPatientsOfCaregiver(int caregiverId)
        {
            // 1. Bu hasta yakınına ait tüm ilişkileri bul
            var relationships = _caregiverPatientDal.GetList(cp => cp.CaregiverId == caregiverId);

            // 2. İlişkilerden hasta ID'lerini (PatientId) ayıkla
            var patientIds = relationships.Select(r => r.PatientId).ToList();

            // 3. Bu ID'lere sahip User nesnelerini (yani hastaları) getir
            var patients = _userDal.GetList(u => patientIds.Contains(u.Id)).ToList();

            return new SuccessDataResult<List<User>>(patients, Messages.PatientsListed);
        }

        public IResult UnfollowPatient(int caregiverId, int patientId)
        {
            var existing = _caregiverPatientDal.Get(cp =>
                cp.CaregiverId == caregiverId && cp.PatientId == patientId);

            if (existing == null)
            {
                return new ErrorResult(Messages.FollowRelationshipNotFound);
            }

            _caregiverPatientDal.Delete(existing);
            return new SuccessResult(Messages.UserUnfollowed);
        }
    }
}