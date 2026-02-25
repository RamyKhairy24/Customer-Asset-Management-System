using CustomerFluent.Models;

namespace CustomerFluent.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICustomerRepository Customers { get; }
        IRepository<Location> Locations { get; }
        IRepository<House> Houses { get; }
        IRepository<Building> Buildings { get; }
        IRepository<PaymentStatus> PaymentStatuses { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}