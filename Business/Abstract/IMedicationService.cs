using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IMedicationService
    {
        IDataResult<Medication> GetById(int medicationId);
        IDataResult<List<Medication>> GetAll();

        /// Belirli bir kullanıcıya (UserId) ait tüm ilaçları getirir.
        IDataResult<List<Medication>> GetByUserId(int userId);

        IResult Add(Medication medication);
        IResult Update(Medication medication);
        IResult Delete(Medication medication);
    }
}