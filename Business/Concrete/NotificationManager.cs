using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System.Collections.Generic;
using System.Linq;

namespace Business.Concrete
{
    public class NotificationManager : INotificationService
    {
        private readonly INotificationDal _notificationDal;

        public NotificationManager(INotificationDal notificationDal)
        {
            _notificationDal = notificationDal;
        }

        public IResult Add(Notification notification)
        {
            _notificationDal.Add(notification);
            return new SuccessResult("Notification added");
        }

        public IDataResult<List<Notification>> GetByPatient(int patientId)
        {
            var list = _notificationDal
            .GetList(n => n.PatientId == patientId)   // <-- أهم شي هون
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

            return new SuccessDataResult<List<Notification>>(list);
        }

    }
}
