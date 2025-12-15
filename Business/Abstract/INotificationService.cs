using Core.Utilities.Results;
using Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract
{
    public interface INotificationService
    {
        IResult Add(Notification notification);
        IDataResult<List<Notification>> GetByPatient(int patientId);
    }
}
