using ProjectBase.Jobs.Core.Interfaces;
using ProjectBase.Jobs.Core.Interfaces.IRepositories;
using ProjectBase.Jobs.Postgres;

namespace ProjectBase.Jobs.Core.ApplicationLogic.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IBillRepository _billRepository;
        private readonly IStatisticBillRepository _statisticBillRepository;
        private readonly IDynamoRepository _dynamoRepository;
        private readonly IProductRepository _productRepository;

        public UnitOfWork(
            AppDbContext context,
            IBillRepository billRepository,
            IStatisticBillRepository statisticBillRepository,
            IDynamoRepository dynamoRepository,
            IProductRepository productRepository)
        {
            _context = context;
            _billRepository = billRepository;
            _statisticBillRepository = statisticBillRepository;
            _dynamoRepository = dynamoRepository;
            _productRepository = productRepository;
        }

        public IBillRepository BillRepository { get => _billRepository; }
        public IStatisticBillRepository StatisticBillRepository { get => _statisticBillRepository; }
        public IDynamoRepository DynamoRepository { get => _dynamoRepository; }
        public IProductRepository ProductRepository { get => _productRepository; }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
