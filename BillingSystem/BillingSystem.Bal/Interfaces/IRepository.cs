using BillingSystem.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BillingSystem.Bal.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T Add(T t);
        Task<T> AddAsync(T t);
        int Count();
        Task<int> CountAsync();
        int Delete(T entity);
        Task<int> DeleteAsync(T entity);
        void Dispose();
        T Find(Expression<Func<T, bool>> match);
        ICollection<T> FindAll(Expression<Func<T, bool>> match);
        Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match);
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> predicate);
        T Get(int id);
        IQueryable<T> GetAll();
        Task<ICollection<T>> GetAllAsync();
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetAsync(int id);
        void Save();
        Task<int> SaveAsync();
        T Update(T t, object key);
        T UpdateEntity(T t, object key);
        int Updatei(T t, object key);
        Task<T> UpdateAsync(T t, object key);
        T GetSingle(object keyValues);
        Int64 Max(Expression<Func<T, long>> predicate);
        int? Create(T entity);
        Task<int> CreateAsync(T entity);
        int? Create(IEnumerable<T> entities);
        int? Update(T entity);
        int? Update(IEnumerable<T> entities);
        int? Delete(IEnumerable<T> entities);
        int Delete(object id);
        int? Delete(string ids);
        int? Save(T entity);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        IQueryable<T> Where(IPagedListParameters pagedListParameters);
        void ExecuteCommand(string sql, SqlParameter[] parameters);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] childrensToInclude);
        IQueryable<T> Where(IList<Expression<Func<T, bool>>> predicates);
        IPagedList Where(Expression<Func<T, bool>> predicate, IPagedListParameters pagedListParameters);
        IPagedList Where(IList<Expression<Func<T, bool>>> predicates, IPagedListParameters pagedListParameters);
    }
}
