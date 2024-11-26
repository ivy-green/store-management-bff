using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Interfaces.IRepositories;
using ProjectBase.Insfracstructure.Data;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProductCouponRepository : RepositoryBase<ProductCoupon>, IProductCouponRepository
    {
        public ProductCouponRepository(AppDBContext context) : base(context)
        {
        }
    }
}
