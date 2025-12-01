using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IScheduleService
    {
        IDataResult<Schedule> GetById(int scheduleId);
        IDataResult<List<Schedule>> GetAll();

        /// Belirli bir ilaca (MedicationId) ait tüm zamanlamaları getirir.
        IDataResult<List<Schedule>> GetByMedicationId(int medicationId);

        IResult Add(Schedule schedule);
        IResult Update(Schedule schedule);
        IResult Delete(Schedule schedule);
    }
}