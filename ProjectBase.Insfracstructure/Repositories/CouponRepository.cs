using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Interfaces.IRepositories;
using ProjectBase.Insfracstructure.Data;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class CouponRepository : RepositoryBase<Coupon>, ICouponRepository
    {
        public CouponRepository(AppDBContext context) : base(context)
        {
        }

        public override async Task Add(Coupon entity)
        {
            // find the exists one
            Coupon? data = await GetByCondition(x => x.Name == entity.Name);
            if (data is not null)
            {
                data.IsDeleted = false;
                Update(entity);
                return;
            }

            await base.Add(entity);
        }
    }
}
