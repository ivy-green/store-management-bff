using Microsoft.EntityFrameworkCore;
using ProjectBase.Jobs.Core.Interfaces.IRepositories;
using ProjectBase.Jobs.Core.Pagination;
using ProjectBase.Jobs.Postgres;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ProjectBase.Jobs.Insfrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        AppDbContext _context;
        public RepositoryBase(AppDbContext context)
        {
            _context = context;
        }
        public virtual async Task Add(TEntity entity)
        {
            await _context.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            //_context.Entry(entity).State = EntityState.Modified;
            _context.Update(entity);
        }

        public async Task AddRange(IEnumerable<TEntity> entity)
            => await _context.AddRangeAsync(entity);

        public void ExplicitLoad<TProperty>(TEntity entity,
            Expression<Func<TEntity, TProperty?>> navigationProperty) where TProperty : class
        {
            _context.Entry(entity)
                    .Reference(navigationProperty)
                    .Load();
        }

        public void ExplicitLoadCollection<TProperty>(TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> navigationProperty) where TProperty : class
        {
            _context.Entry(entity)
                    .Collection(navigationProperty)
                    .Load();
        }

        public async Task<PageList<TEntity>> GetAll(int pageSize, int pageIndex, bool ignoreFilter = false)
        {
            DbSet<TEntity> query = _context.Set<TEntity>();
            int totalRow = await query.CountAsync();

            List<TEntity> datas = ignoreFilter switch
            {
                true => await query.Skip(pageIndex * pageSize)
                                   .IgnoreQueryFilters()
                                   .Take(pageSize)
                                   .AsNoTracking()
                                   .ToListAsync(),
                false => await query.Skip(pageIndex * pageSize)
                                    .Take(pageSize)
                                    .AsNoTracking()
                                    .ToListAsync()
            };

            return new PageList<TEntity>
            {
                TotalRow = totalRow,
                PageIndex = pageIndex,
                PageSize = pageSize,
                PageData = datas
            };
        }

        public async Task<PageList<TEntity>> GetPagedByCondition(
            Expression<Func<TEntity, bool>> predicate,
            int pageIndex,
            int pageSize,
            bool trackChange = false)
        {
            DbSet<TEntity> query = _context.Set<TEntity>();
            int totalRow = await query.Where(predicate).CountAsync();

            List<TEntity> datas = await query
                .Where(predicate)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PageList<TEntity>
            {
                TotalRow = totalRow,
                PageIndex = pageIndex,
                PageSize = pageSize,
                PageData = datas
            };
        }

        // [Function] Get data by condition
        // [Prop] trackChange: Determine if data need to be tracked for update or not
        // [Prop] predicate: Condition to get the list
        public async Task<TEntity> GetByCondition(Expression<Func<TEntity, bool>> predicate,
            bool trackChange = false, bool ignoreFilter = false)
        {
            var query = ignoreFilter switch
            {
                false => _context.Set<TEntity>(),
                true => _context.Set<TEntity>().IgnoreQueryFilters()
            };

            return trackChange switch
            {
                true => await query.Where(predicate).SingleAsync(),
                false => await query.Where(predicate).AsNoTracking().SingleAsync()
            };
        }

        public async Task<List<TEntity>> GetListByCondition(Expression<Func<TEntity, bool>> predicate,
            int pageIndex,
            int pageSize,
            bool trackChange = false)
        {
            return trackChange switch
            {
                true => await _context.Set<TEntity>().Where(predicate)
                                                     .Skip(pageIndex * pageSize)
                                                     .Take(pageSize)
                                                     .ToListAsync(),
                false => await _context.Set<TEntity>().Where(predicate)
                                                     .Skip(pageIndex * pageSize)
                                                     .Take(pageSize)
                                                     .AsNoTracking().ToListAsync()
            };
        }

        public async Task<PageList<TEntity>> GetSortedList(int pageSize,
                                                           int pageIndex,
                                                           string sortString,
                                                           bool isAscending = true)
        {
            DbSet<TEntity> query = _context.Set<TEntity>();
            int totalRow = await query.CountAsync();

            // build order of expression dynamically
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, sortString);
            var lambda = Expression.Lambda(property, parameter);

            var orderByMethodName = isAscending ? "OrderBy" : "OrderByDescending";
            var orderByMethod = typeof(Queryable).GetMethods()
                .First(e => e.Name == orderByMethodName && e.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TEntity), property.Type);

            var orderQuery = (IQueryable<TEntity>?)orderByMethod.Invoke(null, [query, lambda]);
            if (orderQuery is null)
            {
                throw new Exception();
            }


            return new PageList<TEntity>
            {
                TotalRow = totalRow,
                PageIndex = pageIndex,
                PageSize = pageSize,
                PageData = await orderQuery.Skip(pageIndex * pageSize)
                                            .Take(pageSize)
                                            .AsNoTracking()
                                            .ToListAsync(),
            };
        }

        public void Remove(TEntity entity) => _context.Remove(entity);

        public void RemoveRange(IEnumerable<TEntity> entity)
               => _context.RemoveRange(entity);
    }
}
