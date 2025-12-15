using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Context;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfNotificationDal
        : EfEntityRepositoryBase<Notification, IlacTakipContext>, INotificationDal
    {
        public EfNotificationDal(IlacTakipContext context) : base(context)
        {
        }
    }
}
