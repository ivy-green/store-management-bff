using ProjectBase.Domain.Interfaces;
using ProjectBase.Domain.Interfaces.IRepositories;
using ProjectBase.Insfracstructure.Data;

namespace ProjectBase.Application.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IBillRepository _billRepository;
        private readonly IBillDetailsRepository _billDetailsRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly IStatisticBillRepository _statisticBillRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IProductOnSaleRepository _productOnSaleRepository;

        public UnitOfWork(
            AppDBContext context,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IProductRepository productRepository,
            IProductTypeRepository productTypeRepository,
            IBillRepository billRepository,
            IBillDetailsRepository billDetailsRepository,
            IRoleRepository roleRepository,
            IBlacklistRepository blacklistRepository,
            IStatisticBillRepository statisticBillRepository,
            IBranchRepository branchRepository,
            IProductOnSaleRepository productOnSaleRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _productRepository = productRepository;
            _productTypeRepository = productTypeRepository;
            _billRepository = billRepository;
            _billDetailsRepository = billDetailsRepository;
            _roleRepository = roleRepository;
            _blacklistRepository = blacklistRepository;
            _statisticBillRepository = statisticBillRepository;
            _branchRepository = branchRepository;
            _productOnSaleRepository = productOnSaleRepository;
        }

        public IUserRepository UserRepository { get => _userRepository; }
        public IUserRoleRepository UserRoleRepository { get => _userRoleRepository; }
        public IProductRepository ProductRepository { get => _productRepository; }
        public IProductTypeRepository ProductTypeRepository { get => _productTypeRepository; }
        public IBillRepository BillRepository { get => _billRepository; }
        public IBillDetailsRepository BillDetailsRepository { get => _billDetailsRepository; }
        public IRoleRepository RoleRepository { get => _roleRepository; }
        public IBlacklistRepository BlacklistRepository { get => _blacklistRepository; }
        public IStatisticBillRepository StatisticBillRepository { get => _statisticBillRepository; }
        public IBranchRepository BranchRepository { get => _branchRepository; }
        public IProductOnSaleRepository ProductOnSaleRepository { get => _productOnSaleRepository; }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
