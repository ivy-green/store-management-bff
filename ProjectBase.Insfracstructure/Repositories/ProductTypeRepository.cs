using ProjectBase.Domain.Entities;
using ProjectBase.Domain.Interfaces.IRepositories;
using ProjectBase.Insfracstructure.Data;
using System.Diagnostics.CodeAnalysis;
namespace ProjectBase.Insfracstructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProductTypeRepository : RepositoryBase<ProductType>, IProductTypeRepository
    {
        public ProductTypeRepository(AppDBContext context) : base(context)
        {
        }

        public override async Task Add(ProductType entity)
        {
            // find the exists one
            ProductType? data = await GetByCondition(x => x.Name == entity.Name);
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
