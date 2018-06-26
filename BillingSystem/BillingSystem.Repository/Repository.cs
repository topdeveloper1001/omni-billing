using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
using System;
using System.Data.SqlClient;
using System.Globalization;
using BillingSystem.Repository.Interfaces;
using BillingSystem.Model;
using BillingSystem.Repository.Common;
using BillingSystem.Common;
using BillingSystem.Common.Interfaces;

namespace BillingSystem.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected BillingEntities _context;

        public Repository(BillingEntities context)
        {
            _context = context;
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public virtual async Task<ICollection<T>> GetAllAsync()
        {

            return await _context.Set<T>().ToListAsync();
        }

        public virtual T Get(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public virtual async Task<T> GetAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual T Add(T t)
        {

            _context.Set<T>().Add(t);
            _context.SaveChanges();
            return t;
        }

        public virtual async Task<T> AddAsync(T t)
        {
            _context.Set<T>().Add(t);
            await _context.SaveChangesAsync();
            return t;

        }

        public virtual T Find(Expression<Func<T, bool>> match)
        {
            return _context.Set<T>().SingleOrDefault(match);
        }

        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(match);
        }

        public ICollection<T> FindAll(Expression<Func<T, bool>> match)
        {
            return _context.Set<T>().Where(match).ToList();
        }

        public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().Where(match).ToListAsync();
        }

        public virtual int Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            return _context.SaveChanges();
        }

        public virtual async Task<int> DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public virtual T Update(T t, object key)
        {
            if (t == null)
                return null;
            T exist = _context.Set<T>().Find(key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(t);
                _context.SaveChanges();
            }
            return exist;
        }

        public virtual T UpdateEntity(T t, object key)
        {
            if (t == null)
                return null;
            T exist = _context.Set<T>().Find(key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(t);
                _context.SaveChanges();
            }
            return exist;
        }
        public virtual int Updatei(T t, object key)
        {
            if (t == null)
                return 0;
            var result = 0;

            T exist = _context.Set<T>().Find(key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(t);
                result = _context.SaveChanges();
            }
            return result;
        }
        public virtual async Task<T> UpdateAsync(T t, object key)
        {
            if (t == null)
                return null;
            T exist = await _context.Set<T>().FindAsync(key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(t);
                await _context.SaveChangesAsync();
            }
            return exist;
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public virtual void Save()
        {

            _context.SaveChanges();
        }

        public async virtual Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            return query;
        }

        public virtual async Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
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
                    _context.Dispose();
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
            return _context.Set<T>().Find(keyValues);
        }

        public IPagedList GetAll(IPagedListParameters pagedListParameters)
        {
            var pagedResult = new PagedList();
            IQueryable<T> queryResults = _context.Set<T>();

            if (!string.IsNullOrEmpty(pagedListParameters.SerchColumnName) &&
                !string.IsNullOrEmpty(pagedListParameters.SerchColumnValue))
            {
                queryResults = queryResults.Where(pagedListParameters.SerchColumnName,
                                                  pagedListParameters.SerchColumnValue);
            }
            else if (!string.IsNullOrEmpty(pagedListParameters.SerchColumnValue))
            {
                queryResults = queryResults.FullTextSearch(pagedListParameters.SerchColumnValue);
            }

            queryResults = queryResults.OrderBy(pagedListParameters.SortColumn, pagedListParameters.SortAssending);

            pagedResult.TotalRecordCount = queryResults.Count();

            queryResults =
                queryResults.Skip(pagedListParameters.SkipCount).Take(pagedListParameters.TakeCount);

            pagedResult.Data = GenericHelper.GetTypedList(queryResults.AsEnumerable());
            return pagedResult;
        }

        public long Max(Expression<Func<T, long>> predicate)
        {
            return _context.Set<T>().Max(predicate);
        }

        public int? Create(T entity)
        {
            _context.Set<T>().Add(entity);
            return _context.SaveChanges();
        }

        public Task<int> CreateAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            var result = _context.SaveChangesAsync();
            return result;
        }

        public int? Create(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            return _context.SaveChanges();
        }

        public int? Update(T entity)
        {
            return _context.SaveChanges();
        }

        public int? Update(IEnumerable<T> entities)
        {
            return _context.SaveChanges();
        }

        public int? Delete(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            return _context.SaveChanges();
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
            if (_context.Entry(entity).State == EntityState.Unchanged) { return null; }
            if (_context.Entry(entity).State == EntityState.Detached) { _context.Set<T>().Add(entity); }
            return _context.SaveChanges();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public IQueryable<T> Where(IPagedListParameters pagedListParameters)
        {
            IQueryable<T> queryResults = _context.Set<T>().OrderBy(pagedListParameters.SortColumn, pagedListParameters.SortAssending);
            return queryResults;
        }

        public void ExecuteCommand(string sql, SqlParameter[] parameters)
        {
            var query = sql;

            var paramsString = parameters.Any(a => a.ParameterName.Contains("@")) ? string.Join(",", parameters.Select(p => p.ParameterName)) : string.Join(",", parameters.Select(p => $"@{p.ParameterName}"));
            query = $"Exec {sql} {paramsString}";
            _context.Database.ExecuteSqlCommand(query, parameters);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] childrensToInclude)
        {
            var model = _context.Set<T>().Where(predicate);
            foreach (var member in childrensToInclude.Select(children => children.Body as MemberExpression))
            {
                if (member == null) { throw new ArgumentException("'children' should be a member expression", "childrensToInclude"); }
                model.Include(member.Member.Name).Load();
            }
            return model;
        }

        public IQueryable<T> Where(IList<Expression<Func<T, bool>>> predicates)
        {
            IQueryable<T> queryResults = _context.Set<T>();
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
            IQueryable<T> queryResults = _context.Set<T>();
            if (predicates != null)
                queryResults = predicates.Aggregate(queryResults, (current, predicate) => current.Where(predicate));

            queryResults = queryResults.OrderBy(pagedListParameters.SortColumn, pagedListParameters.SortAssending);

            pagedResult.TotalRecordCount = queryResults.Count();
            if (predicates != null)
                queryResults = queryResults.Skip(pagedListParameters.SkipCount).Take(pagedListParameters.TakeCount);

            pagedResult.Data = queryResults.ToList();

            return pagedResult;
        }
    }
}
