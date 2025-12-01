using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class MedicationManager : IMedicationService
    {
        private readonly IMedicationDal _medicationDal;

        public MedicationManager(IMedicationDal medicationDal)
        {
            _medicationDal = medicationDal;
        }

        public IResult Add(Medication medication)
        {
            _medicationDal.Add(medication);
            return new SuccessResult(Messages.MedicationAdded);
        }

        public IResult Delete(Medication medication)
        {
            _medicationDal.Delete(medication);
            return new SuccessResult(Messages.MedicationDeleted);
        }

        public IDataResult<List<Medication>> GetAll()
        {
            var meds = _medicationDal.GetList().ToList();
            return new SuccessDataResult<List<Medication>>(meds, Messages.MedicationsListed);
        }

        public IDataResult<Medication> GetById(int medicationId)
        {
            var med = _medicationDal.Get(m => m.Id == medicationId);
            if (med == null)
            {
                return new ErrorDataResult<Medication>(Messages.MedicationNotFound);
            }
            return new SuccessDataResult<Medication>(med);
        }

        public IDataResult<List<Medication>> GetByUserId(int userId)
        {
            var meds = _medicationDal.GetList(m => m.UserId == userId).ToList();
            return new SuccessDataResult<List<Medication>>(meds, Messages.MedicationsListedForUser);
        }

        public IResult Update(Medication medication)
        {
            _medicationDal.Update(medication);
            return new SuccessResult(Messages.MedicationUpdated);
        }
    }
}