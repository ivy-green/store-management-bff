using ProjectBase.Jobs.Core.Pagination;
using System.Linq.Expressions;

namespace ProjectBase.Jobs.Core.Interfaces.IRepositories
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        Task Add(TEntity entity);
        void Remove(TEntity entity);
        void Update(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entity);
        Task AddRange(IEnumerable<TEntity> entity);
        Task<PageList<TEntity>> GetAll(int pageSize, int pageIndex, bool ignoreFilter = false);
        Task<PageList<TEntity>> GetPagedByCondition(Expression<Func<TEntity, bool>> predicate,
                                                int pageIndex = 0,
                                                int pageSize = int.MaxValue,
                                                bool trackChange = false);
        Task<PageList<TEntity>> GetSortedList(int pageSize,
                                                int pageIndex,
                                                string sortString,
                                                bool isAscending = true);
        Task<TEntity> GetByCondition(Expression<Func<TEntity, bool>> predicate,
                                                bool trackChange = false, bool ignoreFilter = false);
        Task<List<TEntity>> GetListByCondition(Expression<Func<TEntity, bool>> predicate,
                                                int pageIndex = 0,
                                                int pageSize = int.MaxValue,
                                                bool trackChange = false);
        void ExplicitLoad<TProperty>(TEntity entity,
                                     Expression<Func<TEntity, TProperty?>> navigationProperty)
                                        where TProperty : class;
        void ExplicitLoadCollection<TProperty>(TEntity entity,
                                     Expression<Func<TEntity, IEnumerable<TProperty>>> navigationProperty)
                                        where TProperty : class;

    }
}
