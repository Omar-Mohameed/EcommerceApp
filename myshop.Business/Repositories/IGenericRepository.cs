using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Business.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // context.categories.Include("Products").ToList();
        // context.categories.Where(x=>x.id==id).ToList();
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, string? include= null);
        T GetById(int id);
        // context.category.where(x=>x.id==id).tosingleordefault();
        T GetFirstOrDefault(Expression<Func<T, bool>>? predicate=null, string? include=null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
