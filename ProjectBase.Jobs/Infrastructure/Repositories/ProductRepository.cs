using ProjectBase.Jobs.Core.Entities;
using ProjectBase.Jobs.Core.Interfaces.IRepositories;
using ProjectBase.Jobs.Postgres;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Jobs.Insfrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }
    }
}
