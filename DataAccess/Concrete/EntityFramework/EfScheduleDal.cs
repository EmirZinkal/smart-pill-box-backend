using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfScheduleDal : EfEntityRepositoryBase<Schedule, IlacTakipContext>, IScheduleDal
    {
        public EfScheduleDal(IlacTakipContext context) : base(context)
        {
        }
    }
}