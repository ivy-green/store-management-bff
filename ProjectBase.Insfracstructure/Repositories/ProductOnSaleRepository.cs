using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Interfaces.IRepositories;
using ProjectBase.Insfracstructure.Data;
using System.Diagnostics.CodeAnalysis;
namespace ProjectBase.Insfracstructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProductOnSaleRepository : RepositoryBase<ProductOnSale>, IProductOnSaleRepository
    {
        public ProductOnSaleRepository(AppDBContext context) : base(context)
        {
        }
    }
}
