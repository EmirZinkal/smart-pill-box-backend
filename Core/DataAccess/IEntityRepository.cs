using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Core.Entities;

namespace Core.DataAccess
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        T? Get(Expression<Func<T, bool>> filter);//tek bir nesne 
        IList<T> GetList(Expression<Func<T, bool>> filter = null);//ilgili nesnenin listesini başlangıçta filtre null
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}