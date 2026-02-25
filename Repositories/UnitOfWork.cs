using Microsoft.EntityFrameworkCore.Storage;
using CustomerFluent.Data;
using CustomerFluent.Models;

namespace CustomerFluent.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private IDbContextTransaction? _transaction;

        private ICustomerRepository? _customers;
        private IRepository<Location>? _locations;
        private IRepository<House>? _houses;
        private IRepository<Building>? _buildings;
        private IRepository<PaymentStatus>? _paymentStatuses;

        public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ICustomerRepository Customers =>
            _customers ??= new CustomerRepository(_context, 
                Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { }).CreateLogger<CustomerRepository>());

        public IRepository<Location> Locations =>
            _locations ??= new Repository<Location>(_context,
                Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { }).CreateLogger<Repository<Location>>());

        public IRepository<House> Houses =>
            _houses ??= new Repository<House>(_context,
                Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { }).CreateLogger<Repository<House>>());

        public IRepository<Building> Buildings =>
            _buildings ??= new Repository<Building>(_context,
                Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { }).CreateLogger<Repository<Building>>());

        public IRepository<PaymentStatus> PaymentStatuses =>
            _paymentStatuses ??= new Repository<PaymentStatus>(_context,
                Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { }).CreateLogger<Repository<PaymentStatus>>());

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                _logger.LogDebug("Saving changes to database");
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes to database");
                throw;
            }
        }

        public async Task BeginTransactionAsync()
        {
            try
            {
                _logger.LogDebug("Beginning database transaction");
                _transaction = await _context.Database.BeginTransactionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error beginning database transaction");
                throw;
            }
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    _logger.LogDebug("Committing database transaction");
                    await _transaction.CommitAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing database transaction");
                await RollbackTransactionAsync();
                throw;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    _logger.LogDebug("Rolling back database transaction");
                    await _transaction.RollbackAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rolling back database transaction");
                throw;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}