using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class IntakeManager : IIntakeService
    {
        private readonly IIntakeDal _intakeDal;

        public IntakeManager(IIntakeDal intakeDal)
        {
            _intakeDal = intakeDal;
        }

        public IResult Add(Intake intake)
        {
            _intakeDal.Add(intake);
            return new SuccessResult(Messages.OperationSuccessful);
        }

        public IResult Delete(Intake intake)
        {
            _intakeDal.Delete(intake);
            return new SuccessResult(Messages.OperationSuccessful);
        }

        public IDataResult<Intake> GetById(int intakeId)
        {
            var intake = _intakeDal.Get(i => i.Id == intakeId);
            if (intake == null)
            {
                return new ErrorDataResult<Intake>(Messages.IntakeNotFound);
            }
            return new SuccessDataResult<Intake>(intake);
        }

        public IDataResult<List<Intake>> GetByScheduleId(int scheduleId)
        {
            var intakes = _intakeDal.GetList(i => i.ScheduleId == scheduleId).ToList();
            return new SuccessDataResult<List<Intake>>(intakes, Messages.IntakesListed);
        }

        public IResult MarkAsSkipped(int intakeId)
        {
            var intake = _intakeDal.Get(i => i.Id == intakeId);
            if (intake == null)
            {
                return new ErrorResult(Messages.IntakeNotFound);
            }

            intake.Status = IntakeStatus.Skipped;
            intake.TakenTime = null; // Atlandığı için 'alınma zamanı' olmamalı.
            _intakeDal.Update(intake);

            return new SuccessResult(Messages.IntakeMarkedAsSkipped);
        }

        public IResult MarkAsTaken(int intakeId)
        {
            var intake = _intakeDal.Get(i => i.Id == intakeId);
            if (intake == null)
            {
                return new ErrorResult(Messages.IntakeNotFound);
            }

            intake.Status = IntakeStatus.Taken;
            intake.TakenTime = DateTime.UtcNow; // Alındığı zamanı UTC olarak kaydet
            _intakeDal.Update(intake);

            return new SuccessResult(Messages.IntakeMarkedAsTaken);
        }

        public IResult Update(Intake intake)
        {
            _intakeDal.Update(intake);
            return new SuccessResult(Messages.IntakeStatusUpdated);
        }
    }
}