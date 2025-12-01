using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IIntakeService
    {
        IDataResult<Intake> GetById(int intakeId);
        IDataResult<List<Intake>> GetByScheduleId(int scheduleId);

        /// Bir ilacı 'Alındı' (Taken) olarak işaretler.
        IResult MarkAsTaken(int intakeId);

        /// Bir ilacı 'Atlandı' (Skipped) olarak işaretler.
        IResult MarkAsSkipped(int intakeId);

        // Not: 'Missed' (Kaçırıldı) durumu muhtemelen bir arka plan servisi
        // (Background Job) tarafından otomatik olarak ayarlanmalıdır.

        IResult Add(Intake intake);
        IResult Update(Intake intake);
        IResult Delete(Intake intake);
    }
}