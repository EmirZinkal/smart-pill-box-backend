using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class ScheduleManager : IScheduleService
    {
        private readonly IScheduleDal _scheduleDal;

        public ScheduleManager(IScheduleDal scheduleDal)
        {
            _scheduleDal = scheduleDal;
        }

        public IResult Add(Schedule schedule)
        {
            _scheduleDal.Add(schedule);
            return new SuccessResult(Messages.OperationSuccessful);
        }

        public IResult Delete(Schedule schedule)
        {
            _scheduleDal.Delete(schedule);
            return new SuccessResult(Messages.OperationSuccessful);
        }

        public IDataResult<List<Schedule>> GetAll()
        {
            var schedules = _scheduleDal.GetList().ToList();
            return new SuccessDataResult<List<Schedule>>(schedules);
        }

        public IDataResult<Schedule> GetById(int scheduleId)
        {
            var schedule = _scheduleDal.Get(s => s.Id == scheduleId);
            return new SuccessDataResult<Schedule>(schedule);
        }

        public IDataResult<List<Schedule>> GetByMedicationId(int medicationId)
        {
            var schedules = _scheduleDal.GetList(s => s.MedicationId == medicationId).ToList();
            return new SuccessDataResult<List<Schedule>>(schedules);
        }

        public IResult Update(Schedule schedule)
        {
            _scheduleDal.Update(schedule);
            return new SuccessResult(Messages.OperationSuccessful);
        }
    }
}