using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
using System;
using System.Data.SqlClient;
using System.Globalization;
using BillingSystem.Model;
using BillingSystem.Common;
using BillingSystem.Common.Interfaces;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Common.Common;

namespace BillingSystem.Bal
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected BillingEntities _ctx;

        public Repository(BillingEntities ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<T> GetAll()
        {
            return _ctx.Set<T>();
        }

        public virtual async Task<ICollection<T>> GetAllAsync()
        {

            return await _ctx.Set<T>().ToListAsync();
        }

        public virtual T Get(int id)
        {
            return _ctx.Set<T>().Find(id);
        }

        public virtual async Task<T> GetAsync(int id)
        {
            return await _ctx.Set<T>().FindAsync(id);
        }

        public virtual T Add(T t)
        {

            _ctx.Set<T>().Add(t);
            _ctx.SaveChanges();
            return t;
        }

        public virtual async Task<T> AddAsync(T t)
        {
            _ctx.Set<T>().Add(t);
            await _ctx.SaveChangesAsync();
            return t;

        }

        public virtual T Find(Expression<Func<T, bool>> match)
        {
            return _ctx.Set<T>().SingleOrDefault(match);
        }

        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _ctx.Set<T>().SingleOrDefaultAsync(match);
        }

        public ICollection<T> FindAll(Expression<Func<T, bool>> match)
        {
            return _ctx.Set<T>().Where(match).ToList();
        }

        public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await _ctx.Set<T>().Where(match).ToListAsync();
        }

        public virtual int Delete(T entity)
        {
            _ctx.Set<T>().Remove(entity);
            return _ctx.SaveChanges();
        }

        public virtual async Task<int> DeleteAsync(T entity)
        {
            _ctx.Set<T>().Remove(entity);
            return await _ctx.SaveChangesAsync();
        }

        public virtual T Update(T t, object key)
        {
            if (t == null)
                return null;
            T exist = _ctx.Set<T>().Find(key);
            if (exist != null)
            {
                _ctx.Entry(exist).CurrentValues.SetValues(t);
                _ctx.SaveChanges();
            }
            return exist;
        }

        public virtual T UpdateEntity(T t, object key)
        {
            if (t == null)
                return null;
            T exist = _ctx.Set<T>().Find(key);
            if (exist != null)
            {
                _ctx.Entry(exist).CurrentValues.SetValues(t);
                _ctx.SaveChanges();
            }
            return exist;
        }
        public virtual int Updatei(T t, object key)
        {
            if (t == null)
                return 0;
            var result = 0;

            T exist = _ctx.Set<T>().Find(key);
            if (exist != null)
            {
                _ctx.Entry(exist).CurrentValues.SetValues(t);
                result = _ctx.SaveChanges();
            }
            return result;
        }
        public virtual async Task<T> UpdateAsync(T t, object key)
        {
            if (t == null)
                return null;
            T exist = await _ctx.Set<T>().FindAsync(key);
            if (exist != null)
            {
                _ctx.Entry(exist).CurrentValues.SetValues(t);
                await _ctx.SaveChangesAsync();
            }
            return exist;
        }

        public int Count()
        {
            return _ctx.Set<T>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await _ctx.Set<T>().CountAsync();
        }

        public virtual void Save()
        {

            _ctx.SaveChanges();
        }

        public async virtual Task<int> SaveAsync()
        {
            return await _ctx.SaveChangesAsync();
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _ctx.Set<T>().Where(predicate);
            return query;
        }

        public virtual async Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _ctx.Set<T>().Where(predicate).ToListAsync();
        }

        public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {

            IQueryable<T> queryable = GetAll();
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {

                queryable = queryable.Include<T, object>(includeProperty);
            }

            return queryable;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _ctx.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual T GetSingle(object keyValues)
        {
            return _ctx.Set<T>().Find(keyValues);
        }

        public long Max(Expression<Func<T, long>> predicate)
        {
            return _ctx.Set<T>().Max(predicate);
        }

        public int? Create(T entity)
        {
            _ctx.Set<T>().Add(entity);
            return _ctx.SaveChanges();
        }

        public Task<int> CreateAsync(T entity)
        {
            _ctx.Set<T>().Add(entity);
            var result = _ctx.SaveChangesAsync();
            return result;
        }

        public int? Create(IEnumerable<T> entities)
        {
            _ctx.Set<T>().AddRange(entities);
            return _ctx.SaveChanges();
        }

        public int? Update(T entity)
        {
            return _ctx.SaveChanges();
        }

        public int? Update(IEnumerable<T> entities)
        {
            return _ctx.SaveChanges();
        }

        public int? Delete(IEnumerable<T> entities)
        {
            _ctx.Set<T>().RemoveRange(entities);
            return _ctx.SaveChanges();
        }

        public int Delete(object id)
        {
            T entity = GetSingle(Convert.ToInt32(id, CultureInfo.InvariantCulture));
            if (entity == null) return 0;
            Delete(entity);
            return 1;
        }

        public int? Delete(string ids)
        {
            ids = ids.TrimEnd(',');
            var idArray = ids.Split(',');
            var processed = 0;
            foreach (var id in idArray)
            {
                if (string.IsNullOrEmpty(id)) continue;
                Delete(id);
                processed++;
            }
            return processed;
        }

        public int? Save(T entity)
        {
            if (_ctx.Entry(entity).State == EntityState.Unchanged) { return null; }
            if (_ctx.Entry(entity).State == EntityState.Detached) { _ctx.Set<T>().Add(entity); }
            return _ctx.SaveChanges();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _ctx.Set<T>().Where(predicate);
        }

        public IQueryable<T> Where(IPagedListParameters pagedListParameters)
        {
            IQueryable<T> queryResults = _ctx.Set<T>().OrderBy(pagedListParameters.SortColumn, pagedListParameters.SortAssending);
            return queryResults;
        }

        public void ExecuteCommand(string sql, SqlParameter[] parameters)
        {
            var query = sql;

            var paramsString = parameters.Any(a => a.ParameterName.Contains("@")) ? string.Join(",", parameters.Select(p => p.ParameterName)) : string.Join(",", parameters.Select(p => $"@{p.ParameterName}"));
            query = $"Exec {sql} {paramsString}";
            _ctx.Database.ExecuteSqlCommand(query, parameters);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] childrensToInclude)
        {
            var model = _ctx.Set<T>().Where(predicate);
            foreach (var member in childrensToInclude.Select(children => children.Body as MemberExpression))
            {
                if (member == null) { throw new ArgumentException("'children' should be a member expression", "childrensToInclude"); }
                model.Include(member.Member.Name).Load();
            }
            return model;
        }

        public IQueryable<T> Where(IList<Expression<Func<T, bool>>> predicates)
        {
            IQueryable<T> queryResults = _ctx.Set<T>();
            if (predicates != null)
                queryResults = predicates.Aggregate(queryResults, (current, predicate) => current.Where(predicate));

            return queryResults;
        }

        public IPagedList Where(Expression<Func<T, bool>> predicate, IPagedListParameters pagedListParameters)
        {
            IList<Expression<Func<T, bool>>> predicates = new List<Expression<Func<T, bool>>> { predicate };
            return Where(predicates, pagedListParameters);
        }

        public IPagedList Where(IList<Expression<Func<T, bool>>> predicates, IPagedListParameters pagedListParameters)
        {
            var pagedResult = new PagedList();
            IQueryable<T> queryResults = _ctx.Set<T>();
            if (predicates != null)
                queryResults = predicates.Aggregate(queryResults, (current, predicate) => current.Where(predicate));

            queryResults = queryResults.OrderBy(pagedListParameters.SortColumn, pagedListParameters.SortAssending);

            pagedResult.TotalRecordCount = queryResults.Count();
            if (predicates != null)
                queryResults = queryResults.Skip(pagedListParameters.SkipCount).Take(pagedListParameters.TakeCount);

            pagedResult.Data = queryResults.ToList();

            return pagedResult;
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _ctx.Set<T>().Any(predicate);
        }
    }
}
